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

using BrowserDesktop.UI.CustomElement;
using BrowserDesktop.UI.GenericElement;
using DesktopBrowser.UI.GenericElement;
using DesktopBrowser.UIControllers;
using DesktopBrowser.UIControllers.Toolbox;
using System;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UIControllers
{
    public class MenuBar_UIController : UIController
    {
        private MenuBarElement menuBar;
        private ToolboxItem_E avatar;
        private ToolboxItem_E sound;
        private ToolboxItem_E mic;

        public Action ToggleAvatarTracking;
        public Action ToggleAudio;
        public Action ToggleMic;

        public bool Initialized { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
            menuBar = new MenuBarElement(BindVisual("menu-bar-element"));
        }

        private void Start()
        {
            VisualTreeAsset toolVisual = GetUIController("toolboxItems").VisualTA;
            VisualTreeAsset toolboxVisual = GetUIController("toolboxes").VisualTA;
            VisualTreeAsset separatorVisual = GetUIController("toolboxSeparators").VisualTA;

            #region Left Layout

            menuBar.AddSpacerToLeftLayout();
            ToolboxItem_E tool_toolbox = new ToolboxItem_E(toolVisual)
            {
                ItemName = "Toolbox",
                ItemClicked = () =>
                {
                    Debug.Log("<color=green>TODO: </color>" + $"");
                }
            }.SetIcon("menuBar-toolbox", "menuBar-toolbox", true);
            Toolbox_E toolbox_left = new Toolbox_E(toolboxVisual, tool_toolbox)
            {
                toolboxName = ""
            };
            ToolboxSeparatorGenericElement separator_left = new ToolboxSeparatorGenericElement(separatorVisual);
            menuBar.AddLeft(toolbox_left, separator_left);

            #endregion

            #region Right Layout

            ToolboxSeparatorGenericElement separator_right0 = new ToolboxSeparatorGenericElement(separatorVisual);
            avatar = new ToolboxItem_E(toolVisual)
            {
                ItemName = "",
                ItemClicked = () =>
                {
                    //ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();
                    ToggleAvatarTracking();
                }
            }.SetIcon("menuBar-avatarOn", "menuBar-avatarOff", true);
            sound = new ToolboxItem_E(toolVisual)
            {
                ItemName = "",
                ItemClicked = () =>
                {
                    /*ActivateDeactivateAudio.Instance.ToggleAudioStatus();*/
                    ToggleAudio();
                }
            }.SetIcon("menuBar-soundOn", "menuBar-soundOff");
            mic = new ToolboxItem_E(toolVisual)
            {
                ItemName = "",
                ItemClicked = () =>
                {
                    /*ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();*/
                    ToggleMic();
                }
            }.SetIcon("menuBar-micOn", "menuBar-micOff");
            Toolbox_E toolbox_settings = new Toolbox_E(toolboxVisual, avatar, sound, mic)
            {
                toolboxName = ""
            };
            ToolboxSeparatorGenericElement separator_right1 = new ToolboxSeparatorGenericElement(separatorVisual);
            ToolboxItem_E tool_leave = new ToolboxItem_E(toolVisual)
            {
                ItemName = "",
                ItemClicked = () =>
                {
                    //Menu.DialogueBox_UIController.
                    //    Setup("Leave environment", "Are you sure ...?", "YES", "NO", (b) =>
                    //    {
                    //        if (b)
                    //            ConnectionMenu.Instance.Leave();
                    //    }).
                    //    DisplayFrom(uiDocument);
                }
            }.SetIcon("menuBar-leave", "menuBar-leave", true);
            Toolbox_E toolbox_leave = new Toolbox_E(toolboxVisual, tool_leave)
            {
                toolboxName = ""
            };
            menuBar.AddRight(separator_right0, toolbox_settings, separator_right1, toolbox_leave);
            menuBar.AddSpacerToRightLayout();

            #endregion

            Initialized = true;
        }

        public static void AddInMenu(ToolboxGenericElement toolbox)
        {
            //menuBar.ToolboxLayout.AddElement(toolbox);
        }

        public static void AddInSubMenu(ToolboxGenericElement subTools, ToolboxGenericElement parent)
        {
            //Instance.menuBar.AddInSubMenu(subTools, parent);
        }

        /// <summary>
        /// Event called when the status of the avatar tracking changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAvatarTrackingChanged(bool val)
        {
            avatar.ItemButton.SwitchClass(val);
        }
        /// <summary>
        /// Event called when the status of the audio changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAudioStatusChanged(bool val)
        {
            sound.ItemButton.SwitchClass(val);
        }
        /// <summary>
        /// Event called when the status of the microphone changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnMicrophoneStatusChanged(bool val)
        {
            mic.ItemButton.SwitchClass(val);
        }



    }
}