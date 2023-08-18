using inetum.unityUtils;
using System;
using System.Linq;
using System.Net;
using umi3d.baseBrowser.connection;
using umi3d.baseBrowser.preferences;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeScreen : BaseScreen
{
    private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;
    private const string k_navigationItemPath = "UI NEW/NavigationItem";

    private TextField _portalUrl;
    public event Action<ServerPreferences.ServerData> OnConnect;

    public HomeScreen(VisualElement element) : base(element)
    {
        _portalUrl = _root.Q<TextField>("Url");
        _portalUrl.SetPlaceholderText("example.fr");
        _root.Q<Button>("ButtonSubmit").clicked += ConnectWithUrl;

        var elements = _root.Q("Elements");
        var worlds = BaseConnectionProcess.Instance.savedServers.OrderBy(w => w.dateLastConnection).Reverse();
        elements.Clear();
        foreach (var world in worlds)
        {
            elements.Add(GetItemElement(world.serverName, () => OnConnect?.Invoke(world)));
        }
    }

    public TemplateContainer GetItemElement(string name, Action callback)
    {
        var itemAsset = Resources.Load<VisualTreeAsset>(k_navigationItemPath);
        var item = itemAsset.Instantiate();

        item.Q<TextElement>("Name").text = name;

        if (callback != null)
            item.Q<Button>("ButtonElement").clicked += callback;

        return item;
    }

    private void ConnectWithUrl()
    {
        SetCurrentServerAndConnect(_portalUrl.text);
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
        else OnConnect?.Invoke(data);
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