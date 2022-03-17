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
        public Toolbox_E Toolbox { get; private set; } = null;
        public ToolboxItem_E Item { get; private set; } = null;
        public Displayerbox_E Displayerbox { get; private set; } = null;
        public bool IsTool { get; private set; } = false;
    }

    public partial class PinnedToolboxContainerDeep0
    {
        private void OnDestroy()
        {
            Toolbox.Remove();
            Item.Remove();
            Displayerbox.Remove();
        }
    }

    public partial class PinnedToolboxContainerDeep0 : AbstractToolboxesContainer
    {
        protected override void Awake()
        {
            base.Awake();
            Toolbox = new Toolbox_E();
            Item = new ToolboxItem_E()
            {
                OnClicked = () => Select()
            };
            Item.SetItemStatus(true);
            Displayerbox = new Displayerbox_E();
        }

        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            Toolbox.SetToolboxName(menu.Name);
            ToolboxPinnedWindow_E.Instance.Adds(Displayerbox);
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
            base.CollapseImp();
            foreach (AbstractDisplayer child in currentDisplayers)
                if (child is PinnedToolboxContainerDeep1 containerDeep1)
                    containerDeep1.Collapse();
            Displayerbox.Hide();
            Item.Toggle(false);
            //IsChildrenExpand = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
            if (IsTool) Displayerbox.Display();
            Item.Toggle(true);
            Debug.Log($"expand in Deep 0");
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
                Toolbox.Adds(containerDeep1.Item);
                if (containerDeep1.IsTool)
                    ToolboxPinnedWindow_E.Instance.Adds(containerDeep1.Displayerbox);
                else
                    MenuBar_E.Instance.AddToolboxDeep1Plus(containerDeep1.Toolbox);

                //AddChildrenToContainer(containerDeep1);
            }
            else if (element is AbstractWindowInputDisplayer displayer)
            {
                IsTool = true;
                Toolbox.Adds(Item);
                Displayerbox.Add(displayer.Displayer);
            }
        }
    }
}
