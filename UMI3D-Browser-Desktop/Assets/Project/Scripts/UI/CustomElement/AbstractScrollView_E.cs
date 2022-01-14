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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
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

        public IScrollable InitFromPropertiesToScrollView();
        public IScrollable InitFromSrollViewToProperties();
    }

    public partial class AbstractScrollView_E : IScrollable
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

        
        public IScrollable InitFromPropertiesToScrollView()
        {
            throw new System.NotImplementedException();
        }
        public IScrollable InitFromSrollViewToProperties()
        {
            //m_scrollView.
            
            return this;
        }
    }

    public partial class AbstractScrollView_E
    {
        protected ScrollView m_scrollView { get; set; } = null;
        protected Scroller m_verticalScroller { get; set; } = null;
        protected Scroller m_horizontalScroller { get; set; } = null;
        protected Slider m_verticalSlider { get; set; } = null;
        protected Slider m_horizontalSlider { get; set; } = null;
        protected VisualElement m_verticalDraggerContainer { get; set; } = null;
        protected VisualElement m_horizontalDraggerContainer { get; set; } = null;
        protected VisualElement m_verticalDragger { get; set; } = null;
        protected VisualElement m_horizontalDragger { get; set; } = null;
        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;
    }

    public partial class AbstractScrollView_E
    {
        public AbstractScrollView_E() : base() { }
        public AbstractScrollView_E(VisualElement root, string customStyleKey) : base(root, customStyleKey)
        {
            //ScrollView = Root.Q<ScrollView>();
        }
        public AbstractScrollView_E(VisualElement parent, string resourcePath, string customStyleKey, string customStyleBackgroundKey = "") : base(parent, resourcePath, customStyleKey, customStyleBackgroundKey) { }

        public AbstractScrollView_E SetDraggerContainerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_verticalSlider, customStyleKey, customStyleBackgroundKey);
            new Icon_E(m_horizontalSlider, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public AbstractScrollView_E SetVerticalDraggerContainerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_verticalScroller, customStyleKey, null);
            new Icon_E(m_verticalSlider, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public AbstractScrollView_E SetHorizontalDraggerContainerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_horizontalDraggerContainer, customStyleKey, customStyleBackgroundKey);
            return this;
        }

        public AbstractScrollView_E SetDraggerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_verticalDragger, customStyleKey, customStyleBackgroundKey);
            new Icon_E(m_horizontalDragger, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public AbstractScrollView_E SetVerticalDraggerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_verticalDragger, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public AbstractScrollView_E SetHorizontalDraggerStyle(string customStyleKey, string customStyleBackgroundKey = "")
        {
            new Icon_E(m_horizontalDragger, customStyleKey, customStyleBackgroundKey);
            return this;
        }

        public AbstractScrollView_E SetButtonStyle()
        {
            return this;
        }

    }

    public partial class AbstractScrollView_E
    {
        //protected virtual Ini
    }

    public partial class AbstractScrollView_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_scrollView = Root.Q<ScrollView>();
            m_verticalScroller = m_scrollView.verticalScroller;
            m_horizontalScroller = m_scrollView.horizontalScroller;
            m_verticalSlider = m_verticalScroller.slider;
            m_horizontalSlider = m_horizontalScroller.slider;
            //m_verticalDraggerContainer = m_verticalSlider.Q("unity-drag-container");
            //m_horizontalDraggerContainer = m_horizontalSlide.Q("unity-drag-container");
            m_verticalDragger = m_verticalSlider.Q("unity-dragger");
            m_horizontalDragger = m_horizontalSlider.Q("unity-dragger");
        }
        public override void OnApplyUserPreferences()
        {
            //throw new System.NotImplementedException();
        }
    }
}

