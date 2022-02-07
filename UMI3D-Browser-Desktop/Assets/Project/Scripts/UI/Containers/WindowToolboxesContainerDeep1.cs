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
        private ToolboxItem_E m_item;
        private Toolbox_E m_itemChildrenContainer;

        private void Start()
        {
            m_item = new ToolboxItem_E(false);
            m_itemChildrenContainer = new Toolbox_E(false);
            m_item.Hide();
            m_itemChildrenContainer.Hide();
        }

        private WindowToolboxesContainerDeep0 FindContainerDeep0()
        {
            var virtualParent = parent;
            while(virtualParent.parent != null)
            {
                virtualParent = virtualParent.parent;
                if (virtualParent is WindowToolboxesContainerDeep0)
                    return virtualParent as WindowToolboxesContainerDeep0;
            }
            throw new System.Exception("No parent is a WindowToolboxContainerDeep0");
        }
    }

    public partial class WindowToolboxesContainerDeep1
    {
        public ToolboxItem_E Item => m_item;
        public Toolbox_E ItemChildrenContainer => m_itemChildrenContainer;
    }

    public partial class WindowToolboxesContainerDeep1 : AbstractToolboxesContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            base.Display(forceUpdate);
            m_item.SetIcon(menu.icon2D);
            m_item.SetLabel(menu.Name);
            m_item.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            m_item.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
            base.Collapse(forceUpdate);
            m_itemChildrenContainer.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            base.ExpandAs(container, forceUpdate);
            m_itemChildrenContainer.Display();
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
                m_itemChildrenContainer.Adds(containerDeep1.Item);
                var containerDeep0 = FindContainerDeep0();
                containerDeep0.WindowItem.Container.Add(containerDeep1.ItemChildrenContainer);
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