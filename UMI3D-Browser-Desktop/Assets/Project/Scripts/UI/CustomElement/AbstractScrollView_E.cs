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
        public ScrollView ScrollView { get; set; }
        public ScrollDirection scrollDirection { get; set; }
        public float DeltaScroll { get; set; }
        /// <summary>
        /// Scroll animation time in ms.
        /// </summary>
        public int ScrollAnimationTime { get; set; }
        public bool DisplayedScrollButtons { get; set; }
        public ScrollBarDirection DisplayedScrollBar { get; set; }
        public UnityEvent VerticalBackwardButtonPressed { get; set; }
        public UnityEvent VerticalForwardButtonPressed { get; set; }
        public UnityEvent HorizontalBackwardButtonPressed { get; set; }
        public UnityEvent HorizontalForwardButtonPressed { get; set; }

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
        public UnityEvent VerticalBackwardButtonPressed { get; set; }
        public UnityEvent VerticalForwardButtonPressed { get; set; }
        public UnityEvent HorizontalBackwardButtonPressed { get; set; }
        public UnityEvent HorizontalForwardButtonPressed { get; set; }

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
        public AbstractScrollView_E(VisualElement root) : base(root)
        {
            ScrollView = Root.Q<ScrollView>();
        }

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

