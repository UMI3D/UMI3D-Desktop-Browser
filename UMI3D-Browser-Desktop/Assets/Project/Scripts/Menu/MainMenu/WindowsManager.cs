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

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField]
    string minimizeTagName = "minimize-window-btn";
    [SerializeField]
    string maximizeTagName = "fullscreen-btn";

    public string restoreClassName = "restore-btn";

    VisualElement root;
    Button minimize;
    Button maximize;

    public VisualTreeAsset dialogueBoxTreeAsset;

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
        umi3d.common.QuittingManager.ShouldWaitForApplicationToQuit = true;
    }

    private void SetUpCustomTitleBar()
    {
        root = uiDocument.rootVisualElement;
        minimize = root.Q<Button>(minimizeTagName);
        minimize.clickable.clicked += () =>
        {
            ShowWindow(hWnd, 2);
        };

        maximize = uiDocument.rootVisualElement.Q<Button>(maximizeTagName);
        maximize.AddToClassList(restoreClassName);
        maximize.clickable.clicked += () =>
        {
            SwitchFullScreen(false);
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

    private bool WantsToQuit()
    {
        bool wantsToQuit = umi3d.common.QuittingManager.ApplicationIsQuitting;
        if (!wantsToQuit)
            ShowDialogueBoxToQuit();
        return wantsToQuit;
    }

    private void ShowDialogueBoxToQuit()
    {
        DialogueBoxElement dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
        dialogueBox.Setup("Close application", "Are you sure ...?", "YES", "NO", (b) =>
        {
            umi3d.common.QuittingManager.ApplicationIsQuitting = b;
            if (b)
                Application.Quit();
        });
        root.Add(dialogueBox);
        dialogueBox.BringToFront();
    }

    #endregion

    #region Window resizement

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

    private void UpdateWindowWhenResize()
    {
        if (isZoomed) //The window is in Zoomed
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        }

        if (isFullScreen) //The window is in fullscreen
        {
            maximize.visible = true;
            minimize.visible = true;
        }
        else
        {
            maximize.visible = false;
            minimize.visible = false;
        }
    }

    private void SwitchFullScreen(bool value)
    {
        if (value)
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
            isFullScreen = true;
            maximize.visible = true;
            minimize.visible = true;
        }
        else
        {
            Screen.SetResolution(widthWindow, heightWindow, false);
            isFullScreen = false;
            maximize.visible = false;
            minimize.visible = false;
        }
    }

    #endregion

    #endregion
}
