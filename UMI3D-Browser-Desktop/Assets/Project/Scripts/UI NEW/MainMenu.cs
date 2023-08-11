using umi3d.baseBrowser.connection;
using UnityEngine;
using UnityEngine.UIElements;
using umi3d.baseBrowser.preferences;

public class MainMenu : BaseMenu
{
    private HomeScreen _navigationScreen;

    protected override void Start()
    {
        base.Start();

        _navigationScreen = new HomeScreen(_uiDocument.rootVisualElement.Q("Navigation"));
        _navigationScreen.OnConnect += Connect;
    }

    private async void Connect(ServerPreferences.ServerData world)
    {
        BaseConnectionProcess.Instance.currentServer = world;
        BaseConnectionProcess.Instance.ConnectionInitializationFailled +=
            url => OpenErrorBox($"Browser was not able to connect to \n\n\"{url}\"");

        await BaseConnectionProcess.Instance.InitConnect(true);
    }
}
