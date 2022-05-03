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
    public partial class ObjectMenuWindow_E
    {
        protected ScrollView_E s_scrollView { get; set; } = null;

        private static string s_windowUXML = "UI/UXML/ToolboxWindow/pinnedToolboxWindow";
        private static string s_windowStyle = "UI/Style/ToolboxWindow/ToolboxWindow_window";
        private static StyleKeys s_windowKeys = new StyleKeys(null, "", null);

        public void AddRange(params Visual_E[] items)
            => s_scrollView.AddRange(items);
    }

    public partial class ObjectMenuWindow_E : ISingleUI
    {
        public static ObjectMenuWindow_E Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new ObjectMenuWindow_E();
                return s_instance;
            }
        }

        private static ObjectMenuWindow_E s_instance;
    }

    public partial class ObjectMenuWindow_E : AbstractWindow_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            StyleKeys iconKeys = new StyleKeys(null, "objectMenu", "");
            SetWindowIcon(m_iconStyle, iconKeys, true);

            StyleKeys windowNameKeys = new StyleKeys("", "", "");
            SetTopBar("Object Menu", m_topBarStyle, windowNameKeys, true);

            SetCloseButton();
            StyleKeys closeButtonBGKeys = new StyleKeys(null, "", "");
            m_closeButton.UpdateRootStyleAndKeysAndManipulator(m_closeButtonBGStyle, closeButtonBGKeys);
            StyleKeys closeButtonIconKeys = new StyleKeys(null, "", null);
            var closeIcon = new Visual_E(m_closeButtonIconStyle, closeButtonIconKeys);
            m_closeButton.Add(closeIcon);
            LinkMouseBehaviourChanged(m_closeButton, closeIcon);
            m_closeButton.GetRootManipulator().ProcessDuringBubbleUp = true;

            s_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"));
            string dcStyle = "UI/Style/ToolboxWindow/ToolboxWindow_DraggerContainer";
            StyleKeys dcKeys = new StyleKeys(null, "", null);
            s_scrollView.SetVerticalDraggerContainerStyle(dcStyle, dcKeys);
            string dStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Dragger";
            StyleKeys dKeys = new StyleKeys(null, "", "");
            s_scrollView.SetVerticalDraggerStyle(dStyle, dKeys);

            Root.name = "objectMenuWindow";
        }

        private ObjectMenuWindow_E() :
            base(s_windowUXML, s_windowStyle, s_windowKeys)
        { }
    }
}
