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
using BrowserDesktop.preferences;
using System;
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public  partial class UIElementStyleApplicator
    {
        protected GlobalPreferences_SO m_globalPref;
    }

    public partial class UIElementStyleApplicator
    {
        #region Format

        public virtual void AppliesSize(CustomStyleSize customStyle, StyleLength initialLength, Action<StyleLength> applyLength)
        {
            StyleLength length = GetLength(customStyle.Keyword, 
                initialLength, 
                () => customStyle.Value * m_globalPref.ZoomCoef, 
                () => (customStyle.ValueMode == CustomStyleSizeMode.Px) ? customStyle.Value : Length.Percent(customStyle.Value));
            applyLength(length);
        }

        public virtual void AppliesSize(CustomStyleValue<CustomStyleSizeKeyword, int> customStyle, StyleLength initialLength, Action<StyleLength> applyLength)
        {
            StyleLength length = GetLength(customStyle.Keyword,
                initialLength,
                () => customStyle.Value * m_globalPref.ZoomCoef,
                () => customStyle.Value);
            applyLength(length);
        }

        public virtual void AppliesSize(CustomStyleValue<CustomStyleSizeKeyword, float> customStyle, StyleLength initialLength, Action<StyleLength> applyLength)
        {
            StyleLength length = GetLength(customStyle.Keyword,
                initialLength,
                () => customStyle.Value * m_globalPref.ZoomCoef,
                () => customStyle.Value);
            applyLength(length);
        }

        public virtual void AppliesMarginAndPadding(CustomStyleCrossPosition<CustomStyleSizeKeyword, float> customStyle, StyleLength initialBottomLength, StyleLength initialLeftLength, StyleLength initialRightLength, StyleLength initialTopLength, Action<StyleLength, StyleLength, StyleLength, StyleLength> applyLength)
        {
            StyleLength bottomLength = GetLength(customStyle.Keyword,
                initialBottomLength,
                () => customStyle.Value.Bottom * m_globalPref.ZoomCoef,
                () => customStyle.Value.Bottom);
            StyleLength leftLength = GetLength(customStyle.Keyword,
                initialLeftLength,
                () => customStyle.Value.Left * m_globalPref.ZoomCoef,
                () => customStyle.Value.Left);
            StyleLength rightLength = GetLength(customStyle.Keyword,
                initialRightLength,
                () => customStyle.Value.Right * m_globalPref.ZoomCoef,
                () => customStyle.Value.Right);
            StyleLength topLength = GetLength(customStyle.Keyword,
                initialTopLength,
                () => customStyle.Value.Top * m_globalPref.ZoomCoef,
                () => customStyle.Value.Top);
            applyLength(bottomLength, leftLength, rightLength, topLength);
        }

        public virtual void AppliesTextFormat(CustomStyleValue<CustomStyleSimpleKeyword, int> customStyle, TextAnchor textAlign, string text, TextElement textE)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    int value = customStyle.Value;
                    if (value >= text.Length) break;
                    Debug.Log($"text null = [{text == null}]; text length = [{text?.Length}]");
                    text = (value >= 6) ? $"{text.Substring(0, value - 3)}..." : text.Substring(0, value);
                    break;
            }
            textE.text = text;
            textE.style.unityTextAlign = textAlign;
        }

        public virtual StyleLength GetLength(CustomStyleSizeKeyword keyword, StyleLength initialLength, Func<float> resizableValue, Func<Length> unresizableValue)
        {
            StyleLength lenght = new StyleLength();
            switch (keyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    lenght = initialLength;
                    break;
                case CustomStyleSizeKeyword.Default:
                    lenght.keyword = StyleKeyword.Null;
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    lenght.value = resizableValue();
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    lenght.value = unresizableValue();
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
            }
            return lenght;
        }

        

        #endregion

        #region Style

        public virtual void AppliesTextStyle(IStyle style, CustomStyleTextStyle customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        public virtual void AppliesBackground(IStyle style, CustomStyleBackground customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    AppliesBackgroundColor(style, customStyle.Value.BackgroundColor);
                    AppliesImage(style, customStyle.Value.BackgroundImage);
                    AppliesImageColor(style, customStyle.Value.BackgroundImageTintColor);
                    break;
            }
        }

        public virtual void AppliesBorder(IStyle style, CustomStyleBorder customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    AppliesBorderColor(style, customStyle.Value.Color);
                    AppliesBorderWidth(style, customStyle.Value.Width);
                    AppliesBorderRadius(style, customStyle.Value.Radius);
                    break;
            }
        }

        #endregion
    }

    public partial class UIElementStyleApplicator
    {
        #region Background

        protected virtual void AppliesBackgroundColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Custom:
                    style.backgroundColor = customStyle.Value;
                    break;
                case CustomStyleColorKeyword.Primary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Secondary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Tertiary:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        protected virtual void AppliesImageColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Custom:
                    style.unityBackgroundImageTintColor = customStyle.Value;
                    break;
                case CustomStyleColorKeyword.Primary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Secondary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Tertiary:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        protected virtual void AppliesImage(IStyle style, CustomStyleValue<CustomStyleSimpleKeyword, Sprite> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    style.backgroundImage = customStyle.Value.texture;
                    break;
            }
        }

        #endregion

        #region Border

        protected virtual void AppliesBorderColor(IStyle style, CustomStyleCrossPosition<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Custom:
                    CrossPosition<Color> borderColor = customStyle.Value;
                    style.borderTopColor = borderColor.Top;
                    style.borderLeftColor = borderColor.Left;
                    style.borderRightColor = borderColor.Right;
                    style.borderBottomColor = borderColor.Bottom;
                    break;
                case CustomStyleColorKeyword.Primary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Secondary:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleColorKeyword.Tertiary:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        protected virtual void AppliesBorderWidth(IStyle style, CustomStyleCrossPosition<CustomStyleSizeKeyword, float> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    break;
                case CustomStyleSizeKeyword.Default:
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    CrossPosition<float> borderWidth = customStyle.Value;
                    style.borderTopWidth = borderWidth.Top;
                    style.borderLeftWidth = borderWidth.Left;
                    style.borderRightWidth = borderWidth.Right;
                    style.borderBottomWidth = borderWidth.Bottom;
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        protected virtual void AppliesBorderRadius(IStyle style, CustomStyleSquarePosition<CustomStyleSizeKeyword, float> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    break;
                case CustomStyleSizeKeyword.Default:
                    style.borderTopLeftRadius = 0f;
                    style.borderTopRightRadius = 0f;
                    style.borderBottomLeftRadius = 0f;
                    style.borderBottomRightRadius = 0f;
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    SquarePosition<float> borderRadius = customStyle.Value;
                    style.borderTopLeftRadius = borderRadius.TopLeft;
                    style.borderTopRightRadius = borderRadius.TopRight;
                    style.borderBottomLeftRadius = borderRadius.BottomLeft;
                    style.borderBottomRightRadius = borderRadius.BottomRight;
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    throw new System.NotImplementedException();
                    break;
            }
        }

        #endregion
    }
}
