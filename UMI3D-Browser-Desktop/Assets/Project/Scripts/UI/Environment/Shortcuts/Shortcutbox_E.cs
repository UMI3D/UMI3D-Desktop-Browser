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
using umi3dDesktopBrowser.ui.Controller;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Shortcutbox_E
    {
        public static Shortcutbox_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Shortcutbox_E();
                return m_instance;
            }
        }
        public static bool ShouldDisplay { get; set; } = false;
        public static bool ShouldHide { get; set; } = false;

        protected static ScrollView_E m_shortcuts { get; set; } = null;
        protected static float m_width { get; set; } = default;
        protected static List<Shortcut_E> m_shortcutsDisplayed;
        protected static List<Shortcut_E> m_shortcutsWaited;
        protected static KeyBindings_SO m_keyBindings;

        private static Shortcutbox_E m_instance;
        private static string m_shortcutboxUXML => "UI/UXML/Shortcuts/shortcutbox";
        private static string m_shortcutboxStyle => "UI/Style/Shortcuts/Shortcutbox";
        private static StyleKeys m_shortcutboxKeys => new StyleKeys(null, "", null);
    }

    public partial class Shortcutbox_E
    {
        private Shortcutbox_E() :
            base(m_shortcutboxUXML, m_shortcutboxStyle, m_shortcutboxKeys)
        { }

        public void DisplayOrHide()
        {
            if (IsDisplaying)
                Hide();
            else
                Display();
        }

        /// <summary>
        /// Add a shortcut to the shortcutbox.
        /// </summary>
        /// <param name="title">Title of this shortcut</param>
        /// <param name="keys">the keys to press</param>
        public void AddShortcut(string title, params string[] keys)
        {
            ObjectPooling(out Shortcut_E shortcut, m_shortcutsDisplayed, m_shortcutsWaited, () => new Shortcut_E());

            Sprite[] shortcutIcons = new Sprite[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                shortcutIcons[i] = m_keyBindings.GetSpriteFrom(keys[i]);

            shortcut.Setup(title, shortcutIcons);
            m_shortcuts.Add(shortcut);
        }

        public void RemoveShortcut()
        {

        }

        /// <summary>
        /// Remove all shortcuts from the shortcutbox.
        /// </summary>
        public void ClearShortcut()
        {
            m_shortcutsDisplayed.ForEach((shortcut) => m_shortcuts.Remove(shortcut));
            m_shortcutsWaited.AddRange(m_shortcutsDisplayed);
            m_shortcutsDisplayed.Clear();
        }

        private void OnSizeChanged(GeometryChangedEvent e)
        {
            if (e.oldRect.width != e.newRect.width)
                m_width = e.newRect.width;

            if (ShouldDisplay)
                Display();
            if (ShouldHide)
                Hide();
        }

        /// <summary>
        /// Anime the VisualElement.
        /// </summary>
        /// <param name="vE">the VisualElement to be animated.</param>
        /// <param name="value">The animation will be trigger from 0 to this value when isShowing is true. Else, from this value to 0.</param>
        /// <param name="isShowing">The VisualElement should be displayed if true.</param>
        /// <param name="animation">The animation to be perform.</param>
        private void AnimeVisualElement(VisualElement vE, float value, bool isShowing, Action<VisualElement, float> animation)
        {
            Debug.LogWarning("Use of Unity experimental API. May not work in the future. (2021)");
            if (isShowing)
                vE.experimental.animation.Start(0, value, 100, animation);
            else
                vE.experimental.animation.Start(value, 0, 100, animation);
        }
    }

    public partial class Shortcutbox_E : Visual_E
    {
        public override void Reset()
        {
            base.Reset();
            m_shortcutsDisplayed.ForEach((shortcut) => m_shortcuts.Remove(shortcut));
            m_shortcutsWaited.Clear();
            m_shortcutsDisplayed.Clear();
        }

        public override void Display()
        {
            if (m_width <= 0f)
            {
                ShouldDisplay = true;
                return;
            }
            AnimeVisualElement(Root, m_width, true, (elt, val) =>
            {
                elt.style.right = m_width - val;
            });
            IsDisplaying = true;
            ShouldDisplay = false;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            if (m_width <= 0f)
            {
                ShouldHide = true;
                return;
            }
            AnimeVisualElement(Root, m_width, false, (elt, val) =>
            {
                elt.style.right = m_width - val;
            });
            IsDisplaying = false;
            ShouldHide = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/Shortcuts/Shortcutbox_Title";
            StyleKeys titleKeys = new StyleKeys("", "", null);
            new Label_E(title, titleStyle, titleKeys, "Shortcuts");

            var scrollView = Root.Q<ScrollView>();
            m_shortcuts = new ScrollView_E(scrollView);

            Root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            m_shortcutsDisplayed = new List<Shortcut_E>();
            m_shortcutsWaited = new List<Shortcut_E>();
            m_keyBindings = Resources.Load<KeyBindings_SO>("KeyBindings");
        }
    }
}
