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

using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// Button displayer.
    /// </summary>
    public class ButtonDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        public VisualTreeAsset buttonTreeAsset;
        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected ButtonMenuItem menuItem;

        VisualElement container;
        Button button;

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();
            container.style.display = DisplayStyle.Flex;
            button.clickable.clicked += NotifyPress;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return container;
        }

        public override void Hide()
        {
            container.style.display = DisplayStyle.None;
            button.clickable.clicked -= NotifyPress;
        }

        public override void Clear()
        {
            base.Clear();
            button.clickable.clicked -= NotifyPress;
            container.RemoveFromHierarchy();
        }

        /// <summary>
        /// Notify that the button has been pressed.
        /// </summary>
        public void NotifyPress()
        {
            menuItem.NotifyValueChange(!menuItem.GetValue());
        }

        /// <summary>
        /// Set menu item to display and initialise the display.
        /// </summary>
        /// <param name="item">Item to display</param>
        /// <returns></returns>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is ButtonMenuItem)
            {
                menuItem = item as ButtonMenuItem;
                button.text = item.Name;
            }
            else
                throw new System.Exception("MenuItem must be a ButtonInput");
        }

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : 0;
        }

        public void InitAndBindUI()
        {
            if (container == null)
            {
                container = buttonTreeAsset.CloneTree();
                button = container.Q<Button>();
            }
                
        }

        private void OnDestroy()
        {
            container?.RemoveFromHierarchy();
        }
    }
}