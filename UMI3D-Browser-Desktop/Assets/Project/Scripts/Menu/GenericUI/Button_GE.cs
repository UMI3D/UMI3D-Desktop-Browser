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
    public class Button_GE : AbstractGenericAndCustomElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Button_GE, UxmlTraits> { }

        private Button button_B;

        private string text;
        private string textPref;

        /// <summary>
        /// State of the button.
        /// </summary>
        private bool isOn = false;

        private string classOn;
        private string classOff;
        private string iconPref;
        private string currentClass;
        private IEnumerable<string> USSClassesIconPref = new List<string>();

        private System.Action OnClicked;

        protected override void Initialize()
        {
            base.Initialize();

            button_B = this.Q<Button>("button");
        }

        public Button_GE Setup(System.Action onClicked, bool isOn = true, bool isReadyToDisplay = false)
        {
            Initialize();

            this.text = "";
            this.textPref = "";
            //button_B.text = "";

            this.classOn = "";
            this.classOff = "";
            this.currentClass = "";
            this.iconPref = "";

            this.isOn = isOn;

            OnClicked = onClicked;
            button_B.clicked += OnClicked;

            if (isReadyToDisplay)
                ReadyToDisplay();

            return this;
        }

        public Button_GE WithBackgroundImage(string classOn, string classOff, string iconPref)
        {
            this.classOn = classOn;
            this.classOff = classOff;
            this.iconPref = iconPref;

            SwitchClass(this.isOn);

            return this;
        }

        public Button_GE WithText(string text, string textPref)
        {
            this.text = text;
            this.textPref = textPref;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(button_B, textPref, text);

            return this;
        }

        /// <summary>
        /// Switch between USS classes when button is on or off.
        /// </summary>
        /// <param name="value">True if the button should be on, false else.</param>
        public void SwitchClass(bool value)
        {
            isOn = value;
            string className = "darkTheme-"; //TODO to be replace by theme checked.
            if (value)
            {
                currentClass = className + classOn + "-btn";
            }
            else
            {
                currentClass = className + classOff + "-btn";
            }

            UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(button_B, iconPref, currentClass);
        }

        //private void UpdateUSSClasses()
        //{
        //    button_B.ClearClassList();

        //    button_B.AddToClassList(currentClass);
        //    foreach (string USSClass in USSClassesIconPref)
        //        button_B.AddToClassList(USSClass);
        //}

        public override void Remove()
        {
            base.Remove();

            button_B.clicked -= OnClicked;
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed) return;

            if (!string.IsNullOrEmpty(textPref))
                UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(button_B, textPref, text);
            else
                button_B.text = "";

            if (!string.IsNullOrEmpty(iconPref))
                UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(button_B, iconPref, currentClass);
            else
                button_B.ClearClassList();
        }
    }
}