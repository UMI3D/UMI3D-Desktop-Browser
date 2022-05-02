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
        public static ToolboxPinnedWindow_E Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ToolboxPinnedWindow_E();
                }
                return m_instance;
            }
        }

        private static string m_windowUXML = "UI/UXML/ToolboxWindow/pinnedToolboxWindow";
        private static string m_windowStyle = "UI/Style/ToolboxWindow/ToolboxWindow_window";
        private static StyleKeys m_windowKeys = new StyleKeys(null, "", null);
        private static ToolboxPinnedWindow_E m_instance;
    }

    public partial class ToolboxPinnedWindow_E : WindowWithScrollView_E
    {
        private ToolboxPinnedWindow_E() :
            base(m_windowUXML, m_windowStyle, m_windowKeys)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            StyleKeys iconKeys = new StyleKeys(null, "paramettersWindow", "");
            SetWindowIcon(m_iconStyle, iconKeys);

            StyleKeys windowNameKeys = new StyleKeys("", "", "");
            SetTopBar("Toolbox", m_windowNameStyle, windowNameKeys);

            
            StyleKeys closeButtonBGKeys = new StyleKeys(null, "", "");
            StyleKeys closeButtonIconKeys = new StyleKeys(null, "", null);
            SetCloseButton(m_closeButtonBGStyle, closeButtonBGKeys, m_closeButtonIconStyle, closeButtonIconKeys);

            //string svStyle = "UI/Style/ToolboxWindow/parameterWindow_scrollView";
            //StyleKeys svKeys = new StyleKeys(null, "", null);
            string dcStyle = "UI/Style/ToolboxWindow/ToolboxWindow_DraggerContainer";
            StyleKeys dcKeys = new StyleKeys(null, "", null);
            string dStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Dragger";
            StyleKeys dKeys = new StyleKeys(null, "", "");
            SetVerticalScrollView(null, null, dcStyle, dcKeys, dStyle, dKeys);

            Root.name = "toolParameterWindow";
        }
    }
}
