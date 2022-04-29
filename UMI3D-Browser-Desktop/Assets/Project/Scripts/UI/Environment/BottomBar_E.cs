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
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class BottomBar_E
    {
        public Label_E MenuShortcut { get; private set; } = null;
        public Label_E ShortcutShortcut { get; private set; } = null;
        public Button_E Console { get; private set; } = null;
        public Button_E Settings { get; private set; } = null;
        public Label_E Timer { get; private set; } = null;
        public Label_E ParticipantCount { get; private set; } = null;

        private static string s_uxml => "UI/UXML/Menus/bottomBar";
        private static string s_menuStyle => "UI/Style/BottomBar/BottomBar";
        private static StyleKeys s_menuKeys => new StyleKeys(null, "", null);

        private StyleKeys m_notificationOffKeys = new StyleKeys(null, "off", null);
        private StyleKeys m_notificationOnKeys = new StyleKeys(null, "on", null);
        private StyleKeys m_notificationAlertKeys = new StyleKeys(null, "alert", null);

        private StyleKeys m_consoleIconDefaultKeys = new StyleKeys(null, "", null);
        private StyleKeys m_consoleIconLogKeys = new StyleKeys(null, "log", null);
        private StyleKeys m_consoleIconWarningKeys = new StyleKeys(null, "warning", null);
        private StyleKeys m_consoleIconErrorKeys = new StyleKeys(null, "error", null);

        private StyleKeys m_settingsOffKeys = new StyleKeys(null, "off", null);
        private StyleKeys m_settingsOnKeys = new StyleKeys(null, "on", null);
        private StyleKeys m_settingsIconKeys = new StyleKeys(null, "", null);

        public void OpenCloseMenuBar(bool value)
            => MenuShortcut.value = (value) ? "Right Click / Escape - Close Menu" : "Right Click / Escape - Open Menu";
        public void OpenCloseShortcut(bool value)
            => ShortcutShortcut.value = (value) ? "F1 - Close Actions Shortcuts" : "F1 - Open Actions Shortcuts";

        public void UpdateOnOffNotificationIcon(bool value)
            => Console.UpdateButtonKeys((value) ? m_notificationOnKeys : m_notificationOffKeys);
        public void UpdateAlertNotificationIcon()
            => Console.UpdateButtonKeys(m_notificationAlertKeys);
    }

    public partial class BottomBar_E : ISingleUI
    {
        public static BottomBar_E Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new BottomBar_E();
                }
                return s_instance;
            }
        }

        private static BottomBar_E s_instance;
    }

    public partial class BottomBar_E : Visual_E
    {
        private BottomBar_E() :
            base(s_uxml, s_menuStyle, s_menuKeys)
        { }

        public override void Reset()
        {
            base.Reset();
            Console.ResetClickedEvent();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            string leftLabelsStyle = "UI/Style/BottomBar/BottomBar_LeftLabel";
            string rightLabelsStyle = "UI/Style/BottomBar/BottomBar_rightLabel";
            StyleKeys labelsKeys = new StyleKeys("", null, null);

            var menuShortcut = Root.Q<Label>("menuShortcut");
            MenuShortcut = new Label_E(menuShortcut, leftLabelsStyle, labelsKeys, "Right Click - Open Menu");
            var shortcutShortcut = Root.Q<Label>("shortcutShortcut");
            ShortcutShortcut = new Label_E(shortcutShortcut, leftLabelsStyle, labelsKeys, "F1 - Open Actions Shortcuts");

            var timer = Root.Q<Label>("timer");
            Timer = new Label_E(timer, rightLabelsStyle, labelsKeys, "00:00:00");
            var participantCount = Root.Q<Label>("participantCount");
            ParticipantCount = new Label_E(participantCount, rightLabelsStyle, labelsKeys);

            var console = Root.Q<Button>("notification");
            string consoleStyle = "UI/Style/BottomBar/Console";
            StyleKeys consoleKeys = new StyleKeys(null, "", null);
            Console = new Button_E(console, consoleStyle, consoleKeys);
            string consoleIconStyle = "UI/Style/BottomBar/ConsoleIcon";
            var consoleIcon = new Visual_E(consoleIconStyle, m_consoleIconDefaultKeys);
            Console.Add(consoleIcon);
            LinkMouseBehaviourChanged(consoleIcon, Console, false);

            var settings = Root.Q<Button>("settings");
            Settings = new Button_E(settings);
            //settings.clicked += 
            Settings.Toggle(false);
            string settingsStyle = "UI/Style/BottomBar/Settings";
            Settings.AddStateKeys(Settings, settingsStyle, m_settingsOnKeys, m_settingsOffKeys);
            string settingsIconStyle = "UI/Style/BottomBar/SettingsIcon";
            var settingsIcon = new Visual_E(settingsIconStyle, m_settingsIconKeys);
            Settings.Add(settingsIcon);
            LinkMouseBehaviourChanged(settingsIcon, Settings, false);
        }

    }
}