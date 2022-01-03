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

using BrowserDesktop.UI;
using BrowserDesktop.UI.GenericElement;
using DesktopBrowser.UI.GenericElement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    /// <summary>
    /// A ToolboxScrollView has two buttons (previous and next) and a scrollView to contain toolboxes.
    /// </summary>
    /// 
    public partial class ToolboxScrollView_E
    {
        public Action<VisualElement> AddSeparator { get; set; } = (ve) => { Debug.Log("<color=green>TODO: </color>" + $"Add separator in ToolboxScrollView."); };

        private Button_GE backward;
        private VisualElement backwardLayout;
        private Button_GE forward;
        private VisualElement forwardLayout;
        private ScrollView scrollView;

        private List<AbstractGenericAndCustomElement> elements = new List<AbstractGenericAndCustomElement>();
        private int currentIndex = 0;
        private float totalWidth = 0f;

        private float scrollableWidth
        {
            get { return scrollView.contentContainer.layout.width - scrollView.contentViewport.layout.width; }
        }
    }

    public partial class ToolboxScrollView_E
    {
        public ToolboxScrollView_E(VisualElement root) : base(root) { }

        private void TestGoSecond()
        {
            Toolbox_E toolboxTarget = elements[4] as Toolbox_E;
            VisualElement toolboxRoot = toolboxTarget.Root;
            Debug.Log($"toolbox name = [{toolboxTarget.toolboxName}]");

            VisualElement contentContainer = scrollView.contentContainer;
            Debug.Log($"content container childCount = [{contentContainer.childCount}]; offset = [{scrollView.scrollOffset}]");
            Debug.Assert(contentContainer.Contains(toolboxRoot));

            //scrollView.ScrollTo(toolboxRoot);

            Toolbox_E toolbox0 = elements[0] as Toolbox_E;
            scrollView.scrollOffset = new Vector2(toolbox0.resolvedStyle.width, 0f);
            Debug.Log($"Scrolled; offset = [{scrollView.scrollOffset}], toolbox0 width = [{toolbox0.Root.layout.width}]");

            //VisualElement content = scrollView.Q("unity-content-container");
            //Debug.Assert(content.Contains(toolbox0));

            //VisualElement second = scrollView.ElementAt(1);
            ////VisualElement second = scrollView.
            //Debug.Log($"Test go second");
            //if (second is Toolbox_E toolbox) 
            //{
            //    Debug.Log($"toolbox = [{toolbox.toolboxName}]");
            //}
            //if (second is Scroller scroller)
            //{
            //    Debug.Log("scroller");
            //}
            //scrollView.ScrollTo(toolbox0);
        }

        public void DisplayButtons(bool value)
        {
            backwardLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
            forwardLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void AddToolboxes(params Toolbox_E[] toolboxes)
        {
            foreach (Toolbox_E toolbox in toolboxes)
            {
                toolbox.AddTo(scrollView);
                AddSeparator?.Invoke(scrollView);
                elements.Add(toolbox);
                totalWidth += toolbox.resolvedStyle.width;
            }

            //Debug.Log($"scrollable width = [{scrollableWidth}]; contentContainer = [{scrollView.contentContainer.resolvedStyle.width}]; contentViewPort = [{scrollView.contentViewport.layout.width}]; total width = [{totalWidth}]");
            //if (scrollableWidth > 0) DisplayButtons(true);
        }

        //public void AddElement(AbstractGenericAndCustomElement element)
        //{
        //    element.AddTo(scrollView);
        //    AddSeparator?.Invoke(scrollView);
        //    elements.Add(element);
        //}

        //public void AddElements(IEnumerable<AbstractGenericAndCustomElement> elements)
        //{
        //    foreach (AbstractGenericAndCustomElement elt in elements)
        //    {
        //        AddElement(elt);
        //    }
        //}

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            DisplayButtons((scrollableWidth > 0f) ? true : false);
        }
    }

    public partial class ToolboxScrollView_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();

            backward = new Button_GE(Root.Q("backward"))
            {
                OnClicked = () => { },
                IconPref = "scrollView-btn"
            }.SetIcon("menuBar-previous", "menuBar-previous-desable", true);
            forward = new Button_GE(Root.Q("forward"))
            {
                OnClicked = () => { TestGoSecond(); },
                IconPref = "scrollView-btn"
            }.SetIcon("menuBar-next", "menuBar-next-desable", true);
            backwardLayout = Root.Q<VisualElement>("horizontal-layout-backward");
            forwardLayout = Root.Q<VisualElement>("horizontal-layout-forward");
            scrollView = Root.Q<ScrollView>("scrollView");

            scrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            scrollView.contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            DisplayButtons(false);
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed)
                return;
        }
    }
}