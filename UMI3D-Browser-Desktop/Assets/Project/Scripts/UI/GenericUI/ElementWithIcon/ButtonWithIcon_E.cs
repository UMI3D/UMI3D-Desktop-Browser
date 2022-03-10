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
    public partial class ButtonWithIcon_E
    {
        protected static VisualElement m_defaultButtonWithIcon
        {
            get
            {
                Button button = new Button();
                VisualElement icon = new VisualElement();
                icon.name = "icon";
                button.Add(icon);
                return button;
            }
        }
    }

    public partial class ButtonWithIcon_E
    {
        public ButtonWithIcon_E() :
            this(null, null)
        { }
        public ButtonWithIcon_E(string styleResourcePath, StyleKeys keys) :
            this(m_defaultButtonWithIcon, styleResourcePath, keys)
        { }
        public ButtonWithIcon_E(VisualElement visual) :
            this(visual, null, null)
        { }
        public ButtonWithIcon_E(VisualElement visual, string styleResourcePath, StyleKeys keys) :
            base(visual, styleResourcePath, keys)
        { }
        public ButtonWithIcon_E(string visualResourcePath) :
            this (visualResourcePath, null, null)
        { }
        public ButtonWithIcon_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public override void SetButton(string styleResourcePath, StyleKeys keys, Action buttonClicked = null)
            => SetButton(styleResourcePath, keys, null, true, buttonClicked);
        public override void SetButton(string styleResourcePath, StyleKeys onKey, StyleKeys offKey, bool isOn, Action buttonClicked = null)
        {
            Element = new Button_E(m_button, styleResourcePath, onKey, offKey, isOn)
            {
                OnClicked = buttonClicked
            };
        }

        public void ApplyButtonStyleWithIcon()
        {
            var manipulatorIcon = Icon.GetVisualManipulator(m_icon);
            var manipulatorButton = Element.GetVisualManipulator(m_button);
            manipulatorIcon.OnMouseBehaviourChanged += manipulatorButton.ApplyStyle;
        }
    }

    public partial class ButtonWithIcon_E : AbstractClickableWithIcon_E<Button_E>
    {

    }
}