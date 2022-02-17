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
    public partial class Button_E : IClickableElement
    {
        public Action OnClicked { get; set; } = () 
            => { Debug.Log("<color=green>TODO: </color>" + $"Button_E clicked not implemented"); };

        /// <summary>
        /// State of the button.
        /// </summary>
        public bool IsOn { get; protected set; } = false;

        public virtual void Toggle(bool value)
        {
            IsOn = value;
            m_currentKeys = (IsOn) ? m_onKeys : m_offKeys;
            UpdateVisualKeys(Root, m_currentKeys);
        }
    }

    public partial class Button_E
    {
        public string Text
        {
            get => m_button.text;
            set
            {
                if (value == m_button.text)
                    return;
                var (styleSO, _, _) = m_visualStyles[m_button];
                var newValue = m_styleApplicator.GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, value);
                m_button.text = newValue;
            }
        }

        protected Button m_button => (Button)Root;
        protected Action m_clicked { get; set; } = null;
        protected StyleKeys m_onKeys { get; set; } = null;
        protected StyleKeys m_offKeys { get; set; } = null;
        protected StyleKeys m_currentKeys { get; set; } = null;
    }

    public partial class Button_E
    {
        public Button_E(string styleResourcePath, StyleKeys keys) :
            this(new Button(), styleResourcePath, keys)
        { }
        public Button_E(Button button, string styleResourcePath, StyleKeys keys) :
            this(button, styleResourcePath, keys, null, true)
        { }
        public Button_E(string styleResourcePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            this(new Button(), styleResourcePath, onKeys, offKeys, isOn)
        { }
        public Button_E(Button button, string styleResourcePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            base(button, styleResourcePath, (isOn) ? onKeys : offKeys)
        {
            IsOn = isOn;
            m_onKeys = onKeys;
            m_offKeys = offKeys;
            m_currentKeys = (isOn) ? onKeys : offKeys;   
        }

        public void UpdatesStyle(StyleKeys newKeys)
            => UpdatesStyle(newKeys, null, true);
        public void UpdatesStyle(StyleKeys onKeys, StyleKeys offKeys, bool isOn)
        {
            m_onKeys = onKeys;
            m_offKeys = offKeys;
            m_currentKeys = (isOn) ? m_onKeys : m_offKeys;
            UpdateVisualKeys(m_button, m_currentKeys);
        }
    }

    public partial class Button_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_clicked = () => OnClicked?.Invoke();
            m_button.clicked += m_clicked;
        }

        public override void Reset()
        {
            base.Reset();
            m_button.clicked -= m_clicked;
            m_clicked = null;
            m_onKeys = null;
            m_offKeys = null;
            m_currentKeys = null;
    }
    }
}