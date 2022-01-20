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

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct UISize : IUISize
    {
        [SerializeField]
        private CustomStyleSize m_height;
        [SerializeField]
        private CustomStyleSize m_width;
        [SerializeField]
        private CustomStyleSize m_maxHeight;
        [SerializeField]
        private CustomStyleSize m_maxWidth;
        [SerializeField]
        private CustomStyleSize m_minHeight;
        [SerializeField]
        private CustomStyleSize m_minWidth;

        public CustomStyleSize MaxHeight => m_maxHeight;
        public CustomStyleSize MaxWidth => m_maxWidth;
        public CustomStyleSize MinHeight => m_minHeight;
        public CustomStyleSize MinWidth => m_minWidth;
        public CustomStyleSize Height => m_height;
        public CustomStyleSize Width => m_width;
    }
}