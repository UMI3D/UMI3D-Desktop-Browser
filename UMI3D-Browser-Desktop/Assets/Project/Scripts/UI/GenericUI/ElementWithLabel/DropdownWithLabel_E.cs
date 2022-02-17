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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class DropdownWithLabel_E
    {
        public override Dropdown_E Element { get; protected set; } = null;
    }

    public partial class DropdownWithLabel_E
    {
        public DropdownWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath,
                styleResourcePath,
                keys)
        { }

        public void SetMenuStyle(string styleResourcePath, StyleKeys keys)
            => Element.SetMenuStyle(styleResourcePath, keys);
        public void SetMenuItemCheckmark(string styleResourcePath, StyleKeys keys)
            => Element.SetMenuCheckmark(styleResourcePath, keys);
        public void SetMenuItemLabel(string styleResourcePath, StyleKeys keys)
            => Element.SetMenuLabel(styleResourcePath, keys);
    }

    public partial class DropdownWithLabel_E : ClickableWithLabel_E<Dropdown_E>
    {
        public override void SetButton(string styleResourcePath, StyleKeys keys, Action buttonClicked)
        {
            Element = new Dropdown_E(m_button, styleResourcePath, keys);
        }
    }
}