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
        public static bool ShouldDisplay { get; set; } = false;
        public static bool ShouldHide { get; set; } = false;

        protected static ScrollView_E s_shortcuts { get; set; } = null;
        protected static float s_width { get; set; } = default;
        protected static List<Shortcut_E> s_shortcutsDisplayed;
        protected static List<Shortcut_E> s_shortcutsWaited;
        protected static KeyBindings_SO s_keyBindings;

        private static string s_shortcutboxUXML => "UI/UXML/Shortcuts/shortcutbox";
        private static string s_shortcutboxStyle => "UI/Style/Shortcuts/Shortcutbox";
        private static StyleKeys s_shortcutboxKeys => new StyleKeys(null, "", null);

        private static bool s_isRightClickShortcutAdded { get; set; } = false;
        private static Shortcut_E s_rightClickShortcut;
        private static string s_rightClickTitle { get; set; } = null;
    
        /// <summary>
        /// Add a shortcut to the shortcutbox.
        /// </summary>
        /// <param name="title">Title of this shortcut</param>
        /// <param name="keys">the keys to press</param>
        public void AddShortcut(string title, params string[] keys)
        {
            CreateShortcut(out Shortcut_E shortcut, title, keys);
            s_shortcuts.Add(shortcut);
        }

        public void AddShortcutAt(int index, string title, params string[] keys)
        {
            CreateShortcut(out Shortcut_E shortcut, title, keys);
            s_shortcuts.Insert(index, shortcut);
        }

        public void AddRightClickShortcut(string title)
        {
            s_rightClickTitle = title;
            CreateShortcut(out s_rightClickShortcut, title, "Mouse1");
            s_shortcuts.Insert(0, s_rightClickShortcut);
            s_isRightClickShortcutAdded = true;
        }

        public void RemoveRightClickShortcut()
        {
            s_isRightClickShortcutAdded = false;
            s_rightClickTitle = null;
            s_shortcuts.Remove(s_rightClickShortcut);
            s_shortcutsWaited.Add(s_rightClickShortcut);
            s_shortcutsDisplayed.Remove(s_rightClickShortcut);
        }

        public void RemoveShortcut(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Remove all shortcuts from the shortcutbox.
        /// </summary>
        public void ClearShortcut()
        {
            s_shortcutsDisplayed.ForEach((shortcut) => s_shortcuts.Remove(shortcut));
            s_shortcutsWaited.AddRange(s_shortcutsDisplayed);
            s_shortcutsDisplayed.Clear();
        }

        public void ClearShortcutExceptRightClick()
        {
            ClearShortcut();
            if (s_isRightClickShortcutAdded)
                AddRightClickShortcut(s_rightClickTitle);
        }

        private void CreateShortcut(out Shortcut_E shortcut, string title, params string[] keys)
        {
            ObjectPooling(out shortcut, s_shortcutsDisplayed, s_shortcutsWaited, () => new Shortcut_E());

            Sprite[] shortcutIcons = new Sprite[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                shortcutIcons[i] = s_keyBindings.GetSpriteFrom(keys[i]);

            shortcut.Setup(title, shortcutIcons);
        }

        private void OnSizeChanged(GeometryChangedEvent e)
        {
            if (e.oldRect.width != e.newRect.width)
                s_width = e.newRect.width;

            if (ShouldDisplay)
                Display();
            if (ShouldHide)
                Hide();
        }

        private void AnimeWindowVisibility(bool state)
            => Anime(Root, -s_width, 0, 100, state, (elt, val) =>
            {
                elt.style.left = val;
            });
    }

    public partial class Shortcutbox_E : ISingleUI
    {
        public static Shortcutbox_E Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new Shortcutbox_E();
                return s_instance;
            }
        }

        private static Shortcutbox_E s_instance;
    }

    public partial class Shortcutbox_E : AbstractWindow_E
    {
        public override void Reset()
        {
            base.Reset();
            s_shortcutsDisplayed.ForEach((shortcut) => s_shortcuts.Remove(shortcut));
            s_shortcutsWaited.Clear();
            s_shortcutsDisplayed.Clear();
        }

        public override void Display()
        {
            if (s_width <= 0f)
            {
                ShouldDisplay = true;
                return;
            }
            AnimeWindowVisibility(true);
            IsDisplaying = true;
            ShouldDisplay = false;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            if (s_width <= 0f)
            {
                ShouldHide = true;
                return;
            }
            AnimeWindowVisibility(false);
            IsDisplaying = false;
            ShouldHide = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            string titleStyle = "UI/Style/Shortcuts/Shortcutbox_Title";
            StyleKeys titleKeys = new StyleKeys("", "", null);
            SetTopBar("Actions", titleStyle, titleKeys, false);

            var scrollView = Root.Q<ScrollView>();
            s_shortcuts = new ScrollView_E(scrollView);

            Root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            s_shortcutsDisplayed = new List<Shortcut_E>();
            s_shortcutsWaited = new List<Shortcut_E>();
            s_keyBindings = Resources.Load<KeyBindings_SO>("KeyBindings");
        }

        private Shortcutbox_E() :
            base(s_shortcutboxUXML, s_shortcutboxStyle, s_shortcutboxKeys)
        { }
    }
}
