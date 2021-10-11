﻿/*
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
    //private Label debugLabel;

    private bool wantsToQuit = false;

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

    [DllImport("User32.dll")]
    public static extern bool ReleaseCapture();
    delegate void SendMessageDelegate(IntPtr hWnd, uint uMsg, UIntPtr dwData, IntPtr lResult);
    [DllImport("user32.dll")]
    static extern bool SendMessageCallback(IntPtr hWnd, int Msg, int wParam, int lParam, SendMessageDelegate lpCallBack, int dwData);
    private const int WM_SYSCOMMAND = 0x112;
    private const int MOUSE_MOVE = 0xF012;


  
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return "Point (" + X + " ," + Y + ")";
        }
    }

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    [Header("Custom title bar")]

    [SerializeField]
    string minimizeTagName = "minimize-window-btn";
    [SerializeField]
    string maximizeTagName = "fullscreen-btn";
    [SerializeField]
    string closeTagName = "close-window-btn";

    public string maximizeClassName = "maximize-btn";
    public string restoreClassName = "restore-btn";

    VisualElement root;
    Button minimize;
    Button maximize;

    public VisualTreeAsset dialogueBoxTreeAsset;

    #endregion

    #region Fields to remove windows default title bar

    const int SWP_HIDEWINDOW = 0x80; //hide window flag.
    const int SWP_SHOWWINDOW = 0x40; //show window flag.
    const int SWP_NOMOVE = 0x0002; //don't move the window flag.
    const int SWP_NOSIZE = 0x0001; //don't resize the window flag.
    const short SWP_NOZORDER = 0X4; //don't change z order
    const uint WS_SIZEBOX = 0x00040000;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 0x00800000; //window with border
    const int WS_DLGFRAME = 0x00400000; //window with double border but no title
    const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar


    [DllImport("user32.dll")]
    static extern int FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(
        System.IntPtr hWnd, // window handle
        System.IntPtr hWndInsertAfter, // placement order of the window
        short X, // x position
        short Y, // y position
        short cx, // width
        short cy, // height
        uint uFlags // window flags.
    );

    [DllImport("user32.dll")]
    static extern System.IntPtr SetWindowLong(
         System.IntPtr hWnd, // window handle
         int nIndex,
         uint dwNewLong
    );

    [DllImport("user32.dll")]
    static extern System.IntPtr GetWindowLong(
        IntPtr hWnd,
        int nIndex
    );

    IntPtr hWnd;
    IntPtr HWND_TOP = new System.IntPtr(0);
    IntPtr HWND_TOPMOST = new System.IntPtr(-1);
    IntPtr HWND_NOTOPMOST = new System.IntPtr(-2);

    [Header("Remove default title bar")]

    [Tooltip("Hide default windows title bar ?")] [SerializeField] bool hideOnStart = false;

    #endregion

    #endregion

    #region Methods

    void Start()
    {
        Debug.Assert(uiDocument != null);
        SetUpCustomTitleBar();

        hWnd = GetActiveWindow();
        isZoomed = IsZoomed(hWnd);
        isFullScreen = Screen.fullScreen;
        UpdateWindowWhenResize();

        Application.wantsToQuit += WantsToQuit;
    }

    [ContextMenu("WantToQuit")]
    private bool WantsToQuit()
    {
        Debug.LogError("Want to quit = " + wantsToQuit);
        if (!wantsToQuit)
            ShowDialogueBoxToQuit();
        return wantsToQuit;
    }

    [ContextMenu("test")]
    private void ShowDialogueBoxToQuit()
    {
        DialogueBoxElement dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
        dialogueBox.Setup("", "Are you sure ...?", "YES", "NO", (b) =>
        {
            wantsToQuit = b;
            if (b)
                Application.Quit();
        });
        root.Add(dialogueBox);
    }

    void Update()
    {
        CheckForWindowResizement();
    }

    private void SetUpCustomTitleBar()
    {
        root = uiDocument.rootVisualElement;
        //debugLabel = root.Q<Label>("debug-label");
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

        /*
        var close = root.Q<Button>(closeTagName);
        close.clickable.clicked += () =>
        {
            //This will raise the Application.WantsToQuit event and show a dialogue box.
            Application.Quit();
        };
        */
    }

    private void DropCallBack(IntPtr hWnd, uint uMsg, UIntPtr dwData, IntPtr lResult)
    {
        /*var window = GetActiveWindow();
        SetForegroundWindow(window);
        SetFocus(window);
        /*SendMessage(window, WM_NCLBUTTONDOWN, 0, 0);
        SendMessage(window, WM_LBUTTONUP, 0, 0);*/
    }

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
                //ShowWindow(hWnd, 9);
                SwitchFullScreen(false);
            }
        }

        if (!isFullScreen && !isZoomed)
        {
            widthWindow = Screen.width;
            heightWindow = Screen.height;
        }

        //debugLabel.text = "width = " + Screen.width + ", height = " + Screen.height;
        //debugLabel.text = "Zoomed = " + IsZoomed(hWnd) + ", fullscreen = " + Screen.fullScreen;
    }

    private void UpdateWindowWhenResize()
    {
        if (isZoomed) //The window is in Zoomed
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        }

        if (isFullScreen) //The window is in fullscreen
        {
            //ShowWindow(hWnd, 3);
            //Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
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
}
