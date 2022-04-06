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
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;

namespace umi3d.desktopBrowser.menu.Container
{
    public class PinnedToolboxContainerRoot : AbstractToolboxesContainer
    {
        private void Start()
        {
            MenuBar_E.Instance.OnSubMenuMouseDownEvent += CollapseImp;
        }

        protected override void CollapseImp()
        {
            foreach (AbstractDisplayer child in currentDisplayers)
            {
                if (child is PinnedToolboxContainerDeep0 containerDeep0)
                    containerDeep0.Collapse();
            }
            ToolboxPinnedWindow_E.Instance.Hide();
            MenuBar_E.Instance.DisplaySubMenu(false);
            base.CollapseImp();
        }

        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            base.Insert(element, updateDisplay);
            if (element is PinnedToolboxContainerDeep0 containerDeep0)
                MenuBar_E.Instance.AddToolboxDeep0(containerDeep0.Toolbox);
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!base.Remove(element, updateDisplay))
                return false;
            if (element is PinnedToolboxContainerDeep0 containerDeep0)
                MenuBar_E.Instance.RemoveToolboxDeep0(containerDeep0.Toolbox);
            return true;
        }

        protected override void ItemAdded(AbstractDisplayer displayer)
        { }

        protected override void ItemTypeChanged(AbstractToolboxesContainer item)
        { }
    }
}
