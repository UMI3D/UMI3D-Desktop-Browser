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
using DesktopBrowser.UI.CustomElement;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    public partial class Toolbox_E
    {
        public string toolboxName { get; set; }
        public float SpaceBetweenItems { get; set; } = 10f;

        private List<ToolboxItem_E> items = new List<ToolboxItem_E>();
    }

    public partial class Toolbox_E
    {
        public Toolbox_E(string visualResourcePath, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys, string toolboxName, params ToolboxItem_E[] items) : 
            base(visualResourcePath, styleResourcePath, formatAndStyleKeys)
        {
            //Update toolbox name.
            AddItems(items);
        }

        public Toolbox_E AddItems(params ToolboxItem_E[] toolItems)
        {
            //for (int i = 0; i < toolItems.Length; ++i)
            //{
            //    if (i > 0 || this.items.Count > 0) AddSpacer();
            //    toolItems[i].AddTo(itemsContainer);
            //    this.items.Add(toolItems[i]);
            //}
            return this;
        }

        //private void AddSpacer()
        //{
        //    VisualElement horizontalSpacer = new VisualElement();
        //    horizontalSpacer.style.width = SpaceBetweenItems;
        //    itemsContainer.Add(horizontalSpacer);
        //}
    }

    public partial class Toolbox_E : ScrollView_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            //label = this.Q<Label>("toolbox-name");
            //itemsContainer = this.Q<VisualElement>("items-container");
        }

        public override void Remove()
        {
            base.Remove();
            items.ForEach((tool) => { tool.Remove(); });
        }

        ///// <summary>
        ///// Apply user preferences when needed.
        ///// </summary>
        //public override void OnApplyUserPreferences()
        //{
        //    if (!Displayed) return;

        //    UserPreferences.TextAndIconPref.ApplyTextPref(label, "sub-section", toolboxName);
        //}
    }
}

