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
using inetum.unityUtils;
using umi3d.cdk;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui
{
    public partial class MainView
    {
        [SerializeField]
        private UIDocument m_uiDoc;

        [Header("Menu Displayer")]
        [SerializeField]
        [Tooltip("window Toolboxes Displayer manager.")]
        private MenuDisplayManager m_windowToolboxesDM;
        [SerializeField]
        [Tooltip("Pinned Toolboxes Displayer manager.")]
        private MenuDisplayManager m_pinnedToolboxesDM;
        [SerializeField]
        [Tooltip("Object Menu Displayer manager.")]
        private MenuDisplayManager m_objectMenuDM;

        [Header("Menus")]
        [SerializeField]
        private bool m_showMenuBarOnStart = false;
        [SerializeField]
        private bool m_showSettignsOnStart = false;
        [SerializeField]
        private bool m_showShortcutOnStart = false;
        [SerializeField]
        private bool m_showConsoleOnStart = false;

        private VisualElement m_root => m_uiDoc.rootVisualElement;
        private VisualElement m_mainView { get; set; } = null;
        private VisualElement m_viewport { get; set; } = null;
        private VisualElement m_leftSideMenu { get; set; } = null;
        private VisualElement m_leftSideMenuDownUp { get; set; } = null;

        
    }

    public partial class MainView : SingleBehaviour<MainView>
    {
        void Start()
        {
            BindUI();
            InitMenus();
            Display(false);
            DisplayMenus();

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                Display(true);
            });
        }

        private void BindUI()
        {
            m_mainView = m_root.Q("game-menu-container");
            m_viewport = m_root.Q("viewport");
            m_leftSideMenu = m_root.Q("left-side-menu");
            m_leftSideMenuDownUp = m_root.Q("leftSideDownUp");
        }

        private void Update()
        {
            InputMenus();
            UpdateMenus();
        }

        /// <summary>
        /// Displays Main view if true, else hides.
        /// </summary>
        /// <param name="val"></param>
        public void Display(bool val)
        {
            m_mainView.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
