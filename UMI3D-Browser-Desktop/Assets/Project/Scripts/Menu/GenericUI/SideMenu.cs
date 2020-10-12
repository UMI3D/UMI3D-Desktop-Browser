using BrowserDesktop.Cursor;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class SideMenu : Singleton<SideMenu>
    {
        #region Fields

        private bool isDisplayed = false;

        static public bool IsDisplayed { get { return Exists ? Instance.isDisplayed : false; } }

        VisualElement sideMenuScreen;

        Label sessionTime;
        VisualElement sessionInfo;

        VisualElement sideMenu;
        Button hangUpBtn;

        DateTime startOfSession;

        #endregion

        #region Methods 

        private void Start()
        {
            BindUI();

            sideMenuScreen.style.display = DisplayStyle.None;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                sideMenuScreen.style.display = DisplayStyle.Flex;
                startOfSession = DateTime.Now;
            });
        }

        static public void Display(bool display = true)
        {
            if (Exists) Instance._Display(display);
        }

        private void BindUI()
        {
            VisualElement root = ConnectionMenu.Instance.panelRenderer.visualTree;

            BindSideMenu(root);
            BindSessionInfo(root);
        }

        private void BindSideMenu(VisualElement root)
        {
            sideMenuScreen = root.Q<VisualElement>("right-side-menu");
            sideMenu = root.Q<VisualElement>("side-menu");

            hangUpBtn = root.Q<Button>("tool-hang-up");
            hangUpBtn.clickable.clicked += ConnectionMenu.Instance.Leave;
        }

        private void BindSessionInfo(VisualElement root)
        {
            sessionInfo = root.Q<VisualElement>("session-info");
            sessionTime = sessionInfo.Q<Label>("session-time");
        }

        void _Display(bool display = true)
        {
            if (display)
            {
                sideMenu.experimental.animation.Start(sideMenu.resolvedStyle.width,0, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
            } else
            {
                sideMenu.experimental.animation.Start(0, sideMenu.resolvedStyle.width, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
            }

            isDisplayed = display;
            CursorHandler.SetMovement(this, display ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
        }

        void ShowMenu()
        {

        }

        void HideMenu()
        {
            sideMenu.experimental.animation.Start(0, -sideMenu.resolvedStyle.width, 1000, (elt, val) =>
            {
                elt.style.left = val;
            });
        }

        private void Update()
        {
            var time = DateTime.Now - startOfSession;
            sessionTime.text = time.ToString("hh") + ":"+ time.ToString("mm") + ":" + time.ToString("ss");
        }
        #endregion
    }
}