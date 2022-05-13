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

        private static StyleKeys s_consoleDefaultKeys = StyleKeys.Bg("consoleDefault");
        private static StyleKeys s_consoleLogKeys = StyleKeys.Bg("consoleLog");
        private static StyleKeys s_consoleWarningKeys = StyleKeys.Bg("consoleWarning");
        private static StyleKeys s_consoleErrorKeys = StyleKeys.Bg("consoleError");

        public void OpenCloseMenuBar(bool value)
            => MenuShortcut.value = (value) ? "Right Click / Escape - Close Menu" : "Right Click / Escape - Open Menu";
        public void OpenCloseShortcut(bool value)
            => ShortcutShortcut.value = (value) ? "F1 - Close Actions Shortcuts" : "F1 - Open Actions Shortcuts";

        public void UpdateConsole(LogType type)
        {
            if (Console_E.Instance.IsDisplaying)
                return;

            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    Console.Q<Icon_E>().UpdateRootKeys(s_consoleErrorKeys);
                    break;
                case LogType.Assert:
                case LogType.Warning:
                    Console.Q<Icon_E>().UpdateRootKeys(s_consoleWarningKeys);
                    break;
                case LogType.Log:
                    Console.Q<Icon_E>().UpdateRootKeys(s_consoleLogKeys);
                    break;
                default:
                    break;
            }
        }
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

    public partial class BottomBar_E : Box_E
    {
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

            Console = new Button_E(QR<Button>("console"), "Square", StyleKeys.Bg("on"), StyleKeys.Bg("off"));
            Console.AddIconInFront(new Icon_E(), "Square1", s_consoleDefaultKeys);

            Settings = new Button_E(QR<Button>("settings"), "Square", StyleKeys.Bg("on"), StyleKeys.Bg("off"));
            Settings.AddIconInFront(new Icon_E(), "Square1", StyleKeys.Bg("settings"));
        }

        private BottomBar_E() :
            base("UI/UXML/Menus/bottomBar", "BottomBar", StyleKeys.Bg("dark"))
        { }
    }
}