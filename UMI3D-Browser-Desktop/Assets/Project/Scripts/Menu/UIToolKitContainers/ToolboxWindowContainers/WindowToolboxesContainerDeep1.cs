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
        private void OnDestroy()
        {
            ToolboxItem.Remove();
            Toolbox.Remove();
            Displayerbox?.Remove();
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
            ToolboxItem = new ToolboxItem_E(false)
            {
                OnClicked = () => Select()
            };
            SetContainerAsToolbox();
        }

        protected override void SetContainerAsToolbox()
        {
            base.SetContainerAsToolbox();
            Toolbox = new Toolbox_E(false);
        }

        protected override void SetContainerAsTool()
        {
            base.SetContainerAsTool();
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            if (menu.icon2D != null)
                ToolboxItem.SetIcon(menu.icon2D);
            ToolboxItem.Label.value = menu.Name;
            Toolbox.SetToolboxName(menu.Name ?? "");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        protected override void DisplayImp()
        {
            base.DisplayImp();
            ToolboxItem.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void HideImp()
        {
            ToolboxItem.Hide();
            base.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            base.CollapseImp();
            foreach (AbstractDisplayer child in currentDisplayers)
            {
                if (child is WindowToolboxesContainerDeep1 containerDeep1)
                    containerDeep1.Collapse(); 
            }
            switch (ToolType)
            {
                case ItemType.Undefine:
                    break;
                case ItemType.Tool:
                    Displayerbox.Hide();
                    break;
                case ItemType.Toolbox:
                    Toolbox.Hide();
                    break;
                default:
                    break;
            }
            ToolboxItem.Toggle(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
            switch (ToolType)
            {
                case ItemType.Undefine:
                    break;
                case ItemType.Tool:
                    Displayerbox.Display();
                    break;
                case ItemType.Toolbox:
                    Toolbox.Display();
                    break;
                default:
                    break;
            }
            ToolboxItem.Toggle(true);
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
                if (ToolType != ItemType.Toolbox)
                    SetContainerAsToolbox();
                ToolboxItem.SetItemStatus(false);
                Toolbox.Adds(containerDeep1.ToolboxItem);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                if (ToolType != ItemType.Tool)
                    SetContainerAsTool();
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