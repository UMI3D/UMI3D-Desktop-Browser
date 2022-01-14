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
    public enum ScrollDirection { VERTICAL, HORIZONTAL, BOTH }
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
        public ScrollView ScrollView { get; set; } = null;
        public ScrollDirection scrollDirection { get; set; } = ScrollDirection.VERTICAL;
        public float DeltaScroll { get; set; } = 50f;
        public int ScrollAnimationTime { get; set; } = 500;
        public bool DisplayedScrollButtons { get; set; } = false;
        public ScrollBarDirection DisplayedScrollBar { get; set; } = ScrollBarDirection.NONE;
        public UnityEvent VerticalBackwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent VerticalForwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent HorizontalBackwardButtonPressed { get; protected set; } = new UnityEvent();
        public UnityEvent HorizontalForwardButtonPressed { get; protected set; } = new UnityEvent();

        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;

        public IScrollable InitFromPropertiesToScrollView()
        {
            throw new System.NotImplementedException();
        }
        public IScrollable InitFromSrollViewToProperties()
        {
            //throw new System.NotImplementedException();
            return this;
        }
    }

    public partial class AbstractScrollView_E
    {
        public AbstractScrollView_E() : base() { }
        public AbstractScrollView_E(VisualElement root, string customStyleKey) : base(root, customStyleKey)
        {
            //ScrollView = Root.Q<ScrollView>();
        }
        public AbstractScrollView_E(VisualElement parent, string resourcePath, string customStyleKey, string customStyleBackgroundKey = "") : base(parent, resourcePath, customStyleKey, customStyleBackgroundKey) { }

        //protected abstract void DisplaysButtonImpl(ScrollDirection scrollDirection, ScrollButtonDirection buttonDirection, bool value);
    }

    public partial class AbstractScrollView_E : AbstractGenericAndCustomElement
    {
        public override void OnApplyUserPreferences()
        {
            //throw new System.NotImplementedException();
        }
    }
}

