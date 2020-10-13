using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// Button displayer.
    /// </summary>
    public class ButtonDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected ButtonMenuItem menuItem;

        Button button;

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();
            button.style.display = DisplayStyle.Flex;
            button.clickable.clicked += NotifyPress;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return button;
        }

        public override void Hide()
        {
            button.style.display = DisplayStyle.None;
            button.clickable.clicked -= NotifyPress;
        }

        public override void Clear()
        {
            base.Clear();
            button.clickable.clicked -= NotifyPress;
            button.RemoveFromHierarchy();
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
            if (button == null)
                button = new Button();
        }
    }
}