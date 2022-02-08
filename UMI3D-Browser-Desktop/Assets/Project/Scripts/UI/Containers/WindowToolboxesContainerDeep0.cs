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
    public partial class WindowToolboxesContainerDeep0
    {
        protected override void Awake()
        {
            base.Awake();
            WindowItem = new ToolboxWindowItem_E();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            WindowItem.FirstToolbox.SetToolboxName(menu.Name);
        }
    }

    public partial class WindowToolboxesContainerDeep0
    {
        public ToolboxWindowItem_E WindowItem { get; private set; } = null;
    }

    public partial class WindowToolboxesContainerDeep0 : AbstractToolboxesContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            //if (isDisplayed) return;
            //base.Display(forceUpdate);
            //Expand(forceUpdate);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            //if (!isDisplayed) return;
            //base.Hide();
            //Collapse(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
            base.Collapse(forceUpdate);
            WindowItem.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            Debug.Log($"expand as in container deep 0");
            base.ExpandAs(container, forceUpdate);
            WindowItem.Display();
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
                WindowItem.Adds(containerDeep1.Item);
                //WindowItem.ToolboxesContainer.Add(containerDeep1.ItemChildrenContainer);
                containerDeep1.ItemChildrenContainer.AddTo(WindowItem.ToolboxesContainer);

                AddChildrenToContainer(containerDeep1);
            }
        }

        private void AddChildrenToContainer(WindowToolboxesContainerDeep1 containerDeep1)
        {
            foreach (AbstractDisplayer displayer in containerDeep1)
            {
                if (displayer is WindowToolboxesContainerDeep1 containerDeep1Child)
                {
                    //WindowItem.ToolboxesContainer.Add(containerDeep1Child.ItemChildrenContainer);
                    containerDeep1Child.ItemChildrenContainer.AddTo(WindowItem.ToolboxesContainer);
                    AddChildrenToContainer(containerDeep1Child);
                }
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