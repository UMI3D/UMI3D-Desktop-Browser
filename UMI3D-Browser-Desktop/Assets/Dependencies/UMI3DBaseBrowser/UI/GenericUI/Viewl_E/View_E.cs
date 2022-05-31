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
using System;
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class View_E
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
        protected Dictionary<VisualElement, VisualManipulator> m_visualMap;
        protected List<View_E> m_views;
    
        #region Constructors and Destructor
        
        public View_E() : 
            this(null, null) 
        { }
        public View_E(string styleResourcePath, StyleKeys keys) :
            this (new VisualElement(), styleResourcePath, keys)
        { }
        public View_E(VisualElement visual) :
            this (visual, null, null)
        { }
        public View_E(VisualElement visual, string styleResourcePath, StyleKeys keys)
        {
            if (visual == null) throw new NullReferenceException($"visual is null");
            Init(visual, styleResourcePath, keys);
        }
        public View_E(string visualResourcePath, string styleResourcePath, StyleKeys keys)
        {
            VisualElement visual = GetVisualRoot(visualResourcePath);
            Init(visual, styleResourcePath, keys);
        }
        ~View_E()
        {
            Destroy();
        }

        #endregion

        #region Public methods

        public virtual void ToogleVisibility()
        {
            if (IsDisplaying) Hide();
            else Display();
        }
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
        /// <summary>
        /// Query a visualElement child of Root where name = [name if not null].
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual VisualElement QR(string name = null)
            => QR<VisualElement>(name);
        /// <summary>
        /// Query a visualElement of type [V] child of Root where name = [name if not null].
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual V QR<V>(string name = null) where V : VisualElement
            => Root.Q<V>(name);
        public virtual void Add(View_E child)
        {
            if (m_views.Contains(child))
                return;
            m_views.Add(child);
        }
        public virtual void Insert(int index, View_E child)
        {
            if (m_views.Contains(child))
                return;
            m_views.Insert(index, child);
        }
        public virtual void Remove(View_E view)
            => m_views.Remove(view);
        public virtual bool ContainsInHierarchy(View_E view)
        {
            bool result = m_views.Contains(view);
            if (!result)
                m_views.ForEach(delegate (View_E view)
                {
                    if (result)
                        return;
                    result = view.ContainsInHierarchy(view);
                });
            return result;
        }
        public virtual V Q<V>(string name = null) where V : View_E
        {
            bool matchName(View_E view) 
                => (name == null || view.Name == name);

            var toQuery = new Queue<View_E>(m_views);
            var queried = new List<View_E>();

            while (toQuery.Count > 0)
            {
                var current = toQuery.Dequeue();
                if (queried.Contains(current))
                    continue;

                queried.Add(current);
                if (current is V v && matchName(v))
                    return v;

                current.m_views.ForEach((view) =>
                {
                    toQuery.Enqueue(view);
                });
            }
            return null;
        }

        #endregion

        #region Protected methods

        protected void Init(VisualElement visual, string styleResourcePath, StyleKeys keys)
        {
            m_globalPref = GetGlobalPrefSO();
            m_globalPref.ApplyCustomStyle.AddListener(ApplyAllFormatAndStyle);

            m_visualMap = new Dictionary<VisualElement, VisualManipulator>();
            m_views = new List<View_E>();

            this.Root = visual;
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            AddVisualStyle(Root, style_SO, keys);

            Initialize();
            IsInstantiated = true;
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
            if (string.IsNullOrEmpty(resourcePath)) throw new Exception("resourcePath null or empty");
            VisualTreeAsset visualTA = Resources.Load<VisualTreeAsset>(resourcePath);
            if (visualTA == null) throw new NullReferenceException($"[{resourcePath}] return a null visual tree asset.");
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
        protected virtual CustomStyle_SO GetStyleSO(string resourcePath)
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

        /// <summary>
        /// Anime the VisualElement.
        /// </summary>
        /// <param name="vE"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="durationMs"></param>
        /// <param name="fromStartToEnd"></param>
        /// <param name="animation"></param>
        protected virtual UnityEngine.UIElements.Experimental.ValueAnimation<float> Anime(VisualElement vE, float startValue, float endValue, int durationMs, bool fromStartToEnd, Action<VisualElement, float> animation)
        {
            Debug.LogWarning("Use of Unity experimental API. May not work in the future. (2021)");
            UnityEngine.UIElements.Experimental.ValueAnimation<float> valueAnimation;
            if (fromStartToEnd)
                valueAnimation = vE.experimental.animation.Start(startValue, endValue, durationMs, animation);
            else
                valueAnimation = vE.experimental.animation.Start(endValue, startValue, durationMs, animation);

            return valueAnimation;
        }

        #endregion
    }
}