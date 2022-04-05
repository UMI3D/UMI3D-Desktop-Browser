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
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3dDesktopBrowser.ui
{
    public partial class GameMenu
    {
        #region Init Menus

        private void InitMenus()
        {
            InitMenuBar();
            InitToolboxWindow();
            InitToolboxPinnedWindow();
            InitShortcut();
            //Todo Add footer.
        }

        private void InitMenuBar()
        {
            MenuBar_E.Instance.InsertRootTo(m_mainView);

            MenuBar_E.Instance.ToolboxButton.OnClicked = () =>
            {
                m_windowToolboxesDM.Display(true);
            };

            m_viewport.Add(MenuBar_E.Instance.SubMenuLayout);

            MenuBar_E.Instance.OnPinnedUnpinned += (value, menu) =>
            {
                if (value)
                    m_pinnedToolboxesDM.menu.Add(menu);
                else
                    m_pinnedToolboxesDM.menu.Remove(menu);
            };
        }

        private void InitToolboxWindow()
        {
            ToolboxWindow_E.Instance.InsertRootTo(m_viewport);
            ToolboxWindow_E.Instance.OnCloseButtonPressed += () => m_windowToolboxesDM.Collapse(true);
        }

        private void InitToolboxPinnedWindow()
        {
            ToolboxPinnedWindow_E.Instance.InsertRootTo(m_viewport);
            ToolboxPinnedWindow_E.Instance.OnCloseButtonPressed += () => m_pinnedToolboxesDM.Collapse(true);
        }

        private void InitShortcut()
        {
            Shortcutbox_E.Instance.InsertRootTo(m_leftSideMenuDownUp);

            if (m_showShortcutOnStart)
                Shortcutbox_E.ShouldDisplay = true;
            else
                Shortcutbox_E.ShouldHide = true;
        }

        #endregion

        #region Input Menus

        private void InputMenus()
        {
            InputShortcut();
        }

        private void InputShortcut()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Shortcutbox_E.Instance.DisplayOrHide();
        }

        #endregion
    }
}
