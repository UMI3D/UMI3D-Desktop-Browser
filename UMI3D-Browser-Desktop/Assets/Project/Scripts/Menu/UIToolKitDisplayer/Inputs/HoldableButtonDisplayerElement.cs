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

using umi3d.baseBrowser.Menu;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class HoldableButtonDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected HoldableButtonMenuItem menuItem;

        public VisualTreeAsset holdableButtonTreeAsset;

        /// <summary>
        /// Button
        /// </summary>
        Label button;

        /// <summary>
        /// Notify that the button has been pressed.
        /// </summary>
        public void NotifyPressDown()
        {
            menuItem.NotifyValueChange(true);
        }

        public void NotifyPressUp()
        {
            menuItem.NotifyValueChange(false);
        }

        /// <summary>
        /// Set menu item to display and initialise the display.
        /// </summary>
        /// <param name="item">Item to display</param>
        /// <returns></returns>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is HoldableButtonMenuItem)
            {
                menuItem = item as HoldableButtonMenuItem;
                InitAndBindUI();
                button.text = item.Name;
            }
            else
                throw new System.Exception("MenuItem must be a ButtonInput");
        }

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is HoldableButtonMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();
            button.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            button.style.display = DisplayStyle.None;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return button;
        }

        public void InitAndBindUI()
        {
            if (button == null)
            {
                button = holdableButtonTreeAsset.CloneTree().Q<Label>();
                button.RegisterCallback<MouseDownEvent>((e) =>{ //Pointer down does not seem to work with UIToolKit 0.0.4-preview
                    if (e.clickCount == 1)
                        NotifyPressDown();
                });
                button.RegisterCallback<PointerUpEvent>((e) =>
                {
                    if (e.clickCount == 1)
                        NotifyPressUp();
                });
            }
        }

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}