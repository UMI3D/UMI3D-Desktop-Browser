using System.Net;
using System;
using umi3d.baseBrowser.connection;
using umi3d.baseBrowser.preferences;
using umi3d.common;
using UnityEngine;

public class HomeState : MenuState
{
    private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;

    public HomeState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.IsHome = true;

        _machine.NavigationScreen.Next.clicked += ConnectWithUrl;

        var worlds = BaseConnectionProcess.Instance.savedServers;
        _machine.NavigationScreen.Elements.Clear();
        foreach (var world in worlds)
        {
            _machine.NavigationScreen.AddElement(world.serverName, () => Connect(world));
        }
    }

    private void ConnectWithUrl()
    {
        SetCurrentServerAndConnect(_machine.NavigationScreen.PortalUrl.text);
    }

    private async void Connect(ServerPreferences.ServerData world)
    {
        BaseConnectionProcess.Instance.currentServer = world;
        BaseConnectionProcess.Instance.ConnectionSucces += e => BaseConnectionProcess.Instance.GetParameterDtos += _machine.GetParameterDtos;
        BaseConnectionProcess.Instance.ConnectionInitializationFailled +=
            url => _machine.OpenErrorBox($"Browser was not able to connect to \n\n\"{url}\"");

        await BaseConnectionProcess.Instance.InitConnect(true);
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Next.clicked -= ConnectWithUrl;
    }
    public void SetCurrentServerAndConnect(string url)
    {
        string serverUrl = url;
        if (string.IsNullOrEmpty(serverUrl)) return;

        // Get local address if localhost is enterd
        serverUrl.Replace("localhost", GetLocalIPAddress());

        TryToConnect(new ServerPreferences.ServerData { serverUrl = serverUrl.Trim() });
    }

    protected void TryToConnect(ServerPreferences.ServerData data)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && data.serverUrl.StartsWith(GetLocalIPAddress()))
        {
            Debug.Log("TODO: NO INTERNET OR LOCAL CASE !");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            Debug.Log("TODO: INTERNETREACHABILITY CASE !");
        }
        else Connect(data);
    }

    private static string GetLocalIPAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ip.ToString().EndsWith(".1"))
            {
                return ip.ToString();
            }
        }
        //if offline. 
        UMI3DLogger.LogWarning("No public IP found. This computer seems to be offline.", scope);
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }
}