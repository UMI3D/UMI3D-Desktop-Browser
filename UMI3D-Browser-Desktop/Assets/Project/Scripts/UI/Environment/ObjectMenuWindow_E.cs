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
        public static ObjectMenuWindow_E Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ObjectMenuWindow_E();
                }
                return m_instance;
            }
        }

        private static ObjectMenuWindow_E m_instance;
        private static string m_windowUXML = "UI/UXML/ToolboxWindow/toolboxWindow";
        private static string m_windowStyle = "UI/Style/ToolboxWindow/ToolboxWindow_window";
        private static StyleKeys m_windowKeys = new StyleKeys(null, "", null);
    }

    public partial class ObjectMenuWindow_E
    {
        private ObjectMenuWindow_E() :
            base(m_windowUXML, m_windowStyle, m_windowKeys)
        { }
    }

    public partial class ObjectMenuWindow_E : WindowWithScrollView_E
    {

    }
}
