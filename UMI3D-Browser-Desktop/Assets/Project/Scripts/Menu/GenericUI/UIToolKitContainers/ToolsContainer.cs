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

using BrowserDesktop.Menu.Displayer;
using BrowserDesktop.UI.GenericElement;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu.Container
{
    public class ToolsContainer : AbstractMenuDisplayContainer, IDisplayerElement
    {
        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox generic element.")]
        private VisualTreeAsset toolbox_VTA;

        [SerializeField]
        [Tooltip("True if this ToolsContainer is associated with the root of the menu Asset.")]
        private bool isRootContainer = false;

        private List<AbstractDisplayer> toolDisplayers = new List<AbstractDisplayer>();

        private ToolboxGenericElement toolbox;

        private bool initialized = false;

        public override AbstractDisplayer this[int i] 
        {
            get
            {
                if (toolDisplayers.Count <= i)
                    throw new System.Exception("Toolbox container out of range.");
                return toolDisplayers[i];
            }
            set => toolDisplayers[i] = value; 
        }

        public override void Collapse(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override bool Contains(AbstractDisplayer element)
        {
            return toolDisplayers.Contains(element);
        }

        public override int Count()
        {
            return toolDisplayers.Count;
        }

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            throw new System.NotImplementedException();
        }

        public override void Display(bool forceUpdate = false)
        {
            if (isDisplayed)
                return;

            toolbox.style.display = DisplayStyle.Flex;
            toolDisplayers.ForEach((elt) => { elt.Display(); });
            isDisplayed = true;
        }

        public override void Expand(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override int GetIndexOf(AbstractDisplayer element)
        {
            return toolDisplayers.IndexOf(element);
        }

        public VisualElement GetUXMLContent()
        {
            return toolbox;
        }

        public override void Hide()
        {
            throw new System.NotImplementedException();
        }

        public void InitAndBindUI()
        {
            if (initialized) return;
            else initialized = true;

            if (isRootContainer) return;

            toolbox = toolbox_VTA.CloneTree().Q<ToolboxGenericElement>();
            toolbox.Setup(menu.Name);
        }

        //public override void SetMenuItem(AbstractMenuItem menu)
        //{
        //    base.SetMenuItem(menu);
        //    Debug.Log($"set menu in toolsContainer = {menu.Name}");
        //}

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            InitAndBindUI();

            if (element is ToolDisplayer tool)
            {
                Debug.Log($"ToolDisplayer = {tool.menu.Name} in {menu.Name}");
                toolDisplayers.Add(tool);
                if (isRootContainer) Debug.Log($"Is root container {menu.Name}");
                toolbox.AddTool(tool.GetUXMLContent() as ToolboxButtonGenericElement);
            }
            else if (element is ToolsContainer container)
            {
                Debug.Log($"ToolsContainer = {container.menu.Name} in {menu.Name}");
                if (isRootContainer)
                {
                    toolDisplayers.Add(container);
                    Environment.MenuBar_UIController.Instance.AddToolbox(container.GetUXMLContent() as ToolboxGenericElement);
                }
            } else
            {
                throw new System.Exception($"This type of AbstractDisplayer is not supported yet in ToolsContainer.");
            }

            if (updateDisplay)
                Display();
        }

        public override void Insert(AbstractDisplayer element, int index, bool updateDisplay = true)
        {
            InitAndBindUI();
            throw new System.NotImplementedException();
        }

        public override void InsertRange(IEnumerable<AbstractDisplayer> elements, bool updateDisplay = true)
        {
            InitAndBindUI();
            foreach(AbstractDisplayer elt in elements) { Insert(elt, updateDisplay); }
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is umi3d.cdk.menu.Menu) ? 1 : 0;
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (element is ToolDisplayer tool)
            {
                toolDisplayers.Remove(tool);
                (tool.GetUXMLContent() as ToolboxButtonGenericElement).Remove();
            }
            else
            {
                throw new System.Exception($"This type of AbstractDisplayer is not supported yet in ToolsContainer.");
            }

            if (updateDisplay)
                Display();

            return true;
        }

        public override int RemoveAll()
        {
            List<AbstractDisplayer> elements = new List<AbstractDisplayer>(toolDisplayers);
            int count = 0;
            foreach (AbstractDisplayer element in elements)
                if (Remove(element, false)) count++;
            return count;
        }

        public override bool RemoveAt(int index, bool updateDisplay = true)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            return toolDisplayers;
        }
    }
}