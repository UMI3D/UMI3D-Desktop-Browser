/*
Copyright 2019 - 2024 Inetum

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
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class removes the default Windows title bar and set up a custom one.
/// </summary>
public class WindowsManager
{
    /// <summary>
    /// Width of the window when the full screen is deactivated.
    /// </summary>
    static int widthWindow = widthMonitor / 2;
    /// <summary>
    /// Height of the window when the full screen is deactivated.
    /// </summary>
    static int heightWindow = heightMonitor / 2;

    /// <summary>
    /// Width of the monitor.
    /// </summary>
    static int widthMonitor
    {
        get
        {
            return Screen.resolutions[Screen.resolutions.Length - 1].width;
        }
    }
    /// <summary>
    /// Height of the monitor.
    /// </summary>
    static int heightMonitor
    {
        get
        {
            return Screen.resolutions[Screen.resolutions.Length - 1].height;
        }
    }

    /// <summary>
    /// Whether this window is zoomed.<br/>
    /// <br/>
    /// A zoomed window is like a full screen window but with the top bar.
    /// </summary>
    public static bool IsWindowZoomed
    {
        get
        {
            return IsZoomed(window);
        }
    }
    /// <summary>
    /// The last zoomed state.
    /// </summary>
    static bool zoomed = false;

    /// <summary>
    /// Whether this application is in full screen.
    /// </summary>
    public static bool IsWindowInFullScreen
    {
        get
        {
            return IsFullScreen(Screen.fullScreenMode);
        }
    }
    /// <summary>
    /// The last full screen state.
    /// </summary>
    static FullScreenMode fullScreenMode = FullScreenMode.Windowed;
    /// <summary>
    /// The next full screen state.
    /// </summary>
    static FullScreenMode nextFullScreenMode = FullScreenMode.Windowed;

#if UNITY_STANDALONE_WIN

    /// <summary>
    /// External method to minimize or maximize the window.<br/>
    /// <br/>
    /// See SW_HIDE, SW_SHOWNORMAL, SW_SHOWMINIMIZED, SW_SHOWMAXIMIZED
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="nCmdShow"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    /// <summary>
    /// Hide the window.
    /// </summary>
    const int SW_HIDE = 0;
    /// <summary>
    /// Focus and display the window in its original size.
    /// </summary>
    const int SW_SHOWNORMAL = 1;
    /// <summary>
    /// Focus and display the window in a windowed state.
    /// </summary>
    const int SW_SHOWMINIMIZED = 2;
    /// <summary>
    /// Focus and display the window in a full screen state.
    /// </summary>
    const int SW_SHOWMAXIMIZED = 3;

    /// <summary>
    /// Get the application's window.
    /// </summary>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();
    /// <summary>
    /// The active window.
    /// </summary>
    static IntPtr window;

    /// <summary>
    /// Whether this window is zoomed.<br/>
    /// <br/>
    /// A zoomed window is like a full screen window but with the top bar.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool IsZoomed(IntPtr hWnd);

    #endif

    static WindowsManager()
    {
        // Get the active window ptr.
        window = GetActiveWindow();

        // Minimize the window when 'WindowsManagerNotificationKey.Minimize' is sent.
        NotificationHub.Default.Subscribe(
            typeof(WindowsManager).FullName,
            WindowsManagerNotificationKey.Minimize,
            null,
            Minimize
        );

        // Windowed the window when 'WindowsManagerNotificationKey.Windowed' is sent.
        NotificationHub.Default.Subscribe(
            typeof(WindowsManager).FullName,
            WindowsManagerNotificationKey.Maximize,
            null,
            Maximize
        );

        // Switch full screen mode when 'WindowsManagerNotificationKey.FullScreenModeWillChange' is sent.
        NotificationHub.Default.Subscribe(
             typeof(WindowsManager).FullName,
             WindowsManagerNotificationKey.FullScreenModeWillChange,
             null,
             FullScreenWillChange
         );

        //// Set the initial resolution.
        //Screen.SetResolution(
        //    widthMonitor / 2,
        //    heightMonitor / 2,
        //    FullScreenMode.Windowed
        //);
    }

    public static void Update()
    {
        if (nextFullScreenMode != fullScreenMode)
        {
            return;
        }

        //Check if the window is being resized thanks to the original window tab bar button.
        if (IsWindowZoomed != zoomed) 
        {
            zoomed = IsWindowZoomed;
            if (zoomed)
            {
                SwitchFullScreen(true);
            }
        }
        // Check if the window is being resized thanks to the alt + enter shortcut.
        else if (IsWindowInFullScreen != IsFullScreen(fullScreenMode))
        {
            SwitchFullScreen(IsWindowInFullScreen);
        }

    }

    /// <summary>
    /// Whether <paramref name="mode"/> is a full screen mode or not.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    static bool IsFullScreen(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
            case FullScreenMode.FullScreenWindow:
            case FullScreenMode.MaximizedWindow:
                return true;
            case FullScreenMode.Windowed:
                return false;
            default:
                UnityEngine.Debug.LogError($"Not handle");
                return false;
        }
    }

    /// <summary>
    /// Return the platform specific full screen mode.
    /// </summary>
    /// <param name="fullScreen"></param>
    /// <returns></returns>
    static FullScreenMode GetFullScreenMode(bool fullScreen)
    {
#if UNITY_STANDALONE_WIN
        return fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
#elif UNITY_STANDALONE_OSX
        return fullScreen ? FullScreenMode.MaximizedWindow : FullScreenMode.Windowed;
#endif
    }

    /// <summary>
    /// Switch between full screen and window.
    /// </summary>
    /// <param name="fullScreen">Set full screen if true, else window.</param>
    static void SwitchFullScreen(bool fullScreen)
    {
        nextFullScreenMode = GetFullScreenMode(fullScreen);

        // If the window is going to be in full screen save the actual window resolution.
        if (fullScreen && (Screen.width != widthMonitor || Screen.height != heightMonitor))
        {
            widthWindow = Screen.width;
            heightWindow = Screen.height;
        }

        // Set the resolution and the full screen mode.
        Screen.SetResolution(
            fullScreen ? widthMonitor : widthWindow,
            fullScreen ? heightMonitor : heightWindow,
            nextFullScreenMode
        );

        // Notify observers that the full screen mode has changed.
        NotificationHub.Default.Notify(
            typeof(WindowsManager).FullName, 
            WindowsManagerNotificationKey.FullScreenModeChanged, 
            new() { 
                { WindowsManagerNotificationKey.FullScreenModeChangedInfo.Mode, nextFullScreenMode } 
            }
        );

        // Wait one frame so that the window can be resized.
        new Task(async () =>
        {
            // Wait one frame.
            await Task.Yield();
            fullScreenMode = nextFullScreenMode;
        }).Start(TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Hide the window.
    /// </summary>
    /// <param name="notification"></param>
    static void Minimize(Notification notification)
    {
#if UNITY_STANDALONE_WIN
        ShowWindow(window, SW_SHOWMINIMIZED);
#endif
    }

    /// <summary>
    /// If the window is hidden, maximize (display) the window in its previous state, else do nothing.
    /// </summary>
    /// <param name="notification"></param>
    static void Maximize(Notification notification)
    {
#if UNITY_STANDALONE_WIN
        ShowWindow(window, SW_SHOWMAXIMIZED);
#endif
    }

    /// <summary>
    /// Be notified when the full sceen mode will change.
    /// </summary>
    /// <param name="notification"></param>
    static void FullScreenWillChange(Notification notification)
    {
        if (!notification.TryGetInfoT(WindowsManagerNotificationKey.FullScreenModeChangedInfo.Mode, out FullScreenMode mode))
        {
            UnityEngine.Debug.LogError($"[WindowManager] info key is missing.");
            return;
        }

        SwitchFullScreen(mode == FullScreenMode.FullScreenWindow);
    }
}

public static class WindowsManagerNotificationKey
{
    /// <summary>
    /// Hide the window.
    /// </summary>
    public const string Hide = "Hide";

    /// <summary>
    /// Hide the window
    /// </summary>
    public const string Minimize = "Minimize";

    /// <summary>
    ///  If the window is hidden, maximize (display) the window in its previous state, else do nothing.
    /// </summary>
    public const string Maximize = "Windowed";

    /// <summary>
    /// Ask to change the full screen mode.
    /// </summary>
    public const string FullScreenModeWillChange = "FullScreenModeWillChange";

    /// <summary>
    /// The full screen mode has changed.
    /// </summary>
    public const string FullScreenModeChanged = "FullScreenModeChanged";

    public class FullScreenModeChangedInfo
    {
        /// <summary>
        /// Mode of the full screen. <see cref="FullScreenMode"/>
        /// </summary>
        public const string Mode = "Mode";
    }
}
