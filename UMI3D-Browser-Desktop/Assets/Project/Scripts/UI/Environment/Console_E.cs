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
using umi3DBrowser.UICustomStyle;
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
                if (s_instance == null)
                    s_instance = new Console_E();
                return s_instance;
            }
        }

        public event Action NewLogAdded;
        public static Label_E Version { get; private set; } = null;
        public static bool ShouldDisplay { get; set; } = false;
        public static bool ShouldHide { get; set; } = false;

        protected static ScrollView_E s_logs { get; set; } = null;
        protected static ScrollView_E s_details { get; set; } = null;
        protected static float s_width { get; set; } = default;
        protected static List<Label_E> s_logDisplayed;
        protected static List<Label_E> s_logWaited;
        protected static List<Label_E> s_logDetailDisplayed;
        protected static List<Label_E> s_logDetailWaited;
        protected static Dictionary<Label_E, (string, string, string, LogType)> s_logsMap;

        private static Console_E s_instance;
        private static string s_consoleUXML = "UI/UXML/console";
        private static string s_consoleStyle = "UI/Style/Console/Console";
        private static StyleKeys s_consoleKeys = new StyleKeys(null, "", null);
        private static string s_logStyle = "UI/Style/Console/Console_Log";
        private static StyleKeys s_logKeys = new StyleKeys("log", null, null);
        private static StyleKeys s_assertKeys = new StyleKeys("assert", null, null);
        private static StyleKeys m_errorKeys = new StyleKeys("error", null, null);
        private static StyleKeys s_exceptionKeys = new StyleKeys("exception", null, null);
        private static string s_logDetailStyle = "UI/Style/Console/Console_LogDetail";
    }

    public partial class Console_E
    {
        public void DisplayOrHide()
        {
            Debug.Log($"Le Lorem Ipsum est simplement du faux texte employ� dans la composition et la mise en page avant impression. Le Lorem Ipsum est le faux texte standard de l'imprimerie depuis les ann�es 1500, quand un imprimeur anonyme assembla ensemble des morceaux de texte pour r�aliser un livre sp�cimen de polices de texte. Il n'a pas fait que survivre cinq si�cles, mais s'est aussi adapt� � la bureautique informatique, sans que son contenu n'en soit modifi�. Il a �t� popularis� dans les ann�es 1960 gr�ce � la vente de feuilles Letraset contenant des passages du Lorem Ipsum, et, plus r�cemment, par son inclusion dans des applications de mise en page de texte, comme Aldus PageMaker.");
            if (IsDisplaying)
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

            SetLogLabel(out Label_E log, s_logDisplayed, s_logWaited, s_logStyle, type);
            
            VisualManipulator manipulator = log.GetVisualManipulator(log.Root);
            manipulator.MouseBehaviourChanged += (behaviour) 
                => OnLogSelected(behaviour, log);
            
            string time = DateTime.Now.ToLongTimeString();
            log.value = $"[{time}] {logString}";

            s_logs.Add(log);
            s_logsMap.Add(log, (time, logString, stackTrace, type));
            NewLogAdded?.Invoke();
        }

        private void SetLogLabel(out Label_E log, List<Label_E> displayed, List<Label_E> waited, string style, LogType type)
        {
            ObjectPooling(out log, displayed, waited, () => new Label_E(style, null));
            switch (type)
            {
                case LogType.Error:
                    log.UpdateLabelKeys(s_logKeys);
                    break;
                case LogType.Assert:
                    log.UpdateLabelKeys(s_assertKeys);
                    break;
                case LogType.Warning:
                    log.UpdateLabelKeys(s_logKeys);
                    break;
                case LogType.Log:
                    log.UpdateLabelKeys(s_logKeys);
                    break;
                case LogType.Exception:
                    log.UpdateLabelKeys(s_exceptionKeys);
                    break;
                default:
                    break;
            }
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

        private void OnLogSelected(MouseBehaviour behaviour, Label_E logSource)
        {
            if (behaviour != MouseBehaviour.MousePressed)
                return;
            s_details.Clear();

            var (_, logString, stackTrace, type) = s_logsMap[logSource];

            SetLogLabel(out Label_E log, s_logDetailDisplayed, s_logDetailWaited, s_logDetailStyle, type);
            log.value = logString;

            s_details.Add(log);

            string[] traces = stackTrace.Split('\n');
            foreach (string trace in traces)
            {
                SetLogLabel(out Label_E logTrace, s_logDetailDisplayed, s_logDetailWaited, s_logDetailStyle, LogType.Log);
                logTrace.value = trace;
                s_details.Add(logTrace);
            }
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
        public override void Display()
        {
            if (s_width <= 0f)
            {
                ShouldDisplay = true;
                return;
            }
            AnimeVisualElement(Root, s_width, true, (elt, val) =>
            {
                elt.style.right = val;
            });
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
            AnimeVisualElement(Root, s_width, false, (elt, val) =>
            {
                elt.style.right = val;
            });
            IsDisplaying = false;
            ShouldHide = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/Console/Console_Version";
            StyleKeys titleKeys = new StyleKeys("", "", null);
            Version = new Label_E(title, titleStyle, titleKeys);

            var logs = Root.Q<ScrollView>("logs");
            s_logs = new ScrollView_E(logs);

            var details = Root.Q<ScrollView>("details");
            string detailsStyle = "UI/Style/Console/Console_Details";
            StyleKeys detailsKeys = new StyleKeys(null, "", null);
            s_details = new ScrollView_E(details, detailsStyle, detailsKeys);

            Root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            s_logDisplayed = new List<Label_E>();
            s_logWaited = new List<Label_E>();
            s_logDetailDisplayed = new List<Label_E>();
            s_logDetailWaited = new List<Label_E>();
            s_logsMap = new Dictionary<Label_E, (string, string, string, LogType)>();

            Application.logMessageReceived += HandleLog;
        }

        private Console_E() :
            base(s_consoleUXML, s_consoleStyle, s_consoleKeys)
        { }
    }
}