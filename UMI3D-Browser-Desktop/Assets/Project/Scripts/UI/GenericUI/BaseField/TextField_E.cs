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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class TextField_E
    {
        protected TextField m_textField => (TextField)Root;
        protected VisualElement m_textInput { get; set; } = null;
    }

    public partial class TextField_E
    {
        public TextField_E() :
            this(null, null)
        { }
        public TextField_E(string styleResourcePath, StyleKeys keys) :
            this(new TextField(), styleResourcePath, keys)
        { }
        public TextField_E(TextField textField, string styleResourcePath, StyleKeys keys, bool multiline = false, bool isDelayed = false, bool isPasswordField = false, char maskChar = '*') :
            base(textField, styleResourcePath, keys)
        {
            SetTextField(multiline, isDelayed, isPasswordField, maskChar);
        }

        public void SetTextField(bool multiline = false, bool isDelayed = false, bool isPasswordField = false, char maskChar = '*')
        {
            m_textField.multiline = multiline;
            m_textField.isDelayed = isDelayed;
            m_textField.isPasswordField = isPasswordField;
            m_textField.maskChar = maskChar;
        }

        public void SetTextInputStyle(string styleResourcePath, StyleKeys keys)
        {
            AddVisualStyle(m_textInput, styleResourcePath, keys);
        }

        protected void ApplyTextFieldFormat(string newValue)
        {
            if (!m_visualMap.ContainsKey(m_textInput))
            {
                m_textField.value = newValue;
                return;
            }
            var styleSO = GetVisualStyle(m_textInput);
            if (styleSO != null)
                m_textField.value = GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, newValue);
        }
    }

    public partial class TextField_E : AbstractBaseField_E<string>
    {
        //protected override void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        //{
        //    if (styleSO == null || keys == null) return;
        //    if (keys.TextStyleKey != null && m_textInput != null) ApplyTextStyle(styleSO, keys.TextStyleKey, m_textInput.style, mouseBehaviour);
        //    if (keys.BackgroundStyleKey != null) ApplyBackgroundStyle(styleSO, keys.BackgroundStyleKey, style, mouseBehaviour);
        //    if (keys.BorderStyleKey != null) ApplyBorderStyle(styleSO, keys.BorderStyleKey, style, mouseBehaviour);
        //}

        public override string value
        { 
            get => base.value; 
            set
            {
                if (value == m_textField.value)
                    return;
                var previousValue = m_textField.value;
                ApplyTextFieldFormat(value);
                OnValueChanged(previousValue, m_textField.value);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            m_textInput = Root.Q("unity-text-input");
            ValueChanged += (_, newV) => ApplyTextFieldFormat(newV);
        }
    }
}

