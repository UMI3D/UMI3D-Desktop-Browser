/*
Copyright 2019 - 2022 Inetum

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

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct ColorCrossPosition : ICrossPosition<Color>
    {
        [SerializeField]
        private Color m_global;
        [SerializeField]
        private Color m_bottom;
        [SerializeField]
        private Color m_left;
        [SerializeField]
        private Color m_right;
        [SerializeField]
        private Color m_top;

        public Color Bottom => m_bottom;

        public Color Left => m_left;

        public Color Right => m_right;

        public Color Top => m_top;
    }

    [Serializable]
    public struct CustomStyleColorCrossPosition : ICustomStyleValue<CustomStyleSimpleKeyword, ColorCrossPosition>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private ColorCrossPosition m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public ColorCrossPosition Value { get => m_value; set => m_value = value; }
    }
}
