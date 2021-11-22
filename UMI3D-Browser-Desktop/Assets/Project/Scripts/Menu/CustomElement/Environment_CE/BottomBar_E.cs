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

namespace BrowserDesktop.UI.CustomElement
{
    public class BottomBar_E : AbstractGenericAndCustomElement
    {
        public new class UxmlFactory : UxmlFactory<BottomBar_E, UxmlTraits> { }

        private VisualElement leftLayout_VE;
        private VisualElement rightLayout_VE;

        protected override void Initialize()
        {
            base.Initialize();

            leftLayout_VE = this.Q<VisualElement>("left-layout");
            rightLayout_VE = this.Q<VisualElement>("right-layout");
        }

        public BottomBar_E SetupAndDisplay()
        {

            ReadyToDisplay();

            return this;
        }

        public override void OnApplyUserPreferences()
        {
            
        }
    }
}