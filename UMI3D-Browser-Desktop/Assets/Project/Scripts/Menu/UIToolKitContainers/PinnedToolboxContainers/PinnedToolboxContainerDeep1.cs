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
        //public ToolboxItem_E Item { get; private set; } = null;
        public Toolbox_E Toolbox { get; private set; } = null;
        //public Displayerbox_E Displayerbox { get; private set; } = null;
        //public bool IsTool { get; private set; } = false;
    }

    public partial class PinnedToolboxContainerDeep1
    {
        private void OnDestroy()
        {
            ToolboxItem.Remove();
            Toolbox.Remove();
            Displayerbox.Remove();
        }
    }

    public partial class PinnedToolboxContainerDeep1 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            ToolboxItem = new ToolboxItem_E(false)
            {
                OnClicked = () => Select()
            };
            Toolbox = new Toolbox_E(false);
            Displayerbox = new Displayerbox_E();
            isDisplayed = true;
        }

        protected override void SetContainerAsTool()
        {
            throw new System.NotImplementedException();
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
                if (child is PinnedToolboxContainerDeep1 containerDeep1)
                    containerDeep1.Collapse();
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
            if (IsTool) Displayerbox.Display();
            else Toolbox.Display();
            ToolboxItem.Toggle(true);
            Debug.Log($"expand in Deep 1");
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
                IsTool = false;
                ToolboxItem.SetItemStatus(false);
                Toolbox.Adds(containerDeep1.ToolboxItem);
            }
            if (element is AbstractWindowInputDisplayer displayer)
            {
                IsTool = true;
                ToolboxItem.SetItemStatus(true);
                Displayerbox.Add(displayer.Displayer);
            }
        }
    }
}
