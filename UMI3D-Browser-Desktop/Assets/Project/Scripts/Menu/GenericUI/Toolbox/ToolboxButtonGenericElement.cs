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

        /// <summary>
        /// Button with its icon.
        /// </summary>
        private Button button_B;
        /// <summary>
        /// Button's label.
        /// </summary>
        private Label buttonName_L;

        /// <summary>
        /// State of the button.
        /// </summary>
        private bool isOn = false;

        public float LabelWidth
        {
            get => buttonName_L.resolvedStyle.width;
            set => buttonName_L.style.width = value;
        }

        public string LabelText => buttonName_L.text;

        public ToolboxButtonGenericElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
        }

        ~ToolboxButtonGenericElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
        }

        /// <summary>
        /// Setup the button (label, icon and action).
        /// </summary>
        /// <param name="buttonName">Label of the button.</param>
        /// <param name="buttonImage">Icon of the button.</param>
        /// <param name="buttonClicked">Action when button is clicked.</param>
        public void Setup(string buttonName, Sprite buttonImage, Action buttonClicked)
        {
            Setup(buttonName);

            button_B.style.backgroundImage = Background.FromSprite(buttonImage);
            button_B.clicked += buttonClicked;

            OnApplyUserPreferences();
        }

        /// <summary>
        /// Setup the button (label, USS classes, initial state and action).
        /// </summary>
        /// <param name="buttonName">Label of the button.</param>
        /// <param name="classNameOn">USS class when button is On.</param>
        /// <param name="classNameOff">USS class when the button is Off.</param>
        /// <param name="isOn">Initial state of the button (true when active, else false).</param>
        /// <param name="buttonClicked">Action when button is clicked.</param>
        public void Setup(string buttonName, string classNameOn, string classNameOff, bool isOn, Action buttonClicked)
        {
            Setup(buttonName);

            SwitchClass(isOn, classNameOn, classNameOff);

            button_B.clicked += () =>
            {
                SwitchClass(!this.isOn, classNameOn, classNameOff);
                buttonClicked();
            };

            OnApplyUserPreferences();
        }

        /// <summary>
        /// Setup the label of the button and bind the UI.
        /// </summary>
        /// <param name="buttonName">Label of the button.</param>
        private void Setup(string buttonName)
        {
            button_B = this.Q<Button>("toolbox-button");
            buttonName_L = this.Q<Label>("toolbox-button-name");

            buttonName_L.text = buttonName;
        }

        /// <summary>
        /// Switch between USS classes when button is on or off.
        /// </summary>
        /// <param name="value">True if the button should be on, false else.</param>
        /// <param name="classNameOn">USS class when the button is On.</param>
        /// <param name="classNameOff">USS class when the button is Off.</param>
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

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public void OnApplyUserPreferences()
        {
            //TODO
            buttonName_L.style.width = StyleKeyword.Auto;
            UserPreferences.UserPreferences.FontPref.ApplyFont(buttonName_L, "label");
        }

    }
}