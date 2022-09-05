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
using System.Runtime.InteropServices;
using System.Text;
using umi3d.baseBrowser.connection;
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

    #endregion

    #region UI


    LoadingBarElement loadingBar;
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

    protected override void InitUI()
    {
        root.Q<Label>("version").text = BrowserDesktop.BrowserVersion.Version;

        umiLogo = root.Q<VisualElement>("logo");
        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);

        loadingBar = new LoadingBarElement(root.Q<VisualElement>("lock"),
            (c) => { },
            ()=> { },
            () => { }
            );
        loadingBar.Text = "Connecting";
        loadingBar.Hide();

        homeScreen = new HomeScreen
            (
                root, 
                savedServerEntry,
                currentServer,
                savedServers,
                DisplayAdvConnectionScreen,
                DisplayLibManagerScreen,
                DisplayDialogueBox,
                _ResizeElements,
                ConnectHomeScreen
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
                currentServer,
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

        advancedConnectionScreen.Hide();
        sessionScreen.Hide();
        librariesManagerScreen.Hide();
        DisplayHomeScreen();

        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
    }

    private async void ConnectHomeScreen(bool value) {
        StartLoadingAnimation();
        await Connect(value);
        StopLoadingAnimation();
    }


    bool runAnimation = false;
    private async void StartLoadingAnimation()
    {
        runAnimation = true;
        loadingBar.Display();
        loadingBar.OnProgressChange(0);
        float val = 0;
        float delta = 0.05f;
        while (runAnimation)
        {
            await UMI3DAsyncManager.Delay(50);
            val += delta;
            if (val >= 1)
            {
                val = 0.99f;
                delta = -delta;
            }
            else if (val <= 0)
            {
                val = 0.01f;
                delta = -delta;
            }
            loadingBar.OnProgressChange(val);
        }
        loadingBar.Hide();
    }
    private void StopLoadingAnimation()
    {
        runAnimation = false;
    }


    /// <summary>
    /// Displays advanced connection screen.
    /// </summary>
    public override void DisplayHomeScreen()
    {
        homeScreen.Display();
        loadingBar.Hide();
        loadingBar.OnProgressChange(0);
        previousStep = null;
        nextStep = () => homeScreen.SetCurrentServerAndConnect();
    }

    /// <summary>
    /// Displays session screen
    /// </summary>
    public override void DisplaySessionScreen()
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
    public override void DisplayAdvConnectionScreen()
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
    public override void DisplayLibManagerScreen()
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
    public override void DisplayDialogueBox(string title, string message, string optionA, string optionB, System.Action<bool> choiceCallback)
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

        SetUpKeyboardConfiguration();
    }

    private void Update()
    {
        CheckShortcuts();
        if (ShouldDisplaySessionScreen)
        {
            homeScreen.Hide();
            DisplaySessionScreen();
            ShouldDisplaySessionScreen = false;
        }
    }
}
