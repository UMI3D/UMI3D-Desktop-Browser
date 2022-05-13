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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class View_E
    {
        protected virtual void ApplyFormat(CustomStyle_SO style_SO, VisualElement visual)
        {
            if (style_SO == null) return;
            UISize uiSize = style_SO.Size;
            ApplySize(uiSize.Height, (length) => visual.style.height = length);
            ApplySize(uiSize.Width, (length) => visual.style.width = length);
            ApplySize(uiSize.MinHeight, (length) => visual.style.minHeight = length);
            ApplySize(uiSize.MinWidth, (length) => visual.style.minWidth = length);
            ApplySize(uiSize.MaxHeight, (length) => visual.style.maxHeight = length);
            ApplySize(uiSize.MaxWidth, (length) => visual.style.maxWidth = length);

            UIMarginAndPadding marginAndPadding = style_SO.MarginAndPadding;
            ApplySize(marginAndPadding.Margin,
                (bottom, left, right, top) =>
                {
                    visual.style.marginBottom = bottom;
                    visual.style.marginLeft = left;
                    visual.style.marginRight = right;
                    visual.style.marginTop = top;
                });
            ApplySize(marginAndPadding.Padding,
                (bottom, left, right, top) =>
                {
                    visual.style.paddingBottom = bottom;
                    visual.style.paddingLeft = left;
                    visual.style.paddingRight = right;
                    visual.style.paddingTop = top;
                });

            UITextFormat textFormat = style_SO.TextFormat;
            ApplyFontSize(textFormat.FontSize, visual.style);
            ApplySize(textFormat.LetterSpacing, (length) => visual.style.letterSpacing = length);
            ApplySize(textFormat.WordSpacing, (length) => visual.style.wordSpacing = length);
            ApplySize(textFormat.ParagraphSpacing, (length) => visual.style.unityParagraphSpacing = length);
            visual.style.unityTextAlign = textFormat.TextAlign;
            visual.style.whiteSpace = textFormat.TextWrapping;
        }

        protected virtual void ApplySize(CustomStyleSize customStyle, Action<StyleLength> applyLength)
        {
            StyleLength initialLength()
                => new StyleLength(StyleKeyword.Initial);
            float applyZoomCoef(float value)
                => m_globalPref.ApplyZoomCoef(value);

            StyleLength length = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value),
                (customStyle.ValueMode == CustomStyleSizeMode.Px) ? customStyle.Value : Length.Percent(customStyle.Value));
            applyLength(length);
        }

        protected virtual void ApplySize(CustomStyleCrossPosition<CustomStyleSizeKeyword, float> customStyle, Action<StyleLength, StyleLength, StyleLength, StyleLength> applyLength)
        {
            StyleLength initialLength() 
                => new StyleLength(StyleKeyword.Initial);
            float applyZoomCoef(float value) 
                => m_globalPref.ApplyZoomCoef(value);

            StyleLength bottom = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value.Bottom), customStyle.Value.Bottom);
            StyleLength left = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value.Left), customStyle.Value.Left);
            StyleLength right = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value.Right), customStyle.Value.Right);
            StyleLength top = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value.Top), customStyle.Value.Top);
            applyLength(bottom, left, right, top);
        }

        protected virtual void ApplySize(CustomStyleValue<CustomStyleSizeKeyword, float> customStyle, Action<StyleLength> applyLength)
        {
            StyleLength initialLength()
                => new StyleLength(StyleKeyword.Initial);
            float applyZoomCoef(float value)
                => m_globalPref.ApplyZoomCoef(value);

            StyleLength length = GetLength(customStyle.Keyword, initialLength(), applyZoomCoef(customStyle.Value), customStyle.Value);
            applyLength(length);
        }

        protected virtual void ApplyFontSize(CustomStyleValue<CustomStyleSizeKeyword, int> customStyle, IStyle style)
            => AppliesLength(customStyle.Keyword,
                () => style.fontSize = 12,
                () => style.fontSize = customStyle.Value * m_globalPref.ZoomCoef,
                () => style.fontSize = customStyle.Value);

        protected virtual string GetTextAfterFormatting(CustomStyleValue<CustomStyleSimpleKeyword, int> customStyle, string text)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    int value = customStyle.Value;
                    if (text == null || value >= text.Length) break;
                    text = (value >= 6) ? $"{text.Substring(0, value - 3)}..." : text.Substring(0, value);
                    break;
            }
            return text;
        }

        protected virtual StyleLength GetLength(CustomStyleSizeKeyword keyword, StyleLength initialLength, float resizableValue, Length unresizableValue)
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
                    lenght.value = resizableValue;
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    lenght.value = unresizableValue;
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
            }
            return lenght;
        }
    }
}
