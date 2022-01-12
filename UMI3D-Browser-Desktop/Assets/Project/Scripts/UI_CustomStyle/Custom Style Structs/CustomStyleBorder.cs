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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Browser.UICustomStyle
{
    [Serializable]
    public struct CustomStyleBorderColor : ICustomStyleValue<CustomStyleSimpleKeyword, BorderColor>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private BorderColor m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public BorderColor Value { get => m_value; set => m_value = value; }
    }

    [Serializable]
    public struct CustomStyleBorderWidth : ICustomStyleValue<CustomStyleSimpleKeyword, BorderWidth>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private BorderWidth m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public BorderWidth Value { get => m_value; set => m_value = value; }
    }

    [Serializable]
    public struct CustomStyleBorderRadius : ICustomStyleValue<CustomStyleSimpleKeyword, BorderRadius>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private BorderRadius m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public BorderRadius Value { get => m_value; set => m_value = value; }
    }
}