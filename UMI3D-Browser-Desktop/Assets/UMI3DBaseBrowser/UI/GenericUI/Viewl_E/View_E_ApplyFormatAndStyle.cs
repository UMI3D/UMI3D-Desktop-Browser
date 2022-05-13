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
using BrowserDesktop.preferences;
using System;
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class View_E
    {
        protected GlobalPreferences_SO m_globalPref;

        public virtual void ApplyAllFormatAndStyle()
        {
            foreach (VisualElement visual in m_visualMap.Keys)
            {
                var manipulator = m_visualMap[visual];
                manipulator.ApplyFormat();
                manipulator.ApplyStyle();
            }
        }

        protected void AppliesLength(CustomStyleSizeKeyword lengthKeyword, Action defaultAction, Action resizableAction, Action unresizableAction)
        {
            switch (lengthKeyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    break;
                case CustomStyleSizeKeyword.Default:
                    defaultAction();
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    resizableAction();
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    unresizableAction();
                    break;
            }
        }
    }
}

