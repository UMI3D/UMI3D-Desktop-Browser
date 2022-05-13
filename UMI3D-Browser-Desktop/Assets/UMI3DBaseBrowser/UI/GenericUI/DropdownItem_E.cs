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
        protected Icon_E m_checkmark { get; set; } = null;
        protected Label_E m_label { get; set; } = null;
    
        public void SetCheckmark(string partialStylePath, StyleKeys keys)
            => m_checkmark.UpdateRootStyleAndKeysAndManipulator(partialStylePath, keys);
        public void SetLabel(string partialStylePath, StyleKeys keys)
            => m_label.UpdateRootStyleAndKeysAndManipulator(partialStylePath, keys);
    }

    public partial class DropdownItem_E : View_E
    {
        public DropdownItem_E(VisualElement row, string styleResourcePath, StyleKeys keys) :
            base(row, styleResourcePath, keys)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            m_checkmark = new Icon_E();
            //m_checkmark.Root.name = "checkmark";
            //m_checkmark.Root.Insert(0, Root);
            m_label = new Label_E(QR<Label>());
        }
    }
}