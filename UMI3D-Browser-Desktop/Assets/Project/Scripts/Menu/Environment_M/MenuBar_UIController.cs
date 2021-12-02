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

using BrowserDesktop.UI.GenericElement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Environment
{
    public class MenuBar_UIController : umi3d.common.Singleton<MenuBar_UIController>
    {
        [SerializeField]
        private UIDocument uiDocument;

        [SerializeField]
        private VisualTreeAsset toolboxGenericElement_VTA;
        [SerializeField]
        private VisualTreeAsset toolboxButtonGenericElement_VTA;
        [SerializeField]
        private VisualTreeAsset toolboxSeparatorGenericElement_VTA;

        private UI.CustomElement.MenuBarElement menuBar;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(uiDocument != null, "uiDocument null ref in MenuBar_UIController");
            Debug.Assert(toolboxGenericElement_VTA != null, "toolboxGenericElement_VTA null ref in MenuBar_UIController");
            Debug.Assert(toolboxButtonGenericElement_VTA != null, "toolboxButtonGenericElement_VTA null ref in MenuBar_UIController");
            Debug.Assert(toolboxSeparatorGenericElement_VTA != null, "toolboxSeparatorGenericElement_VTA null ref in MenuBar_UIController");

            menuBar = uiDocument.rootVisualElement.Q<VisualElement>("menu-bar").Q<UI.CustomElement.MenuBarElement>();
            menuBar.Setup(toolboxGenericElement_VTA, toolboxButtonGenericElement_VTA, toolboxSeparatorGenericElement_VTA, uiDocument);
        }

        public void AddToolbox(ToolboxGenericElement toolbox)
        {
            menuBar.ToolboxLayout.AddElement(toolbox);
        }

        public static void AddInSubMenu(ToolboxGenericElement subTools, ToolboxGenericElement parent)
        {
            Debug.Assert(Exists, "MenuBar_UIController does not Exists.");

            Instance.menuBar.AddInSubMenu(subTools, parent);
        }

        /// <summary>
        /// Event called when the status of the microphone changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnMicrophoneStatusChanged(bool val)
        {
            menuBar.Mic_TBGE.SwitchClass(val);
        }

        /// <summary>
        /// Event called when the status of the audio changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAudioStatusChanged(bool val)
        {
            menuBar.Sound_TBGE.SwitchClass(val);
        }

        /// <summary>
        /// Event called when the status of the avatar tracking changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAvatarTrackingChanged(bool val)
        {
            menuBar.Avatar_TBGE.SwitchClass(val);
        }



    }
}