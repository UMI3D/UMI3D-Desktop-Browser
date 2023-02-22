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
    /// <summary>
    /// Toggle displayer.
    /// </summary>
    public class ToggleDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        public string LabelText;
        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ElemnetDirection Direction = ElemnetDirection.Leading;

        protected BooleanInputMenuItem menuItem;
        protected Toggle_C toggle;

        private void OnValidate()
        {
            if (toggle != null)
            {
                toggle.Category = Category;
                toggle.Size = Size;
                toggle.Direction = Direction;
                toggle.LocaliseLabel = LabelText;
            }
        }

        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is BooleanInputMenuItem)
            {
                menuItem = item as BooleanInputMenuItem;
                LabelText = menuItem.ToString();
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a BooleanInput");
        }

        public void InitAndBindUI()
        {
            if (toggle != null) return;

            toggle = new Toggle_C 
            { 
                Category = Category, 
                Size = Size, 
                Direction = Direction 
            };
            toggle.name = gameObject.name;
            toggle.LocaliseLabel = LabelText;
            toggle.RegisterValueChangedCallback(OnValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            toggle.UnregisterValueChangedCallback(OnValueChanged);
            toggle.RemoveFromHierarchy();
        }


        public override void Display(bool forceUpdate = false) => toggle.Display();
        public override void Hide() => toggle.Hide();

        public VisualElement GetUXMLContent() => toggle;

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is BooleanInputMenuItem) ? 2 : 0;
        }

        private void OnValueChanged(ChangeEvent<bool> e)
        {
            if (e.previousValue == e.newValue) return;
            menuItem.NotifyValueChange(e.newValue);
        }

        private void OnDestroy()
        {
            toggle?.RemoveFromHierarchy();
        }
    }
}