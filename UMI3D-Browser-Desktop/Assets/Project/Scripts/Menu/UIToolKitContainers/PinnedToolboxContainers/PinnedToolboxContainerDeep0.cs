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
    public partial class PinnedToolboxContainerDeep0
    {
        private void OnDestroy()
        {
            Toolbox?.Remove();
            ToolboxItem?.Remove();
            Displayerbox?.Remove();
        }
    }

    public partial class PinnedToolboxContainerDeep0 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            Toolbox = new Toolbox_E(ToolboxType.Pinned);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void SetContainerAsTool()
        {
            ToolboxItem = new ToolboxItem_E()
            {
                OnClicked = () =>
                {
                    Select();
                    ToolboxPinnedWindow_E.Instance.SetTopBarName(menu.Name);
                    ToolboxPinnedWindow_E.Instance.Display();
                }
            };
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E(DisplayerboxType.ParametersPopup);
            ToolboxPinnedWindow_E.Instance.Adds(Displayerbox);
            base.SetContainerAsTool();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            Toolbox.SetToolboxName(menu.Name);
        }

        protected override void DisplayImp()
        {
            base.DisplayImp();
            Toolbox.Display();
        }

        public override void Hide()
        { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            foreach (AbstractDisplayer child in m_currentDisplayers)
            {
                if (child is PinnedToolboxContainerDeep1 containerDeep1)
                    containerDeep1.Collapse(true);
            }
            if (ToolType == ItemType.Tool)
            {
                Displayerbox.Hide();
                ToolboxItem.Toggle(false);
            }
            base.CollapseImp();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
            if (ToolType == ItemType.Tool)
            {
                Displayerbox.Display();
                ToolboxItem.Toggle(true);
            }
            MenuBar_E.Instance.DisplaySubMenu(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            base.Insert(element, updateDisplay);
            if (element is PinnedToolboxContainerDeep1 containerDeep1)
            {
                if (ToolType != ItemType.Toolbox)
                    SetContainerAsToolbox();
                Toolbox.Adds(containerDeep1.ToolboxItem);
                switch (containerDeep1.ToolType)
                {
                    case ItemType.Undefine:
                        break;
                    case ItemType.Tool:
                        ToolboxPinnedWindow_E.Instance.Adds(containerDeep1.Displayerbox);
                        break;
                    case ItemType.Toolbox:
                        MenuBar_E.Instance.AddToolboxDeep1Plus(containerDeep1.Toolbox);
                        break;
                    default:
                        break;
                }
            }
            else if (element is AbstractWindowInputDisplayer displayer)
            {
                if (ToolType != ItemType.Tool)
                    SetContainerAsTool();
                Toolbox.Adds(ToolboxItem);
                Displayerbox.Add(displayer.Displayer);
            }
        }

        protected override void ItemAdded(AbstractDisplayer displayer)
        { }

        protected override void ItemTypeChanged(AbstractToolboxesContainer item)
        {
            if (item is PinnedToolboxContainerDeep1 containerDeep1Child)
            {
                switch (containerDeep1Child.ToolType)
                {
                    case ItemType.Undefine:
                        break;
                    case ItemType.Tool:
                        ToolboxPinnedWindow_E.Instance.Adds(containerDeep1Child.Displayerbox);
                        break;
                    case ItemType.Toolbox:
                        MenuBar_E.Instance.AddToolboxDeep1Plus(containerDeep1Child.Toolbox);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
