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
        }

        private void SetupMenuBar()
        {
            openToolbox_TGE.Setup("");

            ToolboxButtonGenericElement openToolboxButton_TBGE = toolboxButtonGenericElement_VTA.CloneTree().Q<ToolboxButtonGenericElement>();
            //openToolboxButton_TBGE.Setup()
            openToolbox_TGE.AddTool(openToolboxButton_TBGE);
        }

    }
}