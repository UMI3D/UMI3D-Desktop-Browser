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
        public new class UxmlFactory : UxmlFactory<Button_GE, UxmlTraits> { }

        private Button button_B;
        private string text;
        private string textPref;
        private string iconClass;
        private string iconPref;

        protected override void Initialize()
        {
            base.Initialize();

            button_B = this.Q<Button>("button");
        }

        public Button_GE Setup()
        {
            Initialize();

            this.text = "";
            this.textPref = "";
            this.iconClass = "";
            this.iconPref = "";

            return this;
        }

        public Button_GE WithBackgroundImage(string iconClass, string iconPref)
        {
            this.iconClass = iconClass;
            this.iconPref = iconPref;

            return this;
        }

        public Button_GE WithText(string text, string textPref)
        {
            this.text = text;
            this.textPref = textPref;
            
            return this;
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed) return;

            if (!string.IsNullOrEmpty(textPref))
                UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(button_B, textPref, text);
            if (!string.IsNullOrEmpty(iconPref))
                UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(button_B, iconPref, iconClass);
        }
    }
}