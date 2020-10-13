using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class ButtonInputDisplayerElement : AbstractButtonInputDisplayer, IDisplayerElement
    {
        /// <summary>
        /// Button
        /// </summary>
        Button button;

        public override void Clear()
        {
            base.Clear();
            button.clickable.clicked -= NotifyPress;
            button.RemoveFromHierarchy();
        }

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
            button.clickable.clicked -= NotifyPress;
            button.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (button == null)
                button = new Button();
        }
    }
}