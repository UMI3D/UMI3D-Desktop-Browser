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

    void Start()
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
            ReleaseCapture();
            SendMessage(GetActiveWindow(), WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        });
    }

    void Update()
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
}
