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
using BrowserDesktop.UI;
using BrowserDesktop.UserPreferences;
using DesktopBrowser.UI.CustomElement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    public partial class Label_E
    {
        
    }

    public partial class Label_E
    {
        private TextElement m_label;
    }

    public partial class Label_E
    {
        public Label_E(VisualElement root, string customStyleKey = null, string customStyleBackgroundKey = "") : base(root, customStyleKey, customStyleBackgroundKey)
        {
            //m_label.style
        }
    }

    public partial class Label_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_label = this.Root.Q<TextElement>();
        }
    }
}
