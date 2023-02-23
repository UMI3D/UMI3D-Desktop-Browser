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
using System.Drawing;
using System.Reflection.Emit;
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
        public ElementAlignment LabelDirection = ElementAlignment.Leading;
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

            button.Category = Category;
            button.Height = Size;
            button.Shape = Shape;
            button.Type = Type;
            button.LocalisedLabel = Label;
            button.LabelAndInputDirection = LabelDirection;
            button.IconAlignment = IconAlignment;
            button.LocaliseText = Text;
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void InitAndBindUI()
        {
            if (button != null) return;

            button = new Button_C
            {
                Category = Category,
                Height = Size,
                Shape = Shape,
                Type = Type,
                LocalisedLabel = Label,
                LabelAndInputDirection = LabelDirection,
                IconAlignment = IconAlignment,
                LocaliseText = Text
            };
            button.name = gameObject.name;
            button.ClickedDown += OnClickedDown;
            button.ClickedUp += OnClickedUp;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            button.ClickedDown -= OnClickedDown;
            button.ClickedUp -= OnClickedUp;
            button.RemoveFromHierarchy();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false) => button.Display();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide() => button.Hide();

        /// <summary>
        /// return the button UI element.
        /// </summary>
        /// <returns></returns>
        public VisualElement GetUXMLContent() => button;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : 0;
        }

        /// <summary>
        /// Notify that the menu item is down.
        /// </summary>
        public void OnClickedDown() => menuItem.NotifyValueChange(true);
        /// <summary>
        /// Notify that the menu item is up.
        /// </summary>
        public void OnClickedUp() => menuItem.NotifyValueChange(false);
        /// <summary>
        /// Notify that the menu item has been clicked and that its status changed.
        /// </summary>
        public void OnClicked() => menuItem.NotifyValueChange(!menuItem.GetValue());

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}