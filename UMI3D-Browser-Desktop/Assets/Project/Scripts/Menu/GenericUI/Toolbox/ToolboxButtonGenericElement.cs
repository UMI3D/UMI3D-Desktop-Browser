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
    public class ToolboxButtonGenericElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolboxButtonGenericElement, UxmlTraits> { }
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Button button_B;
        private Label buttonName_L;

        public void Setup(Sprite buttonImage, string buttonName)
        {
            button_B = this.Q<Button>("toolbox-button");
            buttonName_L = this.Q<Label>("toolbox-button-name");

            button_B.style.backgroundImage = Background.FromSprite(buttonImage);
            buttonName_L.text = buttonName;
        }

    }
}