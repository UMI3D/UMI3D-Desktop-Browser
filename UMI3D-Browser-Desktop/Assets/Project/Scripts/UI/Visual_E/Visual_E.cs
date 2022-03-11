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

namespace umi3dDesktopBrowser.ui.viewController
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
        public virtual void Display()
        {
            Root.style.display = DisplayStyle.Flex;
        }
        public virtual void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
        public virtual void Reset()
        {
            ResetAllVisualStyle();
            this.Root = null;
            m_globalPref.ApplyCustomStyle.RemoveListener(ApplyAllFormatAndStyle);
            m_globalPref = null;
            Initialized = false;
        }
        public virtual void InsertRootTo(VisualElement parent)
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
            Root.RemoveFromHierarchy();
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
            m_globalPref = GetGlobalPrefSO();
            m_globalPref.ApplyCustomStyle.AddListener(ApplyAllFormatAndStyle);
            m_styleApplicator = new UIElementStyleApplicator(m_globalPref);
            m_visuals = new List<VisualElement>();
            m_visualStyles = new Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, VisualManipulator)>();
            this.Root = visual;
            AddVisualStyle(Root, style_SO, formatAndStyleKeys);
            if (parent != null) InsertRootTo(parent);
            Initialize();
        }

        protected virtual void Initialize() 
        {
            
            
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

        protected GlobalPreferences_SO GetGlobalPrefSO()
        {
            GlobalPreferences_SO globalPreferences = Resources.Load<GlobalPreferences_SO>("Preferences/GlobalPreferences");
            if (globalPreferences == null) throw new NullReferenceException("Global pref null");
            return globalPreferences;
        }
    }
}