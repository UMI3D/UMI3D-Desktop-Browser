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
        /*
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

        private SubContainer subTools { get; set; } = new SubContainer();
        */

        

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
            Debug.Log("<color=orange>Warn: </color>" + $"element has to be a ToolsContainer or a ToolDisplayer.");
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

            if (updateDisplay)
                UpdateDisplay();
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
                        UpdateDisplay();
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

        #region Display, Hide, Collapse, Extand

        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox generic element.")]
        private VisualTreeAsset toolbox_VTA;

        [SerializeField]
        [Tooltip("Visual Tree Asset of a toolbox button.")]
        private VisualTreeAsset toolboxButton_ge_VTA;

        private ToolboxGenericElement toolbox;
        private ToolboxButtonGenericElement toolButton;

        #region Initialisation and Clear

        /// <summary>
        /// Is this tools container initialized.
        /// </summary>
        /// <return>True if [toolbox] is not null, else False.</return>
        public bool Initialized => toolbox != null;

        public void InitAndBindUI()
        {
            if (Initialized) return;

            toolbox = toolbox_VTA.
                CloneTree().
                Q<ToolboxGenericElement>().
                Setup(menu.Name);

            toolButton = toolboxButton_ge_VTA.
                CloneTree().
                Q<ToolboxButtonGenericElement>().
                Setup(menu.Name, menu.icon2D, Select);
            (parent as ToolsContainer).
                toolbox.
                AddTool(toolButton);
        }

        public override void Clear()
        {
            base.Clear();
        }

        #endregion

        #region Display and Hide

        /// <summary>
        /// True if the tool button corresponding to this container is not null and displayed.
        /// </summary>
        public bool Displayed => toolButton != null && toolButton.Displayed;

        /// <summary>
        /// Display the button that will be use to extand the container.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
            Debug.Log("<color=green>TODO: </color>" + $"Display ToolsContainer");
        }

        public override void Hide()
        {
            Debug.Log("<color=green>TODO: </color>" + $"Hide ToolsContainer");
        }

        private void UpdateDisplay(AbstractDisplayer element)
        {
            if (Contains(element))
                element.Display();
            else
                element.Clear();
        }

        #endregion

        #region Collapse And Expand

        public override void Collapse(bool forceUpdate = false)
        {
            toolbox.Remove();
        }

        /// <summary>
        /// Display the toolbox.
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Expand(bool forceUpdate = false)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// No Use Here.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="forceUpdate"></param>
        public override void ExpandAs(AbstractMenuDisplayContainer container, bool forceUpdate = false)
        {
        }

        #endregion

        #endregion

        public override AbstractMenuDisplayContainer CurrentMenuDisplayContainer()
        {
            throw new System.NotImplementedException();
        }

        public VisualElement GetUXMLContent()
        {
            return toolbox;
        }

        

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<AbstractDisplayer> GetDisplayers()
        {
            throw new System.NotImplementedException();
        }







        #region Old
        /*
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
        */
        #endregion

    }
}