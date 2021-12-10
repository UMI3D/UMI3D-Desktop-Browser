/*
Copyright 2019 Gfi Informatique

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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Container
{
    /// <summary>
    /// A container of toolboxes. This is the root of all the toolboxes.
    /// </summary>
    public class ToolboxesContainer : AbstractMenuDisplayContainer, IDisplayerElement
    {

        #region Toolboxes List

        private List<RootToolsContainer> toolboxes = new List<RootToolsContainer>();

        public override AbstractDisplayer this[int i]
        {
            get
            {
                if (i >= Count())
                    throw new System.Exception("Toolboxes container out of range.");
                return toolboxes[i];
            }
            set 
            {
                if (value is RootToolsContainer rootTools)
                    toolboxes[i] = rootTools;
                else
                    throw new System.Exception("Value has to be a RootToolsConainer.");
            }
        }

        public override bool Contains(AbstractDisplayer element)
        {
            if (element is RootToolsContainer rootTools)
                return toolboxes.Contains(rootTools);
            Debug.LogWarning($"element is not a RootToolsContainer.");
            return false;
        }

        public override int Count()
        {
            return toolboxes.Count;
        }

        public override int GetIndexOf(AbstractDisplayer element)
        {
            if (Contains(element))
                return toolboxes.IndexOf(element as RootToolsContainer);
            else
                throw new System.Exception("toolboxes doesn't containe element.");
        }

        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            return toolboxes;
        }

        #region Insert

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            Insert(element, Count(), updateDisplay);
        }

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (element is RootToolsContainer rootTools)
            {
                if (toolboxes.Contains(rootTools))
                    throw new System.Exception("toolboxes contains already this element");
                else
                    toolboxes.Insert(index, rootTools);
            }
            else
                throw new System.Exception("element has to be a RootToolsConainer.");

            if (updateDisplay)
                Debug.Log("<color=green>TODO: </color>" + $"UpdateDisplay");
        }

        public override void InsertRange(IEnumerable<AbstractDisplayer> elements, bool updateDisplay = true)
        {
            foreach (AbstractDisplayer element in elements)
                Insert(element, updateDisplay);
        }

        #endregion

        #region Remove

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (element is RootToolsContainer rootTools)
            {
                if (toolboxes.Remove(rootTools))
                {
                    if (updateDisplay)
                    {
                        Debug.Log("<color=green>TODO: </color>" + $"UpdateDisplay");
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                throw new System.Exception("element has to be a RootToolsConainer.");
        }

        public override int RemoveAll()
        {
            int count = 0;
            for (int i = 0; i < Count(); ++i)
            {
                if (RemoveAt(i, true))
                    ++count;
            }
            return count;
        }

        public override bool RemoveAt(int index, bool updateDisplay = true)
        {
            return Remove(this[index], updateDisplay);
        }

        #endregion

        #endregion

        #region Display, Hide, Collapse and Expand

        private void UpdateDisplay()
        {

        }

        /// <summary>
        /// Display the button that will be use to extand the container.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            //toolboxes.ForEach((rootTools) => { rootTools.Display(forceUpdate); });
        }

        public override void Hide()
        {
            //toolboxes.ForEach((rootTools) => { rootTools.Hide(); });
        }

        /// <summary>
        /// Not use here.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Collapse(bool forceUpdate = false)
        {
        }

        /// <summary>
        /// Display all Toolboxes in the view.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Expand(bool forceUpdate = false)
        {
            toolboxes.ForEach((toolbox) => toolbox.Expand());
        }

        /// <summary>
        /// Not use here.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
        }

        #endregion

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            throw new System.NotImplementedException();
        }

        public VisualElement GetUXMLContent()
        {
            throw new System.NotImplementedException();
        }

        public void InitAndBindUI()
        {
            throw new System.NotImplementedException();
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            throw new System.NotImplementedException();
        }
    }
}