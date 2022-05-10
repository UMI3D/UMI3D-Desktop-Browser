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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ToolboxPinnedWindow_E
    {
        protected ScrollView_E s_scrollView { get; set; } = null;

        public void AddRange(params View_E[] items)
            => s_scrollView.AddRange(items);
    }

    public partial class ToolboxPinnedWindow_E : ISingleUI
    {
        public static ToolboxPinnedWindow_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ToolboxPinnedWindow_E();
                return m_instance;
            }
        }

        private static ToolboxPinnedWindow_E m_instance;
    }

    public partial class ToolboxPinnedWindow_E : AbstractDraggableWindow_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            StyleKeys iconKeys = new StyleKeys(null, "paramettersWindow", "");
            SetWindowIcon(iconKeys);
            SetTopBar("Toolbox");
            SetCloseButton();

            s_scrollView = new ScrollView_E(QR<ScrollView>());
            s_scrollView.SetVDraggerContainer("draggerContainer", StyleKeys.DefaultBackground);
            s_scrollView.SetVDragger("dragger", StyleKeys.DefaultBackgroundAndBorder);
            s_scrollView.SetBackground("Background", StyleKeys.DefaultBackgroundAndBorder);

            m_bottomBox.style.display = DisplayStyle.None;

            Root.name = "toolParameterWindow";
        }

        private ToolboxPinnedWindow_E() :
            base("draggable")
        { }
    }
}
