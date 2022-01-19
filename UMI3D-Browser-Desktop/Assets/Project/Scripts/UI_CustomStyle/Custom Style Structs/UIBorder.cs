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
    public struct BorderRadius : IBorderRadius
    {
        [SerializeField]
        private float m_global;
        [SerializeField]
        private float m_bottomLeft;
        [SerializeField]
        private float m_bottomRight;
        [SerializeField]
        private float m_topLeft;
        [SerializeField]
        private float m_TopRight;

        public float BottomLeft => m_bottomLeft;

        public float BottomRight => m_bottomRight;

        public float TopLeft => m_topLeft;

        public float TopRight => m_TopRight;
    }

    [Serializable]
    public struct UIBorder : IUIBorder
    {
        [Header("Color")]
        [SerializeField]
        private CustomStyleColorCrossPosition m_color;
        [Header("Width")]
        [SerializeField]
        private CustomStyleFloatCrossPosition m_width;
        [Header("Radius")]
        [SerializeField]
        private CustomStyleBorderRadius m_radius;

        public CustomStyleColorCrossPosition Color => m_color;

        public CustomStyleFloatCrossPosition Width => m_width;

        public CustomStyleBorderRadius Radius => m_radius;
    }
}
