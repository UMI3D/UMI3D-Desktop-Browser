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
using System.Runtime.InteropServices;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowsManager : MonoBehaviour
{
    #region Fields

    #region Fiels to make the title bar working like the windows one.
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool IsZoomed(IntPtr hWnd);

    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool SetCapture(IntPtr hWnd);

    [Header("Custom title bar")]

    [SerializeField]
    string minimizeTagName = "minimize-window-btn";
    [SerializeField]
    string maximizeTagName = "fullscreen-btn";
    [SerializeField]
    string closeTagName = "close-window-btn";

    public Texture2D maximizeTexture;
    public Texture2D restoreTexture;

    public PanelRenderer panelRenderer;

    Button minimize;
    Button maximize;

    #endregion

    #region Fields to remove windows default title bar

    #endregion
    const int SWP_HIDEWINDOW = 0x80; //hide window flag.
    const int SWP_SHOWWINDOW = 0x40; //show window flag.
    const int SWP_NOMOVE = 0x0002; //don't move the window flag.
    const int SWP_NOSIZE = 0x0001; //don't resize the window flag.
    const uint WS_SIZEBOX = 0x00040000;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 0x00800000; //window with border
    const int WS_DLGFRAME = 0x00400000; //window with double border but no title
    const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
    const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
    const int WS_MAXIMIZEBOX = 0x00010000;
    const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

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

    #region Methods

    void Start()
    {
        SetUpCustomTitleBar();

        hWnd = GetActiveWindow();
        if (hideOnStart) ShowWindowBorders(false);
    }

    void Update()
    {
        UpdateCustomTitleBar();
    }

    private void SetUpCustomTitleBar()
    {
        minimize = panelRenderer.visualTree.Q<Button>(minimizeTagName);
        minimize.clickable.clicked += () =>
        {
            ShowWindow(GetActiveWindow(), 2);
        };

        maximize = panelRenderer.visualTree.Q<Button>(maximizeTagName);
        maximize.clickable.clicked += () =>
        {
            if (IsZoomed(GetActiveWindow())) //Check if the window is maximised
                ShowWindow(GetActiveWindow(), 9);
            else
                ShowWindow(GetActiveWindow(), 3);

        };

        var close = panelRenderer.visualTree.Q<Button>(closeTagName);
        close.clickable.clicked += () =>
        {
            Application.Quit();
        };

        var topBar = panelRenderer.visualTree.Q<VisualElement>("top");
        topBar.RegisterCallback<MouseDownEvent>((e) =>
        {
            if (IsZoomed(GetActiveWindow())) //Check if the window is maximised
                ShowWindow(GetActiveWindow(), 9);

            ReleaseCapture();
            SendMessage(GetActiveWindow(), WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            
        });
    }

    private void UpdateCustomTitleBar()
    {
        if (IsZoomed(GetActiveWindow())) //Check if the window is maximised
        {
            maximize.style.backgroundImage = new StyleBackground(restoreTexture);
        }
        else
        {
            maximize.style.backgroundImage = new StyleBackground(maximizeTexture);
        }
    }

    private void ShowWindowBorders(bool value)
    {
        if (Application.isEditor) return; //We don't want to hide the toolbar from our editor!

        int style = GetWindowLong(hWnd, GWL_STYLE).ToInt32(); //gets current style

        if (value)
        {
            SetWindowLong(hWnd, GWL_STYLE, (uint)(style | WS_CAPTION | WS_SIZEBOX)); //Adds caption and the sizebox back.
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window normal.
        }
        else
        {
            SetWindowLong(hWnd, GWL_STYLE, (uint)(style & ~(WS_CAPTION))); //removes caption and the sizebox from current style.
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW); //Make the window render above toolbar.
            ShowWindow(GetActiveWindow(), 3);
        }
    }

    #endregion
}
