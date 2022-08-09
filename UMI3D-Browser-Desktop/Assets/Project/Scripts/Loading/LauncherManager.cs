/*
Copyright 2019 - 2022 Inetum

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
using BrowserDesktop.Controller;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using umi3d.baseBrowser.preferences;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

public class LauncherManager : umi3d.baseBrowser.connection.BaseLauncher
{
    #region Keyboard
    [DllImport("user32.dll")]
    private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

    /// <summary>
    /// Sets up the inputs according to the user's keyboard layout.
    /// For now, if the keyboard is a 'fr-FR', go for an azerty configuration otherwise a qwerty config.
    /// </summary>
    void SetUpKeyboardConfiguration()
    {
        StringBuilder name = new StringBuilder(9);

        GetKeyboardLayoutName(name);

        string str = name.ToString();

        if (str == InputLayoutManager.FR_Fr_KeyboardLayout || str == InputLayoutManager.FR_Be_KeyboardLayout)
        {
            InputLayoutManager.SetCurrentInputLayout("AzertyLayout");
        }
        else
        {
            InputLayoutManager.SetCurrentInputLayout("QwertyLayout");
        }
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
    #endregion

    #region Data

    /// <summary>
    /// The action trigger when the key "enter" is pressed.
    /// </summary>
    private Action nextStep = null;
    /// <summary>
    /// The actin trigger when the key "esc" is pressed
    /// </summary>
    private Action previousStep = null;

    //Session Screen
    public bool ShouldDisplaySessionScreen = false;
    public bool updateResponse = false;
    public bool updateInfo = false;

    #endregion

    #region UI

    HomeScreen homeScreen;
    SessionScreen sessionScreen;
    AdvancedConnectionScreen advancedConnectionScreen;
    LibrariesManagerScreen librariesManagerScreen;

    //Element to be resized
    /// <summary>
    /// UMI3D logo to be resized
    /// </summary>
    VisualElement umiLogo;
    private float height;

    private void InitUI()
    {
        root.Q<Label>("version").text = BrowserDesktop.BrowserVersion.Version;

        umiLogo = root.Q<VisualElement>("logo");
        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);

        homeScreen = new HomeScreen
            (
                root, 
                savedServerEntry,
                currentServerConnectionData,
                DisplayAdvancedConnectionScreen,
                DisplayLibrariesManagerScreen,
                DisplayDialogueBox,
                _ResizeElements,
                Connect
            );
        advancedConnectionScreen = new AdvancedConnectionScreen
            (
                root,
                ref currentConnectionData,
                _ResizeElements,
                DisplayHomeScreen,
                StoreCurrentConnectionDataAndConnect
            );
        sessionScreen = new SessionScreen
            (
                root, 
                sessionEntry,
                masterServer,
                DisplayHomeScreen,
                advancedConnectionScreen.UpdataCurrentConnectionData,
                StoreCurrentConnectionDataAndConnect
            );
        librariesManagerScreen = new LibrariesManagerScreen
            (
                root, 
                libraryEntry,
                document,
                DisplayHomeScreen,
                DisplayDialogueBox
            );

        DisplayHomeScreen();
    }

    /// <summary>
    /// Displays advanced connection screen.
    /// </summary>
    public void DisplayHomeScreen()
    {
        homeScreen.Display();
        previousStep = null;
        nextStep = () => homeScreen.SetCurrentConnectionDataAndConnect();
    }

    /// <summary>
    /// Displays session screen
    /// </summary>
    public void DisplaySessionScreen()
    {
        sessionScreen.Display();
        previousStep = () =>
        {
            sessionScreen.Hide();
            DisplayHomeScreen();
        };
        nextStep = () => sessionScreen.Submit();
    }

    /// <summary>
    /// Displays advanced connection screen.
    /// </summary>
    public void DisplayAdvancedConnectionScreen()
    {
        advancedConnectionScreen.Display();
        previousStep = () =>
        {
            advancedConnectionScreen.Hide();
            DisplayHomeScreen();
        };
        nextStep = () =>
        {
            advancedConnectionScreen.UpdataCurrentConnectionData();
            StoreCurrentConnectionDataAndConnect();
        };
    }

    /// <summary>
    /// Displays Libraries manager screen.
    /// </summary>
    public void DisplayLibrariesManagerScreen()
    {
        librariesManagerScreen.Display();
        previousStep = () =>
        {
            librariesManagerScreen.Hide();
            DisplayHomeScreen();
        };
        nextStep = null;
    }

    /// <summary>
    /// Displays a dialogue box
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="optionA"></param>
    /// <param name="optionB"></param>
    /// <param name="choiceCallback"></param>
    public void DisplayDialogueBox(string title, string message, string optionA, string optionB, Action<bool> choiceCallback)
    {
        DialogueBox_E.Instance.Setup(title, message, optionA, optionB, choiceCallback);
        DialogueBox_E.Instance.DisplayFrom(document);
    }

    /// <summary>
    /// Resize some elements when the window is resized, to make the UI more responsive.
    /// </summary>
    /// <param name="e"></param>
    private void ResizeElements(GeometryChangedEvent e)
    {
        height = e.newRect.height * 0.16f;
        _ResizeElements();
    }

    private void _ResizeElements()
    {
        umiLogo.style.height = height;
        umiLogo.style.minHeight = height;

        umiLogo.style.marginBottom = height * 0.08f;
    }

    #endregion


    protected override void Start()
    {
        base.Start();

        currentServerConnectionData = new ServerPreferences.ServerData();
        currentConnectionData = new ServerPreferences.Data();
        
        SetUpKeyboardConfiguration();

        InitUI();
    }

    

    //private void InitUI()
    //{
    //    root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
    //    backMenuBnt.clickable.clicked += ResetLauncher;
    //}


    private void Update()
    {
        if (ShouldDisplaySessionScreen)
        {
            DisplaySessionScreen();
            ShouldDisplaySessionScreen = false;
        }
        if (updateInfo)
        {
            ServerPreferences.StoreRegisteredServerData(serverConnectionData);
            updateInfo = false;
        }

        CheckShortcuts();
    }

    

    /// <summary>
    /// Initiates the connection to the forge master server.
    /// </summary>
    private async void Connect(ServerPreferences.ServerData server, bool saveInfo = false) 
    {
        bool mediaDtoFound = false;
        bool masterServerFound = false;

        //1. Try to find a master server
        masterServer.ConnectToMasterServer(() =>
        {
            if (mediaDtoFound) return;

            masterServer.RequestInfo((name, icon) =>
            {
                if (mediaDtoFound) return;

                masterServerFound = true;

                if (saveInfo)
                {
                    server.serverName = name;
                    server.serverIcon = icon;
                    updateInfo = true;
                }
            });
            ShouldDisplaySessionScreen = true;
        }, server.serverUrl);

        var text = root.Q<Label>("connectedText");
        Debug.Log(text);
        root.Q<Label>("connectedText").text = "Connected to : " + server.serverUrl;


        //2. try to get a mediaDto
        var media = await ConnectionMenu.GetMediaDto(server);
        if (media == null || masterServerFound) return;
        mediaDtoFound = true;

        server.serverName = media.name;
        //To handle Properly.
        server.serverIcon = media?.icon2D?.variants?.FirstOrDefault()?.url;
        updateInfo = true;

        currentConnectionData.environmentName = media.name;
        currentConnectionData.ip = media.url;
        currentConnectionData.port = null;
        StoreCurrentConnectionDataAndConnect();
    }

    /// <summary>
    /// Store current connection data and initiates the connection to the environment.
    /// </summary>
    private void StoreCurrentConnectionDataAndConnect()
    {
        ServerPreferences.StoreUserData(currentConnectionData);
        StartCoroutine(WaitReady(currentConnectionData));
    }
}
