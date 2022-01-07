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
    public struct UIBorder : IUIBorder
    {
        [Header("Color")]
        [SerializeField]
        private Color m_borderBottomColor;
        [SerializeField]
        private Color m_borderLeftColor;
        [SerializeField]
        private Color m_borderRightColo;
        [SerializeField]
        private Color m_borderTopColor;

        [Header("Width")]
        [SerializeField]
        private float m_borderBottomWidth;
        [SerializeField]
        private float m_borderLeftWidth;
        [SerializeField]
        private float m_borderRightWidth;
        [SerializeField]
        private float m_borderTopWidth;

        [Header("Radius")]
        [SerializeField]
        private float m_borderBottomLeftRadius;
        [SerializeField]
        private float m_borderBottomRightRadius;
        [SerializeField]
        private float m_borderTopLeftRadius;
        [SerializeField]
        private float m_borderTopRightRadius;



        public Color BorderBottomColor => throw new NotImplementedException();

        public float BorderBottomLeftRadius => throw new NotImplementedException();

        public float BorderBottomRightRadius => throw new NotImplementedException();

        public float BorderBottomWidth => throw new NotImplementedException();

        public Color BorderLeftColor => throw new NotImplementedException();

        public float BorderLeftWidth => throw new NotImplementedException();

        public Color BorderRightColo => throw new NotImplementedException();

        public float BorderRightWidth => throw new NotImplementedException();

        public Color BorderTopColor => throw new NotImplementedException();

        public float BorderTopLeftRadius => throw new NotImplementedException();

        public float BorderTopRightRadius => throw new NotImplementedException();

        public float BorderTopWidth => throw new NotImplementedException();
    }
}
