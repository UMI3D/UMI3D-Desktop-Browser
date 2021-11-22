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
    public class ToolboxButtonGenericElement : AbstractGenericAndCustomElement
    {
        #region Fields

        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolboxButtonGenericElement, UxmlTraits> { }

        public float LabelWidth
        {
            get => buttonName_L.resolvedStyle.width;
            set => buttonName_L.style.width = value;
        }

        public string LabelText => buttonName_L.text;

        private string buttonNameText;
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
        private string classOn;
        private string classOff;
        private string currentClass;

        #endregion

        protected override void Initialize()
        {
            base.Initialize();
            button_B = this.Q<Button>("toolbox-button");
            buttonName_L = this.Q<Label>("toolbox-button-name");
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

            this.classOn = classNameOn;
            this.classOff = classNameOff;
            SwitchClass(isOn);

            button_B.clicked += buttonClicked;
        }

        /// <summary>
        /// Setup the label of the button and bind the UI.
        /// </summary>
        /// <param name="buttonName">Label of the button.</param>
        private void Setup(string buttonName)
        {
            Initialize();

            buttonNameText = buttonName;
        }

        /// <summary>
        /// Switch between USS classes when button is on or off.
        /// </summary>
        /// <param name="value">True if the button should be on, false else.</param>
        public void SwitchClass(bool value)
        {
            isOn = value;
            string className = "darkTheme-menuBar-"; //TODO to be replace by theme checked.
            if (value)
            {
                currentClass = className + classOn + "-btn";
            }
            else
            {
                currentClass = className + classOff + "-btn";
            }
            button_B.ClearClassList();
            button_B.AddToClassList(currentClass);
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public override void OnApplyUserPreferences()
        {
            //TODO
            if (!displayed)
                return;

            buttonName_L.style.width = StyleKeyword.Auto;
            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(buttonName_L, "label", buttonNameText);
            UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(button_B, "square-button", iconClass: currentClass);
        }

    }
}