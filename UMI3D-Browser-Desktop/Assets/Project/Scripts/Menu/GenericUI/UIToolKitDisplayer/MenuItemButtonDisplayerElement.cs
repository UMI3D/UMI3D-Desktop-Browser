using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class MenuItemButtonDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        public VisualTreeAsset menuItemTreeAsset;

        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected ButtonMenuItem menuItem;

        /// <summary>
        /// Button
        /// </summary>
        Button button;

        public override void Clear()
        {
            base.Clear();
            button.clickable.clicked += NotifyPress;
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            button.clickable.clicked += NotifyPress;
            button.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            button.clickable.clicked -= NotifyPress;
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
            //TODO
            //button = GetComponent<Button>();
            if (item is ButtonMenuItem)
            {
                menuItem = item as ButtonMenuItem;
            }
            InitAndBindUI();
            button.text = item.Name;
        }

        public override int IsSuitableFor(umi3d.cdk.menu.AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return button;
        }

        public void InitAndBindUI()
        {   
            if (button == null)
                button = menuItemTreeAsset.CloneTree().Q<Button>();
        }

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}