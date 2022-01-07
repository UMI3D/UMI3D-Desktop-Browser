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
using UnityEngine.UIElements;

namespace Browser.UICustomStyle
{
    [Serializable]
    public struct UIText : IUIText
    {
        //[Header("Font")]
        [SerializeField]
        private Font m_font;
        //[SerializeField]
        //private FontDefinition m_unityFontDefinition;
        [SerializeField]
        private FontStyle m_fontStyleAndWeight;
        [SerializeField]
        private float m_fontSize;
        [SerializeField]
        private Color m_color;

        [Space()]
        [SerializeField]
        private float m_letterSpacing;

        [SerializeField]
        private TextOverflow m_textOveflow;



        [SerializeField]
        private float m_unityParagraphSpacing;
        [SerializeField]
        private TextAnchor m_unityTextAlign;
        [SerializeField]
        private TextOverflowPosition m_unityTextOverflowPosition;
        [SerializeField]
        private Color m_unityTextOutlineColor;
        [SerializeField]
        private float m_unityTextOutlineWidth;
        [SerializeField]
        private WhiteSpace m_WhiteSpace;
        [SerializeField]
        private float m_wordSpacing;


        public Color Color => throw new NotImplementedException();

        public float LetterSpacing => throw new NotImplementedException();

        public float FontSize => throw new NotImplementedException();

        public TextOverflow TextOverflow => throw new NotImplementedException();

        public Font UnityFont => throw new NotImplementedException();

        public FontDefinition UnityFontDefinition => throw new NotImplementedException();

        public FontStyle UnityFontStyleAndWeight => throw new NotImplementedException();

        public float UnityParagraphSpacing => throw new NotImplementedException();

        public TextAnchor UnityTextAlign => throw new NotImplementedException();

        public TextOverflowPosition UnityTextOverflowPosition => throw new NotImplementedException();

        public Color UnityTextOutlineColor => throw new NotImplementedException();

        public float UnityTextOutlineWidth => throw new NotImplementedException();

        public WhiteSpace WhiteSpace => throw new NotImplementedException();

        public float WordSpacing => throw new NotImplementedException();
    }
}
