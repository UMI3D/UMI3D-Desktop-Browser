/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using BeardedManStudios.Forge.Networking;
using BrowserDesktop.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using umi3d.baseBrowser.preferences;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common.collaboration;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LauncherManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern long GetKeyboardLayoutName(
      StringBuilder pwszKLID);

    #region Fields

    #region UI Fields

    [SerializeField]
    private UIDocument uiDocument = null;

    [SerializeField]
    private VisualTreeAsset libraryEntryTreeAsset = null;
    [SerializeField]
    private VisualTreeAsset sessionEntry = null;

    private VisualElement root;

    //Element to be resized
    /// <summary>
    /// UMI3D logo to be resized
    /// </summary>
    VisualElement umiLogo;
    private float height;

    //SetDomain screen
    VisualElement urlScreen;
    TextField urlInput;
    /// <summary>
    /// Arrow button to connect to the new server.
    /// </summary>
    Button urlEnterBtn;
    /// <summary>
    /// Dropdown button to display or hide the new connexion url.
    /// </summary>
    Button connectNewServBtn;

    //Session screen
    VisualElement sessionScreen;
    ScrollView sessionList;

    //Advanced Connection screen
    VisualElement advancedConnectionScreen;
    TextField PortInput;
    TextField IpInput;

    //Asset libraries screen
    VisualElement librariesScreen;
    ScrollView librariesList;
    Button backMenuBnt;
    Button nextMenuBnt;

    //Saved Servers slider
    [SerializeField]
    private VisualTreeAsset SavedServerItemTreeAsset;
    SliderElement savedServersSlider;

    #endregion

    #region Data

    private ServerPreferences.ServerData currentServerConnectionData;
    private List<ServerPreferences.ServerData> serverConnectionData = new List<ServerPreferences.ServerData>();
    
    private ServerPreferences.Data currentConnectionData;
    private List<ServerPreferences.Data> connectionData = new List<ServerPreferences.Data>();

    [SerializeField]
    public string currentScene;
    [SerializeField]
    public string sceneToLoad;

    /// <summary>
    /// The action trigger when the enter key is pressed.
    /// </summary>
    private Action nextStep = null;
    private Action previousStep = null;
    /// <summary>
    /// The action to be assigned to the nextMenuBtn.
    /// </summary>
    private Action currentNextButtonAction = null;

    public LaucherOnMasterServer masterServer;

    //Session Screen
    /// <summary>
    /// The item selected by a click with the mouse.
    /// </summary>
    private VisualElement selectedItem = null;
    public bool ShouldDisplaySessionScreen = false;
    public bool updateResponse = false;
    public bool updateInfo = false;
    public List<MasterServerResponse.Server> serverResponses = new List<MasterServerResponse.Server>();

    #endregion

    #endregion

    void Start()
    {
        masterServer = new LaucherOnMasterServer();

        Debug.Assert(uiDocument != null);
        Debug.Assert(libraryEntryTreeAsset != null);
        root = uiDocument.rootVisualElement;

        SetUpKeyboardConfiguration();

        InitUI();
   
        ResetLauncher();
    }

    /// <summary>
    /// Sets up the inputs according to the user's keyboard layout.
    /// For now, if the keyboard is a 'fr-FR', go for an azerty configuration otherwise a qwerty config.
    /// </summary>
    void SetUpKeyboardConfiguration()
    {
        StringBuilder name = new StringBuilder(9);

        GetKeyboardLayoutName(name);

        string str = name.ToString();
        
        if(str == InputLayoutManager.FR_Fr_KeyboardLayout || str == InputLayoutManager.FR_Be_KeyboardLayout)
        {
            InputLayoutManager.SetCurrentInputLayout("AzertyLayout");
        } else
        {
            InputLayoutManager.SetCurrentInputLayout("QwertyLayout");
        }
    }

    #region UI Binding and Displaying

    private void InitUI()
    {
        root.Q<Label>("version").text = BrowserDesktop.BrowserVersion.Version;

        BindSharedUI();

        BindURLScreen();
        BindSessionSreen();
        BindAdvancedConnection();
        BindLibrariesScreen();

        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
    }

    #region Binding

    /// <summary>
    /// To bind UI shared among windows
    /// </summary>
    private void BindSharedUI()
    {
        urlScreen = root.Q<VisualElement>("url-screen");
        sessionScreen = root.Q<VisualElement>("sessionScreen");
        advancedConnectionScreen = root.Q<VisualElement>("advancedConnectionScreen");
        librariesScreen = root.Q<VisualElement>("libraries-manager-screen");

        backMenuBnt = root.Q<Button>("back-menu-btn");
        nextMenuBnt = root.Q<Button>("nextMenuBtn");

        backMenuBnt.clickable.clicked += ResetLauncher;
    }

    private void BindURLScreen()
    {
        urlInput = urlScreen.Q<TextField>("url-input");
        urlEnterBtn = urlScreen.Q<Button>("url-enter-btn");

        urlEnterBtn.clickable.clicked += ()=> SetServer(urlInput.value);// SetDomain;

        urlScreen.Q<VisualElement>("icon-open").style.display = DisplayStyle.None;
        connectNewServBtn = urlScreen.Q<Button>("newConnection");
        connectNewServBtn.clickable.clicked += () => ToggleDisplayElement(urlScreen.Q<VisualElement>("inputs-url-container"));
        connectNewServBtn.clickable.clicked += () => ToggleDisplayElement(urlScreen.Q<VisualElement>("icon-close"));
        connectNewServBtn.clickable.clicked += () => ToggleDisplayElement(urlScreen.Q<VisualElement>("icon-open"));

        urlScreen.Q<Button>("advanced-connection-btn").clickable.clicked += DisplayAdvancedConnection;
        urlScreen.Q<Button>("manage-library-btn").clickable.clicked += DisplayLibraries;

        savedServersSlider = new SliderElement();
        savedServersSlider.SetUp(urlScreen.Q<VisualElement>("slider"));
    }

    private void BindSessionSreen()
    {
        sessionList = sessionScreen.Q<ScrollView>("sessionsList");
        sessionScreen.Q<Button>("pin-enter-btn").clickable.clicked += ()=> masterServer.SendDataSession(sessionScreen.Q<TextField>("pinInput").value,
                                                                                                         (ser) => { serverResponses.Add(ser); 
                                                                                                                    updateResponse = true; });
    }

    private void BindLibrariesScreen()
    {
        librariesList = librariesScreen.Q<ScrollView>("libraries-list");
    }

    private void BindAdvancedConnection()
    {
        IpInput = advancedConnectionScreen.Q<TextField>("IpInput");
        PortInput = advancedConnectionScreen.Q<TextField>("PortInput");
    }

    #endregion

    #region Display Screens

    /// <summary>
    /// Reset the display of the launcher. Display the url screen
    /// </summary>
    private void ResetLauncher()
    {
        umiLogo = urlScreen.Q<VisualElement>("logo");
        ResizeLogo();

        //Display and hide UI
        backMenuBnt.style.display = DisplayStyle.None;
        nextMenuBnt.style.display = DisplayStyle.None;
        urlScreen.style.display = DisplayStyle.Flex;
        sessionScreen.style.display = DisplayStyle.None;
        advancedConnectionScreen.style.display = DisplayStyle.None;
        librariesScreen.style.display = DisplayStyle.None;

        currentConnectionData = ServerPreferences.GetPreviousConnectionData();
        serverConnectionData = ServerPreferences.GetRegisteredServerData();
        DisplayRegisteredServers();
        currentServerConnectionData = ServerPreferences.GetPreviousServerData();

        previousStep = null;
        nextStep = () => SetServer(urlInput.value);
        urlInput.value = currentServerConnectionData.serverName; // currentConnectionData.ip;
    }

    /// <summary>
    /// Display the advanced connection screen and hide the other screens.
    /// </summary>
    private void DisplayAdvancedConnection()
    {
        umiLogo = advancedConnectionScreen.Q<VisualElement>("logo");
        ResizeLogo();

        //Display or hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.Flex;
        urlScreen.style.display = DisplayStyle.None;
        sessionScreen.style.display = DisplayStyle.None;
        advancedConnectionScreen.style.display = DisplayStyle.Flex;

        //Update nextMenuBnt action and nextStep and previousStep
        if (currentNextButtonAction != null)
        {
            nextMenuBnt.clickable.clicked -= currentNextButtonAction;
        }
        currentNextButtonAction = ()=> SetDomain();
        nextMenuBnt.clickable.clicked += currentNextButtonAction;
        nextStep = currentNextButtonAction;
        previousStep = ResetLauncher;

        //Update Ip and Port input
        IpInput.value = currentConnectionData?.ip ?? "localhost";
        PortInput.value = currentConnectionData?.port ?? "";
    }

    /// <summary>
    /// Displays the libraries install on users' computers and allows tehm to unistall these libs.
    /// </summary>
    private void DisplayLibraries()
    {
        //Display and hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.None;
        urlScreen.style.display = DisplayStyle.None;
        librariesScreen.style.display = DisplayStyle.Flex;

        nextStep = null;
        previousStep = ResetLauncher;

        librariesList.Clear();

        Dictionary<string, List<UMI3DResourcesManager.DataFile>> libs = new Dictionary<string, List<UMI3DResourcesManager.DataFile>>();
        foreach (var lib in UMI3DResourcesManager.Libraries)
        {
            if (lib.applications != null)
                foreach (var app in lib.applications)
                {
                    if (!libs.ContainsKey(app)) libs[app] = new List<UMI3DResourcesManager.DataFile>();
                    libs[app].Add(lib);
                }
        }

        foreach (var app in libs)
        {
            foreach (var lib in app.Value)
            {
                // 1. Diplay lib name
                var entry = libraryEntryTreeAsset.CloneTree();
                entry.Q<Label>("library-name").text = lib.key;

                //2. Display environments which use this lib
                var dropdown = entry.Q<DropdownElement>();
                dropdown.SetUp(uiDocument, "dropdown-label-medium");
                dropdown.SetOptions(lib.applications);

                //3. Display lib size
                DirectoryInfo dirInfo = new DirectoryInfo(lib.path);
                double dirSize = DirSize(dirInfo) / Mathf.Pow(10, 6);
                dirSize = Math.Round(dirSize, 2);
                entry.Q<Label>("library-size").text = dirSize.ToString() + " mo"; ;


                //4.Bind the button to unistall this lib
                entry.Q<Button>("library-unistall").clickable.clicked += () =>
                {
                    DialogueBox_E.Instance.Setup("Are you sure ... ?", "This library is required for " + app.Key + " environment", "YES", "NO", (b) =>
                        {
                            if (b)
                            {
                                lib.applications.Remove(app.Key);
                                UMI3DResourcesManager.RemoveLibrary(lib.key);
                                DisplayLibraries();
                            }
                        });
                    DialogueBox_E.Instance.DisplayFrom(uiDocument);
                };
                librariesList.Add(entry);
            }
        }
    }

    /// <summary>
    /// Display the sessions screen and hide the other screes.
    /// </summary>
    private void DisplaySessionScreen()
    {
        //Display or hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.None;
        urlScreen.style.display = DisplayStyle.None;
        advancedConnectionScreen.style.display = DisplayStyle.None;
        sessionScreen.style.display = DisplayStyle.Flex;

        nextStep = ()=> masterServer.SendDataSession(sessionScreen.Q<TextField>("pinInput").value,
                                                     (ser) => { serverResponses.Add(ser); 
                                                                updateResponse = true; });
    }

    #endregion

    private void UpdateSessionList()
    {
        sessionList.Clear();

        foreach (MasterServerResponse.Server session in serverResponses)
        {
            VisualElement item = sessionEntry.CloneTree().Q<VisualElement>("session-entry");
            sessionList.Add(item);
            item.Q<Label>("server-name").text = session.Name;
            item.Q<Label>("users-count").text = session.PlayerCount.ToString();

            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 1)
                {
                    SelectSession(item, session.Address, session.Port);
                }
            });

            item.RegisterCallback<MouseEnterEvent>(e =>
            {
                if (!item.ClassListContains("orange-background"))
                    foreach (var label in item.Q<VisualElement>("server-entry-btn").Children())
                    {

                        label.AddToClassList("orange-text");
                    }
            }
            );
            item.RegisterCallback<MouseLeaveEvent>(e =>
            {
                foreach (var label in item.Q<VisualElement>("server-entry-btn").Children())
                {
                    label.RemoveFromClassList("orange-text");
                }
            }
           );
        }

        serverResponses.Clear();
    }
    
    private void SelectSession(VisualElement itemSelected, string ip, ushort port)
    {
        //TODO color the element
        if(selectedItem != null)
        {
            selectedItem.RemoveFromClassList("orange-background");
            selectedItem.RemoveFromClassList("black-txt");
            foreach (var label in selectedItem.Q<VisualElement>("server-entry-btn").Children())
            {
                label.AddToClassList("orange.text");
                label.RemoveFromClassList("black-txt");
            }
        }

        //Display and set nextMenuBnt action
        string ip_port = ip + ":" + port.ToString();
        if (currentNextButtonAction != null)
        {
            nextMenuBnt.clickable.clicked -= currentNextButtonAction;
        }
        currentNextButtonAction = ()=> SetDomain(ip_port);
        nextMenuBnt.clickable.clicked += currentNextButtonAction;
        nextMenuBnt.style.display = DisplayStyle.Flex;
        

        //Color
        itemSelected.AddToClassList("orange-background");
        itemSelected.AddToClassList("black-txt");
        foreach (var label in itemSelected.Q<VisualElement>("server-entry-btn").Children())
        {
            label.RemoveFromClassList("orange.text");
            label.AddToClassList("black-txt");
        }
        selectedItem = itemSelected;
        //this.currentConnectionData.ip = env.ip;
        //DirectConnect();
    }

    #endregion

    #region Action

    private void Update()
    {
        if (ShouldDisplaySessionScreen)
        {
            DisplaySessionScreen();
            ShouldDisplaySessionScreen = false;
        }
        if (updateResponse)
        {
            UpdateSessionList();
            updateResponse = false;
        }
        if (updateInfo)
        {
            ServerPreferences.StoreRegisteredServerData(serverConnectionData);
            updateInfo = false;
        }

        CheckShortcuts();
    }

    /// <summary>
    /// Toggle display element between DisplayStyle.Flex and DisplayStyle.None
    /// </summary>
    /// <param name="visualElement"></param>
    private void ToggleDisplayElement(VisualElement visualElement)
    {
        visualElement.style.display = DisplayStyle.Flex == visualElement.style.display.value ? DisplayStyle.None : DisplayStyle.Flex;
    }

    /// <summary>
    /// Allows users to use escape and return keys to navigate through the launcher.
    /// </summary>
    private void CheckShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !DialogueBox_E.Instance.IsDisplaying)
            nextStep?.Invoke();
        else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)) && !DialogueBox_E.Instance.IsDisplaying)
            previousStep?.Invoke();
    }

    /// <summary>
    /// Gets the url and port written by users and stores them.
    /// </summary>
    private void SetDomain(string ip = "", string port = "")
    {
        ip = string.IsNullOrEmpty(ip) ? IpInput.value.Trim() : ip;
        port = string.IsNullOrEmpty(port) ? PortInput.value.Trim() : port;
        //string url = urlInput.value;

        if (string.IsNullOrEmpty(ip))
        {
            //urlScreen.Q<Label>("url-error").style.display = DisplayStyle.Flex;
            //urlScreen.Q<Label>("url-error").text = "The domain is empty.";
        } else
        {
            currentConnectionData.ip = ip;
            currentConnectionData.port = port;

            urlScreen.style.display = DisplayStyle.None;
            previousStep = ResetLauncher;
            DirectConnect();
        }
    }

    /// <summary>
    /// Initiates the connection to the environment.
    /// </summary>
    private void DirectConnect()
    {
        //currentConnectionData.environmentName
        ServerPreferences.StoreUserData(currentConnectionData);
      
        StartCoroutine(WaitReady(currentConnectionData));
    }


    private void SetServer(string serverUrl)
    {
        if (string.IsNullOrEmpty(serverUrl)) return;
        serverUrl = serverUrl.Trim();

        if (root.Q<Toggle>("toggleRemember").value)
        {
            if (currentServerConnectionData != null)
            {
                currentServerConnectionData.serverUrl = serverUrl;
                currentServerConnectionData.serverName = null;
                currentServerConnectionData.serverIcon = null;
            }
            else
                currentServerConnectionData = new ServerPreferences.ServerData() { serverUrl = serverUrl };
            serverConnectionData.Add(currentServerConnectionData);
            ServerPreferences.AddRegisterdeServerData(currentServerConnectionData);
            Connect(currentServerConnectionData,true);
        }
        else
            Connect(new ServerPreferences.ServerData() { serverUrl = serverUrl });
    }

    /// <summary>
    /// Initiates the connection to the forge master server.
    /// </summary>
    private async void Connect(ServerPreferences.ServerData server, bool saveInfo = false) 
    {
        var media = await ConnectionMenu.GetMediaDto(server);
        if (media != null)
        {
            ServerPreferences.Data data = new ServerPreferences.Data();

            server.serverName = media.name;
            //To handle Properly.
            server.serverIcon = media?.icon2D?.variants?.FirstOrDefault()?.url;
            updateInfo = true;
            data.environmentName = media.name;
            data.ip = media.url;
            data.port = null;

            StartCoroutine(WaitReady(data));
        }
        else
        {
            masterServer.ConnectToMasterServer(() =>
            {
                masterServer.RequestInfo((name, icon) =>
                {
                    if (saveInfo)
                    {
                        server.serverName = name;
                        server.serverIcon = icon;
                        updateInfo = true;
                    }
                });
                ShouldDisplaySessionScreen = true;
            }
                , server.serverUrl);
            var text = root.Q<Label>("connectedText");
            Debug.Log(text);
            root.Q<Label>("connectedText").text = "Connected to : " + server.serverUrl;
        }
    }

    /// <summary>
    /// Load the environment scene when it is ready.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitReady(ServerPreferences.Data data)
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        while (!ConnectionMenu.Exists)
            yield return new WaitForEndOfFrame();
        ConnectionMenu.Instance.Connect(data);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    private void DisplayRegisteredServers()
    {
        savedServersSlider.ClearItems();
        bool isEmpty = true;
        foreach (ServerPreferences.ServerData env in serverConnectionData)
        {
            isEmpty = false;
            var item = SavedServerItemTreeAsset.CloneTree().Q<VisualElement>("saved-server-item");
            if (env.serverIcon != null) {
                byte[] imageBytes = Convert.FromBase64String(env.serverIcon);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                item.style.backgroundImage = tex;
             }
            item.Q<Label>().text = env.serverName == null ? env.serverUrl : env.serverName;
            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 1)
                {
                    this.currentServerConnectionData.serverName = env.serverName;
                    this.currentServerConnectionData.serverUrl = env.serverUrl;
                    this.currentServerConnectionData.serverIcon = env.serverIcon;
                    //this.currentConnectionData.ip = env.ip;
                    //DirectConnect();// TODO
                    Connect(env,true);
                }
            });
            item.Q<Button>("delete-item").clickable.clicked += () =>
            {
                DialogueBox_E.Instance.Setup(env.serverName, "Delete this server from registered ?", "YES", "NO", (b) =>
                    {
                        if (b)
                        {
                            serverConnectionData.Remove(serverConnectionData.Find(d => d.serverName == env.serverName));
                            ServerPreferences.StoreRegisteredServerData(serverConnectionData);
                            savedServersSlider.RemoveElement(item);
                        }
                    });
                DialogueBox_E.Instance.DisplayFrom(uiDocument);
            };
            savedServersSlider.AddElement(item);
        }
        if (isEmpty)
        {
            root.Q<VisualElement>("saved-servers").style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Returns the size of a directory in bytes.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }
        // Add subdirectory sizes.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }
        return size;
    }

    /// <summary>
    /// Resize some elements when the window is resized, to make the UI more responsive.
    /// </summary>
    /// <param name="e"></param>
    private void ResizeElements(GeometryChangedEvent e)
    {
        height = e.newRect.height * 0.16f;
        ResizeLogo();
    }

    private void ResizeLogo()
    {
        umiLogo.style.height = height;
        umiLogo.style.minHeight = height;

        umiLogo.style.marginBottom = height * 0.08f;
    }

    #endregion
}
