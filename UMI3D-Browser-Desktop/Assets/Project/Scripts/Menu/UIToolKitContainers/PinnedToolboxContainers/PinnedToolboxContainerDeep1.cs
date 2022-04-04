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

        protected override void SetContainerAsToolbox()
        {
            ToolboxItem.OnClicked = () => Select();
            ToolboxItem.SetItemStatus(false);
            Toolbox = new Toolbox_E(ToolboxType.SubPinned);
            Toolbox?.SetToolboxName(menu.Name ?? "");
            base.SetContainerAsToolbox();
        }

        protected override void SetContainerAsTool()
        {
            ToolboxItem.OnClicked = () =>
            {
                Select();
                ToolboxPinnedWindow_E.Instance.SetTopBarName(menu.Name);
                ToolboxPinnedWindow_E.Instance.Display();
            };
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E(DisplayerboxType.ParametersPopup);
            base.SetContainerAsTool();
        }

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
                Toolbox.Adds(containerDeep1.ToolboxItem);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                if (ToolType != ItemType.Tool)
                    SetContainerAsTool();
                Displayerbox.Add(displayer.Displayer);
            }
        }

        protected override void ItemAdded(AbstractDisplayer displayer)
        { }

        protected override void ItemTypeChanged(AbstractToolboxesContainer item)
            => OnItemTypeChanged?.Invoke(item);
    }
}
