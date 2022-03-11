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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ButtonWithLabel_E
    {
        public override Button_E Element { get; protected set; } = null;
    }

    public partial class ButtonWithLabel_E
    {
        public ButtonWithLabel_E(string visualResourcePath) :
            this(visualResourcePath, null, null)
        { }
        public ButtonWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public override void SetButton(string styleResourcePath, StyleKeys keys, Action buttonClicked)
            => SetButton(styleResourcePath, keys, null, true, buttonClicked);
        public virtual void SetButton(string styleResourcePath, StyleKeys onKey, StyleKeys offKey, bool isOn, Action buttonClicked)
        {
            Element = new Button_E(m_button, styleResourcePath, onKey, offKey, isOn)
            {
                OnClicked = buttonClicked
            };
        }
    }

    public partial class ButtonWithLabel_E : ClickableWithLabel_E<Button_E>
    { }
}