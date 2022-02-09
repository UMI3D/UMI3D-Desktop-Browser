/*
Copyright 2019 - 2021 Inetum

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
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine;

namespace umi3dDesktopBrowser.uI.Container
{
    public partial class WindowToolboxesContainerDeep1
    {
        public ToolboxItem_E Item { get; private set; } = null;
        public Toolbox_E ItemChildrenContainer { get; private set; } = null;
    }

    public partial class WindowToolboxesContainerDeep1
    {
        protected override void Awake()
        {
            base.Awake();
            Item = new ToolboxItem_E(false)
            {
                OnClicked = () => Select()
            };
            ItemChildrenContainer = new Toolbox_E(false);
            isDisplayed = true;
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            if (menu.icon2D != null)
                Item.SetIcon(menu.icon2D);
            Item.SetLabel(menu.Name);
            ItemChildrenContainer.SetToolboxName(menu.Name);
        }

        public override void Select()
        {
            base.Select();
            if (parent is WindowToolboxesContainerDeep0 containerDeep0)
            {
                if (!containerDeep0.IsChildrenExpand)
                {
                    containerDeep0.IsChildrenExpand = true;
                    foreach (AbstractDisplayer sibling in containerDeep0.parent)
                    {
                        if (sibling != this.parent && sibling is WindowToolboxesContainerDeep0 siblingDeep0)
                        {
                            siblingDeep0.Collapse();
                        }
                    }
                }
            }
        }

        //private WindowToolboxesContainerDeep0 FindContainerDeep0()
        //{
        //    AbstractMenuDisplayContainer virtualParent = this;
        //    while(virtualParent.parent != null)
        //    {
        //        virtualParent = virtualParent.parent;
        //        if (virtualParent is WindowToolboxesContainerDeep0)
        //            return virtualParent as WindowToolboxesContainerDeep0;
        //    }
        //    throw new System.Exception("No parent is a WindowToolboxContainerDeep0");
        //}
    }

    public partial class WindowToolboxesContainerDeep1 : AbstractToolboxesContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            if (isDisplayed) return;
            base.Display(forceUpdate);
            Item.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            if (!isDisplayed) return;
            base.Hide();
            Item.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
            base.Collapse(forceUpdate);
            ItemChildrenContainer.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            base.ExpandAs(container, forceUpdate);
            ItemChildrenContainer.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            base.Insert(element, updateDisplay);
            if (element is WindowToolboxesContainerDeep1 containerDeep1)
            {
                ItemChildrenContainer.Adds(containerDeep1.Item);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            base.Insert(element, index, updateDisplay);
            Debug.Log("<color=green>TODO: </color>" + $"");
        }
    }
}