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
    public struct UIMarginAndPadding : IUIMarginAndPadding
    {
        [Header("Margin")]
        [SerializeField]
        private float m_marginBottom;
        [SerializeField]
        private float m_marginLeft;
        [SerializeField]
        private float m_marginRight;
        [SerializeField]
        private float m_marginTop;

        [Header("Padding")]
        [SerializeField]
        private float m_paddingBottom;
        [SerializeField]
        private float m_paddingLeft;
        [SerializeField]
        private float m_paddingRight;
        [SerializeField]
        private float m_paddingTop;

        public float MarginBottom => throw new NotImplementedException();

        public float MarginLeft => throw new NotImplementedException();

        public float MarginRight => throw new NotImplementedException();

        public float MarginTop => throw new NotImplementedException();

        public float PaddingBottom => throw new NotImplementedException();

        public float PaddingLeft => throw new NotImplementedException();

        public float PaddingRight => throw new NotImplementedException();

        public float PaddingTop => throw new NotImplementedException();
    }
}
