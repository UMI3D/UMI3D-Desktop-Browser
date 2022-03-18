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
            WindowItem.Remove();
            ToolboxItem?.Remove();
            Displayerbox?.Remove();
        }

        private void PinUnpin(bool value)
            => MenuBar_E.Instance.PinUnpin(value, (Menu)menu);
    }

    public partial class WindowToolboxesContainerDeep0 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            WindowItem = new ToolboxWindowItem_E();
            
            isDisplayed = true;
            isExpanded = true;
        }

        protected override void SetContainerAsTool()
        {
            ToolboxItem = new ToolboxItem_E(false)
            {
                OnClicked = () => Select()
            };
            ToolboxItem.SetItemStatus(true);
            Displayerbox = new Displayerbox_E();
            WindowItem.AddDisplayerbox(Displayerbox);
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
            foreach (AbstractDisplayer child in currentDisplayers)
                if (child is WindowToolboxesContainerDeep1 containerDeep1)
                    containerDeep1.Collapse();
            if (IsTool)
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
            if (IsTool)
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
                IsTool = false;
                WindowItem.AddToolboxItemInFirstToolbox(containerDeep1.ToolboxItem);
                if (containerDeep1.IsTool)
                    WindowItem.AddDisplayerbox(containerDeep1.Displayerbox);
                else
                    WindowItem.AddToolbox(containerDeep1.Toolbox);
                
                AddChildrenToContainer(containerDeep1);
            }
            else if (element is AbstractWindowInputDisplayer displayer)
            {
                if (!IsTool)
                {
                    SetContainerAsTool();
                    IsTool = true;
                }
                WindowItem.AddToolboxItemInFirstToolbox(ToolboxItem);
                Displayerbox.Add(displayer.Displayer);
            }
        }

        private void AddChildrenToContainer(WindowToolboxesContainerDeep1 containerDeep1)
        {
            foreach (AbstractDisplayer displayer in containerDeep1)
            {
                if (displayer is WindowToolboxesContainerDeep1 containerDeep1Child)
                {
                    if (containerDeep1Child.IsTool)
                        WindowItem.AddDisplayerbox(containerDeep1Child.Displayerbox);
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