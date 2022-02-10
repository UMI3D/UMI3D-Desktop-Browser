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
    public partial class ButtonWithLabel_E : IClickableElement
    {
        public Action OnClicked
        {
            get
            {
                return m_buttonE.OnClicked;
            }
            set
            {
                m_buttonE.OnClicked = value;
            }
        }

        public bool IsOn => m_buttonE.IsOn;

        public virtual void Toggle(bool value)
        {
            m_buttonE.Toggle(value);
        }
    }

    public partial class ButtonWithLabel_E
    {
        protected Button_E m_buttonE { get; set; } = null;

        protected Button m_button { get; set; } = null;
        protected Label m_label { get; set; } = null;
    }

    public partial class ButtonWithLabel_E
    {
        public ButtonWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath,
                styleResourcePath,
                keys)
        { }

        public virtual void SetButton(string styleResourcePath, StyleKeys keys, Action buttonClicked)
            => SetButton(styleResourcePath, keys, null, true, buttonClicked);

        public virtual void SetButton(string styleResourcePath, StyleKeys onKey, StyleKeys offKey, bool isOn, Action buttonClicked)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || onKey == null) 
                throw new NullReferenceException($"CustomStyle_SO or StyleKeys is null.");
            m_buttonE = new Button_E(m_button, styleResourcePath, onKey, offKey, isOn)
            {
                OnClicked = buttonClicked
            };
        }

        public virtual void SetLabel(string styleResourcePath, StyleKeys keys)
        {
            CustomStyle_SO style_SO = GetStyleSO(styleResourcePath);
            if (style_SO == null || keys == null) throw new NullReferenceException($"CustomStyle_SO or iconStyleKeys is null.");
            AddVisualStyle(m_label, style_SO, keys);
        }
    }

    public partial class ButtonWithLabel_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_label = Root.Q<Label>();
            m_button = Root.Q<Button>();
        }
    }
}