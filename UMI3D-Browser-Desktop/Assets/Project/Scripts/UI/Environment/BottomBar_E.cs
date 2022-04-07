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
    }

    public partial class BottomBar_E
    {
        public void OpenCloseShortcut(bool value)
            => ShortcutShortcut.value = (value) ? "F1 - Close Shortcut" : "F1 - Open Shortcut";
    }

    public partial class BottomBar_E : Visual_E
    {
        public BottomBar_E() :
            base(m_menuUXML, m_menuStyle, m_menuKeys)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            
            string labelsStyle = "UI/Style/Toolbox/ToolboxName";
            StyleKeys labelsKeys = new StyleKeys("", null, null);

            var menuShortcut = Root.Q<Label>("menuShortcut");
            Debug.Log($"menuShortcut null == [{menuShortcut == null}]");
            MenuShortcut = new Label_E(menuShortcut, labelsStyle, labelsKeys, "Right Click - Open Menu");

            var shortcutShortcut = Root.Q<Label>("shortcutShortcut");
            ShortcutShortcut = new Label_E(shortcutShortcut, labelsStyle, labelsKeys, "F1 - Open Shortcut");

            var timer = Root.Q<Label>("timer");
            Timer = new Label_E(timer, labelsStyle, labelsKeys, "00:00:00");

            var participantCount = Root.Q<Label>("menuShortcut");
            ParticipantCount = new Label_E(participantCount, labelsStyle, labelsKeys);

            var notification = Root.Q<Button>("notification");
            string notificationStyle = "UI/Style/BottomBar/Notification";
            StyleKeys notificationKeys = new StyleKeys(null, "", null);
            Notification = new Button_E(notification, notificationStyle, notificationKeys);
        }

    }
}