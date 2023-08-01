using System.Security.Policy;
using umi3d.baseBrowser.connection;
using UnityEngine;

public class HomeState : MenuState
{
    public HomeState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.IsHome = true;

        _machine.NavigationScreen.Next.clicked += _machine.ToLogin;

        var worlds = BaseConnectionProcess.Instance.savedServers;
        _machine.NavigationScreen.Elements.Clear();
        foreach (var world in worlds)
        {
            _machine.NavigationScreen.AddElement(world.serverName, async () =>
            {
                BaseConnectionProcess.Instance.currentServer = world;
                BaseConnectionProcess.Instance.ConnectionSucces += e => _machine.ToLogin();
                BaseConnectionProcess.Instance.ConnectionInitializationFailled += 
                    url => _machine.OpenErrorBox($"Browser was not able to connect to \n\n\"{url}\"");

                await BaseConnectionProcess.Instance.InitConnect(true);
            });
        }
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Next.clicked -= _machine.ToLogin;
    }
}