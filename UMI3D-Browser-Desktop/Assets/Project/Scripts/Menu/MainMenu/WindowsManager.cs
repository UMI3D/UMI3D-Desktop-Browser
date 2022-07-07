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

using inetum.unityUtils;
using BrowserDesktop.Menu;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using umi3dDesktopBrowser.ui.viewController;
using BrowserDesktop.Cursor;

/// <summary>
/// This class removes the default Windows title bar and set up a custom one.
/// </summary>
public class WindowsManager : MonoBehaviour
{
    #region Fields

    public UIDocument uiDocument;

    private bool isZoomed = false;
    private bool isFullScreen = false;
    private int widthWindow = Screen.width / 2;
    private int heightWindow = Screen.height / 2;

    #region Fiels to make the title bar working like the windows one.

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool IsZoomed(IntPtr hWnd);

    IntPtr hWnd;


    [Header("Custom title bar")]

    [Tooltip("Tag name of the minimize window button UXML element")]
    [SerializeField]
    private readonly string minimizeTagName = "minimize-window-btn";
    [Tooltip("Tag name of the maximize window button UXML element")]
    [SerializeField]
    private readonly string maximizeTagName = "fullscreen-btn";
    [Tooltip("Tag name of the close window button UXML element")]
    [SerializeField]
    private readonly string closeTagName = "close-window-btn";

    VisualElement root;
    Button minimize_B;
    Button maximize_B;
    Button close_B;

    #endregion

    #endregion

    #region Methods

    #region LifeCycle monoBehaviour

    void Start()
    {
        Debug.Assert(uiDocument != null);
        SetUpCustomTitleBar();

        hWnd = GetActiveWindow();
        isZoomed = IsZoomed(hWnd);
        isFullScreen = Screen.fullScreen;
        UpdateWindowWhenResize();

        Application.wantsToQuit += WantsToQuit;
        QuittingManager.ShouldWaitForApplicationToQuit = true;
        DialogueBox_E
            .SetCursorMovementActions
            (
                    (o) => { umi3d.baseBrowser.Controller.BaseCursor.SetMovement(o, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free); },
                    (o) => { umi3d.baseBrowser.Controller.BaseCursor.UnSetMovement(o); }
            );
    }

    /// <summary>
    /// Bind the UI of the title bar.
    /// </summary>
    private void SetUpCustomTitleBar()
    {
        root = uiDocument.rootVisualElement;

        minimize_B = root.Q<Button>(minimizeTagName);
        minimize_B.clickable.clicked += () =>
        {
            ShowWindow(hWnd, 2);
        };

        maximize_B = uiDocument.rootVisualElement.Q<Button>(maximizeTagName);
        maximize_B.clickable.clicked += () =>
        {
            SwitchFullScreen(false);
        };

        close_B = root.Q<Button>(closeTagName);
        close_B.clickable.clicked += () =>
        {
            //This will raise the Application.WantsToQuit event and show a dialogue box.
            Application.Quit();
        };
    }

    private void OnDestroy()
    {
        Application.wantsToQuit -= WantsToQuit;
    }

    void Update()
    {
        CheckForWindowResizement();
    }

    #endregion

    #region Application Quit

    /// <summary>
    /// Show dialogue box to quit when ApplicationIsQuitting is not ready to quit.
    /// </summary>
    /// <returns>True when ApplicationIsQuitting is ready to quit, else false.</returns>
    private bool WantsToQuit()
    {
        bool wantsToQuit = QuittingManager.ApplicationIsQuitting;
        if (!wantsToQuit && !DialogueBox_E.Instance.IsDisplaying)
            ShowDialogueBoxToQuit();
        return wantsToQuit;
    }

    /// <summary>
    /// Show dialogue box to quit when the user whant to quit.
    /// </summary>
    private void ShowDialogueBoxToQuit()
    {
        DialogueBox_E.Instance.Setup("Close application", "Are you sure ...?", "YES", "NO", (b) => 
        { 
            QuittingManager.ApplicationIsQuitting = b;
            if (b) 
                Application.Quit(); 
        });
        DialogueBox_E.Instance.DisplayFrom(uiDocument);
    }

    #endregion

    #region Window resizement

    /// <summary>
    /// Check if the widow is being zoomed or unzoomed, in fullscreen or not.
    /// </summary>
    private void CheckForWindowResizement()
    {
        if ((IsZoomed(hWnd) && !isZoomed) ||
            (!IsZoomed(hWnd) && isZoomed)) //Check if the window is being resized (zoomed or unzoomed)
        {
            isZoomed = IsZoomed(hWnd);
            if (isZoomed)
            {
                SwitchFullScreen(true);
            }
        }
        else //The window has been resized with a shortcut
        {
            if (Screen.fullScreen && !isFullScreen) //Check if the window is being resized without being zoomed
            {
                ShowWindow(hWnd, 3);
                SwitchFullScreen(true);
            }

            else if (!Screen.fullScreen && isFullScreen)
            {
                SwitchFullScreen(false);
            }
        }

        if (!isFullScreen && !isZoomed)
        {
            widthWindow = Screen.width;
            heightWindow = Screen.height;
        }
    }

    /// <summary>
    /// Show or hide resizement button and set resolution.
    /// </summary>
    private void UpdateWindowWhenResize()
    {
        if (isZoomed) //The window is in Zoomed
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        }

        if (isFullScreen) //The window is in fullscreen
        {
            maximize_B.visible = true;
            minimize_B.visible = true;
            close_B.visible = true;
        }
        else
        {
            maximize_B.visible = false;
            minimize_B.visible = false;
            close_B.visible = false;
        }
    }

    /// <summary>
    /// Switch between fullscreen and window.
    /// </summary>
    /// <param name="value">Set fullscreen if true, else window.</param>
    private void SwitchFullScreen(bool value)
    {
        if (value)
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
            isFullScreen = true;
            maximize_B.visible = true;
            minimize_B.visible = true;
            close_B.visible = true;
        }
        else
        {
            Screen.SetResolution(widthWindow, heightWindow, false);
            isFullScreen = false;
            maximize_B.visible = false;
            minimize_B.visible = false;
            close_B.visible = false;
        }
    }

    #endregion

    #endregion
}
