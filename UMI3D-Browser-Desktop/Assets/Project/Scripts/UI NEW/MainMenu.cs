using umi3d.baseBrowser.connection;
using UnityEngine.UIElements;
using umi3d.baseBrowser.preferences;
using System.Security.Policy;
using System;

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

        BaseConnectionProcess.Instance.ConnectionInitialized += ConnectionInitilized;
        BaseConnectionProcess.Instance.ConnectionInitializationFailled += ConnectionFailed;
    }

    private async void Connect(ServerPreferences.ServerData pWorld)
    {
        BaseConnectionProcess.Instance.currentServer = pWorld;

        await BaseConnectionProcess.Instance.InitConnect(true);
    }

    private void ConnectionInitilized(string pUrl)
    {
        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () =>
        {
            CloseInfoBox();
            BaseConnectionProcess.Instance.Leave();
        };
        OpenInfoBox("Connection to a server",
            $"Try connecting to \n\n\"{pUrl}\" \n\nIt may take some time.",
            cancelButton);
    }

    private void ConnectionFailed(string pUrl)
    {
        OpenErrorBox("Failled to connect to server",
            $"Browser was not able to connect to \n\n\"{pUrl}\".");
    }
}
