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

namespace Browser.UICustomStyle
{
    [Serializable]
    public struct UISize : IUISize
    {
        [SerializeField]
        private CustomStylePXAndPercentFloat m_height;
        [SerializeField]
        private CustomStylePXAndPercentFloat m_width;
        [SerializeField]
        private CustomStylePXAndPercentFloat m_maxHeight;
        [SerializeField]
        private CustomStylePXAndPercentFloat m_maxWidth;
        [SerializeField]
        private CustomStylePXAndPercentFloat m_minHeight;
        [SerializeField]
        private CustomStylePXAndPercentFloat m_minWidth;


        public CustomStylePXAndPercentFloat MaxHeight => m_maxHeight;

        public CustomStylePXAndPercentFloat MaxWidth => m_maxWidth;

        public CustomStylePXAndPercentFloat MinHeight => m_minHeight;

        public CustomStylePXAndPercentFloat MinWidth => m_minWidth;

        public CustomStylePXAndPercentFloat Height => m_height;

        public CustomStylePXAndPercentFloat Width => m_width;
    }
}