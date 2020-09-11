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
using UnityEngine;
using UnityEngine.UI;

namespace BrowserDesktop.Menu
{
    [RequireComponent(typeof(Button))]
    public class MenuItemButtonDisplayer : AbstractDisplayer
    {
        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected ButtonMenuItem menuItem;

        /// <summary>
        /// Button
        /// </summary>
        public Button button;

        public override void Clear()
        {
            base.Clear();
            button.onClick.RemoveListener(NotifyPress);
        }


        public override void Display(bool forceUpdate = false)
        {
            button.gameObject.SetActive(true);
            button.onClick.AddListener(NotifyPress);
        }

        public override void Hide()
        {
            button.onClick.RemoveListener(NotifyPress);
            button.gameObject.SetActive(false);
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
            button = GetComponent<Button>();
            if (item is ButtonMenuItem)
            {
                menuItem = item as ButtonMenuItem;
            }
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = item.Name;

        }

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
        }
    }
}