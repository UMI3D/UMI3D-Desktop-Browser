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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ToolboxWindow_E
    {
        public static event Action UnpinnedPressed;

        protected ScrollView_E m_scrollView { get; set; } = null;
        
        public static void OnUnpinnedPressed()
            => UnpinnedPressed?.Invoke();

        public void AddRange(params View_E[] items)
            => m_scrollView.AddRange(items);
    }

    public partial class ToolboxWindow_E : ISingleUI
    {
        public static ToolboxWindow_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ToolboxWindow_E();
                return m_instance;
            }
        }

        private static ToolboxWindow_E m_instance;
    }

    public partial class ToolboxWindow_E : AbstractDraggableWindow_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            SetWindowIcon(new StyleKeys(null, "toolboxesWindow", ""));
            SetTopBar("Toolbox");
            SetCloseButton();

            m_scrollView = new ScrollView_E(QR<ScrollView>());
            m_scrollView.SetVDraggerContainer("DraggerContainer", StyleKeys.DefaultBackground);
            m_scrollView.SetVDragger("Dragger", StyleKeys.DefaultBackgroundAndBorder);

            Button_E unpinned = new Button_E("LargeRectangle", StyleKeys.DefaultBackground);
            unpinned.Clicked += OnUnpinnedPressed;
            unpinned.InsertRootTo(QR("bottomBar"));

            Root.name = "toolboxWindow";
        }

        private ToolboxWindow_E() :
            base("draggable")
        { }
    }
}