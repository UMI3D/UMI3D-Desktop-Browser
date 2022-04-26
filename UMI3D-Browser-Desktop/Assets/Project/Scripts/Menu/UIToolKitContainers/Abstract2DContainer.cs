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
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.desktopBrowser.menu.Container
{
    public abstract partial class Abstract2DContainer
    {
        /// <summary>
        /// Current <see cref="AbstractDisplayer"/> displayed by the container;
        /// </summary>
        protected List<AbstractDisplayer> m_currentDisplayers = new List<AbstractDisplayer>();

        protected AbstractMenuDisplayContainer m_virtualContainer = null;
    }

    public abstract partial class Abstract2DContainer
    {
        protected virtual void DisplayImp()
        {
            gameObject.SetActive(true);
        }

        protected virtual void HideImp()
        {
            foreach (AbstractDisplayer disp in m_virtualContainer)
                disp.Hide();
            this.gameObject.SetActive(false);
        }
    }

    public abstract partial class Abstract2DContainer : AbstractMenuDisplayContainer
    {
        #region Data

        public override AbstractDisplayer this[int i] { get => m_currentDisplayers[i]; set => m_currentDisplayers[i] = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
            => m_currentDisplayers;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Contains(AbstractDisplayer element)
            => m_currentDisplayers.Contains(element);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int Count()
            => m_currentDisplayers.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
            => m_virtualContainer;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override int GetIndexOf(AbstractDisplayer element)
            => m_currentDisplayers.IndexOf(element);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(AbstractMenuItem menu)
            => 0;

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            if (isDisplayed && !forceUpdate)
                return;
            isDisplayed = true;
            DisplayImp();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            if (!isDisplayed)
                return;
            isDisplayed = false;
            HideImp();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void CollapseImp()
        {
            foreach (AbstractDisplayer disp in m_currentDisplayers)
                disp.Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void ExpandImp()
            => ExpandAsImp(this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        protected override void ExpandAsImp(AbstractMenuDisplayContainer container)
        {
            this.gameObject.SetActive(true);

            if (m_virtualContainer != null && m_virtualContainer != container)
                foreach (AbstractDisplayer displayer in m_virtualContainer)
                    displayer.Hide();

            m_virtualContainer = container;

            foreach (AbstractDisplayer disp in m_virtualContainer)
                disp.Display();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            element.transform.SetParent(this.transform);
            m_currentDisplayers.Add(element);
            element.Hide();

            if (updateDisplay)
                Display(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="updateDisplay"></param>
        public override void InsertRange(IEnumerable<AbstractDisplayer> elements, bool updateDisplay = true)
        {
            foreach (AbstractDisplayer e in elements)
                Insert(e, false);

            if (updateDisplay)
                Display(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="updateDisplay"></param>
        /// <returns></returns>
        public override bool RemoveAt(int index, bool updateDisplay = true)
        {
            if ((index < 0) || (index >= Count()))
                return false;

            return Remove(m_currentDisplayers[index], updateDisplay);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int RemoveAll()
        {
            int c = Count();
            foreach (AbstractDisplayer d in new List<AbstractDisplayer>(m_currentDisplayers))
                Remove(d, false);

            return c;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            m_currentDisplayers.Clear();
        }
    }
}
