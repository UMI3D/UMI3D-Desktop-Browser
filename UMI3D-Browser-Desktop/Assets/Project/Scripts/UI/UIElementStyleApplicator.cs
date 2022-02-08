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
        public UIElementStyleApplicator(GlobalPreferences_SO globalPref)
        {
            m_globalPref = globalPref;
        }
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

        public virtual void AppliesFontSize(IStyle style, CustomStyleValue<CustomStyleSizeKeyword, int> customStyle)
            => AppliesLength(customStyle.Keyword,
                () => style.fontSize = 12,
                () => style.fontSize = customStyle.Value * m_globalPref.ZoomCoef,
                () => style.fontSize = customStyle.Value);

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
                    throw new System.NotImplementedException();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    int value = customStyle.Value;
                    if (value >= text.Length) break;
                    text = (value >= 6) ? $"{text.Substring(0, value - 3)}..." : text.Substring(0, value);
                    break;
            }
            textE.text = text;
            textE.style.unityTextAlign = textAlign;
        }

        protected virtual StyleLength GetLength(CustomStyleSizeKeyword keyword, StyleLength initialLength, Func<float> resizableValue, Func<Length> unresizableValue)
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
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    AppliesTextStyleColor(style, customStyle.Value.Color);
                    AppliesOutlineTextStyleColor(style, customStyle.Value.OutlineColor);
                    //TODO Outline width;
                });

        public virtual void AppliesBackground(IStyle style, CustomStyleBackground customStyle)
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    AppliesBackgroundColor(style, customStyle.Value.BackgroundColor);
                    AppliesImage(style, customStyle.Value.BackgroundImage);
                    AppliesImageColor(style, customStyle.Value.BackgroundImageTintColor);
                });

        public virtual void AppliesBorder(IStyle style, CustomStyleBorder customStyle)
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    AppliesBorderColor(style, customStyle.Value.Color);
                    AppliesBorderWidth(style, customStyle.Value.Width);
                    AppliesBorderRadius(style, customStyle.Value.Radius);
                });

        

        #endregion
    }

    public partial class UIElementStyleApplicator
    {
        protected void AppliesMouseBehaviourStyle(CustomStyleExtraSimpleKeyword keyword, Action customAction)
        {
            switch (keyword)
            {
                case CustomStyleExtraSimpleKeyword.Undefined:
                    break;
                case CustomStyleExtraSimpleKeyword.Custom:
                    customAction();
                    break;
            }
        }

        protected void AppliesSimple(CustomStyleSimpleKeyword simpleKeyword, Action defaultAction, Action customAction)
        {
            switch (simpleKeyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Default:
                    defaultAction();
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    customAction();
                    break;
            }
        }

        protected void AppliesColor(CustomStyleColorKeyword colorKeyword, Action defaultAction, Action customAction, Action<Color> themeAction)
        {
            switch (colorKeyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Default:
                    defaultAction();
                    break;
                case CustomStyleColorKeyword.Custom:
                    customAction();
                    break;
                case CustomStyleColorKeyword.Primary:
                    themeAction(m_globalPref.CurrentTheme.Primary);
                    break;
                case CustomStyleColorKeyword.Secondary:
                    themeAction(m_globalPref.CurrentTheme.Secondary);
                    break;
                case CustomStyleColorKeyword.Tertiary:
                    themeAction(m_globalPref.CurrentTheme.Tertiary);
                    break;
            }
        }

        protected void AppliesLength(CustomStyleSizeKeyword lengthKeyword, Action defaultAction, Action resizableAction, Action unresizableAction)
        {
            switch (lengthKeyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    break;
                case CustomStyleSizeKeyword.Default:
                    defaultAction();
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    resizableAction();
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    unresizableAction();
                    break;
            }
        }

        #region Text

        protected virtual void AppliesTextStyleColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
            => AppliesColor(customStyle.Keyword,
                () => style.color = StyleKeyword.Null,
                () => style.color = customStyle.Value,
                (color) => style.color = color);

        protected virtual void AppliesOutlineTextStyleColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
            => AppliesColor(customStyle.Keyword,
                () => style.unityTextOutlineColor = StyleKeyword.Null,
                () => style.unityTextOutlineColor = customStyle.Value,
                (color) => { style.unityTextOutlineColor = color; });

        //protected virtual void AppliesOutlineTextStyleWidth(IStyle style, CustomStyleValue<CustomStyleSimpleKeyword, float> customStyle)
        //    => AppliesLength(customStyle.Keyword,
        //        () => style.unityTextOutlineWidth = StyleKeyword.Null,
        //        () => style.unityTextOutlineWidth = customStyle.Value * m_globalPref.ZoomCoef,
        //        () => style.unityTextOutlineWidth = customStyle.Value);

        #endregion

        #region Background

        protected virtual void AppliesBackgroundColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
            => AppliesColor(customStyle.Keyword,
                () => style.backgroundColor = StyleKeyword.Null,
                () => style.backgroundColor = customStyle.Value,
                (color) => style.backgroundColor = color);

        protected virtual void AppliesImageColor(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
            => AppliesColor(customStyle.Keyword,
                () => style.unityBackgroundImageTintColor = StyleKeyword.Null,
                () => style.unityBackgroundImageTintColor = customStyle.Value,
                (color) => style.unityBackgroundImageTintColor = color);

        protected virtual void AppliesImage(IStyle style, CustomStyleValue<CustomStyleSimpleKeyword, Sprite> customStyle)
            => AppliesSimple(customStyle.Keyword,
                () => style.backgroundImage = StyleKeyword.Null,
                () => style.backgroundImage = customStyle.Value.texture);

        #endregion

        #region Border

        protected virtual void AppliesBorderColor(IStyle style, CustomStyleCrossPosition<CustomStyleColorKeyword, Color> customStyle)
            => AppliesColor(customStyle.Keyword,
                () =>
                {
                    style.borderTopColor = StyleKeyword.Null;
                    style.borderLeftColor = StyleKeyword.Null;
                    style.borderRightColor = StyleKeyword.Null;
                    style.borderBottomColor = StyleKeyword.Null;
                },
                () =>
                {
                    CrossPosition<Color> borderColor = customStyle.Value;
                    style.borderTopColor = borderColor.Top;
                    style.borderLeftColor = borderColor.Left;
                    style.borderRightColor = borderColor.Right;
                    style.borderBottomColor = borderColor.Bottom;
                },
                (color) => { });

        protected virtual void AppliesBorderWidth(IStyle style, CustomStyleCrossPosition<CustomStyleSizeKeyword, float> customStyle)
            => AppliesLength(customStyle.Keyword,
                () =>
                {
                    style.borderTopWidth = StyleKeyword.Null;
                    style.borderLeftWidth = StyleKeyword.Null;
                    style.borderRightWidth = StyleKeyword.Null;
                    style.borderBottomWidth = StyleKeyword.Null;
                },
                () =>
                {
                    CrossPosition<float> borderWidth = customStyle.Value;
                    style.borderTopWidth = borderWidth.Top * m_globalPref.ZoomCoef;
                    style.borderLeftWidth = borderWidth.Left * m_globalPref.ZoomCoef;
                    style.borderRightWidth = borderWidth.Right * m_globalPref.ZoomCoef;
                    style.borderBottomWidth = borderWidth.Bottom * m_globalPref.ZoomCoef;
                },
                () =>
                {
                    CrossPosition<float> borderWidth = customStyle.Value;
                    style.borderTopWidth = borderWidth.Top;
                    style.borderLeftWidth = borderWidth.Left;
                    style.borderRightWidth = borderWidth.Right;
                    style.borderBottomWidth = borderWidth.Bottom;
                });

        protected virtual void AppliesBorderRadius(IStyle style, CustomStyleSquarePosition<CustomStyleSizeKeyword, float> customStyle)
            => AppliesLength(customStyle.Keyword,
                () =>
                {
                    style.borderTopLeftRadius = StyleKeyword.Null;
                    style.borderTopRightRadius = StyleKeyword.Null;
                    style.borderBottomLeftRadius = StyleKeyword.Null;
                    style.borderBottomRightRadius = StyleKeyword.Null;
                },
                () =>
                {
                    SquarePosition<float> borderRadius = customStyle.Value;
                    style.borderTopLeftRadius = borderRadius.TopLeft * m_globalPref.ZoomCoef;
                    style.borderTopRightRadius = borderRadius.TopRight * m_globalPref.ZoomCoef;
                    style.borderBottomLeftRadius = borderRadius.BottomLeft * m_globalPref.ZoomCoef;
                    style.borderBottomRightRadius = borderRadius.BottomRight * m_globalPref.ZoomCoef;
                },
                () =>
                {
                    SquarePosition<float> borderRadius = customStyle.Value;
                    style.borderTopLeftRadius = borderRadius.TopLeft;
                    style.borderTopRightRadius = borderRadius.TopRight;
                    style.borderBottomLeftRadius = borderRadius.BottomLeft;
                    style.borderBottomRightRadius = borderRadius.BottomRight;
                });

        #endregion
    }
}
