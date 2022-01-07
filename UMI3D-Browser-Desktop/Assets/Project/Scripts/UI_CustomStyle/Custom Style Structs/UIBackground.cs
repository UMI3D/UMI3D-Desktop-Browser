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
    public struct UIBackground : IUIBackground
    {
        [SerializeField]
        private Color m_backgroundColor;
        [SerializeField]
        private Background m_backgroundImage;
        [SerializeField]
        private Color m_backgroundImageTintColo;
        [SerializeField]
        private int m_sliceBottom;
        [SerializeField]
        private int m_sliceLeft;
        [SerializeField]
        private int m_sliceRight;
        [SerializeField]
        private int m_sliceTop;

        public Color BackgroundColor => throw new NotImplementedException();

        public Background BackgroundImage => throw new NotImplementedException();

        public Color UnityBackgroundImageTintColor => throw new NotImplementedException();

        public ScaleMode UnityBackgroundScaleMode => throw new NotImplementedException();

        public int UnitySliceBottom => throw new NotImplementedException();

        public int UnitySliceLeft => throw new NotImplementedException();

        public int UnitySliceRight => throw new NotImplementedException();

        public int UnitySliceTop => throw new NotImplementedException();
    }
}
