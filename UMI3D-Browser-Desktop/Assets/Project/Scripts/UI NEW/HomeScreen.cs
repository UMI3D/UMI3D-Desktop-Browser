using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using umi3d.baseBrowser.connection;
using umi3d.baseBrowser.preferences;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.ServerPreferences;

public class HomeScreen : BaseScreen
{
    private const DebugScope k_Const = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;
    private const string k_NavigationItemPath = "UI NEW/NavigationItem";

    private TextField m_PortalUrl;

    private Action m_ReloadPortal;

    public event Action<ServerData> OnConnect;

    public HomeScreen(VisualElement element) : base(element)
    {
        m_PortalUrl = m_Root.Q<TextField>("Url");
        m_PortalUrl.SetPlaceholderText("example.fr");
        m_Root.Q<Button>("ButtonSubmit").clicked += ConnectWithUrl;
        SetupPortals();
    }

    private void SetupPortals()
    {
        LoadLastPortals();

        m_Root.Q<RadioButton>("Last").RegisterValueChangedCallback(e =>
        {
            if (e.newValue)
                LoadLastPortals();
            else
                LoadFavoritePortals();
        });
    }

    private void LoadLastPortals()
    {
        LoadPortals(BaseConnectionProcess.Instance.savedServers.OrderBy(w => w.dateLastConnection).Reverse());
        m_ReloadPortal = LoadLastPortals;
    }
    private void LoadFavoritePortals()
    {
        LoadPortals(BaseConnectionProcess.Instance.savedServers.Where(w => w.isFavorite).OrderBy(w => w.dateLastConnection).Reverse());
        m_ReloadPortal = LoadFavoritePortals;
    }

    private void LoadPortals(IEnumerable<ServerData> pWorlds)
    {
        var elements = m_Root.Q("Elements");
        elements.Clear();
        foreach (var world in pWorlds)
        {
            elements.Add(GetItemElement(world, () => OnConnect?.Invoke(world)));
        }
    }

    public TemplateContainer GetItemElement(ServerData pWorld, Action pCallback)
    {
        var itemAsset = Resources.Load<VisualTreeAsset>(k_NavigationItemPath);
        var item = itemAsset.Instantiate();

        item.Q<TextElement>("Name").text = pWorld.serverName;

        if (pCallback != null)
        {
            item.Q<Button>("ButtonElement").clicked += () =>
            {
                if (!m_IsAButtonAlreadyPressed)
                {
                    ButtonActivated();
                    pCallback?.Invoke();
                }
            };
        }

        var buttons = item.Q("HoverButtons");
        item.RegisterCallback<MouseEnterEvent>(e =>
        {
            buttons.RemoveFromClassList("hidden");
        });
        item.RegisterCallback<MouseLeaveEvent>(e =>
        {
            buttons.AddToClassList("hidden");
        });

        buttons.Q<Button>("Favori").clicked += () =>
        {
            if (!m_IsAButtonAlreadyPressed)
            {
                ButtonActivated();
                pWorld.isFavorite = !pWorld.isFavorite;
                BaseConnectionProcess.Instance.StoreServer();
                m_ReloadPortal?.Invoke();
            }
        };

        buttons.Q<Button>("Delete").clicked += () =>
        {
            if (!m_IsAButtonAlreadyPressed)
            {
                ButtonActivated();
                BaseConnectionProcess.Instance.DeleteServer(pWorld);
                m_ReloadPortal?.Invoke();
            }
        };

        return item;
    }

    private void ConnectWithUrl()
    {
        SetCurrentServerAndConnect(m_PortalUrl.text);
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
        UMI3DLogger.LogWarning("No public IP found. This computer seems to be offline.", k_Const);
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