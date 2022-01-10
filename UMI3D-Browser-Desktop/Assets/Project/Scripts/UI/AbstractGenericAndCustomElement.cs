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

        public void Init(VisualTreeAsset visualTA, string customStyleKey)
        {
            if (Initialized) Reset();
            else Initialized = true;
            Root = visualTA.CloneTree();
            this.Add(Root);
            CustomStyleKey = customStyleKey;
            GetCustomStyle();
            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            Initialize();
        }
        public void Init(VisualElement root, string customStyleKey)
        {
            if (Initialized) Reset();
            else Initialized = true;
            this.Root = root;
            CustomStyleKey = customStyleKey;
            GetCustomStyle();
            m_globalPref = UserPreferences.UserPreferences.GlobalPref;
            Initialize();
        }
        public virtual void Reset()
        {
            this.Root = null;
            this.CustomStyleKey = null;
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
                throw new Exception("CustomStyleKey null or Empty");
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
        public AbstractGenericAndCustomElement(VisualTreeAsset visualTA, string customStyleKey) : this()
        {
            Init(visualTA, customStyleKey);
        }
        public AbstractGenericAndCustomElement(VisualElement root, string customStyleKey) : this()
        {
            Init(root, customStyleKey);
        }
        ~AbstractGenericAndCustomElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
            UserPreferences.UserPreferences.RemoveThemeUpdateListener(GetCustomStyle);
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        protected virtual void Initialize() { }

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
            UISize uiSize = m_customStyle.UISize;
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.Height);
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.Width);
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.MinHeight);
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.MinWidth);
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.MaxHeight);
            ApplyCustomStylePxAndPourcentageFloat(Root.style, uiSize.MaxWidth);
        }

        protected virtual void OnMouseOver() { }
        protected virtual void OnMouseOut() { }
        protected virtual void OnMouseDown() { }
        protected virtual void OnMouseUp() { }

        private void ApplyCustomStylePxAndPourcentageFloat(IStyle style, CustomStylePXAndPercentFloat customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleKeyword.VariableUndefined:
                    break;
                case CustomStyleKeyword.ConstUndefined:
                    break;
                case CustomStyleKeyword.Variable:
                    style.height = customStyle.Value * m_globalPref.ZoomCoef;
                    break;
                case CustomStyleKeyword.Const:
                    style.height = customStyle.Value;
                    break;
            }
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