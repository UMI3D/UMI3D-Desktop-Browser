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
using System;
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
        struct SubContainer
        {
            public ToolsContainer Container { get; private set; }

            public void Register(ToolsContainer tools)
            {
                Debug.Assert(tools != null, "Tools null in SubContainer.Register()");
                Container = tools;
                if ((Container.parent as ToolsContainer).IsRootToolsContainer)
                    (Container.parent.parent as ToolsContainer).subTools.Register(tools);
            }

            public void Unregister()
            {
                Container = null;
            }

            public bool Collapse()
            {
                if (Container == null)
                    return false;

                Container.subTools.Collapse();
                GetToolbox()?.Remove();
                Unregister();
                return true;
            }

            public void Expand()
            {
                Environment.MenuBar_UIController.AddInSubMenu(GetToolbox(), GetParent());
            }

            public bool Equal(ToolsContainer container)
            {
                return this.Container == container;
            }

            private ToolboxGenericElement GetToolbox()
            {
                return (Container?.GetUXMLContent() as ToolboxGenericElement);
            }

            private ToolboxGenericElement GetParent()
            {
                return ((Container?.parent as ToolsContainer)?.GetUXMLContent() as ToolboxGenericElement);
            }

        }

        //public ToolsContainer ExpandedSubContainer { get; set; } = null;

        private SubContainer subTools { get; set; } = new SubContainer();

        /// <summary>
        /// True if this ToolsContainer will be display just after the root container.
        /// </summary>
        public bool IsRootToolsContainer { get; private set; } = false;

        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox generic element.")]
        private VisualTreeAsset toolbox_VTA;

        [SerializeField]
        [Tooltip("ToolDisplayer Prefab.")]
        private ToolDisplayer toolDisplayerPrefab;

        [SerializeField]
        [Tooltip("True if this ToolsContainer is associated with the root of the menu Asset.")]
        private bool isRootContainer = false;
        

        private List<AbstractDisplayer> toolDisplayers = new List<AbstractDisplayer>();

        private ToolboxGenericElement toolbox;

        private bool initialized = false;


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
            else isDisplayed = true;
            if (isRootContainer) return;

            Debug.Log($"Tools display in [{menu.Name}]");

            toolbox.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            if (!isDisplayed) return;
            else isDisplayed = false;
            if (isRootContainer) return;

            Debug.Log($"Tools hide in [{menu.Name}]");

            toolbox.style.display = DisplayStyle.None;
        }

        public override void Collapse(bool forceUpdate = false)
        {
            if (!isExpanded) return;
            else isExpanded = false;
            if (isRootContainer) return;

            subTools.Collapse();

            Debug.Log($"Tools [{menu.Name}] collapse");
            //ExpandedSubContainer?.Collapse();
            //ExpandedSubContainer?.Hide();
            //RemoveFromSubMenu();
            //ExpandedSubContainer = null;
        }

        public override void Expand(bool forceUpdate = false)
        {
            if (isExpanded) return;
            else isExpanded = true;
            if (isRootContainer) return;

            Debug.Log($"Tools [{menu.Name}] Expand");
            //ExpandedSubContainer?.Display();
        }

        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            return subTools.Container;
        }

        #region Private Functions

        //private void RemoveFromSubMenu()
        //{
        //    (ExpandedSubContainer?.GetUXMLContent() as ToolboxGenericElement)?.Remove();
        //}

        /// <summary>
        /// - Collapse expanded SubContainers.
        /// - If the current ExpandedSubContainer is different of subContainer then set and Expand new subContainer.
        /// </summary>
        /// <assert>blabla</assert>
        /// <param name="subContainer"></param>
        private void OnExpandedButtonPressed(ToolsContainer subContainer, bool collapseFromRoot = false)
        {
            //var expanded = subTools.Container;

            //if (subTools.Container != null)
            //    subTools.Collapse();
            //else
            //    CollapseFromRoot();

            //if (expanded != subContainer)
            //    SetExpandedSubContainerAndExpand(subContainer);

            bool isSameContainer = subTools.Equal(subContainer);

            if (collapseFromRoot || !subTools.Collapse())
                CollapseFromRoot();

            if (isSameContainer)
                return;

            subTools.Register(subContainer);
            subTools.Expand();
        }

        /// <summary>
        /// - If this container is a root ToolsContainer then Collapse from the root.
        /// </summary>
        private void CollapseFromRoot()
        {
            ToolsContainer root = this;
            while(!root.isRootContainer)
            {
                root = root.parent as ToolsContainer;
            }

            root.subTools.Collapse();

            //if (IsRootToolsContainer)
            //    (parent as ToolsContainer)?.ExpandedSubContainer?.parent?.Collapse();
        }

        /// <summary>
        /// - Set ExpandedSubContainer and root ExpandedSubContainer.
        /// - Expand
        /// </summary>
        /// <param name="subContainer"></param>
        //private void SetExpandedSubContainerAndExpand(ToolsContainer subContainer)
        //{
        //    //ExpandedSubContainer = subContainer;
        //    SetRootExpandedSubContainer(subContainer);
        //    //InsertInSubMenuBar(subContainer);
        //    Expand();
        //}

        /// <summary>
        /// - If this container is a root toolsContainer then set the root ExpandedSubContainer.
        /// </summary>
        /// <param name="subContainer"></param>
        //private void SetRootExpandedSubContainer(ToolsContainer subContainer)
        //{
        //    if (IsRootToolsContainer)
        //        (parent as ToolsContainer).ExpandedSubContainer = subContainer;
        //}

        //private void InsertInSubMenuBar(ToolsContainer subContainer)
        //{
        //    //Debug.Log("<color=green>TODO: </color>" + $"InsertSubContainerInSubMenuBar()");
        //    Environment.MenuBar_UIController.AddInSubMenu(subContainer.GetUXMLContent() as ToolboxGenericElement, this.toolbox);
        //}

        #endregion

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

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (isRootContainer) ? 2 : 1;
        }

        #region Insert

        public override void Insert(AbstractDisplayer element, bool updateDisplay = true)
        {
            InitAndBindUI();

            if (element is ToolDisplayer tool)
                Insert(tool);
            else if (element is ToolsContainer container)
                Insert(container);
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

        #region Private Functions

        /// <summary>
        /// - If this container is the root then insert the subContainer in the view.
        /// - Else insert a new toolDisplayer that will display the subContainer when pressed.
        /// </summary>
        /// <param name="subContainer"></param>
        private void Insert(ToolsContainer subContainer)
        {
            //Debug.Log($"Insert ToolsContainer [{subContainer.menu.Name}] in [{menu.Name}] for gameObject [{gameObject.name}]");
            if (isRootContainer)
                InsertAndDisplayContainerInMenuBar(subContainer);
            else
                CreateAndInsertToolDisplayerFor(subContainer);
        }

        /// <summary>
        /// - Add container in displayers list.
        /// - Add Toolbox_ge in View.
        /// </summary>
        /// <param name="subContainer"></param>
        private void InsertAndDisplayContainerInMenuBar(ToolsContainer subContainer)
        {
            toolDisplayers.Add(subContainer);
            subContainer.IsRootToolsContainer = true;
            Environment.MenuBar_UIController.Instance.AddToolbox(subContainer.GetUXMLContent() as ToolboxGenericElement);
        }

        /// <summary>
        /// - Create And Setup container as ToolDisplayer.
        /// - Insert ToolDisplyer.
        /// </summary>
        /// <param name="subContainer"></param>
        private void CreateAndInsertToolDisplayerFor(ToolsContainer subContainer)
        {
            ToolDisplayer toolDisplayer;
            CreateAndSetup(out toolDisplayer, subContainer);
            Insert(toolDisplayer);
        }

        /// <summary>
        /// - Instantiate ToolDisplayer.
        /// - Setup ToolDisplayer menu and ButtonPressed action.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="for_subContainer"></param>
        private void CreateAndSetup(out ToolDisplayer tool, ToolsContainer for_subContainer)
        {
            tool = Instantiate(toolDisplayerPrefab);
            tool.SetMenuItem(for_subContainer.menu);
            tool.OnButtonPressed = () => { OnExpandedButtonPressed(for_subContainer); };
        }

        /// <summary>
        /// - Add tool in toolDisplayers list.
        /// - Add tool in toolbox_ge.
        /// </summary>
        /// <param name="tool"></param>
        private void Insert(ToolDisplayer tool)
        {
            Debug.Log($"Insert Tool [{tool.menu.Name}] in [{menu.Name}]");
            toolDisplayers.Add(tool);
            toolbox.AddTool(tool.GetUXMLContent() as ToolboxButtonGenericElement);
        }

        #endregion

        #endregion

        #region Remove

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

        #endregion

    }
}