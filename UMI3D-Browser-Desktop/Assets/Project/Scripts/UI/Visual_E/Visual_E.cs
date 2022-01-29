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
using umi3DBrowser.UICustomStyle;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using BrowserDesktop.preferences;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Visual_E : ICustomElement
    {
        public VisualElement Root { get; protected set; } = null;
        public bool Initialized { get; protected set; } = false;
        public bool AttachedToHierarchy { get; protected set; } = false;
        public bool Displayed { get; protected set; } = false;
        public DisplayStyle RootDisplayStyle
        {
            get => Root.resolvedStyle.display;
            set => Root.style.display = value;
        }
        public Visibility RootVisibility
        {
            get => Root.resolvedStyle.visibility;
            set => Root.style.visibility = value;
        }
        public Rect RootLayout { get => Root.layout; }
        public virtual void Reset()
        {
            ResetAllVisualStyle();
            this.Root = null;
            m_globalPref.ApplyCustomStyle.RemoveListener(ApplyAllFormatAndStyle);
            m_globalPref = null;
            Initialized = false;
        }
        public virtual void AddTo(VisualElement parent)
        {
            if (!Initialized) 
                throw new Exception($"VisualElement Added without being Initialized.");
            if (parent == null)
                throw new Exception($"Try to Add [{Root}] to a parent null.");
            parent.Add(Root);
            AttachedToHierarchy = true;
        }
        
        public virtual void Remove()
        {
            //if (!Displayed) return;
            //else Displayed = false;
            this.RemoveFromHierarchy();
            AttachedToHierarchy = false;
        }
    }

    public partial class Visual_E
    {
        public Visual_E() : base() { }
        public Visual_E(VisualElement visual) : this()
        {
            if (visual == null) throw new NullReferenceException($"visual is null");
            Init(null, visual, null, null);
        }
        public Visual_E(VisualElement parent, VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys) : this()
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            if (visual == null) throw new NullReferenceException($"visual is null");
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(parent, visual, style_SO, formatAndStyleKeys);
        }
        public Visual_E(VisualElement visual, string styleResourcePath, StyleKeys formatAndStyleKeys) : this()
        {
            if (visual == null) throw new NullReferenceException($"visual is null");
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(null, visual, style_SO, formatAndStyleKeys);
        }
        public Visual_E(string visualResourcePath, string styleResourcePath, StyleKeys formatAndStyleKeys) : this()
        {
            VisualElement visual = GetVisualRoot(visualResourcePath);
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(null, visual, style_SO, formatAndStyleKeys);
        }
        public Visual_E(VisualElement parent, string visualResourcePath, string styleResourcePath, StyleKeys formatAndStyleKeys) : this()
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            VisualElement visual = GetVisualRoot(visualResourcePath);
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(parent, visual, style_SO, formatAndStyleKeys);
        }
        public Visual_E(VisualElement parent, VisualElement visual, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys) : this()
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            if (visual == null) throw new NullReferenceException($"visual is null");
            Init(parent, visual, style_SO, formatAndStyleKeys);
        }
        ~Visual_E()
        {
            Reset();
            //UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
            //UserPreferences.UserPreferences.RemoveThemeUpdateListener(ApplyCustomStyle);
        }
    }

    public partial class Visual_E
    {
        protected void Init(VisualElement parent, VisualElement visual, CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys)
        {
            if (Initialized) Reset();
            Initialized = true;
            m_visuals = new List<VisualElement>();
            m_visualStyles = new Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, UnityAction, EventCallback<MouseOverEvent>, EventCallback<MouseOutEvent>, EventCallback<MouseCaptureEvent>, EventCallback<MouseUpEvent>)>();
            this.Root = visual;
            AddVisualStyle(Root, style_SO, formatAndStyleKeys);
            if (parent != null) AddTo(parent);
            Initialize();
        }

        protected virtual void Initialize() 
        {
            m_globalPref = UserPreferences.GlobalPref;
            m_globalPref.ApplyCustomStyle.AddListener(ApplyAllFormatAndStyle);
        }

        protected virtual VisualElement GetVisualRoot(string resourcePath)
        {
            VisualTreeAsset visualTA = Resources.Load<VisualTreeAsset>(resourcePath);
            Debug.Assert(visualTA != null, $"[{resourcePath}] return a null visual tree asset.");
            Debug.Assert(visualTA.CloneTree().childCount == 1, $"[{resourcePath}] must have a single visual as root.");
            IEnumerator<VisualElement> iterator = visualTA.CloneTree().Children().GetEnumerator();
            iterator.MoveNext();
            return iterator.Current;
        }

        protected CustomStyle_SO GetStyleSO(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) return null;
            CustomStyle_SO style_SO = Resources.Load<CustomStyle_SO>(resourcePath);
            Debug.Assert(style_SO != null, $"[{resourcePath}] return a null CustomStyle_SO.");
            return style_SO;
        }

        /// <summary>
        /// To be used in Custom Element that are already added to the UIDocument.
        /// </summary>
        protected virtual void ReadyToDisplay()
        {
            Displayed = true;
        }
    }

    public partial class Visual_E
    {
        protected UIElementStyleApplicator m_uIElementStyleApplicator = new UIElementStyleApplicator();
        protected GlobalPreferences_SO m_globalPref;

        

        public virtual void ApplyAllFormatAndStyle()
        {
            ApplyAllFormat();
            ApplyAllStyle();
        }

        protected void ApplyFormatAndStyle(CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (style_SO == null) return;
            ApplyFormat(style_SO, formatAndStyleKeys, style);
            ApplyStyle(style_SO, formatAndStyleKeys, style, mouseBehaviour);
        }

        #region Format of the element

        protected void ApplyAllFormat()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var style = m_visualStyles[visual];
                ApplyFormat(style.Item1, style.Item2, visual.style);
            }
        }

        protected void ApplyFormat(CustomStyle_SO style_SO, StyleKeys formatAndStyleKeys, IStyle style)
        {
            ApplySize(style_SO, style);
            ApplyMarginAndPadding(style_SO, style);
            ApplyTextFormat(style_SO, formatAndStyleKeys.Text, style);
        }

        protected void ApplySize(CustomStyle_SO style_SO, IStyle style)
        {
            if (style_SO == null) return;
            UISize uiSize = style_SO.UISize;
            StyleLength length = new StyleLength();
            
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.Height, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.height = length;
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.Width, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.width = length;
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.MinHeight, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.minHeight = length;
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.MinWidth, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.minWidth = length;
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.MaxHeight, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.maxHeight = length;
            length = m_uIElementStyleApplicator.GetPxAndPourcentageFloatLength(uiSize.MaxWidth, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                style.maxWidth = length;
        }

        protected void ApplyMarginAndPadding(CustomStyle_SO style_SO, IStyle style)
        {
            throw new NotImplementedException();
        }

        protected void ApplyTextFormat(CustomStyle_SO style_SO, string text, IStyle style)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Theme style of the element

        protected void ApplyAllStyle()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var style = m_visualStyles[visual];
                ApplyStyle(style.Item1, style.Item2, visual.style, m_mouseBehaviourFromState);
            }
        }

        protected void ApplyStyle(CustomStyle_SO styleSO, StyleKeys formatAndStyleKeys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (styleSO == null || formatAndStyleKeys == null) return;
            if (formatAndStyleKeys.TextStyleKey != null) ApplyTextStyle(styleSO, formatAndStyleKeys.TextStyleKey, style, mouseBehaviour);
            if (formatAndStyleKeys.BackgroundStyleKey != null) ApplyBackgroundStyle(styleSO, formatAndStyleKeys.BackgroundStyleKey, style, mouseBehaviour);
            if (formatAndStyleKeys.BorderStyleKey != null) ApplyBorderStyle(styleSO, formatAndStyleKeys.BorderStyleKey, style, mouseBehaviour);
        }

        protected void ApplyTextStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style_SO, style, mouseBehaviour, () => style_SO.GetTextStyle(null, styleKey), m_uIElementStyleApplicator.AppliesTextStyle);

        protected void ApplyBackgroundStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style_SO, style, mouseBehaviour, () => style_SO.GetBackground(null, styleKey), m_uIElementStyleApplicator.AppliesBackground);

        protected void ApplyBorderStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style_SO, style, mouseBehaviour, () => style_SO.GetBorder(null, styleKey), m_uIElementStyleApplicator.AppliesBorder);

        protected void ApplyAtomStyle<T>(CustomStyle_SO style_SO, IStyle style, MouseBehaviour mouseBehaviour, Func<ICustomisableByMouseBehaviour<T>> getUIStyle, Action<IStyle, T> styleApplicator)
        {
            if (style_SO == null) return;
            ICustomisableByMouseBehaviour<T> uiStyle = getUIStyle();
            switch (mouseBehaviour)
            {
                case MouseBehaviour.MouseOut:
                    styleApplicator(style, uiStyle.Default);
                    break;
                case MouseBehaviour.MouseOver:
                    styleApplicator(style, uiStyle.MouseOver);
                    break;
                case MouseBehaviour.MousePressed:
                    styleApplicator(style, uiStyle.MousePressed);
                    break;
            }
        }

        #endregion
    }

    public partial class Visual_E : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        /// <summary>
        /// Get Root.layout
        /// </summary>
        public new Rect layout { get => RootLayout; }
    }
}