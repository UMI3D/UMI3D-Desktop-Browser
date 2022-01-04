using BrowserDesktop.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public enum ScrollDirection { VERTICAL, HORIZONTAL, BOTH }
    public enum ScrollBarDirection { VERTICAL, HORIZONTAL, BOTH, NONE }
    public enum ScrollButtonDirection { BACKWARD, FORWARD }

    public interface IScrollable
    {
        public ScrollDirection scrollDirection { get; set; }
        public float DeltaScroll { get; set; }
        /// <summary>
        /// Scroll animation time in ms.
        /// </summary>
        public int ScrollAnimationTime { get; set; }
        public bool DisplayedScrollButtons { get; set; }
        public ScrollBarDirection DisplayedScrollBar { get; set; }
    }
    public abstract partial class AbstractScrollView_E : IScrollable
    {
        public ScrollDirection scrollDirection { get; set; } = ScrollDirection.VERTICAL;
        public float DeltaScroll { get; set; } = 50f;
        public int ScrollAnimationTime { get; set; } = 500;
        public bool DisplayedScrollButtons { get; set; } = false;
        public ScrollBarDirection DisplayedScrollBar { get; set; } = ScrollBarDirection.NONE;

        protected VisualElement m_backwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_backwardHorizontalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardVerticalButtonLayout { get; set; } = null;
        protected VisualElement m_forwardHorizontalButtonLayout { get; set; } = null;
    }

    public abstract partial class AbstractScrollView_E
    {
        public AbstractScrollView_E(VisualElement root) : base(root) { }

        protected abstract void DisplaysButtonImpl(ScrollDirection scrollDirection, ScrollButtonDirection buttonDirection, bool value);
    }

    public abstract partial class AbstractScrollView_E : AbstractGenericAndCustomElement
    {
        public override void OnApplyUserPreferences()
        {
            throw new System.NotImplementedException();
        }
    }
}

