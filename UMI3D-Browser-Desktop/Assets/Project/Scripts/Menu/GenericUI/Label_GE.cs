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
    public class Label_GE : AbstractGenericAndCustomElement
    {
        public new class UxmlFactory : UxmlFactory<Label_GE, UxmlTraits> { }

        private Label label_L;
        private string text;
        private string textPref;

        private UserPreferences.TextPreferences_SO.TextPref.TextFormat textFormat;

        protected override void Initialize()
        {
            base.Initialize();

            this.label_L = this.Q<Label>("label");
        }

        public Label_GE Setup(string text, string textPref, bool isReadyToDisplay = false)
        {
            Initialize();

            this.text = text;
            this.textPref = textPref;

            if (isReadyToDisplay)
                ReadyToDisplay();

            return this;
        }

        public void Update(string text)
        {
            this.text = text;
            textFormat.SetFormat(label_L, text);
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed) return;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(label_L, textPref, text);
            textFormat = UserPreferences.UserPreferences.TextAndIconPref.GetTextFormat(textPref);
        }
    }
}
