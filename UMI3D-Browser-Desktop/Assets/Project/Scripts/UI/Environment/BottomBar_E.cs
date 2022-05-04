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
            => Console.UpdateRootKeys((value) ? m_notificationOnKeys : m_notificationOffKeys);
        public void UpdateAlertNotificationIcon()
            => Console.UpdateRootKeys(m_notificationAlertKeys);
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

    public partial class BottomBar_E : View_E
    {
        private BottomBar_E() :
            base(s_uxml, s_menuStyle, StyleKeys.DefaultBackground)
        { }

        public override void Reset()
        {
            base.Reset();
            Console.ResetClickedEvent();
        }

        protected override void Initialize()
        {
            base.Initialize();

            MenuShortcut = new Label_E(QR<Label>("menuShortcut"), "BottomBar_LeftLabel", StyleKeys.DefaultText, "Right Click - Open Menu");
            ShortcutShortcut = new Label_E(QR<Label>("shortcutShortcut"), "BottomBar_LeftLabel", StyleKeys.DefaultText, "F1 - Open Actions Shortcuts");
            Timer = new Label_E(QR<Label>("timer"), "BottomBar_rightLabel", StyleKeys.DefaultText, "00:00:00");
            ParticipantCount = new Label_E(QR<Label>("participantCount"), "BottomBar_rightLabel", StyleKeys.DefaultText);

            Console = new Button_E(Root.Q<Button>("notification"), "Square1", StyleKeys.DefaultBackground);
            string consoleIconStyle = "UI/Style/BottomBar/ConsoleIcon";
            var consoleIcon = new View_E(consoleIconStyle, m_consoleIconDefaultKeys);
            Console.Add(consoleIcon);
            LinkMouseBehaviourChanged(Console, consoleIcon);
            Console.GetRootManipulator().ProcessDuringBubbleUp = true;

            Settings = new Button_E(Root.Q<Button>("settings"));
            Settings.Toggle(false);
            Settings.AddStateKeys(Settings, "Square1", StyleKeys.Bg("on"), StyleKeys.Bg("off"));
            string settingsIconStyle = "UI/Style/BottomBar/SettingsIcon";
            var settingsIcon = new View_E(settingsIconStyle, m_settingsIconKeys);
            Settings.Add(settingsIcon);
            LinkMouseBehaviourChanged(Settings, settingsIcon);
            Settings.GetRootManipulator().ProcessDuringBubbleUp = true;
        }

    }
}