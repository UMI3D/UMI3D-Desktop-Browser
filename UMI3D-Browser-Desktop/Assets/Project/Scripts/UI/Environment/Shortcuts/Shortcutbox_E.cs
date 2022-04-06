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
using umi3d.cdk.menu;
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
                {
                    m_instance = new Shortcutbox_E();
                }
                return m_instance;
            }
        }
        public static bool ShouldDisplay { get; set; } = false;
        public static bool ShouldHide { get; set; } = false;

        protected static ScrollView_E m_shortcuts { get; set; } = null;
        protected static float m_width { get; set; } = default;
        protected static List<Shortcut_E> m_shortcutsDisplayed;
        protected static List<Shortcut_E> m_shortcutsWaited;
        //protected Controller.KeyBindings_SO keyBindings;

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
            if (Displayed)
                Hide();
            else
                Display();
        }

        public void AddShortcut(string title, params string[] keys)
        {
            Shortcut_E shortcut;
            ObjectPooling(out shortcut, m_shortcutsDisplayed, m_shortcutsWaited);

            //Sprite[] shortcutIcons = new Sprite[keys.Length];
            //for (int i = 0; i < keys.Length; ++i)
            //    shortcutIcons[i] = keyBindings.GetSpriteFrom(keys[i]);
        }

        public void RemoveShortcut()
        {

        }

        public void ClearShortcut()
        {

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

        private void ObjectPooling<T>(out T vE, List<T> listDisplayed, List<T> listWaited) where T : new()
        {
            if (listWaited.Count == 0)
                vE = new T();
            else
            {
                vE = listWaited[listWaited.Count - 1];
                listWaited.RemoveAt(listWaited.Count - 1);
            }
            listDisplayed.Add(vE);
        }
    }

    public partial class Shortcutbox_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            var title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/Shortcuts/Shortcutbox_Title";
            StyleKeys titleKeys = new StyleKeys();
            new Label_E(title, titleStyle, titleKeys, "Shortcuts");

            var scrollView = Root.Q<ScrollView>();
            m_shortcuts = new ScrollView_E(scrollView);

            Root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            m_shortcutsDisplayed = new List<Shortcut_E>();
            m_shortcutsWaited = new List<Shortcut_E>();
        }

        public override void Display()
        {
            AnimeVisualElement(Root, m_width, true, (elt, val) =>
            {
                elt.style.right = m_width - val;
            });
            Displayed = true;
            ShouldDisplay = false;
        }

        public override void Hide()
        {
            AnimeVisualElement(Root, m_width, false, (elt, val) =>
            {
                elt.style.right = m_width - val;
            });
            Displayed = false;
            ShouldHide = false;
        }
    }
}
