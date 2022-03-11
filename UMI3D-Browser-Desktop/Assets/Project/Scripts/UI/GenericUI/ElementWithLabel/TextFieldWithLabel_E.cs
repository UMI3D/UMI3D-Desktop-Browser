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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class TextFieldWithLabel_E
    {
        protected TextField m_textField { get; set; } = null;
    }

    public partial class TextFieldWithLabel_E
    {
        public TextFieldWithLabel_E(string visualResourcePath) :
            this(visualResourcePath, null, null)
        { }
        public TextFieldWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public virtual void SetTextField(string styleResourcePath, StyleKeys keys)
        {
            if (Element == null)
                Element = new TextField_E(m_textField, styleResourcePath, keys);
            else
                throw new System.NotImplementedException();
        }
    }

    public partial class TextFieldWithLabel_E : AbstractElementWithLabel_E<TextField_E>
    {
        public override TextField_E Element { get; protected set; }

        protected override void Initialize()
        {
            base.Initialize();
            m_textField = Root.Q<TextField>();
        }
    }
}

