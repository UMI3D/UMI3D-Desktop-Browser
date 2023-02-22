/*
Copyright 2019 - 2022 Inetum

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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class DropdownDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        protected DropDownInputMenuItem menuItem;

        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ElemnetDirection Direction = ElemnetDirection.Leading;

        Dropdown_C dropdown;

        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is DropDownInputMenuItem)
            {
                menuItem = item as DropDownInputMenuItem;
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a DropDownInput");
        }

        public void InitAndBindUI()
        {
            if (dropdown != null) return;

            dropdown = new Dropdown_C
            {
                Category = Category,
                Size = Size,
                Direction = Direction
            };
            dropdown.name = gameObject.name;
            dropdown.LocalisedLabel = menuItem.ToString();
            dropdown.LocalisedOptions = menuItem.options;
            dropdown.index = menuItem.options.IndexOf(menuItem.GetValue());
            dropdown.RegisterValueChangedCallback(OnValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            dropdown.UnregisterValueChangedCallback(OnValueChanged);
            dropdown.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false) => dropdown.Display();
        public override void Hide() => dropdown.Hide();

        public VisualElement GetUXMLContent() => dropdown;

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is DropDownInputMenuItem) ? 2 : 0;
        }

        private void OnValueChanged(ChangeEvent<string> e)
        {
            if (e.previousValue == e.newValue) return;
            menuItem.NotifyValueChange(e.newValue);
        }

        private void OnDestroy()
        {
            dropdown?.RemoveFromHierarchy();
        }
    }
}