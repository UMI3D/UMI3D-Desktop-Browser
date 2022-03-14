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
using inetum.unityUtils;
using System.Collections;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class reprensents the right side menu and can be used to display two menus : 
    ///     - the main menu (toolboxmenu)
    ///     - the settings menu (composed by the events and interactions menu)
    /// They can't be displayed at the same time (the settings menu takes priority if not empty)
    /// 
    /// The right side menu can be just displayed (when the settings menu is not empty but the view is still center)
    /// or displayed and expanded (and consequently interactable because the view is free).
    /// 
    /// </summary>
    public class SideMenu : SingleBehaviour<SideMenu>
    {
        #region Fields

        private bool isDisplayed = false;
        static public bool IsDisplayed { get { return Exists ? Instance.isDisplayed : false; } }
        private bool isExpanded = false;
        static public bool IsExpanded { get { return Exists ? Instance.isExpanded : false; } }

        public UIDocument uiDocument;

        [Header("Side menu general settings")]


        [Header("Toolbox menu")]
        public MenuDisplayManager toolBoxMenuDisplayManager;
        public MenuAsset ToolboxMenu;


        Label shortcutsRightClick;

        #endregion

        #region Methods 

        private void Start()
        {
            BindUI();

            

            InteractionMapper.Instance.toolboxMenu = ToolboxMenu.menu;
        }
       
        #region UI Bindings

        private void BindUI()
        {
            Debug.Assert(uiDocument != null);
            VisualElement root = uiDocument.rootVisualElement;

            
            shortcutsRightClick = root.Q<Label>("shortcuts-right-click");
        }


        #endregion

        /// <summary>
        /// Displays or not the right side menu. 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="display">expand the menu or not</param>
        static public void Display(bool display, bool expand)
        {
            if (Exists)
            {

                CursorHandler.SetMovement(Instance, expand ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
            }
        }

        void LateUpdate()
        {

            if (false)
            {
                if (IsExpanded)
                    shortcutsRightClick.text = "Right click : close main menu";
                else
                    shortcutsRightClick.text = "Right click : open main menu";
            } else
            {
                if (IsExpanded)
                    shortcutsRightClick.text = "Right click : close interaction menu";
                else
                    shortcutsRightClick.text = "Right click : open interaction menu";
            }
        }

        #endregion
    }
}