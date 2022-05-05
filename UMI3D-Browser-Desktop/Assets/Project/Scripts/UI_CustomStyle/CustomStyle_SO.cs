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
        public UITextStyle GetTextStyle(string key, string styleName)
        {
            key = key.ToLower();
            foreach (UITextStyle themeText in m_textStyle)
            {
                if (themeText.Key.ToLower() == key)
                    return themeText;
            }
            throw new KeyNotFoundException(key, styleName);
        }
        public UIBackground GetBackground(string key, string styleName)
        {
            key = key.ToLower();
            foreach (UIBackground background in m_background)
            {
                if (background.Key.ToLower() == key)
                    return background;
            }
            throw new KeyNotFoundException(key, styleName);
        }
        public UIBorder GetBorder(string key, string styleName)
        {
            key = key.ToLower();
            foreach (UIBorder border in m_border)
            {
                if (border.Key.ToLower() == key)
                    return border;
            }
            throw new KeyNotFoundException(key, styleName);
        }
    }

    [CreateAssetMenu(fileName ="NewCustomUIStyle", menuName = "Browser_SO/CustomUIStyle")]
    public partial class CustomStyle_SO : ScriptableObject
    {
        [HideInInspector]
        public UnityEvent AppliesFormatAndStyle = new UnityEvent();

        public string Key => name.ToLower();
        public UIDisplay Display => m_display;
        //public UIPosition UIPosition => m_uIPosition;
        public UISize Size => m_size;
        public UIMarginAndPadding MarginAndPadding => m_marginAndPadding;
        public UITextFormat TextFormat => m_textFormat;

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
        private UITextFormat m_textFormat = new UITextFormat();
        [Space()]
        [SerializeField]
        private UIThemeStyle[] m_themeStyles;

        public ICustomisableByMouseBehaviour<CustomStyleTextStyle> GetTextStyle(Theme_SO theme, string key)
            => GetStyle(theme, (themeStyle) => themeStyle.GetTextStyle(key, name));
        public ICustomisableByMouseBehaviour<CustomStyleBackground> GetBackground(Theme_SO theme, string key)
            => GetStyle(theme, (themeStyle) => themeStyle.GetBackground(key, name));
        public ICustomisableByMouseBehaviour<CustomStyleBorder> GetBorder(Theme_SO theme, string key)
            => GetStyle(theme, (themeStyle) => themeStyle.GetBorder(key, name));
        
        protected ICustomisableByMouseBehaviour<T> GetStyle<T>(Theme_SO theme, Func<UIThemeStyle, ICustomisableByMouseBehaviour<T>> styleGetter)
        {
            if (m_themeStyles.Length == 0) throw new ListEmptyException("themeStyle", name);
            foreach (UIThemeStyle themeStyle in m_themeStyles)
            {
                if (themeStyle.Theme == theme)
                    return styleGetter(themeStyle);
            }
            if (m_themeStyles[0].Theme == null) return styleGetter(m_themeStyles[0]);
            throw new ThemeNotFoundException(theme, this.name);
        }

        [ContextMenu("Apply Custom Style")]
        private void ApplyCustomStyleInInspector()
            => AppliesFormatAndStyle.Invoke();
    }
}