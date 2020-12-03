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
using System.Collections;
using System.Security.Cryptography;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    public class SideMenu : Singleton<SideMenu>
    {
        #region Fields

        private bool isDisplayed = false;
        static public bool IsDisplayed { get { return Exists ? Instance.isDisplayed : false; } }
        private bool isExpanded = false;
        static public bool IsExpanded { get { return Exists ? Instance.isExpanded : false; } }

        public PanelRenderer panelRenderer;

        [Header("Side menu general settings")]

        VisualElement rightSideMenuContainer;
        VisualElement interactionMenu;
        VisualElement toolBoxMenu;
        VisualElement eventsMenu;

        [Header("Toolbox menu")]
        public MenuDisplayManager toolBoxMenuDisplayManager;
        public MenuAsset ToolboxMenu;

        Button backCircularMenu;

        Label shortcutsRightClick;

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
            
            shortcutsRightClick = root.Q<Label>("shortcuts-right-click");
        }

        private void BindRightSideMenu(VisualElement root)
        {
            rightSideMenuContainer = root.Q<VisualElement>("right-side-menu-container");
            interactionMenu = root.Q<VisualElement>("interaction-menu");
            toolBoxMenu = root.Q<VisualElement>("toolbox-menu");
            eventsMenu = root.Q<VisualElement>("information-pop-up-events");

            backCircularMenu = root.Q<Button>("interaction-menu-back");
            backCircularMenu.clickable.clicked += () =>
            {
                Display(false, false);
            };

            root.Q<Button>("toolbox-menu-back").clickable.clicked += () =>
            {
                Display(false, false);
            };

            root.Q<VisualElement>("game-menu").RegisterCallback<MouseDownEvent>(e =>
            {
                if ((e.clickCount == 1) && (isExpanded) && !wasExpandedLastFrame)
                {
                    Display(false, false);
                }
            });
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
                Instance.backCircularMenu.style.display = DisplayStyle.None;

                Instance._Display(display);
                Instance.Expand(expand);

                CursorHandler.SetMovement(Instance, expand ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);

                bool isCircularMenuEmpty = CircularMenu.Instance.IsEmpty();
                if (!isCircularMenuEmpty || EventMenu.NbEventsDIsplayed > 0)
                {
                    Instance.DisplayPauseMenu(false);
                    if (expand && isCircularMenuEmpty)
                        EventMenu.Expand(true);
                    else
                        EventMenu.Expand(false);
                }
                else
                {
                    Instance.DisplayPauseMenu(true);
                }
                    
            }
        }

        void DisplayPauseMenu(bool val)
        {
            if (val)
            {
                toolBoxMenu.style.display =  DisplayStyle.Flex;
                interactionMenu.style.display = DisplayStyle.None;
                eventsMenu.style.display = DisplayStyle.None;
                backCircularMenu.style.display = DisplayStyle.None;
            } else
            {
                toolBoxMenu.style.display = DisplayStyle.None;
                interactionMenu.style.display = CircularMenu.Instance.IsEmpty() ? DisplayStyle.None : DisplayStyle.Flex;
                eventsMenu.style.display = EventMenu.NbEventsDIsplayed == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            }
            
        }

        /// <summary>
        /// Displays or not the right side menu
        /// </summary>
        /// <param name="display"></param>
        void _Display(bool display = true)
        {
            if (isDisplayed == display)
                return;

            isDisplayed = display;
            if (display)
            {
                toolBoxMenuDisplayManager.Display(true);

                rightSideMenuContainer.experimental.animation.Start(rightSideMenuContainer.resolvedStyle.width,0, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
            } else
            {
                toolBoxMenuDisplayManager.Hide(true);

                rightSideMenuContainer.experimental.animation.Start(0, rightSideMenuContainer.resolvedStyle.width, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
            }
        }

        private bool wasExpandedLastFrame = false;
        
        /// <summary>
        /// Expands or not the menu to make it use the full height of the screen. 
        /// </summary>
        /// <param name="expand"></param>
        void Expand(bool expand)
        {
            if (isExpanded == expand)
                return;

            isExpanded = expand;

            if (expand)
            {
                backCircularMenu.style.display = DisplayStyle.Flex;
                rightSideMenuContainer.experimental.animation.Start(0, 1, 200, (elt, val) =>
                {
                    elt.style.flexGrow = val;
                });
                StartCoroutine(ResetWasExpandedLastFrame());
            } else
            {
                backCircularMenu.style.display = DisplayStyle.None;
                rightSideMenuContainer.style.flexGrow = 0;
            }
        }
       
        IEnumerator ResetWasExpandedLastFrame()
        {
            wasExpandedLastFrame = true;
            yield return null;
            wasExpandedLastFrame = false;
        }

        /// <summary>
        /// Manages the display of the menu as a popup when different elements with interactions are hovered.
        /// </summary>
        int nbOfCircularDisplayersLastFrame = 0;
        int nbEventsDisplayedLastFrame = 0;
        void LateUpdate()
        {
            int nbOfCircularDisplayers = CircularMenu.Instance.Count();
            int nbEventsDisplayed = EventMenu.NbEventsDIsplayed;

            if ((nbOfCircularDisplayersLastFrame != nbOfCircularDisplayers) || (nbEventsDisplayed != nbEventsDisplayedLastFrame))
            {
                if (IsDisplayed && !IsExpanded && (nbOfCircularDisplayers == 0 && nbEventsDisplayed == 0))
                {
                    CircularMenu.Instance.CloseMenu();
                    Display(false, false);
                }

                else if (!IsDisplayed && (nbOfCircularDisplayers > 0 || nbEventsDisplayed > 0))
                {
                    if (nbOfCircularDisplayers > 0)
                        CircularMenu.Instance.Display();
                    Display(true, false);
                }

            }

            nbOfCircularDisplayersLastFrame = nbOfCircularDisplayers;
            nbEventsDisplayedLastFrame = nbEventsDisplayed;

            if (nbOfCircularDisplayersLastFrame == 0 && nbEventsDisplayedLastFrame == 0)
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