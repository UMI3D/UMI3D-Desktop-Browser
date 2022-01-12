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

        public Color BorderBottomColor => m_bottom;

        public Color BorderLeftColor => m_left;

        public Color BorderRightColor => m_right;

        public Color BorderTopColor => m_top;
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

        public float BorderBottomWidth => m_bottom;

        public float BorderLeftWidth => m_left;

        public float BorderRightWidth => m_right;

        public float BorderTopWidth => m_top;
    }

    [Serializable]
    public struct BorderRadius : IBorderRadius
    {
        [SerializeField]
        private CustomStyleFloat m_global;
        [SerializeField]
        private float m_bottomLeft;
        [SerializeField]
        private float m_bottomRight;
        [SerializeField]
        private float m_topLeft;
        [SerializeField]
        private float m_TopRight;

        public float BorderBottomLeftRadius => m_bottomLeft;

        public float BorderBottomRightRadius => m_bottomRight;

        public float BorderTopLeftRadius => m_topLeft;

        public float BorderTopRightRadius => m_TopRight;
    }

    [Serializable]
    public struct UIBorder : IUIBorder
    {
        [Header("Color")]
        [SerializeField]
        private BorderColor m_borderColor;
        [Header("Width")]
        [SerializeField]
        private BorderWidth m_borderWidth;
        [Header("Radius")]
        [SerializeField]
        private BorderRadius m_borderRadius;
        
        public Color BorderBottomColor => m_borderColor.BorderBottomColor;

        public float BorderBottomLeftRadius => m_borderRadius.BorderBottomLeftRadius;

        public float BorderBottomRightRadius => m_borderRadius.BorderBottomRightRadius;

        public float BorderBottomWidth => m_borderWidth.BorderBottomWidth;

        public Color BorderLeftColor => m_borderColor.BorderLeftColor;

        public float BorderLeftWidth => m_borderWidth.BorderLeftWidth;

        public Color BorderRightColor => m_borderColor.BorderRightColor;

        public float BorderRightWidth => m_borderWidth.BorderRightWidth;

        public Color BorderTopColor => m_borderColor.BorderTopColor;

        public float BorderTopLeftRadius => m_borderRadius.BorderTopLeftRadius;

        public float BorderTopRightRadius => m_borderRadius.BorderTopRightRadius;

        public float BorderTopWidth => m_borderWidth.BorderTopWidth;
    }
}
