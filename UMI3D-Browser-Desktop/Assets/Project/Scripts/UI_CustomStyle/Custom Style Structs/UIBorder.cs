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
    public struct BorderColor : IBorderColor
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
    public struct BorderWidth : IBorderWidth
    {
        [SerializeField]
        private float m_global;
        [SerializeField]
        private float m_bottom;
        [SerializeField]
        private float m_left;
        [SerializeField]
        private float m_right;
        [SerializeField]
        private float m_top;

        public float Bottom => m_bottom;

        public float Left => m_left;

        public float Right => m_right;

        public float Top => m_top;
    }

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
        private CustomStyleBorderColor m_color;
        [Header("Width")]
        [SerializeField]
        private CustomStyleBorderWidth m_width;
        [Header("Radius")]
        [SerializeField]
        private CustomStyleBorderRadius m_radius;

        public CustomStyleBorderColor Color => m_color;

        public CustomStyleBorderWidth Width => m_width;

        public CustomStyleBorderRadius Radius => m_radius;
    }
}
