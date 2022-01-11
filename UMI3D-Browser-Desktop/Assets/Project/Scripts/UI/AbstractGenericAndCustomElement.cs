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
        protected virtual void Initialize() 
        {
            Root.RegisterCallback<MouseOverEvent>(OnMouseOver);
            Root.RegisterCallback<MouseOutEvent>(OnMouseOut);
            Root.RegisterCallback<MouseDownEvent>(OnMouseDown);
            Root.RegisterCallback<MouseUpEvent>(OnMouseUp);
            //Root.RegisterCallback<MouseEnterEvent>(e => Debug.Log($"Mouse enter"));

            if (m_customStyle != null)
            {
                ApplyCustomSize();
                ApplyCustomBackground(CustomStyleBackgroundMode.MouseOut);
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
            
            length = GetPxAndPourcentageFloatLength(uiSize.Height);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.height = length;
            length = GetPxAndPourcentageFloatLength(uiSize.Width);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.width = length;
            length = GetPxAndPourcentageFloatLength(uiSize.MinHeight);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.minHeight = length;
            length = GetPxAndPourcentageFloatLength(uiSize.MinWidth);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.minWidth = length;
            length = GetPxAndPourcentageFloatLength(uiSize.MaxHeight);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.maxHeight = length;
            length = GetPxAndPourcentageFloatLength(uiSize.MaxWidth);
            Debug.Log($"StyleLength = [{length.keyword}, {length.value}]");
            if (length.keyword != StyleKeyword.Null)
                Root.style.maxWidth = length;

            //Root.style.ra
        }

        protected void ApplyCustomBackground(CustomStyleBackgroundMode backgroundMode)
        {
            if (m_customStyle == null) return;
            UIBackground uIBackground = m_customStyle.UIBackground;
            CustomBackgrounds customBackgrounds = uIBackground.GetCustomBackgrounds(CustomStyleBackgroundKey);
            switch (backgroundMode)
            {
                case CustomStyleBackgroundMode.MouseOut:
                    ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundDefault);
                    break;
                case CustomStyleBackgroundMode.MouseOver:
                    ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundMouseOver);
                    break;
                case CustomStyleBackgroundMode.MousePressed:
                    ApplyBackgroundToVisual(Root.style, customBackgrounds.BackgroundMousePressed);
                    break;
            }

            //Root.style.borderBottomLeftRadius = 10f;
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

        protected virtual StyleLength GetPxAndPourcentageFloatLength(CustomStylePXAndPercentFloat customStyle)
        {
            StyleLength lenght = new StyleLength();
            float floatLenght = -1;
            switch (customStyle.Keyword)
            {
                case CustomStyleKeyword.VariableUndefined:
                case CustomStyleKeyword.ConstUndefined:
                    lenght.keyword = StyleKeyword.Null;
                    break;
                case CustomStyleKeyword.Variable:
                    floatLenght = customStyle.Value * m_globalPref.ZoomCoef;
                    lenght = floatLenght;
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
                case CustomStyleKeyword.Const:
                    floatLenght = customStyle.Value;
                    lenght = (customStyle.ValueMode == CustomStyleValueMode.Px) ? floatLenght : Length.Percent(floatLenght);
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
            }
            return lenght;
        }

        protected virtual void ApplyBackgroundToVisual(IStyle style, CustomStyleBackground customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Variable:
                    ApplyBackgroundColorToVisual(style, customStyle.Value.BackgroundColor);
                    ApplyImageToVisual(style, customStyle.Value.BackgroundImage);
                    ApplyImageTintColorToVisual(style, customStyle.Value.BackgroundImageTintColor);
                    break;
            }
        }

        protected virtual void ApplyBackgroundColorToVisual(IStyle style, CustomStyleColor customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Variable:
                    style.backgroundColor = customStyle.Value;
                    break;
            }
        }

        protected virtual void ApplyImageTintColorToVisual(IStyle style, CustomStyleColor customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Variable:
                    style.unityBackgroundImageTintColor = customStyle.Value;
                    break;
            }
        }

        protected virtual void ApplyImageToVisual(IStyle style, CustomStyleImage customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Variable:
                    style.backgroundImage = customStyle.Value.texture;
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