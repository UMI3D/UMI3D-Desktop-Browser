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
        public float DeltaScroll { get; set; } = 50f;
        /// <summary>
        /// Scroll animation time in ms.
        /// </summary>
        public int ScrollAnimationTime { get; set; } = 500;
    }

    public partial class ToolboxScrollView_E
    {
        private Button_GE backward;
        private VisualElement backwardLayout;
        private bool m_backwardLayoutDisplayed { get; set; } = false;
        private Button_GE forward;
        private VisualElement forwardLayout;
        private bool m_forwardLayoutDisplayed { get; set; } = false;
        private ScrollView scrollView;

        private List<AbstractGenericAndCustomElement> elements = new List<AbstractGenericAndCustomElement>();
        //private int currentIndex = 0;

        private float m_scrolledWidth { get; set; } = 0f;

        private float scrollableWidth
        {
            get { return scrollView.contentContainer.layout.width - scrollView.contentViewport.layout.width; }
        }
        //private VisualElement scrollViewContentContainer
        //{
        //    get => scrollView.contentContainer;
        //}

        private enum ButtonType { BACKWARD, FORWARD }
    }

    public partial class ToolboxScrollView_E
    {
        public ToolboxScrollView_E(VisualElement root) : base(root) { }

        

        public void DisplayButtons(bool value)
        {
            DisplaysBackwardButton(value);
            DisplaysForwardButton(value);
        }
        public void DisplaysForwardButton(bool value)
        {
            m_forwardLayoutDisplayed = value;
            //forwardLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
            forwardLayout.style.visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }
        public void DisplaysBackwardButton(bool value)
        {
            m_backwardLayoutDisplayed = value;
            //backwardLayout.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
            backwardLayout.style.visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }

        public void AddToolboxes(params Toolbox_E[] toolboxes)
        {
            foreach (Toolbox_E toolbox in toolboxes)
            {
                toolbox.AddTo(scrollView);
                AddSeparator.Invoke(scrollView);
                elements.Add(toolbox);
            }
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

        
    }

    public partial class ToolboxScrollView_E
    {
        private void OnForwardPressed() => ButtonPressed(ButtonType.FORWARD);

        private void OnBackwardPressed() => ButtonPressed(ButtonType.BACKWARD);

        private void ButtonPressed(ButtonType buttonType)
        {
            if (buttonType == ButtonType.BACKWARD && !m_forwardLayoutDisplayed) DisplaysForwardButton(true);
            if (buttonType == ButtonType.FORWARD && !m_backwardLayoutDisplayed) DisplaysBackwardButton(true);

            //m_scrolledWidth -= scrollViewContentContainer[currentIndex].layout.width;
            //m_scrolledWidth -= scrollViewContentContainer[currentIndex - 1].layout.width;

            m_scrolledWidth += (buttonType == ButtonType.BACKWARD) ? -DeltaScroll : DeltaScroll;

            if (buttonType == ButtonType.BACKWARD && m_scrolledWidth < 0f)
            {
                m_scrolledWidth = 0f;
                DisplaysBackwardButton(false);
            }
            if (buttonType == ButtonType.FORWARD && m_scrolledWidth > scrollableWidth)
            {
                m_scrolledWidth = scrollableWidth;
                DisplaysForwardButton(false);
            }

            scrollView.experimental.animation.Start
                (scrollView.scrollOffset.x,
                m_scrolledWidth,
                ScrollAnimationTime,
                (ve, value) => { scrollView.scrollOffset = new Vector2(value, 0f); });

            //currentIndex += (buttonType == ButtonType.BACKWARD) ? -2 : 2;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            m_scrolledWidth = scrollView.scrollOffset.x;

            DisplaysBackwardButton((scrollableWidth > 0f && m_scrolledWidth != 0f) ? true : false);
            DisplaysForwardButton((scrollableWidth > 0f && m_scrolledWidth != scrollableWidth) ? true : false);
        }
    }

    public partial class ToolboxScrollView_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();

            backward = new Button_GE(Root.Q("backward"))
            {
                OnClicked = () => { OnBackwardPressed(); }
            }.SetIcon(Root.Q("backward"),"scrollView-btn", "menuBar-previous", "menuBar-previous-desable", true);
            forward = new Button_GE(Root.Q("forward"))
            {
                OnClicked = () => { OnForwardPressed(); }
            }.SetIcon(Root.Q("forward"),"scrollView-btn", "menuBar-next", "menuBar-next-desable", true);
            backwardLayout = Root.Q<VisualElement>("horizontal-layout-backward");
            forwardLayout = Root.Q<VisualElement>("horizontal-layout-forward");

            //new AbstractScrollView_E(Root.Q<ScrollView>("scrollView"), null)
            //    .InitFromSrollViewToProperties();


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