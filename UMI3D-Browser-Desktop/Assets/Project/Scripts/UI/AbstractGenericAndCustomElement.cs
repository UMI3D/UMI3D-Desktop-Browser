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
using Browser.UICustomStyle;
using DesktopBrowser.UI.CustomElement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI
{
    public abstract partial class AbstractGenericAndCustomElement : ICustomElement
    {
        public string CustomStyleKey { get; protected set; } = null;
        public string CustomStyleBackgroundKey { get; protected set; }
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
            this.CustomStyleKey = customStyleKey;
            this.CustomStyleBackgroundKey = customStyleBackgroundKey;
            Initialize();
        }
        public void Init(VisualElement root, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            Initialized = true;
            this.Root = root;
            this.CustomStyleKey = customStyleKey;
            this.CustomStyleBackgroundKey = customStyleBackgroundKey;
            Initialize();
        }
        public void Init(VisualElement parent, string resourcePath, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            Initialized = true;
            this.Root = GetVisualRoot(resourcePath);
            AddTo(parent);
            this.CustomStyleKey = customStyleKey;
            this.CustomStyleBackgroundKey = customStyleBackgroundKey;
            Initialize();
        }
        public virtual void Reset()
        {
            this.CustomStyleKey = null;
            this.CustomStyleBackgroundKey = null;
            Root.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            Root.UnregisterCallback<MouseOutEvent>(OnMouseOut);
            Root.UnregisterCallback<MouseCaptureEvent>(OnMouseDown);
            Root.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            this.Root = null;
            m_customStyle?.ApplyCustomStyle.RemoveListener(ApplyCustomStyle);
            m_customStyle = null;
            m_globalPref.ApplyCustomStyle.RemoveListener(ApplyCustomStyle);
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
            return GetCustomStyle(CustomStyleKey);
        }

        public abstract void OnApplyUserPreferences();
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        public AbstractGenericAndCustomElement() : base() 
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
            UserPreferences.UserPreferences.AddThemeUpdateListener(ApplyCustomStyle);
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
            UserPreferences.UserPreferences.RemoveThemeUpdateListener(ApplyCustomStyle);
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

            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            m_globalPref.ApplyCustomStyle.AddListener(ApplyCustomStyle);
            if (GetCustomStyle())
            {
                m_customStyle.ApplyCustomStyle.AddListener(ApplyCustomStyle);
                m_customStyle.ApplyCustomStyle.Invoke();
                ApplyCustomStyle();
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
            m_customStyle = UserPreferences.UserPreferences.GetCustomStyle(key);
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

        protected (MousePressedState, MousePositionState) m_mouseState;

        protected virtual void OnMouseOver(MouseOverEvent e)
        {
            m_mouseState = (m_mouseState.Item1, MousePositionState.Over);
            ApplyCustomBackground();
        }
        protected virtual void OnMouseOut(MouseOutEvent e)
        {
            m_mouseState = (m_mouseState.Item1, MousePositionState.Out);
            ApplyCustomBackground();
        }
        protected virtual void OnMouseDown(MouseCaptureEvent e)
        {
            Debug.Log($"Mouse button pressed");
            m_mouseState = (MousePressedState.Pressed, m_mouseState.Item2);
            ApplyCustomBackground();
        }
        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (e.button != 0) return;
            m_mouseState = (MousePressedState.Unpressed, m_mouseState.Item2);
            Debug.Log($"Mouse button up (button pressed = [{e.pressedButtons}], button = [{e.button}])");
            ApplyCustomBackground();
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected CustomStyleToUIElementApplicator m_customStyleToUIElement = new CustomStyleToUIElementApplicator();
        protected UserPreferences.GlobalPreferences_SO m_globalPref;
        protected CustomStyle_SO m_customStyle;

        protected void ApplyCustomStyle()
        {
            if (m_customStyle != null)
            {
                ApplyCustomSize();
                ApplyCustomBackground(Root.style, CustomStyleBackgroundMode.MouseOut);
                ApplyCustomBorder();
            }
        }

        protected void ApplyCustomSize()
        {
            if (m_customStyle == null) return;

            UISize uiSize = m_customStyle.UISize;
            StyleLength length = new StyleLength();
            
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.Height, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.height = length;
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.Width, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.width = length;
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.MinHeight, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.minHeight = length;
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.MinWidth, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.minWidth = length;
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.MaxHeight, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.maxHeight = length;
            length = m_customStyleToUIElement.GetPxAndPourcentageFloatLength(uiSize.MaxWidth, m_globalPref.ZoomCoef);
            if (length.keyword != StyleKeyword.Null)
                Root.style.maxWidth = length;
        }

        protected void ApplyCustomBackground()
        {
            //switch (m_mouseState)
            //{
            //    case (MousePressedState.Unpressed, MousePositionState.Out):
            //}
            var result = m_mouseState switch
            {
                (MousePressedState.Unpressed, MousePositionState.Out) => CustomStyleBackgroundMode.MouseOut,
                (MousePressedState.Unpressed, MousePositionState.Over) => CustomStyleBackgroundMode.MouseOver,
                _ => CustomStyleBackgroundMode.MousePressed
            };
            ApplyCustomBackground(Root.style, result);
        }

        protected void ApplyCustomBackground(IStyle style, CustomStyleBackgroundMode backgroundMode)
        {
            if (m_customStyle == null) return;
            UIBackground uIBackground = m_customStyle.UIBackground;
            BackgroundsByTheme customBackgrounds = uIBackground.GetCustomBackgrounds(CustomStyleBackgroundKey, m_globalPref.CustomStyleTheme);
            switch (backgroundMode)
            {
                case CustomStyleBackgroundMode.MouseOut:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(style, customBackgrounds.BackgroundDefault);
                    break;
                case CustomStyleBackgroundMode.MouseOver:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(style, customBackgrounds.BackgroundMouseOver);
                    break;
                case CustomStyleBackgroundMode.MousePressed:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(style, customBackgrounds.BackgroundMousePressed);
                    break;
            }
        }

        protected void ApplyCustomBorder()
        {
            if (m_customStyle == null) return;
            UIBorder uIBorder = m_customStyle.UIBorder;
            m_customStyleToUIElement.ApplyBorderColorToVisual(Root.style, uIBorder.Color);
            m_customStyleToUIElement.ApplyBorderWidthToVisual(Root.style, uIBorder.Width);
            m_customStyleToUIElement.ApplyBorderRadiusToVisual(Root.style, uIBorder.Radius);
        }
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