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

        private bool isOn = false;

        public void Setup(string buttonName, Sprite buttonImage, Action buttonClicked)
        {
            button_B = this.Q<Button>("toolbox-button");
            buttonName_L = this.Q<Label>("toolbox-button-name");

            buttonName_L.text = buttonName;
            button_B.style.backgroundImage = Background.FromSprite(buttonImage);
            button_B.clicked += buttonClicked;
        }

        public void Setup(string buttonName, string classNameOn, string classNameOff, bool isOn, Action buttonClicked)
        {
            Setup(buttonName, buttonClicked);

            SwitchClass(isOn, classNameOn, classNameOff);

            button_B.clicked += () =>
            {
                SwitchClass(!this.isOn, classNameOn, classNameOff);
                buttonClicked();
            };

        }

        private void SwitchClass(bool value, string classNameOn, string classNameOff)
        {
            isOn = value;
            string className = "darkTheme-menuBar-";
            if (value)
            {
                className += classNameOn + "-btn";
            }
            else
            {
                className += classNameOff + "-btn";
            }
            button_B.ClearClassList();
            button_B.AddToClassList(className);
        }

        private void Setup(string buttonName, Action buttonClicked)
        {
            button_B = this.Q<Button>("toolbox-button");
            buttonName_L = this.Q<Label>("toolbox-button-name");

            buttonName_L.text = buttonName;
        }

    }
}