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
using BrowserDesktop;
using System;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3dDesktopBrowser.ui
{
    public partial class MainView
    {
        private DateTime m_startOfSession = new DateTime();

        #region Init Menus

        private void InitMenus()
        {
            InitMenuBar();
            InitSettings();
            InitToolboxesWindows();
            InitObjectMenuWindow();
            InitShortcut();
            InitConsole();
            InitNotification2D();
            InitBottomBar();
            InitEmoteWindow();

            UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener( ResetMenus);
        }

        private void InitMenuBar()
        {
            MenuBar_E.Instance.InsertRootTo(m_mainView);

            MenuBar_E.Instance.ToolboxButton.Clicked += () =>
            {
                m_windowToolboxesDM.Display(true);
            };

            m_viewport.Add(MenuBar_E.Instance.SubMenuLayout);

            MenuBar_E.Instance.PinnedUnpinned += (value, menu) =>
            {
                if (value)
                    m_pinnedToolboxesDM.menu.Add(menu);
                else
                    m_pinnedToolboxesDM.menu.Remove(menu);
            };
        }

        private void InitSettings()
        {
            Settingbox_E.Instance.InsertRootTo(m_viewport);
            Settingbox_E.Instance.UpdateTopBarName($"option                       {BrowserVersion.Version}");
        }

        private void InitEmoteWindow()
        {
            EmoteWindow_E.Instance.InsertRootTo(m_viewport);
        }

        private void InitToolboxesWindows()
        {
            ToolboxWindow_E.Instance.InsertRootTo(m_viewport);
            ToolboxWindow_E.Instance.CloseButtonPressed += () => m_windowToolboxesDM.Collapse(true);
            m_windowToolboxesDM.menu.onContentChange.AddListener(() 
                => MenuBar_E.AreThereToolboxes = m_windowToolboxesDM.menu.Count > 0);

            ToolboxPinnedWindow_E.Instance.InsertRootTo(m_viewport);
            ToolboxPinnedWindow_E.Instance.CloseButtonPressed += () => m_pinnedToolboxesDM.Collapse(true);
        }

        private void InitObjectMenuWindow()
        {
            ObjectMenuWindow_E.Instance.InsertRootTo(m_viewport);
            ObjectMenuWindow_E.Instance.CloseButtonPressed += () => m_pinnedToolboxesDM.Collapse(true);
        }

        private void InitShortcut()
        {
            Shortcutbox_E.Instance.InsertRootTo(m_leftSideMenuDownUp);
        }

        private void InitConsole()
        {
            Console_E.Instance.InsertRootTo(m_viewport);
            Console_E.Instance.UpdateTopBarName(BrowserVersion.Version);
        }

        private void InitNotification2D()
        {
            umi3d.baseBrowser.ui.viewController.Notificationbox2D_E.Instance.InsertRootTo(m_viewport);
            umi3d.baseBrowser.ui.viewController.Notificationbox2D_E.Instance.MaxNotification = () => m_maxNotification2DDisplayed;
            var root = umi3d.baseBrowser.ui.viewController.Notificationbox2D_E.Instance.Root;
            root.style.right = 0f;
            root.style.top = 0f;
            root.style.bottom = 0f;
            umi3d.baseBrowser.ui.viewController.Notificationbox2D_E.Instance.HighPriorityBox.style.maxHeight = new UnityEngine.UIElements.Length(40, UnityEngine.UIElements.LengthUnit.Percent);
        }

        private void InitBottomBar()
        {
            BottomBar_E.Instance.InsertRootTo(m_mainView);

            MenuBar_E.Instance.DisplayedOrHidden += BottomBar_E.Instance.OpenCloseMenuBar;
            BottomBar_E.Instance.Console.Clicked += Console_E.Instance.ToogleVisibility;
            if (!m_DisplayConsol) BottomBar_E.Instance.Console.Hide();
            BottomBar_E.Instance.Settings.Clicked += Settingbox_E.Instance.ToogleVisibility;
            BottomBar_E.Instance.Emotes.Clicked += EmoteWindow_E.Instance.ToogleVisibility;
            BottomBar_E.Instance.ReportBug.Clicked += BugReporter.Instance.DisplayPopUp;
            Shortcutbox_E.Instance.DisplayedOrHidden += BottomBar_E.Instance.OpenCloseShortcut;
            Console_E.Instance.DisplayedOrHidden += BottomBar_E.Instance.Console.Toggle;
            Console_E.Instance.NewLogAdded += BottomBar_E.Instance.UpdateConsole;
            Settingbox_E.Instance.DisplayedOrHidden += BottomBar_E.Instance.Settings.Toggle;
            EmoteWindow_E.Instance.DisplayedOrHidden += BottomBar_E.Instance.Emotes.Toggle;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => m_startOfSession = DateTime.Now);
            UMI3DCollaborationEnvironmentLoader.OnUpdateUserList += () => {
                int usersCount = UMI3DCollaborationEnvironmentLoader.Instance.UserList.Count;
                BottomBar_E.Instance.ParticipantCount.value = (usersCount < 2) ? $"{usersCount} participant" : $"{usersCount} participants";
            };
        }

        private void ResetBottomBar()
        {
            MenuBar_E.Instance.DisplayedOrHidden -= BottomBar_E.Instance.OpenCloseMenuBar;
            BottomBar_E.Instance.Console.Clicked -= Console_E.Instance.ToogleVisibility;
            BottomBar_E.Instance.Settings.Clicked -= Settingbox_E.Instance.ToogleVisibility;
            BottomBar_E.Instance.ReportBug.Clicked -= BugReporter.Instance.DisplayPopUp;
            BottomBar_E.Instance.Emotes.Clicked -= EmoteWindow_E.Instance.ToogleVisibility;
            Shortcutbox_E.Instance.DisplayedOrHidden -= BottomBar_E.Instance.OpenCloseShortcut;
            Console_E.Instance.DisplayedOrHidden -= BottomBar_E.Instance.Console.Toggle;
            Console_E.Instance.NewLogAdded -= BottomBar_E.Instance.UpdateConsole;
            Settingbox_E.Instance.DisplayedOrHidden -= BottomBar_E.Instance.Settings.Toggle;
            EmoteWindow_E.Instance.DisplayedOrHidden -= BottomBar_E.Instance.Emotes.Toggle;
        }

        #endregion

        private void ResetMenus()
        {
            ResetBottomBar();
            MenuBar_E.DestroySingleton();
            BottomBar_E.DestroySingleton();
            Settingbox_E.DestroySingleton();
            Shortcutbox_E.DestroySingleton();
            EmoteWindow_E.DestroySingleton();
            Console_E.DestroySingleton();
            ToolboxWindow_E.DestroySingleton();
            ToolboxPinnedWindow_E.DestroySingleton();
            ObjectMenuWindow_E.DestroySingleton();
            m_pinnedToolboxesDM.menu.RemoveAll();
        }

        private void DisplayMenus()
        {
            if (m_showMenuBarOnStart) MenuBar_E.Instance.Display();
            else MenuBar_E.Instance.Hide();

            if (m_showSettignsOnStart) Settingbox_E.Instance.Display();
            else Settingbox_E.Instance.Hide();

            if (m_showShortcutOnStart) Shortcutbox_E.Instance.Display();
            else Shortcutbox_E.Instance.Hide();

            if (m_showConsoleOnStart) Console_E.Instance.Display();
            else Console_E.Instance.Hide();

            if (m_showEmotesOnStart) EmoteWindow_E.Instance.Display();
            else EmoteWindow_E.Instance.Hide();
        }

        #region Input Menus (To be move in the DesktopController)

        private void InputMenus()
        {
            InputShortcut();
        }

        private void InputShortcut()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Shortcutbox_E.Instance.ToogleVisibility();
        }

        #endregion

        #region Update Menus

        private void UpdateMenus()
        {
            UpdateBottomBar();
        }

        private void UpdateBottomBar()
        {
            var time = DateTime.Now - m_startOfSession;
            BottomBar_E.Instance.Timer.value = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");
        }

        #endregion
    }
}
