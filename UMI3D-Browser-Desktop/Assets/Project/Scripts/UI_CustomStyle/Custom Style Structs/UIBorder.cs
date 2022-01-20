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
    public struct UIBorder : IUIBorder
    {
        [Header("Color")]
        [SerializeField]
        private CustomStyleCrossPosition<CustomStyleColorKeyword, Color> m_color;
        [Header("Width")]
        [SerializeField]
        private CustomStyleCrossPosition<CustomStyleSizeKeyword, float> m_width;
        [Header("Radius")]
        [SerializeField]
        private CustomStyleSquarePosition<CustomStyleSimpleKeyword, float> m_radius;

        public CustomStyleCrossPosition<CustomStyleColorKeyword, Color> Color => m_color;
        public CustomStyleCrossPosition<CustomStyleSizeKeyword, float> Width => m_width;
        public CustomStyleSquarePosition<CustomStyleSimpleKeyword, float> Radius => m_radius;
    }
}
