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
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class SideMenu : Singleton<SideMenu>
    {
        #region Fields

        private bool isDisplayed = false;
        static public bool IsDisplayed { get { return Exists ? Instance.isDisplayed : false; } }

        public PanelRenderer panelRenderer;

        [Header("Side menu general settings")]

        VisualElement rightSideMenuContainer;

        [Header("Toolbox menu")]
        public MenuDisplayManager toolBoxMenuDisplayManager;
        public MenuAsset ToolboxMenu;

        #endregion

        #region Methods 

        private void Start()
        {
            BindUI();

            rightSideMenuContainer.style.display = DisplayStyle.None;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                rightSideMenuContainer.style.display = DisplayStyle.Flex;
            });

            InteractionMapper.Instance.toolboxMenu = ToolboxMenu.menu;
        }
       
        #region UI Bindings

        private void BindUI()
        {
            VisualElement root = panelRenderer.visualTree;

            BindRightSideMenu(root);
        }

        private void BindRightSideMenu(VisualElement root)
        {
            rightSideMenuContainer = root.Q<VisualElement>("right-side-menu-container");
        }

        #endregion

        /// <summary>
        /// Displays the side menu.
        /// </summary>
        /// <param name="display"></param>
        static public void Display(bool display = true)
        {
            if (Exists) Instance._Display(display);
        }

        void _Display(bool display = true)
        {
            if (display)
            {
                toolBoxMenuDisplayManager.Display(true);

                rightSideMenuContainer.experimental.animation.Start(rightSideMenuContainer.resolvedStyle.width,0, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });

                //To remove 
                CircularMenu.Instance.HideMenu();
            } else
            {
                toolBoxMenuDisplayManager.Hide(true);

                rightSideMenuContainer.experimental.animation.Start(0, rightSideMenuContainer.resolvedStyle.width, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });

                //To remove 
                var viewport = panelRenderer.visualTree.Q<VisualElement>("game-menu");
                viewport.style.backgroundColor = new Color(0, 0, 0, 0);
                viewport.style.display = DisplayStyle.None;
                CircularMenu.Instance.ShowMenu();
            }

            isDisplayed = display;
            CursorHandler.SetMovement(this, display ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
        }

        #endregion
    }
}