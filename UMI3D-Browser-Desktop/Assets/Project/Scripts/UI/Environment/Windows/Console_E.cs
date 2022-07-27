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
using umi3d.baseBrowser.ui.viewController;
using umi3DBrowser.UICustomStyle;
using umi3dDesktopBrowser.ui.Controller;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Console_E
    {
        #region Fields

        public event Action<LogType> NewLogAdded;
        private Coroutine m_coroutine;

        protected static ScrollView_E s_logs { get; set; } = null;
        //protected static ScrollView_E s_details { get; set; } = null;
        protected static Label_E s_lastSelectedLog { get; set; } = null;
        protected static List<Label_E> s_logDisplayed;
        protected static List<Label_E> s_logWaited;
        //protected static List<Label_E> s_logDetailDisplayed;
        //protected static List<Label_E> s_logDetailWaited;
        protected static Dictionary<Label_E, (string, string, string, LogType)> s_logsMap;

        private static StyleKeys s_logKeys = new StyleKeys("log", null, "unselected");
        private static StyleKeys s_logKeysSelected = new StyleKeys("log", null, "selected");
        private static StyleKeys s_assertKeys = new StyleKeys("assert", null, "unselected");
        private static StyleKeys s_assertKeysSelected = new StyleKeys("assert", null, "selected");
        private static StyleKeys m_errorKeys = new StyleKeys("error", null, "unselected");
        private static StyleKeys s_exceptionKeys = new StyleKeys("exception", null, "unselected");
        private static StyleKeys s_exceptionKeysSelected = new StyleKeys("exception", null, "selected");

        #endregion

        #region Private Methods

        private IEnumerator DisplayWithoutAnimation()
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Root.style.right = 0;
        }

        private IEnumerator HideWithoutAnimation()
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Root.style.right = -Root.resolvedStyle.width;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                return;

            SetLogLabel(out Label_E log, s_logDisplayed, s_logWaited, "Corps2", type);
            
            //VisualManipulator manipulator = log.GetRootManipulator();
            //manipulator.MouseBehaviourChanged += (behaviour) 
            //    => OnLogSelected(behaviour, log);
            
            string time = DateTime.Now.ToLongTimeString();
            log.value = $"[{time}] {logString}";

            s_logs.Add(log);
            s_logsMap.Add(log, (time, logString, stackTrace, type));
            NewLogAdded?.Invoke(type);
        }

        private void SetLogLabel(out Label_E log, List<Label_E> displayed, List<Label_E> waited, string style, LogType type)
        {
            ObjectPooling(out log, displayed, waited, () => new Label_E(style, null));
            UpdateLogStyle(log, type, false);
        }

        //private void OnLogSelected(MouseBehaviour behaviour, Label_E logSource)
        //{
        //    if (behaviour != MouseBehaviour.MousePressed)
        //        return;
            
        //    s_details.Clear();
        //    var (_, logString, stackTrace, type) = s_logsMap[logSource];
        //    UpdateLogStyle(s_lastSelectedLog, type, false);
        //    s_lastSelectedLog = logSource;
        //    UpdateLogStyle(s_lastSelectedLog, type, true);

        //    SetLogLabel(out Label_E log, s_logDetailDisplayed, s_logDetailWaited, "Console_LogDetail", type);
        //    log.value = logString;

        //    s_details.Add(log);

        //    string[] traces = stackTrace.Split('\n');
        //    foreach (string trace in traces)
        //    {
        //        SetLogLabel(out Label_E logTrace, s_logDetailDisplayed, s_logDetailWaited, "Console_LogDetail", LogType.Log);
        //        logTrace.value = trace;
        //        s_details.Add(logTrace);
        //    }
        //}

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

        //private IEnumerator AnimeWindowVisibility(bool state)
        //{
        //    yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
        //    Anime(Root, -Root.resolvedStyle.width, 0, 100, state, (elt, val) =>
        //    {
        //        elt.style.right = val;
        //    });
        //}

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
        public static void DestroySingleton()
        {
            if (s_instance == null) return;
            if (Instance.m_coroutine != null) UIManager.StopCoroutine(Instance.m_coroutine);
            s_instance.Destroy();
            s_instance = null;
        }

        private static Console_E s_instance;
    }

    public partial class Console_E : AbstractPinnedWindow_E
    {
        public override void Reset()
        {
            base.Reset();
            //Todo
        }

        public override void Display()
        {
            m_coroutine = UIManager.StartCoroutine(DisplayWithoutAnimation());
            Settingbox_E.Instance.Hide();
            EmoteWindow_E.Instance.Hide();
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            m_coroutine = UIManager.StartCoroutine(HideWithoutAnimation());
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetTopBar("");

            var logs = Root.Q<ScrollView>("logs");
            s_logs = new ScrollView_E(logs);

            //var details = Root.Q<ScrollView>("details");
            //string detailsStyle = "UI/Style/Console/Console_Details";
            //s_details = new ScrollView_E(details, detailsStyle, StyleKeys.DefaultBackground);

            s_logDisplayed = new List<Label_E>();
            s_logWaited = new List<Label_E>();
            //s_logDetailDisplayed = new List<Label_E>();
            //s_logDetailWaited = new List<Label_E>();
            s_logsMap = new Dictionary<Label_E, (string, string, string, LogType)>();

            Application.logMessageReceived += HandleLog;
        }

        private Console_E() :
            base("console", "Console", StyleKeys.DefaultBackground)
        { }
    }
}
