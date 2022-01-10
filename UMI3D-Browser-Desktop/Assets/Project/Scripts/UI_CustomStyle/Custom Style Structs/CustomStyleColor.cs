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
    public ref struct ColorRef
    {
        public Color Value { get; set; }
    }

    [Serializable]
    public struct CustomStyleColor : ICustomStyleValue<Color>
    {
        [SerializeField]
        private CustomStyleKeyword m_keyword;
        [SerializeField]
        private Color m_value;

        public CustomStyleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public Color Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyle[Keyword=[{m_keyword}], Value=[{m_value}]";
        }
    }
}
