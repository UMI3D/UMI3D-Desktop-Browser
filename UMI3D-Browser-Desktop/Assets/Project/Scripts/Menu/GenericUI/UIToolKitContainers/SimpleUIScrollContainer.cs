using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class SimpleUIScrollContainer : SimpleUIContainer2D
    {
        ScrollView scrollView;

        protected override void BindUI()
        {
            base.BindUI();
            scrollView = contentElement as ScrollView;
            if (scrollView == null)
            {
                Debug.LogError("The content element must be a scrollview for this container");
            }
        }

        public override void Expand(bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.Flex;
            base.Expand(forceUpdate);
        }

        public override void Collapse(bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.None;
            base.Collapse(forceUpdate);
        }

        public override void ExpandAs(umi3d.cdk.menu.view.AbstractMenuDisplayContainer Container, bool forceUpdate = false)
        {
            scrollView.style.display = DisplayStyle.Flex;
            base.ExpandAs(Container, forceUpdate);
        }
    }
}