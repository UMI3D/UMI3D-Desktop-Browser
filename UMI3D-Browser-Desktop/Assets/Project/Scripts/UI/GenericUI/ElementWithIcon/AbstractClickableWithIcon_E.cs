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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public abstract partial class AbstractClickableWithIcon_E<B> : IClickableElement where B : IClickableElement
    {
        public Action OnClicked
        {
            get => Element.OnClicked;
            set => Element.OnClicked = value;
        }

        public bool IsOn => Element.IsOn;

        public virtual void Toggle(bool value)
            => Element.Toggle(value);
    }

    public abstract partial class AbstractClickableWithIcon_E<B>
    {
        protected Button m_button { get; set; } = null;
    }

    public abstract partial class AbstractClickableWithIcon_E<B>
    {
        public AbstractClickableWithIcon_E(VisualElement visual, string styleResourcePath, StyleKeys keys) :
            base(visual, styleResourcePath, keys)
        { }
        public AbstractClickableWithIcon_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public abstract void SetButton(string styleResourcePath, StyleKeys keys, Action buttonClicked = null);
        public abstract void SetButton(string styleResourcePath, StyleKeys onKey, StyleKeys offKey, bool isOn, Action buttonClicked = null);
    }

    public abstract partial class AbstractClickableWithIcon_E<B> : AbstractElementWithIcon_E<B>
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_button = Root.Q<Button>();
        }
    }
}

