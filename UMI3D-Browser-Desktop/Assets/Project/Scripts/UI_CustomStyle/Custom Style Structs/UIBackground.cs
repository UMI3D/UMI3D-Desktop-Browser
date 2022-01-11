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
    public struct CustomBackground : IBackground
    {
        [SerializeField]
        private CustomStyleColor m_color;
        [SerializeField]
        private CustomStyleImage m_image;
        [SerializeField]
        private CustomStyleColor m_imageTintColor;

        public CustomStyleColor BackgroundColor => m_color;

        public CustomStyleImage BackgroundImage => m_image;

        public CustomStyleColor BackgroundImageTintColor => m_imageTintColor;
    }

    [Serializable]
    public struct CustomBackgrounds : IBackgrounds
    {
        [SerializeField]
        private string m_backgroundKey;
        [SerializeField]
        private CustomStyleBackground m_backgroundDefault;
        [SerializeField]
        private CustomStyleBackground m_backgroundMouseOver;
        [SerializeField]
        private CustomStyleBackground m_backgroundMousePressed;

        public string Key => m_backgroundKey;

        public CustomStyleBackground BackgroundDefault => m_backgroundDefault;

        public CustomStyleBackground BackgroundMouseOver => m_backgroundMouseOver;

        public CustomStyleBackground BackgroundMousePressed => m_backgroundMousePressed;
    }

    [Serializable]
    public struct UIBackground : IUIBackground
    {
        //[SerializeField]
        //private CustomStyleBackground m_backgroundDefault;
        //[SerializeField]
        //private CustomStyleBackground m_backgroundMouseOver;
        //[SerializeField]
        //private CustomStyleBackground m_backgroundMousePressed;
        [SerializeField]
        private CustomBackgrounds[] m_backgrounds;
        [SerializeField]
        private int m_sliceBottom;
        [SerializeField]
        private int m_sliceLeft;
        [SerializeField]
        private int m_sliceRight;
        [SerializeField]
        private int m_sliceTop;

        public CustomBackgrounds GetCustomBackgrounds(string key)
        {
            foreach (CustomBackgrounds customBackgrounds in m_backgrounds)
            {
                if (customBackgrounds.Key == key)
                    return customBackgrounds;
            }
            throw new Exception("Key not found");
        }

        public ScaleMode UnityBackgroundScaleMode => throw new NotImplementedException();

        public int SliceBottom => throw new NotImplementedException();

        public int SliceLeft => throw new NotImplementedException();

        public int SliceRight => throw new NotImplementedException();

        public int SliceTop => throw new NotImplementedException();
    }
}
