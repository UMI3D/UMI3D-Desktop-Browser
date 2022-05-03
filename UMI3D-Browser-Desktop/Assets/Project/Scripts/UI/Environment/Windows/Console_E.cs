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
using System.Collections;
using System.Collections.Generic;
using umi3DBrowser.UICustomStyle;
using umi3dDesktopBrowser.ui.Controller;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Console_E
    {
        public event Action NewLogAdded;

        protected static ScrollView_E s_logs { get; set; } = null;
        protected static ScrollView_E s_details { get; set; } = null;
        protected static Label_E s_lastSelectedLog { get; set; } = null;
        protected static List<Label_E> s_logDisplayed;
        protected static List<Label_E> s_logWaited;
        protected static List<Label_E> s_logDetailDisplayed;
        protected static List<Label_E> s_logDetailWaited;
        protected static Dictionary<Label_E, (string, string, string, LogType)> s_logsMap;

        private static string s_consoleUXML = "UI/UXML/console";
        private static string s_consoleStyle = "UI/Style/Console/Console";
        private static string s_logStyle = "UI/Style/Console/Console_Log";
        private static StyleKeys s_logKeys = new StyleKeys("log", null, "unselected");
        private static StyleKeys s_logKeysSelected = new StyleKeys("log", null, "selected");
        private static StyleKeys s_assertKeys = new StyleKeys("assert", null, "unselected");
        private static StyleKeys s_assertKeysSelected = new StyleKeys("assert", null, "selected");
        private static StyleKeys m_errorKeys = new StyleKeys("error", null, "unselected");
        private static StyleKeys s_exceptionKeys = new StyleKeys("exception", null, "unselected");
        private static StyleKeys s_exceptionKeysSelected = new StyleKeys("exception", null, "selected");
        private static string s_logDetailStyle = "UI/Style/Console/Console_LogDetail";

        #region Private Methods

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
            UpdateLogStyle(log, type, false);
        }

        private void OnLogSelected(MouseBehaviour behaviour, Label_E logSource)
        {
            if (behaviour != MouseBehaviour.MousePressed)
                return;
            
            s_details.Clear();
            var (_, logString, stackTrace, type) = s_logsMap[logSource];
            UpdateLogStyle(s_lastSelectedLog, type, false);
            s_lastSelectedLog = logSource;
            UpdateLogStyle(s_lastSelectedLog, type, true);

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

        private void UpdateLogStyle(Label_E log, LogType type, bool isSelected)
        {
            if (log == null)
                return;

            switch (type)
            {
                case LogType.Error:
                    log.UpdateLabelKeys((isSelected) ? s_logKeysSelected : s_logKeys);
                    break;
                case LogType.Assert:
                    log.UpdateLabelKeys((isSelected) ? s_assertKeysSelected : s_assertKeys);
                    break;
                case LogType.Warning:
                    log.UpdateLabelKeys((isSelected) ? s_logKeysSelected : s_logKeys);
                    break;
                case LogType.Log:
                    log.UpdateLabelKeys((isSelected) ? s_logKeysSelected : s_logKeys);
                    break;
                case LogType.Exception:
                    log.UpdateLabelKeys((isSelected) ? s_exceptionKeysSelected : s_exceptionKeys);
                    break;
                default:
                    break;
            }
        }

        private IEnumerator AnimeWindowVisibility(bool state)
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Anime(Root, -Root.resolvedStyle.width, 0, 100, state, (elt, val) =>
            {
                elt.style.right = val;
            });
        }

        #endregion
    }

    public partial class Console_E : ISingleUI
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

        private static Console_E s_instance;
    }

    public partial class Console_E : AbstractWindow_E
    {
        protected override string m_topBarStyle => "UI/Style/Console/Console_Version";

        public override void Reset()
        {
            base.Reset();
            //Todo
        }

        public override void Display()
        {
            UIManager.StartCoroutine(AnimeWindowVisibility(true));
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            UIManager.StartCoroutine(AnimeWindowVisibility(false));
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetTopBar("", m_topBarStyle, StyleKeys.DefaultTextAndBackground, false);

            var logs = Root.Q<ScrollView>("logs");
            s_logs = new ScrollView_E(logs);

            var details = Root.Q<ScrollView>("details");
            string detailsStyle = "UI/Style/Console/Console_Details";
            s_details = new ScrollView_E(details, detailsStyle, StyleKeys.DefaultBackground);

            s_logDisplayed = new List<Label_E>();
            s_logWaited = new List<Label_E>();
            s_logDetailDisplayed = new List<Label_E>();
            s_logDetailWaited = new List<Label_E>();
            s_logsMap = new Dictionary<Label_E, (string, string, string, LogType)>();

            Application.logMessageReceived += HandleLog;
        }

        private Console_E() :
            base(s_consoleUXML, s_consoleStyle, StyleKeys.DefaultBackground)
        { }
    }
}
