using umi3d.baseBrowser.connection;
using UnityEngine.UIElements;
using umi3d.baseBrowser.preferences;

public class MainMenu : BaseMenu
{
    private HomeScreen m_NavigationScreen;

    protected override void Start()
    {
        m_NavigationScreen = new HomeScreen(m_UiDocument.rootVisualElement.Q("Navigation"));
        m_NavigationScreen.OnConnect += Connect;
        m_Screens.Add(m_NavigationScreen);
        m_MainScreen = m_NavigationScreen;

        base.Start();
    }

    private async void Connect(ServerPreferences.ServerData world)
    {
        BaseConnectionProcess.Instance.currentServer = world;
        BaseConnectionProcess.Instance.ConnectionInitializationFailled +=
            url => OpenErrorBox($"Browser was not able to connect to \n\n\"{url}\"");

        await BaseConnectionProcess.Instance.InitConnect(true);
    }
}
