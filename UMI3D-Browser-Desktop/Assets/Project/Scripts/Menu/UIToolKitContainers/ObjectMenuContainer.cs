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
using umi3d.DesktopBrowser.menu.Displayer;
using umi3dDesktopBrowser.ui.viewController;

namespace umi3d.desktopBrowser.menu.Container
{
    public partial class ObjectMenuContainer
    {
        public Displayerbox_E Displayerbox { get; protected set; } = null;
    }

    public partial class ObjectMenuContainer : Abstract2DContainer
    {
        private void Awake()
        {
            Displayerbox = new Displayerbox_E(DisplayerboxType.ParametersPopup);
            ObjectMenuWindow_E.Instance.AddRange(Displayerbox);
        }

        protected override void CollapseImp()
        {
            ObjectMenuWindow_E.Instance.Hide();
            base.CollapseImp();
        }

        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            base.ExpandAsImp(container);
            ObjectMenuWindow_E.Instance.Display();
        }

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            base.Insert(element, updateDisplay);
            element.Display();
            if (element is AbstractWindowInputDisplayer displayer)
                Displayerbox.Add(displayer.Displayer);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (element is AbstractMenuDisplayContainer menuContainer)
                menuContainer.parent = this;

            element.transform.SetParent(this.transform);
            element.transform.SetSiblingIndex(index);

            m_currentDisplayers.Insert(index, element);
            element.Hide();
            if (updateDisplay)
                Display(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        /// <returns></returns>
        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (!m_currentDisplayers.Remove(element))
                return false;
            if (updateDisplay)
                Display(true);

            return true;
        }
    }
}
