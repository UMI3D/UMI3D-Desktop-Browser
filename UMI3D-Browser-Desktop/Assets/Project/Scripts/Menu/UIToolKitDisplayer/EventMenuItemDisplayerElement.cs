﻿/*
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

using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class EventMenuItemDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        public VisualTreeAsset menuItemTreeAsset;

        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected EventMenuItem menuItem;

        /// <summary>
        /// Button
        /// </summary>
        Button button;

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

        /// <summary>
        /// Notify that the button has been pressed.
        /// </summary>
        public void NotifyPress()
        {
            menuItem?.NotifyValueChange(!menuItem.GetValue());
        }

        /// <summary>
        /// Set menu item to display and initialise the display.
        /// </summary>
        /// <param name="item">Item to display</param>
        /// <returns></returns>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is EventMenuItem)
            {
                menuItem = item as EventMenuItem;
            }
            InitAndBindUI();
            button.text = item.Name;
        }

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is EventMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
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
                button = menuItemTreeAsset.CloneTree().Q<Button>();
                button.clickable.clicked += NotifyPress;
            }
        }

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}