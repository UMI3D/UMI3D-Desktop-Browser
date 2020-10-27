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

using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class DropDownInputDisplayerElement : AbstractDropDownInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset dropdownTreeAsset;

        public bool isInGame = false;

        DropdownElement dropdown;

        public override void Clear()
        {
            base.Clear();
            dropdown.RemoveFromHierarchy();
            dropdown.OnValueChanged -= OnValueChanged;
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();
            dropdown.ClearOptions();
            dropdown.SetOptions(menuItem.options);
            dropdown.SetValue(menuItem.options.IndexOf(GetValue()));
            dropdown.OnValueChanged += OnValueChanged;
            dropdown.SetLabel(menuItem.ToString());

            dropdown.style.display = DisplayStyle.Flex;
        }

        private void OnValueChanged(int i)
        {
            NotifyValueChange(menuItem.options[i]);
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return dropdown;
        }

        public override void Hide()
        {
            dropdown.OnValueChanged -= OnValueChanged;
            dropdown.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (dropdown == null)
            {
                dropdown = dropdownTreeAsset.CloneTree().Q<DropdownElement>();
                dropdown.SetUp(isInGame);
            }
        }

        private void OnDestroy()
        {
            dropdown?.RemoveFromHierarchy();
        }
    }
}