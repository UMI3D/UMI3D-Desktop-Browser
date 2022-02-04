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
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.uI.viewController;

namespace umi3dDesktopBrowser.uI.Container
{
    public partial class WindowToolboxesContainerRoot
    {

    }

    public partial class WindowToolboxesContainerRoot : AbstractToolboxesContainer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            base.Display(forceUpdate);
            MenuBar_E.Instance.DisplayToolboxButton(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            MenuBar_E.Instance.DisplayToolboxButton(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
            base.Collapse(forceUpdate);
            ToolboxWindow_E.Instance.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            base.ExpandAs(container, forceUpdate);
            ToolboxWindow_E.Instance.Display();
        }

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="element"></param>
        ///// <param name="updateDisplay"></param>
        //public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        //{
        //    if (element is AbstractMenuDisplayContainer menuContainer)
        //    {
        //        menuContainer.parent = this;
        //    }

        //    element.transform.SetParent(this.transform);

        //    currentDisplayers.Add(element);

        //    element.Hide();
        //    if (updateDisplay)
        //        Display(true);
        //}

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="element"></param>
        ///// <param name="index"></param>
        ///// <param name="updateDisplay"></param>
        //public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        //{
        //    if (element is AbstractMenuDisplayContainer menuContainer)
        //    {
        //        menuContainer.parent = this;
        //    }

        //    element.transform.SetParent(this.transform);
        //    element.transform.SetSiblingIndex(index);

        //    currentDisplayers.Insert(index, element);
        //    element.Hide();
        //    if (updateDisplay)
        //        Display(true);
        //}
    }
}