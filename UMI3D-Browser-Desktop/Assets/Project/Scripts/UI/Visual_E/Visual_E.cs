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
    public partial class Visual_E
    {
        /// <summary>
        /// Name of the view.
        /// </summary>
        public string Name { get; set; } = null;
        /// <summary>
        /// Visual root of this custom element.
        /// </summary>
        public VisualElement Root { get; protected set; } = null;
        /// <summary>
        /// True if this has been instantiated.
        /// </summary>
        public bool IsInstantiated { get; protected set; } = false;
        /// <summary>
        /// True if this has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; } = false;
        /// <summary>
        /// True if this view is displayed.
        /// </summary>
        public bool IsDisplaying { get; protected set; } = false;
        /// <summary>
        /// Event raised when the view is displayed (true) or hide (false);
        /// </summary>
        public event Action<bool> DisplayedOrHidden;

        /// <summary>
        /// Maps the visualElements with their styles and VisualManipulator.
        /// </summary>
        protected Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, VisualManipulator)> m_visualStylesMap;
        protected List<Visual_E> m_views;
    }

    public partial class Visual_E
    {
        #region Constructors and Destructor
        
        public Visual_E(VisualElement visual)
        {
            if (visual == null) throw new NullReferenceException($"visual is null");
            Init(null, visual, null, null);
        }
        public Visual_E(VisualElement parent, VisualElement visual, string styleResourcePath, StyleKeys keys)
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            if (visual == null) throw new NullReferenceException($"visual is null");
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(parent, visual, style_SO, keys);
        }
        public Visual_E(VisualElement visual, string styleResourcePath, StyleKeys keys)
        {
            if (visual == null) throw new NullReferenceException($"visual is null");
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(null, visual, style_SO, keys);
        }
        public Visual_E(string visualResourcePath, string styleResourcePath, StyleKeys keys)
        {
            VisualElement visual = GetVisualRoot(visualResourcePath);
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(null, visual, style_SO, keys);
        }
        public Visual_E(VisualElement parent, string visualResourcePath, string styleResourcePath, StyleKeys keys)
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            VisualElement visual = GetVisualRoot(visualResourcePath);
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            Init(parent, visual, style_SO, keys);
        }
        public Visual_E(VisualElement parent, VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys)
        {
            if (parent == null) throw new NullReferenceException($"parent is null");
            if (visual == null) throw new NullReferenceException($"visual is null");
            Init(parent, visual, style_SO, keys);
        }
        ~Visual_E()
        {
            Destroy();
        }

        #endregion

        #region Public methods

        public virtual void Display()
        {
            Root.style.display = DisplayStyle.Flex;
            IsDisplaying = true;
            DisplayedOrHidden?.Invoke(true);
        }
        public virtual void Hide()
        {
            Root.style.display = DisplayStyle.None;
            IsDisplaying = false;
            DisplayedOrHidden?.Invoke(false);
        }
        /// <summary>
        /// Reset properties set after Initialization at default (should be abled to set the properties againe).
        /// </summary>
        public virtual void Reset()
        { }
        /// <summary>
        /// Prepare for destruction.
        /// </summary>
        public virtual void Destroy()
        {
            Reset();
            ResetAllVisualStyle();
            this.Root = null;
            m_globalPref.ApplyCustomStyle.RemoveListener(ApplyAllFormatAndStyle);
            m_globalPref = null;
            IsInstantiated = false;
        }
        /// <summary>
        /// Add the Root VisualElement as a child of [partent] at the position [index].
        /// </summary>
        /// <param name="index"></param>
        /// <param name="parent"></param>
        public virtual void InsertRootAtTo(int index, VisualElement parent)
        {
            if (!IsInstantiated)
                throw new Exception($"VisualElement Added without being Initialized.");
            if (parent == null)
                throw new Exception($"Try to Add [{Root}] to a parent null.");
            parent.Insert(index, Root);
        }
        /// <summary>
        /// Add the Root VisualElement as a child of [partent].
        /// </summary>
        /// <param name="parent"></param>
        public virtual void InsertRootTo(VisualElement parent)
        {
            if (!IsInstantiated)
                throw new Exception($"VisualElement Added without being Initialized.");
            if (parent == null)
                throw new Exception($"Try to Add [{Root}] to a parent null.");
            parent.Add(Root);
        }
        /// <summary>
        /// Remove the Root VisualElement from the hierarchy.
        /// </summary>
        public virtual void RemoveRootFromHierarchy()
        {
            Root.RemoveFromHierarchy();
            IsDisplaying = false;
        }
        public virtual void Add(Visual_E child)
        {
            if (m_views.Contains(child))
                return;
            m_views.Add(child);
        }
        public virtual void Insert(int index, Visual_E child)
        {
            if (m_views.Contains(child))
                return;
            m_views.Insert(index, child);
        }
        public virtual void Remove(Visual_E view)
            => m_views.Remove(view);
        public virtual bool ContainsInHierarchy(Visual_E view)
        {
            bool result = m_views.Contains(view);
            if (!result)
                m_views.ForEach(delegate (Visual_E view)
                {
                    if (result)
                        return;
                    result = view.ContainsInHierarchy(view);
                });
            return result;
        }
        public virtual V Q<V>(string name = null) where V : Visual_E
        {
            bool matchName(Visual_E view) 
                => (name == null || (name != null && view.Name == name));
            
            V resultHorizontal = null;
            V resultVertical = null;
            m_views.ForEach(delegate (Visual_E view)
            {
                if (view is V v && matchName(v) && resultHorizontal == null)
                    resultHorizontal = v;
                if (resultHorizontal == null && resultVertical == null)
                    resultVertical = view.Q<V>(name);
            });
            return (resultHorizontal != null) ? resultHorizontal : resultVertical;
        }

        #endregion

        #region Protected methods

        protected void Init(VisualElement parent, VisualElement visual, CustomStyle_SO style_SO, StyleKeys keys)
        {
            m_globalPref = GetGlobalPrefSO();
            m_globalPref.ApplyCustomStyle.AddListener(ApplyAllFormatAndStyle);
            m_styleApplicator = new UIElementStyleApplicator(m_globalPref);

            m_visualStylesMap = new Dictionary<VisualElement, (CustomStyle_SO, StyleKeys, VisualManipulator)>();
            m_views = new List<Visual_E>();

            this.Root = visual;
            AddVisualStyle(Root, style_SO, keys);

            Initialize();
            IsInstantiated = true;

            if (parent != null) InsertRootTo(parent);
        }

        protected virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        protected void OnDisplayedOrHiddenTrigger(bool value)
            => DisplayedOrHidden?.Invoke(value);

        #region Get Resources

        /// <summary>
        /// Get the VisualTreeAsset from [resourcePath] and return the first child of the new VisualElement.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        protected virtual VisualElement GetVisualRoot(string resourcePath)
        {
            VisualTreeAsset visualTA = Resources.Load<VisualTreeAsset>(resourcePath);
            Debug.Assert(visualTA != null, $"[{resourcePath}] return a null visual tree asset.");
            Debug.Assert(visualTA.CloneTree().childCount == 1, $"[{resourcePath}] must have a single visual as root.");
            IEnumerator<VisualElement> iterator = visualTA.CloneTree().Children().GetEnumerator();
            iterator.MoveNext();
            return iterator.Current;
        }

        /// <summary>
        /// Get the CustomStyle_SO and return it.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        protected CustomStyle_SO GetStyleSO(string resourcePath)
        {
            if (resourcePath == "") throw new Exception("resourcePath empty");
            if (resourcePath == null) return null;
            CustomStyle_SO style_SO = Resources.Load<CustomStyle_SO>(resourcePath);
            Debug.Assert(style_SO != null, $"[{resourcePath}] return a null CustomStyle_SO.");
            return style_SO;
        }

        /// <summary>
        /// Get the GlobalPreferences_SO and return it.
        /// </summary>
        /// <returns></returns>
        protected GlobalPreferences_SO GetGlobalPrefSO()
        {
            GlobalPreferences_SO globalPreferences = Resources.Load<GlobalPreferences_SO>("Preferences/GlobalPreferences");
            if (globalPreferences == null) throw new NullReferenceException("Global pref null");
            return globalPreferences;
        }

        #endregion

        protected virtual void ObjectPooling<T>(out T vE, List<T> listDisplayed, List<T> listWaited, Func<T> init)
        {
            if (listWaited.Count == 0)
                vE = init();
            else
            {
                vE = listWaited[listWaited.Count - 1];
                listWaited.RemoveAt(listWaited.Count - 1);
            }
            listDisplayed.Add(vE);
        }

        #endregion
    }
}