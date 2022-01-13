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
using System.Collections;
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
            else Initialized = true;
            this.Root = visualTA.CloneTree();
            this.Add(Root);
            this.CustomStyleKey = customStyleKey;
            this.CustomStyleBackgroundKey = customStyleBackgroundKey;
            GetCustomStyle();
            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            Initialize();
        }
        public void Init(VisualElement root, string customStyleKey, string customStyleBackgroundKey = "")
        {
            if (Initialized) Reset();
            else Initialized = true;
            this.Root = root;
            this.CustomStyleKey = customStyleKey;
            this.CustomStyleBackgroundKey = customStyleBackgroundKey;
            GetCustomStyle();
            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            Initialize();
        }
        public virtual void Reset()
        {
            this.CustomStyleKey = null;
            this.CustomStyleBackgroundKey = null;
            Root.UnregisterCallback<MouseOverEvent>(OnMouseOver);
            Root.UnregisterCallback<MouseOutEvent>(OnMouseOut);
            Root.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            Root.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            this.Root = null;
            m_customStyle = null;
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

        public virtual void GetCustomStyle() 
        {
            if (string.IsNullOrEmpty(CustomStyleKey))
                return; //throw new Exception("CustomStyleKey null or Empty");
            m_customStyle = UserPreferences.UserPreferences.GetCustomStyle(CustomStyleKey);
        }

        public abstract void OnApplyUserPreferences();
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        public AbstractGenericAndCustomElement() : base() 
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
            UserPreferences.UserPreferences.AddThemeUpdateListener(GetCustomStyle);
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
        ~AbstractGenericAndCustomElement()
        {
            Reset();
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
            UserPreferences.UserPreferences.RemoveThemeUpdateListener(GetCustomStyle);
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected CustomStyleToUIElementApplicator m_customStyleToUIElement = new CustomStyleToUIElementApplicator();

        protected virtual void Initialize() 
        {
            Root.RegisterCallback<MouseOverEvent>(OnMouseOver);
            Root.RegisterCallback<MouseOutEvent>(OnMouseOut);
            Root.RegisterCallback<MouseDownEvent>(OnMouseDown);
            Root.RegisterCallback<MouseUpEvent>(OnMouseUp);

            if (m_customStyle != null)
            {
                ApplyCustomSize();
                ApplyCustomBackground(CustomStyleBackgroundMode.MouseOut);
                ApplyCustomBorder();
            }
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
        protected UserPreferences.GlobalPreferences_SO m_globalPref;
        protected CustomStyle_SO m_customStyle;

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

        protected void ApplyCustomBackground(CustomStyleBackgroundMode backgroundMode)
        {
            if (m_customStyle == null) return;
            UIBackground uIBackground = m_customStyle.UIBackground;
            BackgroundsByTheme customBackgrounds = uIBackground.GetCustomBackgrounds(CustomStyleBackgroundKey, m_globalPref.CustomStyleTheme);
            switch (backgroundMode)
            {
                case CustomStyleBackgroundMode.MouseOut:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundDefault);
                    break;
                case CustomStyleBackgroundMode.MouseOver:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundMouseOver);
                    break;
                case CustomStyleBackgroundMode.MousePressed:
                    m_customStyleToUIElement.ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundMousePressed);
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


        protected virtual void OnMouseOver(MouseOverEvent e) 
        {
            ApplyCustomBackground(CustomStyleBackgroundMode.MouseOver);
        }
        protected virtual void OnMouseOut(MouseOutEvent e) 
        {
            ApplyCustomBackground(CustomStyleBackgroundMode.MouseOut);
        }
        protected virtual void OnMouseDown(MouseDownEvent e) 
        {
            ApplyCustomBackground(CustomStyleBackgroundMode.MousePressed);
        }
        protected virtual void OnMouseUp(MouseUpEvent e) 
        {
            ApplyCustomBackground(CustomStyleBackgroundMode.MouseOver);
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