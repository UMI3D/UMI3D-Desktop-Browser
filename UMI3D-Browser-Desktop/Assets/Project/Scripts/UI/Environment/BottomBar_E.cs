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
        public static BottomBar_E Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BottomBar_E();
                }
                return m_instance;
            }
        }

        public Label_E MenuShortcut { get; private set; } = null;
        public Label_E ShortcutShortcut { get; private set; } = null;
        public Button_E Notification { get; private set; } = null;
        public Label_E Timer { get; private set; } = null;
        public Label_E ParticipantCount { get; private set; } = null;

        private static BottomBar_E m_instance;
        private static string m_menuUXML => "UI/UXML/Menus/bottomBar";
        private static string m_menuStyle => "UI/Style/Menus/Menus";
        private static StyleKeys m_menuKeys => new StyleKeys(null, "", null);
        private StyleKeys m_notificationOffKeys = new StyleKeys(null, "off", null);
        private StyleKeys m_notificationOnKeys = new StyleKeys(null, "on", null);
        private StyleKeys m_notificationAlertKeys = new StyleKeys(null, "alert", null);
    }

    public partial class BottomBar_E
    {
        public void OpenCloseMenuBar(bool value)
            => MenuShortcut.value = (value) ? "Right Click / Escape - Close Menu" : "Right Click / Escape - Open Menu";
        public void OpenCloseShortcut(bool value)
            => ShortcutShortcut.value = (value) ? "F1 - Close Actions Shortcuts" : "F1 - Open Actions Shortcuts";

        public void UpdateOnOffNotificationIcon(bool value)
            => Notification.UpdateButtonKeys((value) ? m_notificationOnKeys : m_notificationOffKeys);
        public void UpdateAlertNotificationIcon()
            => Notification.UpdateButtonKeys(m_notificationAlertKeys);
    }

    public partial class BottomBar_E : Visual_E
    {
        public BottomBar_E() :
            base(m_menuUXML, m_menuStyle, m_menuKeys)
        { }

        public override void Reset()
        {
            base.Reset();
            Notification.ResetClickedEvent();
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

            var notification = Root.Q<Button>("notification");
            string notificationStyle = "UI/Style/BottomBar/Notification";
            Notification = new Button_E(notification, notificationStyle, m_notificationOffKeys);
        }

    }
}