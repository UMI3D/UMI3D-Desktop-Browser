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
    public class ToolboxSeparatorGenericElement : Visual_E
    {
        private VisualElement separator_VE;

        //public ToolboxSeparatorGenericElement(string visualResourcePath, ) : base(visualResourcePath) { }

        protected override void Initialize()
        {
            base.Initialize();

            separator_VE = this.Q<VisualElement>("separator");
        }

        public ToolboxSeparatorGenericElement Setup(bool isReadyToDisplay = false)
        {
            Initialize();

            //separator_VE.AddToClassList("darkTheme-menuBar-separator");

            if (isReadyToDisplay)
                ReadyToDisplay();

            return this;
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed) return;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyIconPref(separator_VE, "separator");
        }
    }
}