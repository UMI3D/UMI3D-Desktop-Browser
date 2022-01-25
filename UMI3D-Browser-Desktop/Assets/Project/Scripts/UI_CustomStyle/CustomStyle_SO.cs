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
        private Theme_SO m_theme;
        [SerializeField]
        private UITextStyle[] m_textStyle;
        [SerializeField]
        private UIBackground[] m_background;
        [SerializeField]
        private UIBorder[] m_border;

        public Theme_SO Theme => m_theme;
        public UITextStyle GetTextStyle(string key)
        {
            key = key.ToLower();
            string[] subKeys = key.Split('-');
            foreach (UITextStyle themeText in m_textStyle)
            {
                if (themeText.Key == subKeys[0])
                    return themeText;
            }
            throw new KeyNotFoundException(subKeys[0], "UIThemeStyle");
        }
        public UIBackground GetBackground(string key)
        {
            key = key.ToLower();
            string[] subKeys = key.Split('-');
            foreach (UIBackground background in m_background)
            {
                if (background.Key == subKeys[0])
                    return background;
            }
            throw new KeyNotFoundException(subKeys[0], "UIThemeStyle");
        }
        public UIBorder GetBorder(string key)
        {
            key = key.ToLower();
            string[] subKeys = key.Split('-');
            foreach (UIBorder border in m_border)
            {
                if (border.Key == subKeys[0])
                    return border;
            }
            throw new KeyNotFoundException(subKeys[0], "UIThemeStyle");
        }
    }

    [CreateAssetMenu(fileName ="NewCustomUIStyle", menuName = "Browser_SO/CustomUIStyle")]
    public partial class CustomStyle_SO : ScriptableObject
    {
        [HideInInspector]
        public UnityEvent AppliesFormatAndStyle = new UnityEvent();

        [Header("Formatting Style")]
        [SerializeField]
        private UIDisplay m_display = new UIDisplay();
        //[SerializeField]
        //private UIPosition m_uIPosition = new UIPosition();
        [SerializeField]
        private UISize m_size = new UISize();
        [SerializeField]
        private UIMarginAndPadding m_marginAndPadding = new UIMarginAndPadding();
        [SerializeField]
        private UIFormattingText m_textFormat = new UIFormattingText();
        [Space()]
        [SerializeField]
        private UIThemeStyle[] m_themeStyles;

        [ContextMenu("Apply Custom Style")]
        private void ApplyCustomStyleInInspector() => AppliesFormatAndStyle.Invoke();
    }

    public partial class CustomStyle_SO
    {
        public string Key => name.ToLower();
        public UIDisplay UIDisplay => m_display;
        //public UIPosition UIPosition => m_uIPosition;
        public UISize UISize => m_size;
        public UIMarginAndPadding UIMarginAndPadding => m_marginAndPadding;

        public UITextStyle GetTextStyle(Theme_SO theme, string key)
        {
            foreach (UIThemeStyle themeStyle in m_themeStyles)
            {
                if (themeStyle.Theme == theme)
                    return themeStyle.GetTextStyle(key);
            }
            throw new ThemeNotFoundException(theme, this.name);
        }
        public UIBackground GetBackground(Theme_SO theme, string key)
        {
            foreach (UIThemeStyle themeStyle in m_themeStyles)
            {
                if (themeStyle.Theme == theme)
                    return themeStyle.GetBackground(key);
            }
            throw new ThemeNotFoundException(theme, this.name);
        }

        public UIBorder GetBorder(Theme_SO theme, string key)
        {
            foreach (UIThemeStyle themeStyle in m_themeStyles)
            {
                if (themeStyle.Theme == theme)
                    return themeStyle.GetBorder(key);
            }
            throw new ThemeNotFoundException(theme, this.name);
        }
    }
}