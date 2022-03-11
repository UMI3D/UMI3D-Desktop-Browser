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
using System.Collections.Generic;
using umi3DBrowser.UICustomStyle;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class DropdownItem_E
    {
        protected VisualElement m_checkmark { get; set; } = null;
        protected Label m_label { get; set; } = null;
    }

    public partial class DropdownItem_E
    {
        public DropdownItem_E(VisualElement row, string styleResourcePath, StyleKeys keys) :
            base(row, styleResourcePath, keys)
        { }

        public void SetCheckmark(string styleResourcePath, StyleKeys keys)
        {
            AddVisualStyle(m_checkmark, styleResourcePath, keys);
        }
        public void SetLabel(string styleResourcePath, StyleKeys keys)
        {
            AddVisualStyle(m_label, styleResourcePath, keys);
        }
    }

    public partial class DropdownItem_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_checkmark = Root.Q();
            m_label = Root.Q<Label>();
        }
    }
}