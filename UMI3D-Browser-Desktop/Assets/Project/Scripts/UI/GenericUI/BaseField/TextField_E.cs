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

namespace umi3dDesktopBrowser.uI.viewController
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

        protected void ApplyTextFormat(CustomStyle_SO style_SO)
        {
            if (style_SO == null) return;
            UITextFormat textFormat = style_SO.TextFormat;
            m_uIElementStyleApplicator.AppliesFontSize(m_textInput.style, textFormat.FontSize);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.LetterSpacing,
                m_textInput.style.letterSpacing,
                (length) => m_textInput.style.letterSpacing = length);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.WordSpacing,
                m_textInput.style.wordSpacing,
                (length) => m_textInput.style.wordSpacing = length);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.ParagraphSpacing,
                m_textInput.style.unityParagraphSpacing,
                (length) => m_textInput.style.unityParagraphSpacing = length);

            switch (textFormat.NumberOfVisibleCharacter.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    m_textField.maxLength = -1;
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    m_textField.maxLength = textFormat.NumberOfVisibleCharacter.Value;
                    break;
            }
        }
    }

    public partial class TextField_E : AbstractBaseField_E<string>
    {
        protected override void ApplyFormat(CustomStyle_SO style_SO, StyleKeys keys, VisualElement visual)
        {
            ApplySize(style_SO, visual.style);
            ApplyMarginAndPadding(style_SO, visual.style);
            ApplyTextFormat(style_SO);
        }
        protected override void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (styleSO == null || keys == null) return;
            if (keys.TextStyleKey != null) ApplyTextStyle(styleSO, keys.TextStyleKey, m_textInput.style, mouseBehaviour);
            if (keys.BackgroundStyleKey != null) ApplyBackgroundStyle(styleSO, keys.BackgroundStyleKey, style, mouseBehaviour);
            if (keys.BorderStyleKey != null) ApplyBorderStyle(styleSO, keys.BorderStyleKey, style, mouseBehaviour);
        }

        protected override void OnValueChandedEvent(ChangeEvent<string> e)
        {
            base.OnValueChandedEvent(e);
            UpdateVisualText(m_field, e.newValue);
        }

        protected override void Initialize()
        {
            base.Initialize();
            m_textInput = Root.Q("unity-text-input");
            //if (m_textInput == null) throw new Exception("null textInput");
        }
    }
}

