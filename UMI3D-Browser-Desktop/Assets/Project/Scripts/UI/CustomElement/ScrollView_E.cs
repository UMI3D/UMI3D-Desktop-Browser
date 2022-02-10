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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public enum ScrollDirection { Vertical, Horizontal, HorizontalAndVertical }
    public enum ScrollBarDirection { VERTICAL, HORIZONTAL, BOTH, NONE }
    public enum ScrollButtonDirection { BACKWARD, FORWARD }

    public interface IScrollable
    {
        //public ScrollView ScrollView { get; set; }
        public ScrollDirection scrollDirection { get; set; }
        public float DeltaScroll { get; set; }
        /// <summary>
        /// Scroll animation time in ms.
        /// </summary>
        public int ScrollAnimationTime { get; set; }
        public bool DisplayedScrollButtons { get; set; }
        public ScrollBarDirection DisplayedScrollBar { get; set; }
        public UnityEvent VerticalBackwardButtonPressed { get; }
        public UnityEvent VerticalForwardButtonPressed { get; }
        public UnityEvent HorizontalBackwardButtonPressed { get; }
        public UnityEvent HorizontalForwardButtonPressed { get; }
        public float SpaceBetweenItems { get; set; }
    }

    public partial class ScrollView_E : IScrollable
    {
        public ScrollDirection scrollDirection { get; set; } = ScrollDirection.Vertical;
        public float DeltaScroll { get; set; } = 50f;
        public int ScrollAnimationTime { get; set; } = 500;
        public bool DisplayedScrollButtons { get; set; } = false;
        public ScrollBarDirection DisplayedScrollBar { get; set; } = ScrollBarDirection.NONE;
        public UnityEvent VerticalBackwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent VerticalForwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent HorizontalBackwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent HorizontalForwardButtonPressed { get; protected set; } = new UnityEvent();
        public float SpaceBetweenItems { get; set; } = 10f;
        public Func<VisualElement, Visual_E> AddSeparator { get; set; } = null;

        public virtual void Adds(params Visual_E[] items)
        {
            foreach (Visual_E item in items)
            {
                Visual_E separator = null;
                if (m_items.Count > 0 && AddSeparator != null)
                    separator = AddSeparator(Scroll_View);
                m_items.Add((item, separator));
                item.InsertRootTo(Scroll_View);
            }
        }
    }

    public partial class ScrollView_E
    {
        public ScrollView Scroll_View { get; private set; } = null;

        protected Scroller m_verticalScroller { get; set; } = null;
        protected Scroller m_horizontalScroller { get; set; } = null;
        protected Slider m_verticalSlider { get; set; } = null;
        protected Slider m_horizontalSlider { get; set; } = null;
        protected VisualElement m_verticalDraggerContainer { get; set; } = null;
        protected VisualElement m_horizontalDraggerContainer { get; set; } = null;
        protected VisualElement m_verticalDragger { get; set; } = null;
        protected VisualElement m_horizontalDragger { get; set; } = null;

        protected VisualElement m_backwardVerticalButton { get; set; } = null;
        protected VisualElement m_backwardHorizontalButton { get; set; } = null;
        protected VisualElement m_forwardVerticalButton { get; set; } = null;
        protected VisualElement m_forwardHorizontalButton { get; set; } = null;

        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;

        protected List<(Visual_E, Visual_E)> m_items { get; set; } = null;

        
    }

    public partial class ScrollView_E
    {
        public ScrollView_E(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys) : 
            base(visual, styleResourcePath, formatAndStyleKeys) { }
        public ScrollView_E(string visualResourcePath, string styleResourcePath, StyleKeys formatAndStyleKeys) :
            base(visualResourcePath, styleResourcePath, formatAndStyleKeys) { }
        public ScrollView_E(VisualElement parent, string visualResourcePath, string styleResourcePath, StyleKeys formatAndStyleKeys) : 
            base(parent, visualResourcePath, styleResourcePath, formatAndStyleKeys) { }

        public ScrollView_E SetDraggerContainerStyle(string styleResourcePath, StyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_verticalSlider, styleResourcePath, formatAndStyleKeys);
            new Visual_E(m_horizontalSlider, styleResourcePath, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetVerticalDraggerContainerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            //m_verticalDraggerContainer.ClearClassList();
            //new Icon_E(m_verticalScroller, customStyleKey, null);
            new Visual_E(m_verticalScroller, customStyleKey, formatAndStyleKeys);
            m_verticalScroller.style.opacity = 1f;
            m_verticalScroller.style.alignItems = Align.Center;
            //m_verticalScroller.style.bac
            //new Icon_E(m_verticalSlider, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public ScrollView_E SetHorizontalDraggerContainerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_horizontalDraggerContainer, customStyleKey, formatAndStyleKeys);
            return this;
        }

        public ScrollView_E SetDraggerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_verticalDragger, customStyleKey, formatAndStyleKeys);
            new Visual_E(m_horizontalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetVerticalDraggerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            m_verticalDragger.ClearClassList();
            new Visual_E(m_verticalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetHorizontalDraggerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_horizontalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetVerticalBackwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_backwardVerticalButton, 
                buttonContainer, 
                () => m_backwardVerticalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public ScrollView_E SetVerticalForwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_forwardVerticalButton, 
                buttonContainer, 
                () => m_forwardVerticalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public ScrollView_E SetHorizontalBackwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_backwardHorizontalButton, 
                buttonContainer, 
                () => m_backwardHorizontalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public ScrollView_E SetHorizontalForwarddButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_forwardHorizontalButton, 
                buttonContainer, 
                () => m_forwardHorizontalButtonLayout = buttonLayout,
                styleResourceKey, 
                styleKeys);
    }

    public partial class ScrollView_E
    {
        //protected void AddSpace(Action<IStyle> applySpace)
        //{
        //    VisualElement space = new VisualElement();
        //    applySpace(space.style);
        //    m_scrollView.Add(space);
        //}
        //protected void AddVerticalSpace() => AddSpace((style) => style.height = SpaceBetweenItems);
        //protected void AddHorizontalSpace() => AddSpace((style) => style.width = SpaceBetweenItems);

        protected ScrollView_E SetButton(VisualElement button, VisualElement buttonContainer, Action bindButtonLayout, string styleResourceKey, StyleKeys styleKeys)
        {
            if (buttonContainer == null) throw new NullReferenceException("Button container null");
            buttonContainer.Add(button);
            AddVisualStyle(button, styleResourceKey, styleKeys);
            bindButtonLayout();
            return this;
        }

        protected void OnGeometryChanged(GeometryChangedEvent e)
        {
            VerticalSliderValueChanged(m_verticalSlider.value);
            HorizontalSliderValueChanged(m_horizontalSlider.value);
        }
        
        protected void SliderValueChanged(float value, Slider slider, VisualElement backwardButton, VisualElement forwardButton)
        {
            if (backwardButton != null) backwardButton.visible = (value > slider.lowValue) ? true : false;
            if (forwardButton != null) forwardButton.visible = (value < slider.highValue) ? true : false;
            //Debug.Log($"value = [{value}]; low = [{slider.lowValue}]; high = [{slider.highValue}]");
            //if (backwardButton != null) backwardButton.style.display = (value > slider.lowValue) ? DisplayStyle.Flex : DisplayStyle.None;
            //if (forwardButton != null) forwardButton.style.display = (value < slider.highValue) ? DisplayStyle.Flex : DisplayStyle.None;
        }
        protected void VerticalSliderValueChanged(float value)
            => SliderValueChanged(value, m_verticalSlider, m_backwardVerticalButtonLayout, m_forwardVerticalButtonLayout);
        protected void HorizontalSliderValueChanged(float value)
            => SliderValueChanged(value, m_horizontalSlider, m_backwardHorizontalButtonLayout, m_forwardHorizontalButtonLayout);
    }

    public partial class ScrollView_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            Scroll_View = Root.Q<ScrollView>();
            m_verticalScroller = Scroll_View.verticalScroller;
            m_horizontalScroller = Scroll_View.horizontalScroller;
            m_verticalSlider = m_verticalScroller.slider;
            m_horizontalSlider = m_horizontalScroller.slider;

            //m_verticalDraggerContainer = m_verticalSlider.Q("unity-drag-container");
            //m_horizontalDraggerContainer = m_horizontalSlider.Q("unity-drag-container");

            m_verticalDraggerContainer = m_verticalSlider.Q("unity-tracker");
            m_horizontalDraggerContainer = m_horizontalSlider.Q("unity-tracker");

            m_verticalDragger = m_verticalSlider.Q("unity-dragger");
            m_horizontalDragger = m_horizontalSlider.Q("unity-dragger");

            m_backwardVerticalButton = m_verticalScroller.lowButton;
            m_forwardVerticalButton = m_verticalScroller.highButton;
            m_backwardHorizontalButton = m_horizontalScroller.lowButton;
            m_forwardHorizontalButton = m_horizontalScroller.highButton;

            Scroll_View.contentContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            Scroll_View.contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            m_verticalScroller.valueChanged += VerticalSliderValueChanged;
            m_horizontalScroller.valueChanged += HorizontalSliderValueChanged;

            m_items = new List<(Visual_E, Visual_E)>();
        }

        public override void Reset()
        {
            base.Reset();
            throw new System.NotImplementedException();
        }
    }
}

