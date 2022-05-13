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
    public struct UIBackground : ICustomisableByMouseBehaviour<CustomStyleBackground>
    {
        [SerializeField]
        private string m_key;
        [Header("Default")]
        [SerializeField]
        private CustomStyleBackground m_default;
        [Header("Mouse Over")]
        [SerializeField]
        private CustomStyleBackground m_mouseOver;
        [Header("Mouse Pressed")]
        [SerializeField]
        private CustomStyleBackground m_mousePressed;

        public string Key => m_key;
        public CustomStyleBackground Default => m_default;
        public CustomStyleBackground MouseOver => m_mouseOver;
        public CustomStyleBackground MousePressed => m_mousePressed;

        //public ScaleMode UnityBackgroundScaleMode => throw new NotImplementedException();
    }
}