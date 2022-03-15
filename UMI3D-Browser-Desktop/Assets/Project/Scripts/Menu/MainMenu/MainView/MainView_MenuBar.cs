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
        private void InitMenus()
        {
            InitMenuBar();
            

            ToolboxWindow_E
            .Instance
            .InsertRootTo(m_viewport);

            ToolboxWindow_E
                .Instance
                .Hide();

            //Todo Add footer.
        }

        private void InitMenuBar()
        {
            MenuBar_E.Instance.InsertRootTo(m_mainView);

            MenuBar_E.Instance.ToolboxButton.OnClicked = () =>
            {
                ToolboxWindow_E.Instance.Display();
                m_windowToolboxesDM.Hide(true);
                m_windowToolboxesDM.Display(true);
            };

            m_viewport.Insert(0, MenuBar_E.Instance.SubMenuLayout);

            MenuBar_E.Instance.OnPinned += Pin;
        }

        private void Pin(Menu menu)
        {
            if (menu == null) Debug.Log($"menu null");
            menu.OnDestroy.RemoveAllListeners();
            m_pinnedToolboxesDM.menu.Add(menu);
            m_pinnedToolboxesDM.Display(true);
        }
    }
}
