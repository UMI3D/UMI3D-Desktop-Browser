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
    public partial class WindowToolboxesContainerDeep0
    {
        public ToolboxWindowItem_E WindowItem { get; private set; } = null;
    }

    public partial class WindowToolboxesContainerDeep0
    {
        private void OnDestroy()
        {
            WindowItem.RemoveRootFromHierarchy();
            ToolboxItem?.RemoveRootFromHierarchy();
            Displayerbox?.RemoveRootFromHierarchy();
            ToolboxWindow_E.UnpinnedPressed -= () => WindowItem.PinUnpin(false);
        }

        /// <summary>
        /// Pin (if true) or unpin this container.
        /// </summary>
        /// <param name="value"></param>
        private void PinUnpin(bool value)
            => MenuBar_E.Instance.PinUnpin(value, (Menu)menu);
    }

    public partial class WindowToolboxesContainerDeep0 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            WindowItem = new ToolboxWindowItem_E();
            ToolboxWindow_E.UnpinnedPressed += () => WindowItem.PinUnpin(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void SetContainerAsTool()
        {
            ToolboxItem = new ToolboxItem_E(false);
            ToolboxItem.Clicked += Select;
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E(DisplayerboxType.ToolboxesPopup);
            WindowItem.AddDisplayerbox(Displayerbox);
            base.SetContainerAsTool();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            WindowItem.SetFirstToolboxName(menu.Name);
            WindowItem.OnPinnedUnpinned += PinUnpin;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void DisplayImp()
        {
            base.DisplayImp();
            WindowItem.Display();
        }

        /// <summary>
        /// ContainerDeep0 are never hidden.
        /// </summary>
        protected override void HideImp()
        {
            WindowItem.Hide();
            base.HideImp();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            foreach (AbstractDisplayer child in m_currentDisplayers)
            {
                if (child is WindowToolboxesContainerDeep1 containerDeep1)
                    containerDeep1.Collapse();
            }
            if (ToolType == ItemType.Tool)
            {
                Displayerbox.Hide();
                ToolboxItem.Toggle(false);
            }
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
                WindowItem.AddToolboxItemInFirstToolbox(containerDeep1.ToolboxItem);
                AddBoxToWindowItem(containerDeep1);
            }
            else if (element is AbstractWindowInputDisplayer displayer)
            {
                if (ToolType != ItemType.Tool)
                    SetContainerAsTool();
                WindowItem.AddToolboxItemInFirstToolbox(ToolboxItem);
                Displayerbox.AddRange(displayer.Displayer);
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

        /// <summary>
        /// Add displayerbox or toolbox from [item] to this container windowItem.
        /// </summary>
        /// <param name="item"></param>
        public void AddBoxToWindowItem(AbstractToolboxesContainer item)
        {
            if (!(item is WindowToolboxesContainerDeep1 containerDeep1))
                return;
            switch (containerDeep1.ToolType)
            {
                case ItemType.Undefine:
                    break;
                case ItemType.Tool:
                    WindowItem.AddDisplayerbox(containerDeep1.Displayerbox);
                    break;
                case ItemType.Toolbox:
                    WindowItem.AddToolbox(containerDeep1.Toolbox);
                    break;
                default:
                    break;
            }
        }
    }
}