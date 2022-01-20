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
    public struct Backgrounds : IBackgrounds
    {
        [SerializeField]
        private string m_key;
        [SerializeField]
        private CustomStyleTheme m_theme;
        [Header("Default")]
        [SerializeField]
        private CustomStyleBackground m_backgroundDefault;
        [Header("Mouse Over")]
        [SerializeField]
        private CustomStyleBackground m_backgroundMouseOver;
        [Header("Mouse Pressed")]
        [SerializeField]
        private CustomStyleBackground m_backgroundMousePressed;

        public string Key => m_key;
        public CustomStyleTheme Theme => m_theme;
        public CustomStyleBackground BackgroundDefault => m_backgroundDefault;
        public CustomStyleBackground BackgroundMouseOver => m_backgroundMouseOver;
        public CustomStyleBackground BackgroundMousePressed => m_backgroundMousePressed;
    }

    [Serializable]
    public struct UIBackground : IUIBackground
    {
        [SerializeField]
        private Backgrounds[] m_backgrounds;

        public Backgrounds GetBackgroundsByTheme(CustomStyleTheme theme)
        {
            foreach (Backgrounds backgroundsByThem in m_backgrounds)
            {
                if (backgroundsByThem.Theme == theme)
                    return backgroundsByThem;
            }
            throw new Exception("Theme not found");
        }

        public ScaleMode UnityBackgroundScaleMode => throw new NotImplementedException();
    }
}
