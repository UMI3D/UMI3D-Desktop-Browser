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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.desktopBrowser.menu.Container
{
    public partial class WindowToolboxesContainerDeep0
    {
        public bool IsChildrenExpand { get; set; } = false;
        public ToolboxWindowItem_E WindowItem { get; private set; } = null;
    }

    public partial class WindowToolboxesContainerDeep0
    {
        protected override void Awake()
        {
            base.Awake();
            WindowItem = new ToolboxWindowItem_E();
            isDisplayed = true;
            isExpanded = true;
        }

        private void OnDestroy()
        {
            WindowItem.Remove();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            WindowItem.SetFirstToolboxName(menu.Name);
        }
    }

    public partial class WindowToolboxesContainerDeep0 : AbstractToolboxesContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            WindowItem.Display();
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
            foreach (AbstractDisplayer child in currentDisplayers)
            {
                ((WindowToolboxesContainerDeep1)child).Collapse();
            }
            IsChildrenExpand = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            //base.ExpandAs(container, forceUpdate);
            //WindowItem.Display();
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
                WindowItem.AddToolboxItemInFirstToolbox(containerDeep1.Item);
                if (containerDeep1.IsTool)
                {
                    WindowItem.AddDisplayerbox(containerDeep1.Displayerbox);
                    containerDeep1.Item.SetItemStatus(true);
                }
                else
                    WindowItem.AddToolbox(containerDeep1.Toolbox);
                

                AddChildrenToContainer(containerDeep1);
            }
        }

        private void AddChildrenToContainer(WindowToolboxesContainerDeep1 containerDeep1)
        {
            foreach (AbstractDisplayer displayer in containerDeep1)
            {
                if (displayer is WindowToolboxesContainerDeep1 containerDeep1Child)
                {
                    if (containerDeep1Child.IsTool)
                    {
                        WindowItem.AddDisplayerbox(containerDeep1Child.Displayerbox);
                        containerDeep1Child.Item.SetItemStatus(true);
                    }
                    else
                        WindowItem.AddToolbox(containerDeep1Child.Toolbox);
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