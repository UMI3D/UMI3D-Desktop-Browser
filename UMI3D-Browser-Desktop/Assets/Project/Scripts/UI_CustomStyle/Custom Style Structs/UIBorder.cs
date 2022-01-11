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
        private Color m_globalColor;
        [SerializeField]
        private Color m_borderBottomColor;
        [SerializeField]
        private Color m_borderLeftColor;
        [SerializeField]
        private Color m_borderRightColor;
        [SerializeField]
        private Color m_borderTopColor;

        public Color BorderBottomColor => m_borderBottomColor;

        public Color BorderLeftColor => m_borderLeftColor;

        public Color BorderRightColor => m_borderRightColor;

        public Color BorderTopColor => m_borderTopColor;
    }

    [Serializable]
    public struct BorderWidth : IBorderWidth
    {
        [SerializeField]
        private float m_globalWidth;
        [SerializeField]
        private float m_borderBottomWidth;
        [SerializeField]
        private float m_borderLeftWidth;
        [SerializeField]
        private float m_borderRightWidth;
        [SerializeField]
        private float m_borderTopWidth;

        public float BorderBottomWidth => m_borderBottomWidth;

        public float BorderLeftWidth => m_borderLeftWidth;

        public float BorderRightWidth => m_borderRightWidth;

        public float BorderTopWidth => m_borderTopWidth;
    }

    [Serializable]
    public struct BorderRadius : IBorderRadius
    {
        [SerializeField]
        private float m_globalRadius;
        [SerializeField]
        private float m_borderBottomLeftRadius;
        [SerializeField]
        private float m_borderBottomRightRadius;
        [SerializeField]
        private float m_borderTopLeftRadius;
        [SerializeField]
        private float m_borderTopRightRadius;

        public float BorderBottomLeftRadius => m_borderBottomLeftRadius;

        public float BorderBottomRightRadius => m_borderBottomRightRadius;

        public float BorderTopLeftRadius => m_borderTopLeftRadius;

        public float BorderTopRightRadius => m_borderTopRightRadius;
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

        public float BorderBottomLeftRadius => throw new NotImplementedException();

        public float BorderBottomRightRadius => throw new NotImplementedException();

        public float BorderBottomWidth => throw new NotImplementedException();

        public Color BorderLeftColor => m_borderColor.BorderLeftColor;

        public float BorderLeftWidth => throw new NotImplementedException();

        public Color BorderRightColor => m_borderColor.BorderRightColor;

        public float BorderRightWidth => throw new NotImplementedException();

        public Color BorderTopColor => m_borderColor.BorderTopColor;

        public float BorderTopLeftRadius => throw new NotImplementedException();

        public float BorderTopRightRadius => throw new NotImplementedException();

        public float BorderTopWidth => throw new NotImplementedException();
    }
}
