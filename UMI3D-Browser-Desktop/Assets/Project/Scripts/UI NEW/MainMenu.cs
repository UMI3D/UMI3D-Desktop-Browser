using System;
using System.Collections.Generic;
using umi3d.baseBrowser.connection;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = UnityEngine.UIElements.Button;
using Label = UnityEngine.UIElements.Label;
using inetum.unityUtils;

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

    public void GetParameterDtos(umi3d.common.interaction.form.Form form, Action<FormAnswerDto> callback)
    {
        Debug.Log("===== NEW FORM RECEIVED =====");
        if (form == null)
        {
            callback.Invoke(null);
            Debug.Log("Form is null");
            return;
        }
        Debug.Log("Form name : " + form.Name);

        FormAnswerDto answer = new FormAnswerDto()
        {
            boneType = 0,
            hoveredObjectId = 0,
            id = form.Id,
            toolId = 0,
            answers = new List<ParameterSettingRequestDto>()
        };

        ToLogin(GetVisualElements(form, answer), () => callback?.Invoke(answer));
    }

    private List<VisualElement> GetVisualElements(umi3d.common.interaction.form.Form form, FormAnswerDto to)
    {
        var result = new List<VisualElement>();
        foreach (var page in form.Pages)
        {
            var pageView = new VisualElement() { name = page.Name };
            Debug.Log("== Page : " + page.Name);
            pageView.Add(GroupToVisualElement(page.Group));

            result.Add(pageView);
        }
        return result;
    }

    private VisualElement GroupToVisualElement(Group group)
    {
        var result = new VisualElement();
        if (group == null) return new VisualElement() { name = "Group null" };
        var children = group.Children;
        if (children == null) return new VisualElement() { name = "Group Empty" };

        foreach (var div in group.Children)
        {
            // Label
            var label = div as umi3d.common.interaction.form.Label;
            if (label != null)
            {
                result.Add(new Label(label.Text));
                continue;
            }

            // Group
            var childGroup = div as Group;
            if (childGroup != null)
                result.Add(GroupToVisualElement(childGroup));

            // Inputs
            switch (div)
            {
                case umi3d.common.interaction.form.Text text:
                    var textElement = new TextField(text.Label);
                    textElement.value = text.Value;
                    textElement.SetPlaceholderText(text.PlaceHolder);
                    textElement.isPasswordField = text.Type == TextType.Password;
                    result.Add(textElement);
                    break;
                case umi3d.common.interaction.form.Button button:
                    var buttonElement = new Button();
                    buttonElement.text = button.Label;
                    result.Add(buttonElement);
                    break;
                case umi3d.common.interaction.form.Range<int> rangeInt:
                    break;
                case umi3d.common.interaction.form.Range<float> rangeFloat:
                    break;
                default:
                    break;
            }
        }

        return result;
    }
}
