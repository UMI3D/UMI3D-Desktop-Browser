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
    public class ToolboxGenericElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolboxGenericElement, UxmlTraits> { }
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Label toolboxName_L;
        private VisualElement toolboxContainer_VE;

        private List<ToolboxButtonGenericElement> tools_TBGEs = new List<ToolboxButtonGenericElement>();
        private float labelToolsMaxWidth = 0f;

        public ToolboxGenericElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
        }

        ~ToolboxGenericElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
        }

        public void Setup(string toolboxName, ToolboxButtonGenericElement tool)
        {
            Setup(toolboxName);
            AddTool(tool);
            OnApplyUserPreferences();
        }

        public void Setup(string toolboxName, ToolboxButtonGenericElement[] tools)
        {
            Setup(toolboxName);
            AddTools(tools);
            OnApplyUserPreferences();
        }

        private void Setup(string toolboxName)
        {
            toolboxName_L = this.Q<Label>("toolbox-name");
            toolboxContainer_VE = this.Q<VisualElement>("toolbox-container");

            toolboxName_L.text = toolboxName;
        }

        private void AddTools(ToolboxButtonGenericElement[] tools)
        {
            for (int i = 0; i < tools.Length; ++i)
            {
                if (i > 0)
                {
                    VisualElement horizontalSpacer = new VisualElement();
                    horizontalSpacer.style.width = 10;
                    toolboxContainer_VE.Add(horizontalSpacer);
                }

                tools_TBGEs.Add(tools[i]);
                toolboxContainer_VE.Add(tools[i]);
            }
        }

        private void AddTool(ToolboxButtonGenericElement tool)
        {
            toolboxContainer_VE.Add(tool);

            //TODO resize container.
        }

        private void AddTool(string toolName, Sprite toolIcon, Action toolAction)
        {

        }

        private IEnumerator ResizeLabelTools()
        {
            yield return null;
            yield return null;
            labelToolsMaxWidth = 0f;
            foreach (ToolboxButtonGenericElement tool in tools_TBGEs)
            {
                if (labelToolsMaxWidth < tool.LabelWidth) labelToolsMaxWidth = tool.LabelWidth;
                Debug.Log("Label width (bis) = " + tool.LabelWidth);
                tool.TestWidth();
            }
            /*foreach (ToolboxButtonGenericElement tool in tools_TBGEs)
            {
                tool.LabelWidth = labelToolsMaxWidth;
            }*/
            Debug.Log("Max width = " + labelToolsMaxWidth);
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public void OnApplyUserPreferences()
        {
            //TODO
            UserPreferences.UserPreferences.FontPref.ApplyFont(toolboxName_L, "sub-title");
            UserPreferences.UserPreferences.Instance.StartCoroutine(ResizeLabelTools());
        }

    }
}
