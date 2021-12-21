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
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Environment
{
    public class MenuBar_UIController : Singleton<MenuBar_UIController>
    {
        [SerializeField]
        private GlobalUIController<MenuBarElement> controller = new GlobalUIController<MenuBarElement>();

        private MenuBarElement menuBar;
        private ToolboxButtonGenericElement avatar;
        private ToolboxButtonGenericElement sound;
        private ToolboxButtonGenericElement mic;

        protected override void Awake()
        {
            base.Awake();
            menuBar = controller.
                BindVisual("menu-bar").
                    Setup();
        }

        private void Start()
        {
            #region Left Layout

            VisualTreeAsset ToolVisual = Tools_UIController.Visual;
            VisualTreeAsset ToolboxVisual = Toolboxes_UIController.Visual;

            menuBar.AddSpacerToLeftLayout();
            ToolboxItem_E toolbox_tool = new ToolboxItem_E(ToolVisual)
            {
                ItemName = "Toolbox",
                ItemClicked = () =>
                {
                    Debug.Log("<color=green>TODO: </color>" + $"");
                }
            }.SetIcon("toolbox", "toolbox");
            Toolbox_E toolbox_left = new Toolbox_E(ToolboxVisual, toolbox_tool)
            {
                toolboxName = ""
            };
            menuBar.AddLeft(toolbox_left);
            menuBar.
                AddLeft(
                Separator_UIController.
                    CloneAndSetup());

            #endregion

            #region Right Layout

            menuBar.
                AddRight(
                Separator_UIController.
                    CloneAndSetup());
            avatar = Tools_UIController.
                CloneAndSetup(
                "",
                "avatarOn",
                "avatarOff",
                true,
                () => { /*ActivateDeactivateAvatarTracking.Instance.ToggleTrackingStatus();*/ });
            sound = Tools_UIController.
                CloneAndSetup(
                "",
                "soundOn",
                "soundOff",
                true,
                () => { /*ActivateDeactivateAudio.Instance.ToggleAudioStatus();*/ });
            mic = Tools_UIController.
                CloneAndSetup(
                "",
                "micOn",
                "micOff",
                false,
                () => { /*ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus();*/ });
            menuBar.
                AddRight(
                Toolboxes_UIController.
                    CloneAndSetup("").
                        AddTool(avatar).
                        AddTool(sound).
                        AddTool(mic));
            menuBar.
                AddRight(
                Separator_UIController.
                    CloneAndSetup());
            ToolboxButtonGenericElement leave = Tools_UIController.
                CloneAndSetup(
                "",
                "leave",
                "leave",
                true,
                () =>
                {
                    //Menu.DialogueBox_UIController.
                    //    Setup("Leave environment", "Are you sure ...?", "YES", "NO", (b) =>
                    //    {
                    //        if (b)
                    //            ConnectionMenu.Instance.Leave();
                    //    }).
                    //    DisplayFrom(uiDocument);
                });
            menuBar.
                AddRight(
                Toolboxes_UIController.
                    CloneAndSetup("").
                        AddTool(leave));
            menuBar.
                AddSpacerToRightLayout();

            #endregion
        }

        public static void AddInMenu(ToolboxGenericElement toolbox)
        {
            Debug.Assert(Exists, "MenuBar_UIController does not Exists.");
            Instance.menuBar.ToolboxLayout.AddElement(toolbox);
        }

        public static void AddInSubMenu(ToolboxGenericElement subTools, ToolboxGenericElement parent)
        {
            Debug.Assert(Exists, "MenuBar_UIController does not Exists.");
            Instance.menuBar.AddInSubMenu(subTools, parent);
        }

        /// <summary>
        /// Event called when the status of the avatar tracking changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAvatarTrackingChanged(bool val)
        {
            avatar.SwitchClass(val);
        }
        /// <summary>
        /// Event called when the status of the audio changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAudioStatusChanged(bool val)
        {
            sound.SwitchClass(val);
        }
        /// <summary>
        /// Event called when the status of the microphone changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnMicrophoneStatusChanged(bool val)
        {
            mic.SwitchClass(val);
        }



    }
}