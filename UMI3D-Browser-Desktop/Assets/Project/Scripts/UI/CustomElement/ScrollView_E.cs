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

        public void Adds(params VisualElement[] items)
        {

        }
    }

    public partial class ScrollView_E
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

    public partial class ScrollView_E
    {
        public ScrollView_E(VisualElement visual, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) : 
            base(visual, styleResourcePath, formatAndStyleKeys) { }
        public ScrollView_E(string visualResourcePath, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) :
            base(visualResourcePath, styleResourcePath, formatAndStyleKeys) { }
        public ScrollView_E(VisualElement parent, string visualResourcePath, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) : 
            base(parent, visualResourcePath, styleResourcePath, formatAndStyleKeys) { }

        public ScrollView_E SetDraggerContainerStyle(string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_verticalSlider, styleResourcePath, formatAndStyleKeys);
            new Visual_E(m_horizontalSlider, styleResourcePath, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetVerticalDraggerContainerStyle(string customStyleKey, FormatAndStyleKeys formatAndStyleKeys)
        {
            //new Icon_E(m_verticalScroller, customStyleKey, null);
            new Visual_E(m_verticalScroller, customStyleKey, formatAndStyleKeys);
            m_verticalScroller.style.opacity = 1f;
            //new Icon_E(m_verticalSlider, customStyleKey, customStyleBackgroundKey);
            return this;
        }
        public ScrollView_E SetHorizontalDraggerContainerStyle(string customStyleKey, FormatAndStyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_horizontalDraggerContainer, customStyleKey, formatAndStyleKeys);
            return this;
        }

        public ScrollView_E SetDraggerStyle(string customStyleKey, FormatAndStyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_verticalDragger, customStyleKey, formatAndStyleKeys);
            new Visual_E(m_horizontalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetVerticalDraggerStyle(string customStyleKey, FormatAndStyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_verticalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }
        public ScrollView_E SetHorizontalDraggerStyle(string customStyleKey, FormatAndStyleKeys formatAndStyleKeys)
        {
            new Visual_E(m_horizontalDragger, customStyleKey, formatAndStyleKeys);
            return this;
        }

        public ScrollView_E SetButtonStyle()
        {
            return this;
        }

    }

    public partial class ScrollView_E
    {
        //protected virtual Ini
    }

    public partial class ScrollView_E : Visual_E
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
    }
}

