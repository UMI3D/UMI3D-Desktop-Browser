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

        /*[SerializeField]
        private */

        private ToolboxGenericElement openToolbox_TGE;
        private ToolboxGenericElement settings_TGE;
        private ToolboxGenericElement leaveEnvironment_TGE;

        void Start()
        {
            Debug.Assert(uiDocument != null, "uiDocument null ref in MenuBar_UIController");
            Debug.Assert(toolboxGenericElement_VTA != null, "toolboxGenericElement_VTA null ref in MenuBar_UIController");
            Debug.Assert(toolboxButtonGenericElement_VTA != null, "toolboxButtonGenericElement_VTA null ref in MenuBar_UIController");

            UIBinding();
            SetupMenuBar();
        }

        private void UIBinding()
        {
            var menuBar = uiDocument.rootVisualElement.Q<VisualElement>("menu-bar").Q<UI.CustomElement.MenuBarElement>();
            openToolbox_TGE = menuBar.Q<VisualElement>("toolbox-openBtn").Q<ToolboxGenericElement>();
            settings_TGE = menuBar.Q<VisualElement>("environment-settings").Q<ToolboxGenericElement>();
            leaveEnvironment_TGE = menuBar.Q<VisualElement>("environment-leaveBtn").Q<ToolboxGenericElement>();
        }

        private void SetupMenuBar()
        {
            openToolbox_TGE.Setup("");
            settings_TGE.Setup("");
            leaveEnvironment_TGE.Setup("");

            ToolboxButtonGenericElement openToolboxButton_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            openToolboxButton_TBGE.Setup("Toolbox", "toolbox", "toolbox", true, () => { Debug.Log("TODO"); });
            openToolbox_TGE.AddTool(openToolboxButton_TBGE);

            ToolboxButtonGenericElement avatar_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            avatar_TBGE.Setup("", "avatarOn", "avatarOff", true, () => { Debug.Log("TODO"); });
            //settings_TGE.AddTool(avatar_TBGE);

            ToolboxButtonGenericElement sound_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            sound_TBGE.Setup("", "soundOn", "soundOff", true, () => { Debug.Log("TODO"); });
            //settings_TGE.AddTool(sound_TBGE);

            ToolboxButtonGenericElement mic_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            mic_TBGE.Setup("", "micOn", "micOff", true, () => { Debug.Log("TODO"); });
            //settings_TGE.AddTool(mic_TBGE);

            settings_TGE.AddTools(new ToolboxButtonGenericElement[3] { avatar_TBGE, sound_TBGE, mic_TBGE });

            ToolboxButtonGenericElement leave_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            leave_TBGE.Setup("", "leave", "leave", true, () => { Debug.Log("TODO"); });
            leaveEnvironment_TGE.AddTool(leave_TBGE);
        }

    }
}