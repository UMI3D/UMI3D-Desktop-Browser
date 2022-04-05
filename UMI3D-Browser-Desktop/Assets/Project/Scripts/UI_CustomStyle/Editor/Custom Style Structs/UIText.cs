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
using UnityEngine;
//using UnityEngine.UIElements;

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct UITextFormat
    {
        [SerializeField]
        private CustomStyleValue<CustomStyleSizeKeyword, int> m_fontSize;
        [SerializeField]
        private CustomStyleValue<CustomStyleSizeKeyword, float> m_letterSpacing;
        [SerializeField]
        private CustomStyleValue<CustomStyleSizeKeyword, float> m_wordSpacing;
        [SerializeField]
        private CustomStyleValue<CustomStyleSizeKeyword, float> m_paragraphSpacing;
        [SerializeField]
        private CustomStyleValue<CustomStyleSimpleKeyword, int> m_numberOfVisibleCharacter;
        [SerializeField]
        private TextAnchor m_textAlign;

        public CustomStyleValue<CustomStyleSizeKeyword, int> FontSize => m_fontSize;
        public CustomStyleValue<CustomStyleSizeKeyword, float> LetterSpacing => m_letterSpacing;
        public CustomStyleValue<CustomStyleSizeKeyword, float> WordSpacing => m_wordSpacing;
        public CustomStyleValue<CustomStyleSizeKeyword, float> ParagraphSpacing => m_paragraphSpacing;
        public CustomStyleValue<CustomStyleSimpleKeyword, int> NumberOfVisibleCharacter => m_numberOfVisibleCharacter;
        public TextAnchor TextAlign => m_textAlign;
    }

    [Serializable]
    public struct UITextStyle : ICustomisableByMouseBehaviour<CustomStyleTextStyle>
    {
        [SerializeField]
        private string m_key;
        [Header("Default")]
        [SerializeField]
        private CustomStyleTextStyle m_default;
        [Header("Mouse Over")]
        [SerializeField]
        private CustomStyleTextStyle m_mouseOver;
        [Header("Mouse Pressed")]
        [SerializeField]
        private CustomStyleTextStyle m_mousePressed;

        public string Key => m_key;
        public CustomStyleTextStyle Default => m_default;
        public CustomStyleTextStyle MouseOver => m_mouseOver;
        public CustomStyleTextStyle MousePressed => m_mousePressed;
    }
}
