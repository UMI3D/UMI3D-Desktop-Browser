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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI.GenericElement
{
    public class ToolboxGenericElement : AbstractGenericAndCustomElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolboxGenericElement, UxmlTraits> { }

        private string toolboxNameText;
        private Label toolboxName_L;
        private VisualElement toolboxContainer_VE;

        private List<ToolboxButtonGenericElement> buttons = new List<ToolboxButtonGenericElement>();

        protected override void Initialize()
        {
            base.Initialize();

            toolboxName_L = this.Q<Label>("toolbox-name");
            toolboxContainer_VE = this.Q<VisualElement>("toolbox-container");
        }

        public ToolboxGenericElement Setup(string toolboxName, ToolboxButtonGenericElement tool)
        {
            Setup(toolboxName);
            AddTool(tool);
            return this;
        }

        public ToolboxGenericElement Setup(string toolboxName, ToolboxButtonGenericElement[] tools)
        {
            Setup(toolboxName);
            AddTools(tools);
            return this;
        }

        public ToolboxGenericElement Setup(string toolboxName)
        {
            Initialize();

            toolboxNameText = toolboxName;

            return this;
        }

        private void AddTools(ToolboxButtonGenericElement[] tools)
        {
            for (int i = 0; i < tools.Length; ++i)
            {
                if (i > 0)
                {
                    AddSpacer();
                }

                tools[i].AddTo(toolboxContainer_VE);
                buttons.Add(tools[i]);
            }
        }

        private void AddSpacer()
        {
            VisualElement horizontalSpacer = new VisualElement();
            horizontalSpacer.style.width = 10;
            toolboxContainer_VE.Add(horizontalSpacer);
        }

        public ToolboxGenericElement AddTool(ToolboxButtonGenericElement tool)
        {
            if (buttons.Count > 0)
                AddSpacer();
            tool.AddTo(toolboxContainer_VE);
            buttons.Add(tool);
            return this;
        }

        public override void Remove()
        {
            base.Remove();
            buttons.ForEach((tool) => { tool.Remove(); });
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public override void OnApplyUserPreferences()
        {
            if (!Displayed) return;
            
            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(toolboxName_L, "sub-section", toolboxNameText);
        }

    }
}
