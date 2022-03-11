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
using umi3d.DesktopBrowser.menu.Displayer;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.desktopBrowser.menu.Container
{
    public partial class WindowToolboxesContainerDeep1
    {
        public ToolboxItem_E Item { get; private set; } = null;
        public Toolbox_E Toolbox { get; private set; } = null;
        public Displayerbox_E Displayerbox { get; private set; } = null;

        public bool IsTool { get; private set; } = false;
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
            Toolbox = new Toolbox_E(false);
            Displayerbox = new Displayerbox_E();
            isDisplayed = true;
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            if (menu.icon2D != null)
                Item.SetIcon(menu.icon2D);
            Item.Label.value = menu.Name;
            Toolbox.SetToolboxName(menu.Name ?? "");
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
            Toolbox.Hide();
            Displayerbox.Hide();
            Item.Toggle(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            base.ExpandAs(container, forceUpdate);
            if (IsTool) Displayerbox.Display();
            else Toolbox.Display();
            Item.Toggle(true);
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
                IsTool = false;
                Toolbox.Adds(containerDeep1.Item);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                IsTool = true;
                Displayerbox.Add(displayer.Displayer);
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