using System;
using umi3d.baseBrowser.connection;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;
using umi3d.baseBrowser.preferences;
using UnityEngine.SceneManagement;
using inetum.unityUtils;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private HomeScreen _navigationScreen;
    private FormScreen _connectionScreen;
    private VisualElement _errorBox;

    private void Start()
    {
        Debug.Assert(_uiDocument != null);

        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        _uiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        InitUiElements();
        InitLocalisation();
    }

    private void InitUiElements()
    {
        _navigationScreen = new HomeScreen(_uiDocument.rootVisualElement.Q("Navigation"));
        _navigationScreen.OnConnect += Connect;
        _navigationScreen.Show();

        _connectionScreen = new FormScreen(_uiDocument.rootVisualElement.Q("Connection"));
        _connectionScreen.Hide();

        _errorBox = _uiDocument.rootVisualElement.Q("ErrorBox");
        _errorBox.Q<Button>("ButtonOk").clicked += () => _errorBox.AddToClassList("hidden");
        _errorBox.AddToClassList("hidden");
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

    private async void Connect(ServerPreferences.ServerData world)
    {
        BaseConnectionProcess.Instance.currentServer = world;
        BaseConnectionProcess.Instance.ConnectionSucces += e => BaseConnectionProcess.Instance.GetParameterDtos += GetParameterDtos;
        BaseConnectionProcess.Instance.ConnectionInitializationFailled +=
            url => OpenErrorBox($"Browser was not able to connect to \n\n\"{url}\"");

        BaseConnectionProcess.Instance.LoadingLauncher += (media) =>
        {
            Debug.Log("LOADING");
            SceneManager.LoadScene("Environment");
        };

        await BaseConnectionProcess.Instance.InitConnect(true);
    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto form, Action<FormAnswerDto> callback)
    {
        _connectionScreen.GetParameterDtos(form, callback);
        _navigationScreen.Hide();
        _connectionScreen.Show();
    }
}
