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
using UnityEngine.Events;

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct UIThemeStyle
    {
        [SerializeField]
        private string m_theme;
        [SerializeField]
        private UIThemeText[] m_uIText;
        [SerializeField]
        private UIBackground[] m_uIBackground;
        [SerializeField]
        private UIBorder[] m_uIBorder;
    }

    [CreateAssetMenu(fileName ="NewCustomUIStyle", menuName = "Browser_SO/CustomUIStyle")]
    public class CustomStyle_SO : ScriptableObject
    {
        [Header("Formatting Style")]
        [SerializeField]
        private UIDisplay m_uIDisplay = new UIDisplay();
        //[SerializeField]
        //private UIPosition m_uIPosition = new UIPosition();
        [SerializeField]
        private UISize m_uISize = new UISize();
        [SerializeField]
        private UIMarginAndPadding m_uIMarginAndPadding = new UIMarginAndPadding();
        [SerializeField]
        private UIFormattingText m_formattingText = new UIFormattingText();

        [Space()]
        [SerializeField]
        private UIThemeStyle[] m_themeStyle;

        public string Key => name.ToLower();
        public UIDisplay UIDisplay => m_uIDisplay;
        //public UIPosition UIPosition => m_uIPosition;
        public UISize UISize => m_uISize;
        public UIMarginAndPadding UIMarginAndPadding => m_uIMarginAndPadding;

        public UIBackground GetBackgroundsByKey(string key)
        {
            throw new Exception("Theme not found");
        }

        public UIBorder GetBorderByKey(string key)
        {
            throw new System.NotImplementedException();
        }

        [HideInInspector]
        public UnityEvent ApplyCustomStyle = new UnityEvent();

        [ContextMenu("Apply Custom Style")]
        private void ApplyCustomStyleInInspector() => ApplyCustomStyle.Invoke();
    }
}