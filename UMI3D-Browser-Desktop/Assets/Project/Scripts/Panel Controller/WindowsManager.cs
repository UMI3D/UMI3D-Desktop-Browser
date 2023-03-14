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
using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// This class removes the default Windows title bar and set up a custom one.
/// </summary>
public class WindowsManager : MonoBehaviour
{
    #region Fields

    public Action<bool> FullScreenEnabled;
    public Action DisplayDialogueBoxToQuit;

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

    #endregion

    #endregion

    #region Methods

    #region LifeCycle monoBehaviour

    void Start()
    {
        hWnd = GetActiveWindow();
        isZoomed = IsZoomed(hWnd);
        isFullScreen = Screen.fullScreen;
        UpdateWindowWhenResize();

        Application.wantsToQuit += WantsToQuit;
        QuittingManager.ShouldWaitForApplicationToQuit = true;
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
        if (!wantsToQuit) DisplayDialogueBoxToQuit?.Invoke();
        return wantsToQuit;
    }

    #endregion

    #region Window resizement

    public void Minimize() => ShowWindow(hWnd, 2);
    public void Maximize() => SwitchFullScreen(false);

    /// <summary>
    /// Check if the widow is being zoomed or unzoomed, in fullscreen or not.
    /// </summary>
    private void CheckForWindowResizement()
    {
        if ((IsZoomed(hWnd) && !isZoomed) ||
            (!IsZoomed(hWnd) && isZoomed)) //Check if the window is being resized (zoomed or unzoomed)
        {
            isZoomed = IsZoomed(hWnd);
            if (isZoomed) SwitchFullScreen(true);
        }
        else //The window has been resized with a shortcut
        {
            if (Screen.fullScreen && !isFullScreen) //Check if the window is being resized without being zoomed
            {
                ShowWindow(hWnd, 3);
                SwitchFullScreen(true);
            }

            else if (!Screen.fullScreen && isFullScreen) SwitchFullScreen(false);
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
        if (isZoomed) Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        FullScreenEnabled?.Invoke(isFullScreen);
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
            FullScreenEnabled?.Invoke(true);
        }
        else
        {
            Screen.SetResolution(widthWindow, heightWindow, false);
            isFullScreen = false;
            FullScreenEnabled?.Invoke(false);
        }
    }

    #endregion

    #endregion
}
