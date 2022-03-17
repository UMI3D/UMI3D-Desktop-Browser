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
        

        private void OnDestroy()
        {
            Item.Remove();
            Toolbox.Remove();
            Displayerbox.Remove();
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        protected override void DisplayImp()
        {
            base.DisplayImp();
            Item.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void HideImp()
        {
            Toolbox.Hide();
            Displayerbox.Hide();
            base.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            base.CollapseImp();
            foreach (AbstractDisplayer child in currentDisplayers)
                if (child is WindowToolboxesContainerDeep1 containerDeep1)
                    containerDeep1.Collapse();
            Item.Toggle(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
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
            Debug.Log("<color=green>TODO: </color>" + $"insert from Deep1 [{menu.Name}] with [{element.menu.Name}]");
            base.Insert(element, updateDisplay);
            if (element is WindowToolboxesContainerDeep1 containerDeep1)
            {
                Debug.Log("<color=green>TODO: </color>" + $"insert container from Deep1 [{menu.Name}] with [{element.menu.Name}]");
                IsTool = false;
                Item.SetItemStatus(false);
                Toolbox.Adds(containerDeep1.Item);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                Debug.Log("<color=green>TODO: </color>" + $"insert displayer from Deep1 [{menu.Name}] with [{element.menu.Name}]");
                IsTool = true;
                Item.SetItemStatus(true);
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