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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI.GenericElement
{
    public class ToolBoxGenericElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolBoxGenericElement, UxmlTraits> { }
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Label toolboxName_L;
        private VisualElement toolboxContainer_VE;

        public void Setup(string toolboxName)
        {
            toolboxName_L = this.Q<Label>("toolbox-name");
            toolboxContainer_VE = this.Q<VisualElement>("toolbox-container");

            toolboxName_L.text = toolboxName;
        }

        public void AddTool(ToolboxButtonGenericElement tool)
        {
            toolboxContainer_VE.Add(tool);

            //TODO resize container.
        }
        
    }
}
