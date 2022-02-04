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

namespace umi3dDesktopBrowser.uI.Container
{
    public partial class AbstractToolboxesContainer
    {
        /// <summary>
        /// Number of <see cref="AbstractDisplayer"/> maximum displayed at the same time.
        /// </summary>
        public int maxElementsDisplayed = 4;

        /// <summary>
        /// Current <see cref="AbstractDisplayer"/> displayed by the container;
        /// </summary>
        protected List<AbstractDisplayer> currentDisplayers = new List<AbstractDisplayer>();

        protected AbstractToolboxesContainer virtualContainer = null;
    }

    public partial class AbstractToolboxesContainer : AbstractMenuDisplayContainer
    {
        protected virtual void Awake()
        {
            virtualContainer = this;
        }

        public override AbstractDisplayer this[int i] { get => currentDisplayers[i]; set => currentDisplayers[i] = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
            foreach (AbstractDisplayer disp in currentDisplayers)
            {
                disp.Hide();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Contains(AbstractDisplayer element)
        {
            return currentDisplayers.Contains(element);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            return currentDisplayers.Count;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            return virtualContainer;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            this.gameObject.SetActive(true);

            this.name = menu.Name + "-" + GetType().ToString().Split('.').Last();

            isDisplayed = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Expand(bool forceUpdate = false)
        {
            ExpandAs(this, forceUpdate);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            if (isExpanded && !forceUpdate)
            {
                return;
            }
            this.gameObject.SetActive(true);

            if (virtualContainer != null && virtualContainer != container)
            {
                foreach (AbstractDisplayer displayer in virtualContainer)
                {
                    displayer.Hide();
                }
            }

            virtualContainer = container as AbstractToolboxesContainer;

            foreach (AbstractDisplayer disp in virtualContainer)
            {
                disp.Display();
                //disp.transform.SetParent(this.transform);
            }

            isExpanded = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override int GetIndexOf(AbstractDisplayer element)
        {
            return currentDisplayers.IndexOf(element);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
            isDisplayed = false;
            foreach (AbstractDisplayer disp in virtualContainer)
            {
                disp.Hide();
            }
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (element is AbstractMenuDisplayContainer menuContainer)
            {
                menuContainer.parent = this;
            }

            element.transform.SetParent(this.transform);

            currentDisplayers.Add(element);

            element.Hide();
            if (updateDisplay)
                Display(true);
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
            {
                menuContainer.parent = this;
            }

            element.transform.SetParent(this.transform);
            element.transform.SetSiblingIndex(index);

            currentDisplayers.Insert(index, element);
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
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="updateDisplay"></param>
        /// <returns></returns>
        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            bool r = currentDisplayers.Remove(element);
            if (updateDisplay)
                Display(true);

            element.transform.SetParent(this.transform);

            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int RemoveAll()
        {
            int c = Count();
            foreach (AbstractDisplayer d in new List<AbstractDisplayer>(currentDisplayers))
                Remove(d, false);

            return c;
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

            return Remove(currentDisplayers[index], updateDisplay);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            return currentDisplayers;
        }

        public override void Clear()
        {
            base.Clear();
            currentDisplayers.Clear();
        }

        /// <summary>
        /// Finds the <see cref="AbstractMenuDisplayContainer"/> root of the menu.
        /// </summary>
        /// <returns></returns>
        public AbstractMenuDisplayContainer FindRoot()
        {
            var tmp = parent;

            while (tmp?.parent != null)
            {
                tmp = tmp.parent;
            }

            return tmp;
        }

        public int GetDepth()
        {
            var tmp = parent;
            int res = 0;

            while (tmp != null)
            {
                tmp = tmp.parent;
                res++;
            }

            return res;
        }
    }
}