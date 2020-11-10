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

using BrowserMenu;
using System;
using System.Runtime.InteropServices;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowsManager : MonoBehaviour
{
    #region Fields

    public PanelRenderer panelRenderer;

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

    Button minimize;
    Button maximize;

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
            DialogueBoxElement dialogueBox = PauseMenu.Instance.dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
            dialogueBox.Setup("", "Are you sure ...?", "YES", "NO", (b) =>
            {
                if (b)
                    Application.Quit();
            });
            panelRenderer.visualTree.Add(dialogueBox);
        };

        var topBar = panelRenderer.visualTree.Q<VisualElement>("top");
        topBar.RegisterCallback<MouseDownEvent>((e) =>
        {
            if (IsZoomed(GetActiveWindow())) //Check if the window is maximised
            {
                //1. Store "local" mousePosition before resizing
                Vector2 offset = e.mousePosition;
                //2. Resize
                ShowWindow(GetActiveWindow(), 9);
                //3. Get resizing rate
                float rate = (float) Screen.width / (float) GetSystemMetrics(16);

                //4. Set Position
                POINT p;
                if (GetCursorPos(out p))
                {
                    Vector2 topLeftHandCorner = new Vector2(p.X, p.Y) - offset * rate;
                    SetWindowPos(hWnd, IntPtr.Zero, (short)topLeftHandCorner.x, (short) topLeftHandCorner.y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                }
            }
            
            ReleaseCapture();
            SendMessageCallback(hWnd, WM_SYSCOMMAND, MOUSE_MOVE, 0, DropCallBack, 0); 
        });

    }

    private void DropCallBack(IntPtr hWnd, uint uMsg, UIntPtr dwData, IntPtr lResult)
    {
        /*var window = GetActiveWindow();
        SetForegroundWindow(window);
        SetFocus(window);
        /*SendMessage(window, WM_NCLBUTTONDOWN, 0, 0);
        SendMessage(window, WM_LBUTTONUP, 0, 0);*/
    }


    private void UpdateCustomTitleBar()
    {
        maximize.ClearClassList();
        if (IsZoomed(GetActiveWindow())) //Check if the window is maximised
        {
            maximize.AddToClassList(restoreClassName);
        }
        else
        {
            maximize.AddToClassList(maximizeClassName);
        }
    }

    /// <summary>
    /// Shows or hides the default windows borders to resize the window.
    /// </summary>
    /// <param name="value"></param>
    private void ShowWindowBorders(bool value)
    {
        if (Application.isEditor) return; //Not to hide the editor toolbar!

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


            // Seems useless but for now it's a trick to remove the titlebar without having to resize first the window
            if (IsZoomed(GetActiveWindow()))//Check if the window is maximised
            {    
                ShowWindow(GetActiveWindow(), 9);
                ShowWindow(GetActiveWindow(), 3);
            }
            else
            {
                ShowWindow(GetActiveWindow(), 3);
                ShowWindow(GetActiveWindow(), 9);
            }
        }
    }

    #endregion
}
