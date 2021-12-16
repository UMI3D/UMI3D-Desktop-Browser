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
using System.Collections.Generic;
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
        private ScrollView scrollView_SV;

        private List<AbstractGenericAndCustomElement> elements = new List<AbstractGenericAndCustomElement>();
        private AbstractGenericAndCustomElement currentElement;

        private System.Action<VisualElement> addSeparator;

        protected override void Initialize()
        {
            base.Initialize();

            backward_BGE = this.Q<VisualElement>("backward-B").Q<Button_GE>();
            forward_BGE = this.Q<VisualElement>("forward-B").Q<Button_GE>();
            scrollView_SV = this.Q<ScrollView>("toolbox-SV");
        }

        public ToolboxMenuBarSV_E Setup(string buttonClass, string buttonIconPref, System.Action<VisualElement> addSeparator = null)
        {
            Initialize();

            backward_BGE.
                Setup(isReadyToDisplay: true, onClicked: () =>
                {
                    //scrollView_SV.ScrollTo(currentElement);
                }).
                WithBackgroundImage($"{buttonClass}-previous", $"{buttonClass}-previous-disable", buttonIconPref);

            this.Q<VisualElement>("left-separator").Q<ToolboxSeparatorGenericElement>().Setup(true);

            forward_BGE.
                Setup(isReadyToDisplay: true, onClicked: () =>
                {
                    //scrollView_SV.ScrollTo(currentElement);
                }).
                WithBackgroundImage($"{buttonClass}-next", $"{buttonClass}-next-disable", buttonIconPref);

            this.Q<VisualElement>("right-separator").Q<ToolboxSeparatorGenericElement>().Setup(true);

            this.addSeparator = addSeparator;

            return this;
        }

        public void AddElement(AbstractGenericAndCustomElement element)
        {
            element.AddTo(scrollView_SV);
            addSeparator?.Invoke(scrollView_SV);
            elements.Add(element);
        }

        public void AddElements(IEnumerable<AbstractGenericAndCustomElement> elements)
        {
            foreach (AbstractGenericAndCustomElement elt in elements)
            {
                AddElement(elt);
            }
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed)
                return;
        }
    }
}