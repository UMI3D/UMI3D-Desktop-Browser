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

using BrowserDesktop.Cursor;
using System;
using umi3d.cdk;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
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

        [Header("Side menu general settings")]
        VisualElement sideMenuScreen;

        Label sessionTime;
        VisualElement sessionInfo;

        VisualElement sideMenu;
        Button hangUpBtn;

        DateTime startOfSession;

        [Header("Toolbox menu")]
        public MenuDisplayManager toolBoxMenuDisplayManager;
        public MenuAsset ToolboxMenu;

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

            InteractionMapper.Instance.toolboxMenu = ToolboxMenu.menu;
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
                toolBoxMenuDisplayManager.Display(true);

                sideMenu.experimental.animation.Start(sideMenu.resolvedStyle.width,0, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
                //To remove 
                var viewport = ConnectionMenu.Instance.panelRenderer.visualTree.Q<VisualElement>("game-menu");
                viewport.style.backgroundColor = Color.white;
                viewport.style.display = DisplayStyle.Flex;
            } else
            {
                toolBoxMenuDisplayManager.Hide(true);

                sideMenu.experimental.animation.Start(0, sideMenu.resolvedStyle.width, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });

                //To remove 
                var viewport = ConnectionMenu.Instance.panelRenderer.visualTree.Q<VisualElement>("game-menu");
                viewport.style.backgroundColor = new Color(0, 0, 0, 0);
                viewport.style.display = DisplayStyle.None;
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