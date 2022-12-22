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
    /// Displayer for button that display text.
    /// </summary>
    public class ButtonDisplayer : AbstractDisplayer, baseBrowser.Menu.IDisplayerElement
    {
        public string Label;
        public string Text;
        public ElementCategory Category;
        public ElementSize Size = ElementSize.Medium;
        public ButtonShape Shape = ButtonShape.Square;
        public ButtonType Type = ButtonType.Default;
        public ElemnetDirection LabelDirection = ElemnetDirection.Leading;
        public ElementAlignment IconAlignment = ElementAlignment.Center;

        /// <summary>
        /// Button menu item to display.
        /// </summary>
        public ButtonMenuItem menuItem;
        /// <summary>
        /// A button that display a text.
        /// </summary>
        protected Button_C button;

        private void OnValidate()
        {
            if (button == null) return;
            button.Set(Category, Size, Shape, Type, Label, LabelDirection, IconAlignment);
            button.text = Text;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.Exception"></exception>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is ButtonMenuItem)
            {
                menuItem = item as ButtonMenuItem;
                Text = menuItem.Name;
                InitAndBindUI();
            }
            else throw new System.Exception("MenuItem must be a ButtonInput");
        }

        public void InitAndBindUI()
        {
            if (button != null) return;

            button = new Button_C(Category, Size, Shape, Type, Label, LabelDirection, IconAlignment);
            button.name = gameObject.name;
            button.text = Text;
            if (menuItem.IsHoldable)
            {
                button.ClickedDown += OnClickedDown;
                button.ClickedUp += OnClickedUp;
            }
            else button.clicked += OnClicked;
        }

        public override void Clear()
        {
            base.Clear();
            if (menuItem.IsHoldable)
            {
                button.ClickedDown -= OnClickedDown;
                button.ClickedUp -= OnClickedUp;
            }
            else button.clicked -= OnClicked;
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false) => button.Display();

        public override void Hide() => button.Hide();

        public VisualElement GetUXMLContent() => button;

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : 0;
        }

        public void OnClickedDown() => menuItem.NotifyValueChange(true);
        public void OnClickedUp() => menuItem.NotifyValueChange(false);
        public void OnClicked() => menuItem.NotifyValueChange(!menuItem.GetValue());

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}