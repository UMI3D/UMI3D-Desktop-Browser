using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class HoldableButtonDisplayerElement : AbstractDisplayer, IDisplayerElement
    {
        /// <summary>
        /// Button menu item to display.
        /// </summary>
        protected HoldableButtonMenuItem menuItem;

        /// <summary>
        /// Button
        /// </summary>
        Button button;


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
                button = new Button();
        }
    }
}