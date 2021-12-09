using BrowserDesktop.Menu.Displayer;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Container
{
    public class RootToolsContainer : AbstractMenuDisplayContainer, IDisplayerElement
    {
        #region toolboxesAndTools List

        private List<AbstractDisplayer> toolboxesAndTools = new List<AbstractDisplayer>();

        public override AbstractDisplayer this[int i]
        {
            get
            {
                if (i >= Count())
                    throw new System.Exception("Tools container out of range.");
                return toolboxesAndTools[i];
            }
            set
            {
                if (MatchType(value))
                    toolboxesAndTools[i] = value;
                else
                    throw new System.Exception("Value has to be a ToolsContainer or a ToolDisplayer.");
            }
        }

        public override bool Contains(AbstractDisplayer element)
        {
            if (MatchType(element))
                return toolboxesAndTools.Contains(element);
            Debug.LogWarning($"element has to be a ToolsContainer or a ToolDisplayer.");
            return false;
        }

        public override int Count()
        {
            return toolboxesAndTools.Count;
        }

        public override int GetIndexOf(AbstractDisplayer element)
        {
            if (Contains(element))
                return toolboxesAndTools.IndexOf(element);
            else
                throw new System.Exception("toolboxesAndTools doesn't containe element.");
        }

        #region Insert

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            Insert(element, Count(), updateDisplay);
        }

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            if (!MatchType(element))
                throw new System.Exception("element has to be a ToolsContainer or a ToolDisplayer.");
            else
            {
                if (toolboxesAndTools.Contains(element))
                    throw new System.Exception("toolboxesAndTools contains already this element");
                else
                    toolboxesAndTools.Insert(index, element);
            }

            if(updateDisplay)
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
            if (MatchType(element))
            {
                if (toolboxesAndTools.Remove(element))
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
                throw new System.Exception("element has to be a ToolsContainer or a ToolDisplayer.");
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

        protected virtual bool MatchType(AbstractDisplayer element)
        {
            if (element is ToolsContainer || element is ToolDisplayer)
                return true;
            else
                return false;
        }

        #endregion

        public override void Display(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override void Hide()
        {
            throw new System.NotImplementedException();
        }

        #region Collapse And Expand

        public override void Collapse(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override void Expand(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
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

        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            throw new System.NotImplementedException();
        }
    }
}

