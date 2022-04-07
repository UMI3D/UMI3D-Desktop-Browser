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
    public partial class Console_E
    {
        public static Console_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Console_E();
                return m_instance;
            }
        }

        public event Action NewLogAdded;
        public static Label_E Version { get; private set; } = null;
        public static bool ShouldDisplay { get; set; } = false;
        public static bool ShouldHide { get; set; } = false;

        protected static ScrollView_E m_logs { get; set; } = null;
        protected static float m_width { get; set; } = default;
        protected static List<Label_E> m_logDisplayed;
        protected static List<Label_E> m_logWaited;

        private static Console_E m_instance;
        private static string m_consoleUXML = "UI/UXML/console";
        private static string m_consoleStyle = "UI/Style/Console/Console";
        private static StyleKeys m_consoleKeys = new StyleKeys(null, "", null);
        private static string m_logStyle = "UI/Style/Console/Console_Log";
        private static StyleKeys m_logKeys = new StyleKeys("log", null, null);
        private static StyleKeys m_assertKeys = new StyleKeys("assert", null, null);
        private static StyleKeys m_errorKeys = new StyleKeys("error", null, null);
        private static StyleKeys m_exceptionKeys = new StyleKeys("exception", null, null);
    }

    public partial class Console_E
    {
        public void DisplayOrHide()
        {
            if (Displayed)
                Hide();
            else
                Display();
        }

        public void AddLog()
        {
            //TODO
        }

        public void ClearLog()
        {
            //TODO
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                return;

            ObjectPooling(out Label_E log, m_logDisplayed, m_logWaited, () => new Label_E(m_logStyle, null));
            m_logs.Adds(log);
            NewLogAdded?.Invoke();
            log.value = logString;

            switch (type)
            {
                case LogType.Error:
                    log.UpdateLabelKeys(m_logKeys);
                    break;
                case LogType.Assert:
                    log.UpdateLabelKeys(m_assertKeys);
                    break;
                case LogType.Warning:
                    log.UpdateLabelKeys(m_logKeys);
                    break;
                case LogType.Log:
                    log.UpdateLabelKeys(m_logKeys);
                    break;
                case LogType.Exception:
                    log.UpdateLabelKeys(m_exceptionKeys);
                    break;
                default:
                    break;
            }
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
            if (!isShowing)
                vE.experimental.animation.Start(0, -value, 100, animation);
            else
                vE.experimental.animation.Start(-value, 0, 100, animation);
        }
    }

    public partial class Console_E : Visual_E
    {
        private Console_E() :
            base(m_consoleUXML, m_consoleStyle, m_consoleKeys)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            var title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/Console/Console_Version";
            StyleKeys titleKeys = new StyleKeys("", "", null);
            Version = new Label_E(title, titleStyle, titleKeys);

            var scrollView = Root.Q<ScrollView>();
            m_logs = new ScrollView_E(scrollView);

            Root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            m_logDisplayed = new List<Label_E>();
            m_logWaited = new List<Label_E>();

            Application.logMessageReceived += HandleLog;
        }

        public override void Display()
        {
            AnimeVisualElement(Root, m_width, true, (elt, val) =>
            {
                elt.style.right = val;
            });
            Displayed = true;
            ShouldDisplay = false;
            OnDisplayedTrigger(true);
        }

        public override void Hide()
        {
            AnimeVisualElement(Root, m_width, false, (elt, val) =>
            {
                elt.style.right = val;
            });
            Displayed = false;
            ShouldHide = false;
            OnDisplayedTrigger(false);
        }
    }
}
