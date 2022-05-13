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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class View_E
    {
        protected virtual void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (styleSO == null || keys == null) return;
            if (keys.TextStyleKey != null) ApplyAtomStyle(style, mouseBehaviour, styleSO.GetTextStyle(m_globalPref.CurrentTheme, keys.TextStyleKey), AppliesTextStyle);
            if (keys.BackgroundStyleKey != null) ApplyAtomStyle(style, mouseBehaviour, styleSO.GetBackground(m_globalPref.CurrentTheme, keys.BackgroundStyleKey), AppliesBackground);
            if (keys.BorderStyleKey != null) ApplyAtomStyle(style, mouseBehaviour, styleSO.GetBorder(m_globalPref.CurrentTheme, keys.BorderStyleKey), AppliesBorder);
        }

        protected virtual void AppliesTextStyle(IStyle style, CustomStyleTextStyle customStyle)
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    //TODO Font style (bold, italic...)
                    AppliesTextStyleColor(style, customStyle.Value.Color);
                    AppliesOutlineTextStyleColor(style, customStyle.Value.OutlineColor);
                    //TODO Outline width;
                });

        protected virtual void AppliesBackground(IStyle style, CustomStyleBackground customStyle)
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    AppliesBackgroundColor(style, customStyle.Value.BackgroundColor);
                    AppliesImage(style, customStyle.Value.BackgroundImage);
                    AppliesImageColor(style, customStyle.Value.BackgroundImageTintColor);
                });

        protected virtual void AppliesBorder(IStyle style, CustomStyleBorder customStyle)
            => AppliesMouseBehaviourStyle(customStyle.Keyword,
                () =>
                {
                    AppliesBorderColor(style, customStyle.Value.Color);
                    AppliesBorderWidth(style, customStyle.Value.Width);
                    AppliesBorderRadius(style, customStyle.Value.Radius);
                });

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
                (color) =>
                {
                    style.borderTopColor = color;
                    style.borderLeftColor = color;
                    style.borderRightColor = color;
                    style.borderBottomColor = color;
                });

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

        #region Switch on Enum

        protected void ApplyAtomStyle<T>(IStyle style, MouseBehaviour mouseBehaviour, ICustomisableByMouseBehaviour<T> uiStyle, Action<IStyle, T> styleApplicator)
        {
            switch (mouseBehaviour)
            {
                case MouseBehaviour.MouseOut:
                    styleApplicator(style, uiStyle.Default);
                    break;
                case MouseBehaviour.MouseOver:
                    styleApplicator(style, uiStyle.MouseOver);
                    break;
                case MouseBehaviour.MousePressed:
                    styleApplicator(style, uiStyle.MousePressed);
                    break;
            }
        }

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
                case CustomStyleColorKeyword.MenuPrimaryLight:
                    themeAction(m_globalPref.CurrentTheme.MenuPrimaryLight);
                    break;
                case CustomStyleColorKeyword.MenuPrimaryDark:
                    themeAction(m_globalPref.CurrentTheme.MenuPrimaryDark);
                    break;
                case CustomStyleColorKeyword.MenuSecondaryLight:
                    themeAction(m_globalPref.CurrentTheme.MenuSecondaryLight);
                    break;
                case CustomStyleColorKeyword.MenuSecondaryDark:
                    themeAction(m_globalPref.CurrentTheme.MenuSecondaryDark);
                    break;
                case CustomStyleColorKeyword.MenuTransparentLight:
                    themeAction(m_globalPref.CurrentTheme.MenuTransparentLight);
                    break;
                case CustomStyleColorKeyword.MenuTransparentDark:
                    themeAction(m_globalPref.CurrentTheme.MenuTransparentDark);
                    break;
                case CustomStyleColorKeyword.LabelPrimaryLight:
                    themeAction(m_globalPref.CurrentTheme.LabelPrimaryLight);
                    break;
                case CustomStyleColorKeyword.LabelPrimaryDark:
                    themeAction(m_globalPref.CurrentTheme.LabelPrimaryDark);
                    break;
                case CustomStyleColorKeyword.LabelSecondaryLight:
                    themeAction(m_globalPref.CurrentTheme.LabelSecondaryLight);
                    break;
                case CustomStyleColorKeyword.LabelSecondaryDark:
                    themeAction(m_globalPref.CurrentTheme.LabelSecondaryDark);
                    break;
                case CustomStyleColorKeyword.IconPrimaryLight:
                    themeAction(m_globalPref.CurrentTheme.IconPrimaryLight);
                    break;
                case CustomStyleColorKeyword.IconPrimaryDark:
                    themeAction(m_globalPref.CurrentTheme.IconPrimaryDark);
                    break;
                case CustomStyleColorKeyword.IconSecondaryLight:
                    themeAction(m_globalPref.CurrentTheme.IconSecondaryLight);
                    break;
                case CustomStyleColorKeyword.IconSecondaryDark:
                    themeAction(m_globalPref.CurrentTheme.IconSecondaryDark);
                    break;
            }
        }

        #endregion
    }
}
