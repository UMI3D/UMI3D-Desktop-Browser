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

using BrowserDesktop.UI.GenericElement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI.CustomElement
{
    public class ToolboxMenuBarSV_E : AbstractGenericAndCustomElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ToolboxMenuBarSV_E, UxmlTraits> { }

        private Button_GE backward_BGE;
        private Button_GE forward_BGE;
        private ScrollView toolbox_SV;

        protected override void Initialize()
        {
            base.Initialize();

            backward_BGE = this.Q<VisualElement>("backward-B").Q<Button_GE>();
            forward_BGE = this.Q<VisualElement>("forward-B").Q<Button_GE>();
            toolbox_SV = this.Q<ScrollView>("toolbox-SV");
        }

        public void Setup()
        {
            Initialize();

            backward_BGE.
                Setup().
                WithBackgroundImage("", "", "");
            forward_BGE.
                Setup().
                WithBackgroundImage("", "", ""); ;
        }

        public override void OnApplyUserPreferences()
        {
            if (!displayed)
                return;
        }
    }
}