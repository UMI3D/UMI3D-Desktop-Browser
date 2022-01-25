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
using DesktopBrowser.UI.CustomElement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace BrowserDesktop.UI
{
    public abstract partial class AbstractGenericAndCustomElement : ICustomElement
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

        public void Init(VisualTreeAsset visualTA, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            Initialized = true;
            this.Root = visualTA.CloneTree();
            this.Add(Root);
            this.m_rootStyleSOKey = customStyleKey;
            //this.m_rootBackgroundStyleKey = customStyleBackgroundKey;
            Initialize();
        }
        public void Init(VisualElement root, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            Initialized = true;
            this.Root = root;
            this.m_rootStyleSOKey = customStyleKey;
            //this.m_rootBackgroundStyleKey = customStyleBackgroundKey;
            Initialize();
        }
        public void Init(VisualElement parent, string resourcePath, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            Initialized = true;
            this.Root = GetVisualRoot(resourcePath);
            AddTo(parent);
            this.m_rootStyleSOKey = customStyleKey;
            //this.m_rootBackgroundStyleKey = customStyleBackgroundKey;
            Initialize();
        }
        public virtual void Reset()
        {
            this.m_rootStyleSOKey = null;
            //this.m_rootBackgroundStyleKey = null;
            Root.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            Root.UnregisterCallback<MouseOutEvent>(OnMouseOut);
            Root.UnregisterCallback<MouseCaptureEvent>(OnMouseDown);
            Root.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            this.Root = null;
            //m_rootCustomStyleSO?.AppliesFormatAndStyle.RemoveListener(formatAndStyleMethodsDictionary[Root.style]);
            m_rootCustomStyleSO = null;
            m_globalPref.ApplyCustomStyle.RemoveListener(ApplyAllFormatAndStyle);
            m_globalPref = null;
            Initialized = false;
        }
        public virtual void AddTo(VisualElement parent)
        {
            if (!Initialized) 
                throw new System.Exception($"VisualElement Added without being Initialized.");
            //ReadyToDisplay();
            //parent.Add(this);
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

        public virtual bool GetCustomStyle() 
        {
            return GetCustomStyle(m_rootStyleSOKey);
        }

        public virtual void OnApplyUserPreferences() { }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        public AbstractGenericAndCustomElement() : base() 
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
            //UserPreferences.UserPreferences.AddThemeUpdateListener(ApplyCustomStyle);
        }
        public AbstractGenericAndCustomElement(VisualTreeAsset visualTA) : this()
        {
            Init(visualTA, null);
        }
        public AbstractGenericAndCustomElement(VisualElement root) : this()
        {
            Init(root, null);
        }
        public AbstractGenericAndCustomElement(VisualTreeAsset visualTA, string customStyleKey, string customStyleBackgroundKey = "") : this()
        {
            Init(visualTA, customStyleKey, customStyleBackgroundKey);
        }
        public AbstractGenericAndCustomElement(VisualElement root, string customStyleKey, string customStyleBackgroundKey = "") : this()
        {
            Init(root, customStyleKey, customStyleBackgroundKey);
        }
        public AbstractGenericAndCustomElement(VisualElement parent, string resourcePath, string customStyleKey, string customStyleBackgroundKey = "") : this()
        {
            Init(parent, resourcePath, customStyleKey, customStyleBackgroundKey);
        }
        ~AbstractGenericAndCustomElement()
        {
            Reset();
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
            //UserPreferences.UserPreferences.RemoveThemeUpdateListener(ApplyCustomStyle);
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected virtual void Initialize() 
        {
            Root.RegisterCallback<MouseOverEvent>(OnMouseOver);
            Root.RegisterCallback<MouseOutEvent>(OnMouseOut);
            Root.RegisterCallback<MouseCaptureEvent>(OnMouseDown);
            Root.RegisterCallback<MouseUpEvent>(OnMouseUp);
            //formatAndStyleMethodsDictionary.Add(Root.style, () => { ApplyCustomStyle(m_rootCustomStyleSO, m_rootTextFormatKey, m_rootTextStyleKey, m_rootBackgroundStyleKey, m_rootBorderStyleKey, Root.style, m_mouseBehaviourFromState); });

            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            m_globalPref.ApplyCustomStyle.AddListener(ApplyAllFormatAndStyle);
            if (GetCustomStyle())
            {
                //m_rootCustomStyleSO.AppliesFormatAndStyle.AddListener(formatAndStyleMethodsDictionary[Root.style]);
                m_rootCustomStyleSO.AppliesFormatAndStyle.Invoke();
            }
        }

        protected virtual VisualElement GetVisualRoot(string resourcePath)
        {
            VisualTreeAsset visualTA = Resources.Load<VisualTreeAsset>(resourcePath);
            Debug.Assert(visualTA != null, $"[{resourcePath}] return a null visual tree asset");
            Debug.Assert(visualTA.CloneTree().childCount == 1, $"[{resourcePath}] must have a single visual as root.");
            IEnumerator<VisualElement> iterator = visualTA.CloneTree().Children().GetEnumerator();
            iterator.MoveNext();
            return iterator.Current;
        }

        protected bool GetCustomStyle(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            m_rootCustomStyleSO = UserPreferences.UserPreferences.GetCustomStyle(key);
            return true;
        }

        /// <summary>
        /// To be used in Custom Element that are already added to the UIDocument.
        /// </summary>
        protected virtual void ReadyToDisplay()
        {
            Displayed = true;
            OnApplyUserPreferences();
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected enum MousePressedState
        {
            Unpressed,
            Pressed
        }
        protected enum MousePositionState
        {
            Out,
            Over
        }

        protected (MousePressedState, MousePositionState) m_mouseState { get; set; }
        protected MouseBehaviour m_mouseBehaviourFromState
        {
            get
            {
                return m_mouseState switch
                {
                    (MousePressedState.Unpressed, MousePositionState.Out) => MouseBehaviour.MouseOut,
                    (MousePressedState.Unpressed, MousePositionState.Over) => MouseBehaviour.MouseOver,
                    _ => MouseBehaviour.MousePressed
                };
            }
        }

        protected virtual void OnMouseOver(MouseOverEvent e)
        {
            m_mouseState = (m_mouseState.Item1, MousePositionState.Over);
            ApplyAllStyle();
        }
        protected virtual void OnMouseOut(MouseOutEvent e)
        {
            m_mouseState = (m_mouseState.Item1, MousePositionState.Out);
            ApplyAllStyle();
        }
        protected virtual void OnMouseDown(MouseCaptureEvent e)
        {
            Debug.Log($"Mouse button pressed");
            m_mouseState = (MousePressedState.Pressed, m_mouseState.Item2);
            ApplyAllStyle();
        }
        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (e.button != 0) return;
            m_mouseState = (MousePressedState.Unpressed, m_mouseState.Item2);
            Debug.Log($"Mouse button up (button pressed = [{e.pressedButtons}], button = [{e.button}])");
            ApplyAllStyle();
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected UIElementStyleApplicator m_uIElementStyleApplicator = new UIElementStyleApplicator();
        protected UserPreferences.GlobalPreferences_SO m_globalPref;
        protected CustomStyle_SO m_rootCustomStyleSO { get; set; } = null;
        protected string m_rootStyleSOKey { get; set; } = null;
        //protected string m_rootTextFormatKey { get; set; } = null;
        //protected string m_rootTextStyleKey { get; set; } = null;
        //protected string m_rootBackgroundStyleKey { get; set; } = null;
        //protected string m_rootBorderStyleKey { get; set; } = null;

        protected class FormatAndStyleKeys
        {
            public string TextFormatKey { get; set; }
            public string TextStyleKey { get; set; }
            public string BackgroundStyleKey { get; set; }
            public string BorderStyleKey { get; set; }
        }

        protected List<VisualElement> m_visuals;
        protected Dictionary<VisualElement, (CustomStyle_SO, FormatAndStyleKeys, UnityAction)> m_visualStyles;

        //protected Dictionary<IStyle, UnityAction> formatAndStyleMethodsDictionary;

        protected void RegisterStyleEvent()
        {
            //foreach (VisualElement visual in m_visuals)
            //{
            //    var style = m_visualStyles[visual];
            //    for
            //}
        }

        protected void AddVisualStyle(VisualElement visual, CustomStyle_SO style_SO, FormatAndStyleKeys formatAndStyleKeys)
        {
            if (m_visuals.Contains(visual)) return;
            UnityAction action = () => 
            { 
                ApplyCustomStyle(style_SO, formatAndStyleKeys, visual.style, m_mouseBehaviourFromState); 
            };
            m_visualStyles.Add(visual, (style_SO, formatAndStyleKeys, action));
            style_SO.AppliesFormatAndStyle.AddListener(action);
        }

        public virtual void ApplyAllFormatAndStyle()
        {
            ApplyAllFormat();
            ApplyAllStyle();
        }

        protected void ApplyCustomStyle(CustomStyle_SO style_SO, FormatAndStyleKeys formatAndStyleKeys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            ApplyFormat(style_SO, formatAndStyleKeys.TextFormatKey, style);
            ApplyStyle(style_SO, formatAndStyleKeys.TextStyleKey, formatAndStyleKeys.BackgroundStyleKey, formatAndStyleKeys.BorderStyleKey, style, mouseBehaviour);
        }

        #region Format of the element

        protected void ApplyAllFormat()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var style = m_visualStyles[visual];
                ApplyFormat(style.Item1, style.Item2.TextFormatKey, visual.style);
            }
        }

        protected void ApplyFormat(CustomStyle_SO style_SO, string textFormatKey, IStyle style)
        {
            ApplySize(style_SO, style);
            ApplyMarginAndPadding(style_SO, style);
            ApplyTextFormat(style_SO, textFormatKey, style);
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

        protected void ApplyTextFormat(CustomStyle_SO style_SO, string formatKey, IStyle style)
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
                ApplyStyle(style.Item1, style.Item2.TextStyleKey, style.Item2.BackgroundStyleKey, style.Item2.BorderStyleKey, visual.style, m_mouseBehaviourFromState);
            }
        }

        protected void ApplyStyle(CustomStyle_SO styleSO, string textStyleKey, string backgroundStyleKey, string borderStyleKey, IStyle style, MouseBehaviour mouseBehaviour)
        {
            ApplyTextStyle(styleSO, textStyleKey, style, mouseBehaviour);
            ApplyBackgroundStyle(styleSO, backgroundStyleKey, style, mouseBehaviour);
            ApplyBorderStyle(styleSO, borderStyleKey, style, mouseBehaviour);
        }

        protected void ApplyTextStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (style_SO == null || styleKey == null) return;
            UITextStyle textStyle = style_SO.GetTextStyle(null, styleKey);
            switch (mouseBehaviour)
            {
                case MouseBehaviour.MouseOut:
                    m_uIElementStyleApplicator.AppliesTextStyle(style, textStyle.Default);
                    break;
                case MouseBehaviour.MouseOver:
                    m_uIElementStyleApplicator.AppliesTextStyle(style, textStyle.MouseOver);
                    break;
                case MouseBehaviour.MousePressed:
                    m_uIElementStyleApplicator.AppliesTextStyle(style, textStyle.MousePressed);
                    break;
            }
        }

        protected void ApplyBackgroundStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (style_SO == null) return;
            UIBackground background = style_SO.GetBackground(null, styleKey);
            switch (mouseBehaviour)
            {
                case MouseBehaviour.MouseOut:
                    m_uIElementStyleApplicator.AppliesBackground(style, background.Default);
                    break;
                case MouseBehaviour.MouseOver:
                    m_uIElementStyleApplicator.AppliesBackground(style, background.MouseOver);
                    break;
                case MouseBehaviour.MousePressed:
                    m_uIElementStyleApplicator.AppliesBackground(style, background.MousePressed);
                    break;
            }
        }

        protected void ApplyBorderStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (style_SO == null) return;
            UIBorder border = style_SO.GetBorder(null, styleKey);
            switch (mouseBehaviour)
            {
                case MouseBehaviour.MouseOut:
                    m_uIElementStyleApplicator.AppliesBorder(style, border.Default);
                    break;
                case MouseBehaviour.MouseOver:
                    m_uIElementStyleApplicator.AppliesBorder(style, border.MouseOver);
                    break;
                case MouseBehaviour.MousePressed:
                    m_uIElementStyleApplicator.AppliesBorder(style, border.MousePressed);
                    break;
            }
        }

        #endregion
    }

    public abstract partial class AbstractGenericAndCustomElement : VisualElement
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