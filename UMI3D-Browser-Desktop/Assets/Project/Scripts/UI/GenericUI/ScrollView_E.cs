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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ScrollView_E
    {
        public Func<Visual_E> CreateSeparator { get; set; } = null;
        public ScrollView Scroll_View { get; private set; } = null;

        protected Scroller m_verticalScroller { get; set; } = null;
        protected Scroller m_horizontalScroller { get; set; } = null;
        protected Slider m_verticalSlider { get; set; } = null;
        protected Slider m_horizontalSlider { get; set; } = null;
        protected VisualElement m_verticalDragger { get; set; } = null;
        protected VisualElement m_horizontalDragger { get; set; } = null;

        protected RepeatButton m_backwardVerticalButton { get; set; } = null;
        protected RepeatButton m_backwardHorizontalButton { get; set; } = null;
        protected RepeatButton m_forwardVerticalButton { get; set; } = null;
        protected RepeatButton m_forwardHorizontalButton { get; set; } = null;

        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;

        protected List<Visual_E> m_elements { get; set; } = null;
        protected List<Visual_E> m_separatorsDisplayed { get; set; } = null;
        protected List<Visual_E> m_separatorsWaited { get; set; } = null;
    }

    public partial class ScrollView_E
    {
        public ScrollView_E(VisualElement visual) :
            this(visual, null, null)
        { }
        public ScrollView_E(VisualElement visual, string styleResourcePath, StyleKeys keys) : 
            base(visual, styleResourcePath, keys) 
        { }
        public ScrollView_E(string visualResourcePath) :
            this(visualResourcePath, null, null)
        { }
        public ScrollView_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys) 
        { }
        public ScrollView_E(VisualElement parent, string visualResourcePath, string styleResourcePath, StyleKeys keys) : 
            base(parent, visualResourcePath, styleResourcePath, keys) 
        { }

        public virtual void Adds(params Visual_E[] items)
        {
            foreach (Visual_E item in items)
            {
                m_elements.Add(item);
                item.InsertRootTo(Scroll_View);
            }
            UpdateSeparator();
        }

        public virtual void InsertAt(Visual_E item, int index)
        {
            Debug.Log("<color=green>TODO: </color>" + $"insert at");
        }

        public virtual void Remove(Visual_E item)
        {
            if (!m_elements.Contains(item))
                return;

            item.Remove();
            m_elements.Remove(item);
            UpdateSeparator();
        }

        protected virtual void UpdateSeparator()
        {
            if (CreateSeparator == null)
                return;

            m_separatorsDisplayed.ForEach((separator) => separator.Remove());
            m_separatorsWaited.AddRange(m_separatorsDisplayed);
            m_separatorsDisplayed.Clear();

            Action<Visual_E> addSeparators = (elt) =>
            {
                int eltIndex = Scroll_View.IndexOf(elt.Root);
                if (eltIndex == 0) return;
                ObjectPooling(out Visual_E separator, m_separatorsDisplayed, m_separatorsWaited, CreateSeparator);
                separator.InsertRootAtTo(eltIndex, Scroll_View);
            };

            m_elements.ForEach(addSeparators);
        }

        #region Styles

        #region Dragger Container

        public virtual void SetVerticalDraggerContainerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
            => SetDraggerContainerStyle(m_verticalScroller, customStyleKey, formatAndStyleKeys);
        public virtual void SetHorizontalDraggerContainerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
            => SetDraggerContainerStyle(m_horizontalScroller, customStyleKey, formatAndStyleKeys);
        protected virtual void SetDraggerContainerStyle(Scroller scroller, string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            new Visual_E(scroller, customStyleKey, formatAndStyleKeys);
            scroller.style.opacity = 1f;
            scroller.style.alignItems = Align.Center;
        }

        #endregion

        #region Dragger

        public virtual void SetVerticalDraggerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
            => SetDraggerStyle(m_verticalDragger, customStyleKey, formatAndStyleKeys);
        public virtual void SetHorizontalDraggerStyle(string customStyleKey, StyleKeys formatAndStyleKeys)
            => SetDraggerStyle(m_horizontalDragger, customStyleKey, formatAndStyleKeys);
        public virtual void SetDraggerStyle(VisualElement dragger, string customStyleKey, StyleKeys formatAndStyleKeys)
        {
            dragger.ClearClassList();
            new Visual_E(dragger, customStyleKey, formatAndStyleKeys);
        }

        #endregion

        #region Button

        public virtual void SetVerticalBackwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_backwardVerticalButton, 
                buttonContainer, 
                () => m_backwardVerticalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public virtual void SetVerticalForwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_forwardVerticalButton, 
                buttonContainer, 
                () => m_forwardVerticalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public virtual void SetHorizontalBackwardButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_backwardHorizontalButton, 
                buttonContainer, 
                () => m_backwardHorizontalButtonLayout = buttonLayout, 
                styleResourceKey, 
                styleKeys);
        public virtual void SetHorizontalForwarddButtonStyle(VisualElement buttonContainer, VisualElement buttonLayout, string styleResourceKey, StyleKeys styleKeys)
            => SetButton(m_forwardHorizontalButton, 
                buttonContainer, 
                () => m_forwardHorizontalButtonLayout = buttonLayout,
                styleResourceKey, 
                styleKeys);

        /// <summary>
        /// Sets button style.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="buttonContainer">The new container of this button.</param>
        /// <param name="bindButtonLayout">Bind the layout of the button containing the button container.</param>
        /// <param name="styleResourceKey"></param>
        /// <param name="styleKeys"></param>
        protected virtual void SetButton(VisualElement button, VisualElement buttonContainer, Action bindButtonLayout, string styleResourceKey, StyleKeys styleKeys)
        {
            if (buttonContainer == null) throw new NullReferenceException("Button container null");
            buttonContainer.Add(button);
            AddVisualStyle(button, styleResourceKey, styleKeys);
            bindButtonLayout();
        }

        #endregion

        #endregion

        #region Events

        protected void OnGeometryChanged(GeometryChangedEvent e)
        {
            VerticalSliderValueChanged(m_verticalSlider.value);
            HorizontalSliderValueChanged(m_horizontalSlider.value);
        }

        protected void SliderValueChanged(float value, Slider slider, VisualElement backwardButton, VisualElement forwardButton)
        {
            if (backwardButton != null) backwardButton.visible = (value > slider.lowValue) ? true : false;
            if (forwardButton != null) forwardButton.visible = (value < slider.highValue) ? true : false;
        }
        protected void VerticalSliderValueChanged(float value)
            => SliderValueChanged(value, m_verticalSlider, m_backwardVerticalButtonLayout, m_forwardVerticalButtonLayout);
        protected void HorizontalSliderValueChanged(float value)
            => SliderValueChanged(value, m_horizontalSlider, m_backwardHorizontalButtonLayout, m_forwardHorizontalButtonLayout);

        #endregion
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

            m_elements = new List<Visual_E>();
            m_separatorsDisplayed = new List<Visual_E>();
            m_separatorsWaited = new List<Visual_E>();
        }

        public override void Reset()
        {
            base.Reset();
            throw new System.NotImplementedException();
        }
    }
}

