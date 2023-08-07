using System;
using System.Collections.Generic;
using umi3d.baseBrowser.connection;
using umi3d.common.interaction;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private NavigationScreen _navigationScreen;
    private ConnectionScreen _connectionScreen;
    private VisualElement _errorBox;

    public NavigationScreen NavigationScreen => _navigationScreen;
    public ConnectionScreen ConnectionScreen => _connectionScreen;

    private MenuState _currentState;
    private MenuState _homeState;
    private MenuState _loginState;
    private MenuState _organisationState;
    private MenuState _highLevelLoginState;
    private MenuState _worldState;
    private MenuState _placeSpawnState;
    private MenuState _spawnState;

    private void Start()
    {
        Debug.Assert(_uiDocument != null);

        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        _uiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        InitUiElements();
        InitStateMachine();
        InitLocalisation();
    }

    private void InitUiElements()
    {
        _navigationScreen = new NavigationScreen(_uiDocument.rootVisualElement.Q("Navigation"));
        _navigationScreen.Root.Q<Button>("ButtonLogOut").clicked += () => {
            BaseConnectionProcess.Instance.Leave();
            ToHome();
        };
        _navigationScreen.Hide();

        _connectionScreen = new ConnectionScreen(_uiDocument.rootVisualElement.Q("Connection"));
        _connectionScreen.Hide();

        _errorBox = _uiDocument.rootVisualElement.Q("ErrorBox");
        _errorBox.Q<Button>("ButtonOk").clicked += () => _errorBox.AddToClassList("hidden");
        _errorBox.AddToClassList("hidden");
    }

    private void InitStateMachine()
    {
        _homeState = new HomeState(this);
        _loginState = new LoginState(this);
        _organisationState = new OrganisationState(this);
        _highLevelLoginState = new HighLevelLoginState(this);
        _worldState = new WorldState(this);
        _placeSpawnState = new PlaceSpawnState(this);
        _spawnState = new SpawnState(this);

        ChangeState(_homeState);
    }

    public void ToHome() => ChangeState(_homeState);
    public void ToLogin(List<VisualElement> elements, Action callback)
    {
        ChangeState(_loginState);
        _loginState.SetData(elements, callback);
    }
    public void ToOrganisation() => ChangeState(_organisationState);
    public void ToHighLevelLogin() => ChangeState(_highLevelLoginState);
    public void ToWorld() => ChangeState(_worldState);
    public void ToPlaceSpawn() => ChangeState(_placeSpawnState);
    public void ToSpawn() => ChangeState(_spawnState);

    private void ChangeState(MenuState newState)
    {
        if (_currentState != null)
            _currentState.Exit();

        _currentState = newState;

        _currentState.Enter();
    }

    private void InitLocalisation()
    {
        var labels = _uiDocument.rootVisualElement.Query<TextElement>().ToList();

        foreach (var label in labels)
        {
            if (label.text == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(label.text);
            if (trad != null)
                label.text = trad;
        }
    }

    public void OpenErrorBox(string message)
    {
        _errorBox.Q<TextElement>("Message").text = message;
        _errorBox.RemoveFromClassList("hidden");
    }

    public void GetParameterDtos(ConnectionFormDto form, Action<FormAnswerDto> callback)
    {
        if (form == null)
        {
            callback.Invoke(null);
            return;
        }

        Debug.Log("===== NEW FORM RECEIVED =====");
        Debug.Log("Form name : " + form.name);

        FormAnswerDto answer = new FormAnswerDto()
        {
            boneType = 0,
            hoveredObjectId = 0,
            id = form.id,
            toolId = 0,
            answers = new List<ParameterSettingRequestDto>()
        };

        if (form.name == "Connection")
        {
            ToLogin(GetVisualElements(form, answer), () => callback?.Invoke(answer));
        }
    }

    private List<VisualElement> GetVisualElements(ConnectionFormDto form, FormAnswerDto to)
    {
        var result = new List<VisualElement>();
        foreach (var item in form.fields)
        {
            var requestDto = new ParameterSettingRequestDto()
            {
                toolId = form.id,
                id = item.id,
                parameter = item.GetValue(),
                hoveredObjectId = 0
            };

            switch (item)
            {
                case BooleanParameterDto booleanParameterDto:
                    var toggle = new ToggleButton_C();
                    toggle.name = item.name;
                    toggle.label = item.name;
                    toggle.RegisterValueChangedCallback(e =>
                    {
                        booleanParameterDto.value = e.newValue;
                        requestDto.parameter = e.newValue;
                    });
                    result.Add(toggle);
                    break;
                case FloatRangeParameterDto floatRangeParameterDto:
                    Debug.LogWarning("TODO Field : " + item.name + "(floatRange)");
                    break;
                case EnumParameterDto<string> enumParameterDto:
                    Debug.LogWarning("TODO Field : " + item.name + "(enum)");
                    break;
                case StringParameterDto stringParameterDto:
                    var text = new TextField();
                    text.name = item.name;
                    text.label = item.name;
                    text.RegisterValueChangedCallback(e =>
                    {
                        stringParameterDto.value = e.newValue;
                        requestDto.parameter = e.newValue;
                    });
                    if (text.name == "Password")
                    {
                        text.isPasswordField = true;
                    }
                    result.Add(text);
                    break;
                case LocalInfoRequestParameterDto localInfoRequestParameterDto:
                    Debug.LogWarning("TODO Field : " + item.name + "(localInfo)");
                    break;
                default:
                    Debug.LogError("Field not recognized : " + item.name);
                    break;
            }
            to.answers.Add(requestDto);
        }
        return result;
    }
}
