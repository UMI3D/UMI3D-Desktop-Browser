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
    public partial class PinnedToolboxContainerDeep1
    {
        private void OnDestroy()
        {
            ToolboxItem?.Remove();
            Toolbox?.Remove();
            Displayerbox?.Remove();
        }
    }

    public partial class PinnedToolboxContainerDeep1 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            ToolboxItem = new ToolboxItem_E(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void SetContainerAsToolbox()
        {
            ToolboxItem.Clicked = () => Select();
            ToolboxItem.SetItemStatus(false);
            Toolbox = new Toolbox_E(ToolboxType.SubPinned);
            Toolbox?.SetToolboxName(menu.Name ?? "");
            base.SetContainerAsToolbox();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void SetContainerAsTool()
        {
            ToolboxItem.Clicked = () =>
            {
                Select();
                ToolboxPinnedWindow_E.Instance.SetTopBarName(menu.Name);
                ToolboxPinnedWindow_E.Instance.Display();
            };
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E(DisplayerboxType.ParametersPopup);
            base.SetContainerAsTool();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            if (menu.icon2D != null)
                ToolboxItem.SetIcon(menu.icon2D);
            ToolboxItem.Label.value = menu.Name;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        protected override void DisplayImp()
            => base.DisplayImp();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void HideImp()
            => base.Hide();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            base.CollapseImp();
            foreach (AbstractDisplayer child in m_currentDisplayers)
            {
                if (child is PinnedToolboxContainerDeep1 containerDeep1)
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
        /// <param name="forceUpdate"></param>
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
            if (element is PinnedToolboxContainerDeep1 containerDeep1)
            {
                if (ToolType != ItemType.Toolbox)
                    SetContainerAsToolbox();
                Toolbox.Add(containerDeep1.ToolboxItem);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                if (ToolType != ItemType.Tool)
                    SetContainerAsTool();
                Displayerbox.Add(displayer.Displayer);
            }
            FindDeep0AndAddBoxToPinnedWindows();
        }

        /// <summary>
        /// Recursively search for container deep 0 and add box to pinned windows.
        /// </summary>
        private void FindDeep0AndAddBoxToPinnedWindows()
        {
            var parent = this.parent;
            while (parent != null && !(parent is PinnedToolboxContainerDeep0))
                parent = parent.parent;

            if (parent == null)
                throw new System.Exception("Parent null, not WindowToolboxesContainerDeep0");

            (parent as PinnedToolboxContainerDeep0).AddBoxToPinnedWindows(this);
        }
    }
}
