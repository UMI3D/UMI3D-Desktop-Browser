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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ScrollView_E
    {
        #region Fields

        public event Action<float, float, float> HSliderValueChanged;
        public event Action<float, float, float> VSliderValueChanged;

        public Func<View_E> CreateSeparator { get; set; } = null;
        public ScrollView Scroll_View { get; protected set; } = null;
        public RepeatButton VBackwardButton { get; protected set; } = null;
        public RepeatButton HBackwardButton { get; protected set; } = null;
        public RepeatButton VForwardButton { get; protected set; } = null;
        public RepeatButton HForwardButton { get; protected set; } = null;

        protected Scroller m_verticalScroller { get; set; } = null;
        protected Scroller m_horizontalScroller { get; set; } = null;
        protected Slider m_verticalSlider { get; set; } = null;
        protected Slider m_horizontalSlider { get; set; } = null;
        protected VisualElement m_verticalDragger { get; set; } = null;
        protected VisualElement m_horizontalDragger { get; set; } = null;

        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;

        protected List<View_E> m_separatorsDisplayed { get; set; } = null;
        protected List<View_E> m_separatorsWaited { get; set; } = null;

        #endregion

        public virtual void AddRange(params View_E[] items)
        {
            foreach (View_E item in items)
                Add(item);
        }

        /// <summary>
        /// Clear the scrollview.
        /// </summary>
        public virtual void Clear()
        {
            m_views.ForEach((elt) => elt.RemoveRootFromHierarchy());
            m_views.Clear();
            UpdateSeparator();
        }

        protected virtual void UpdateSeparator()
        {
            if (CreateSeparator == null)
                return;

            m_separatorsDisplayed.ForEach((separator) => separator.RemoveRootFromHierarchy());
            m_separatorsWaited.AddRange(m_separatorsDisplayed);
            m_separatorsDisplayed.Clear();
            
            m_views.ForEach(delegate (View_E elt)
            {
                int eltIndex = Scroll_View.IndexOf(elt.Root);
                if (eltIndex == 0) return;
                ObjectPooling(out View_E separator, m_separatorsDisplayed, m_separatorsWaited, CreateSeparator);
                separator.InsertRootAtTo(eltIndex, Scroll_View);
            });
        }

        #region Styles

        #region Dragger Container

        public void SetVDraggerContainer(string partialStylePath, StyleKeys keys)
            => SetDraggerContainerStyle(m_verticalScroller, partialStylePath, keys);
        public void SetHDraggerContainer(string partialStylePath, StyleKeys keys)
            => SetDraggerContainerStyle(m_horizontalScroller, partialStylePath, keys);
        protected virtual void SetDraggerContainerStyle(Scroller scroller, string partialStylePath, StyleKeys keys)
        {
            AddVisualStyle(scroller, partialStylePath, keys);
            scroller.style.opacity = 1f;
            scroller.style.alignItems = Align.Center;
        }

        #endregion

        #region Dragger

        public void SetVDragger(string partialStylePath, StyleKeys keys)
            => SetDraggerStyle(m_verticalDragger, partialStylePath, keys);
        public void SetHDragger(string partialStylePath, StyleKeys keys)
            => SetDraggerStyle(m_horizontalDragger, partialStylePath, keys);
        public virtual void SetDraggerStyle(VisualElement dragger, string partialStylePath, StyleKeys keys)
        {
            dragger.ClearClassList();
            AddVisualStyle(dragger, partialStylePath, keys);
        }

        #endregion

        #region Button

        public void SetVBackwardButtonStyle(string partialStylePath, StyleKeys keys)
            => SetButton(VBackwardButton, partialStylePath, keys);
        public void SetVForwardButtonStyle(string partialStylePath, StyleKeys keys)
            => SetButton(VForwardButton, partialStylePath, keys);
        public void SetHBackwardButton(string partialStylePath, StyleKeys keys)
            => SetButton(HBackwardButton, partialStylePath, keys);
        public void SetHForwarddButton(string partialStylePath, StyleKeys keys)
            => SetButton(HForwardButton, partialStylePath, keys);

        /// <summary>
        /// Sets button style.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="buttonContainer">The new container of this button.</param>
        /// <param name="bindButtonLayout">Bind the layout of the button containing the button container.</param>
        /// <param name="styleResourceKey"></param>
        /// <param name="styleKeys"></param>
        protected virtual void SetButton(VisualElement button, string partialStylePath, StyleKeys keys)
            => AddVisualStyle(button, partialStylePath, keys);

        #endregion

        #endregion

        #region Events

        protected void OnGeometryChanged(GeometryChangedEvent e)
        {
            OnVSliderValueChanged(m_verticalSlider.value);
            OnHSliderValueChanged(m_horizontalSlider.value);
        }

        protected void OnVSliderValueChanged(float value)
            => VSliderValueChanged?.Invoke(value, m_verticalSlider.lowValue, m_verticalSlider.highValue);
        protected void OnHSliderValueChanged(float value)
            => HSliderValueChanged?.Invoke(value, m_horizontalSlider.lowValue, m_horizontalSlider.highValue);

        #endregion
    }

    public partial class ScrollView_E : View_E
    {
        #region Constructors

        public ScrollView_E() :
            this(new ScrollView())
        { }
        public ScrollView_E(ScrollView scrollView) :
            this(scrollView, null, null)
        { }
        public ScrollView_E(ScrollView scrollView, string partialStylePath, StyleKeys keys) :
            base(scrollView, partialStylePath, keys)
        { }
        public ScrollView_E(string visualResourcePath) :
            this(visualResourcePath, null, null)
        { }
        public ScrollView_E(string visualResourcePath, string partialStylePath, StyleKeys keys) :
            base(visualResourcePath, partialStylePath, keys)
        { }

        #endregion

        public override void Add(View_E child)
        {
            base.Add(child);
            child.InsertRootTo(Scroll_View);
            UpdateSeparator();
        }
        public override void Insert(int index, View_E item)
        {
            base.Insert(index, item);
            m_views.Insert(index, item);
            item.InsertRootAtTo(index, Scroll_View);
            UpdateSeparator();
        }
        public override void Remove(View_E item)
        {
            if (!m_views.Contains(item))
                return;
            base.Remove(item);
            item.RemoveRootFromHierarchy();
            UpdateSeparator();
        }

        public override void Reset()
        {
            base.Reset();
            Clear();
            m_separatorsWaited.Clear();
            VSliderValueChanged = null;
            HSliderValueChanged = null;
        }

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

            VBackwardButton = m_verticalScroller.lowButton;
            VForwardButton = m_verticalScroller.highButton;
            HBackwardButton = m_horizontalScroller.lowButton;
            HForwardButton = m_horizontalScroller.highButton;

            Scroll_View.contentContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            Scroll_View.contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            m_verticalScroller.valueChanged += OnVSliderValueChanged;
            m_horizontalScroller.valueChanged += OnHSliderValueChanged;

            m_separatorsDisplayed = new List<View_E>();
            m_separatorsWaited = new List<View_E>();
        }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Scrollviews/{resourcePath}";
            return base.GetStyleSO(path);
        }
    }
}

