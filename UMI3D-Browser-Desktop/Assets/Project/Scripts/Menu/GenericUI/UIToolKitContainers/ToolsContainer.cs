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
        [Tooltip("Visual Tree Asset of a toolboxButton generic element.")]
        private ToolDisplayer toolDisplayerPrefab;

        [SerializeField]
        [Tooltip("True if this ToolsContainer is associated with the root of the menu Asset.")]
        private bool isRootContainer = false;
        private bool isRootTools = false;
        public bool IsRootTools
        {
            get => isRootTools;
            set => isRootTools = value;
        }

        private List<AbstractDisplayer> toolDisplayers = new List<AbstractDisplayer>();

        private ToolboxGenericElement toolbox;

        private bool initialized = false;

        public ToolsContainer ExpandedContainer { get; set; }


        #region ToolDisplayers list

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

        public override bool Contains(AbstractDisplayer element)
        {
            return toolDisplayers.Contains(element);
        }

        public override int Count()
        {
            return toolDisplayers.Count;
        }

        public override int GetIndexOf(AbstractDisplayer element)
        {
            return toolDisplayers.IndexOf(element);
        }

        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            return toolDisplayers;
        }

        #endregion

        #region Display, Hide and Expand, Collapse

        public override void Display(bool forceUpdate = false)
        {
            if (isDisplayed && !forceUpdate) return;
            if (isRootContainer) return;

            Debug.Log($"Tools display in [{menu.Name}]");

            toolbox.style.display = DisplayStyle.Flex;

            isDisplayed = true;
        }

        public override void Hide()
        {
            if (!isDisplayed) return;
            if (isRootContainer) return;

            Debug.Log($"Tools hide in [{menu.Name}]");

            toolbox.style.display = DisplayStyle.None;

            isDisplayed = false;
        }

        public override void Collapse(bool forceUpdate = false)
        {
            if (!isExpanded) return;
            if (isRootContainer) return;

            Debug.Log($"Tools [{menu.Name}] collapse");
            ExpandedContainer?.Collapse();
            ExpandedContainer?.Hide();          
            ExpandedContainer = null;

            isExpanded = false;
        }

        public override void Expand(bool forceUpdate = false)
        {
            if (isExpanded) return;
            if (isRootContainer) return;

            Debug.Log($"Tools [{menu.Name}] Expand");
            ExpandedContainer?.Display();

            isExpanded = true;
        }

        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            return ExpandedContainer;
        }

        private void OnContainerExpanded(ToolsContainer subContainer)
        {
            if (ExpandedContainer != null && ExpandedContainer == subContainer)
                return;
            else if (ExpandedContainer == null && isRootTools)
                (parent as ToolsContainer)?.ExpandedContainer?.Collapse();
            else
                Collapse();

            ExpandedContainer = subContainer;
            if (isRootTools)
                (parent as ToolsContainer).ExpandedContainer = subContainer;
            Expand();
        }

        #endregion



        public VisualElement GetUXMLContent()
        {
            return toolbox;
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

        #region Insert

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            InitAndBindUI();

            if (element is ToolDisplayer tool)
                InsertToolDisplayer(tool);
            else if (element is ToolsContainer container)
                InsertSubContainer(container);
            else
                throw new System.Exception($"This type of AbstractDisplayer is not supported yet in ToolsContainer.");

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

        /// <summary>
        /// - If this container is the root then insert the subContainer in the view.
        /// - Else insert a new toolDisplayer that will display the subContainer when pressed.
        /// </summary>
        /// <param name="subContainer"></param>
        private void InsertSubContainer(ToolsContainer subContainer)
        {
            Debug.Log($"Insert ToolsContainer [{subContainer.menu.Name}] in [{menu.Name}] for gameObject [{gameObject.name}]");
            if (isRootContainer)
            {
                toolDisplayers.Add(subContainer);
                Environment.MenuBar_UIController.Instance.AddToolbox(subContainer.GetUXMLContent() as ToolboxGenericElement);
            }
            else
            {
                ToolDisplayer toolDisplayer;
                CreateAndSetupToolAsContainer(out toolDisplayer, subContainer);
                InsertToolDisplayer(toolDisplayer);
            }
        }

        /// <summary>
        /// - Add tool in toolDisplayers list.
        /// - Add tool in toolbox UI.
        /// </summary>
        /// <param name="tool"></param>
        private void InsertToolDisplayer(ToolDisplayer tool)
        {
            Debug.Log($"Insert Tool [{tool.menu.Name}] in [{menu.Name}]");
            toolDisplayers.Add(tool);
            toolbox.AddTool(tool.GetUXMLContent() as ToolboxButtonGenericElement);
        }

        /// <summary>
        /// - Instantiate ToolDisplayer.
        /// - Setup ToolDisplayer menu and ButtonPressed action.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="subContainer"></param>
        private void CreateAndSetupToolAsContainer(out ToolDisplayer tool, ToolsContainer subContainer)
        {
            tool = Instantiate(toolDisplayerPrefab);
            tool.SetMenuItem(subContainer.menu);
            tool.OnButtonPressed = () => { OnContainerExpanded(subContainer); };
        }

        #endregion

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (isRootContainer) ? 2 : 1;
        }

        public override bool Remove(AbstractDisplayer element, bool updateDisplay = true)
        {
            if (element is ToolDisplayer tool)
            {
                toolDisplayers.Remove(tool);
                (tool.GetUXMLContent() as ToolboxButtonGenericElement).Remove();
            }
            else if (element is ToolsContainer subContainer)
            {
                toolDisplayers.Remove(subContainer);
                (subContainer.GetUXMLContent() as ToolboxGenericElement).Remove();
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

        
    }
}