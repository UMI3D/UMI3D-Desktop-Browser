using GLTFast.Schema;
using umi3d.baseBrowser.connection;
using UnityEngine;
using UnityEngine.UIElements;

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
    public void ToLogin() => ChangeState(_loginState);
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

    public void OpenErrorBox(string message)
    {
        _errorBox.Q<TextElement>("Message").text = message;
        _errorBox.RemoveFromClassList("hidden");
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
}
