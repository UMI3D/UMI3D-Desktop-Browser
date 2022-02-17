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
using umi3DBrowser.UICustomStyle;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using BrowserDesktop.preferences;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Visual_E
    {
        protected UIElementStyleApplicator m_uIElementStyleApplicator;
        protected GlobalPreferences_SO m_globalPref;

        public virtual void ApplyAllFormatAndStyle()
        {
            ApplyAllFormat();
            ApplyAllStyle();
        }

        #region Format of the element

        protected void ApplyAllFormat()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var style = m_visualStyles[visual];
                ApplyFormat(style.Item1, style.Item2, visual);
            }
        }

        protected virtual void ApplyFormat(CustomStyle_SO style_SO, StyleKeys keys, VisualElement visual)
        {
            ApplySize(style_SO, visual.style);
            ApplyMarginAndPadding(style_SO, visual.style);
            if (visual is TextElement textE)
                ApplyTextFormat(style_SO, keys?.Text, textE);
        }

        protected void ApplySize(CustomStyle_SO style_SO, IStyle style)
        {
            if (style_SO == null) return;
            UISize uiSize = style_SO.Size;
            m_uIElementStyleApplicator.AppliesSize(uiSize.Height, style.height, (length) => style.height = length);
            m_uIElementStyleApplicator.AppliesSize(uiSize.Width, style.width, (length) => style.width = length);
            m_uIElementStyleApplicator.AppliesSize(uiSize.MinHeight, style.minHeight, (length) => style.minHeight = length);
            m_uIElementStyleApplicator.AppliesSize(uiSize.MinWidth, style.minWidth, (length) => style.minWidth = length);
            m_uIElementStyleApplicator.AppliesSize(uiSize.MaxHeight, style.maxHeight, (length) => style.maxHeight = length);
            m_uIElementStyleApplicator.AppliesSize(uiSize.MaxWidth, style.maxWidth, (length) => style.maxWidth = length);
        }

        protected void ApplyMarginAndPadding(CustomStyle_SO style_SO, IStyle style)
        {
            if (style_SO == null) return;
            UIMarginAndPadding marginAndPadding = style_SO.MarginAndPadding;
            m_uIElementStyleApplicator.AppliesMarginAndPadding(marginAndPadding.Margin,
                style.marginBottom,
                style.marginLeft,
                style.marginRight,
                style.marginTop,
                (bottom, left, right, top) =>
                {
                    style.marginBottom = bottom;
                    style.marginLeft = left;
                    style.marginRight = right;
                    style.marginTop = top;
                });
            m_uIElementStyleApplicator.AppliesMarginAndPadding(marginAndPadding.Padding,
                style.paddingBottom,
                style.paddingLeft,
                style.paddingRight,
                style.paddingTop,
                (bottom, left, right, top) =>
                {
                    style.paddingBottom = bottom;
                    style.paddingLeft = left;
                    style.paddingRight = right;
                    style.paddingTop = top;
                });
        }

        protected void ApplyTextFormat(CustomStyle_SO style_SO, string text, TextElement textE)
        {
            if (style_SO == null) return;
            UITextFormat textFormat = style_SO.TextFormat;
            m_uIElementStyleApplicator.AppliesFontSize(textE.style, textFormat.FontSize);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.LetterSpacing,
                textE.style.letterSpacing,
                (length) => textE.style.letterSpacing = length);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.WordSpacing,
                textE.style.wordSpacing,
                (length) => textE.style.wordSpacing = length);
            m_uIElementStyleApplicator
                .AppliesSize(textFormat.ParagraphSpacing,
                textE.style.unityParagraphSpacing,
                (length) => textE.style.unityParagraphSpacing = length);
            m_uIElementStyleApplicator
                .AppliesTextFormat(textFormat.NumberOfVisibleCharacter,
                textFormat.TextAlign, text, textE);
        }

        #endregion

        #region Theme style of the element

        protected void ApplyAllStyle()
        {
            foreach (VisualElement visual in m_visuals)
            {
                var (_, _, manipulator) = m_visualStyles[visual];
                manipulator.AppliesStyle();
            }
        }

        protected virtual void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            if (styleSO == null || keys == null) return;
            if (keys.TextStyleKey != null) ApplyTextStyle(styleSO, keys.TextStyleKey, style, mouseBehaviour);
            if (keys.BackgroundStyleKey != null) ApplyBackgroundStyle(styleSO, keys.BackgroundStyleKey, style, mouseBehaviour);
            if (keys.BorderStyleKey != null) ApplyBorderStyle(styleSO, keys.BorderStyleKey, style, mouseBehaviour);
        }

        protected void ApplyTextStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetTextStyle(m_globalPref.CurrentTheme, styleKey), m_uIElementStyleApplicator.AppliesTextStyle);

        protected void ApplyBackgroundStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetBackground(m_globalPref.CurrentTheme, styleKey), m_uIElementStyleApplicator.AppliesBackground);

        protected void ApplyBorderStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetBorder(m_globalPref.CurrentTheme, styleKey), m_uIElementStyleApplicator.AppliesBorder);

        protected void ApplyAtomStyle<T>(IStyle style, MouseBehaviour mouseBehaviour, Func<ICustomisableByMouseBehaviour<T>> getUIStyle, Action<IStyle, T> styleApplicator)
        {
            ICustomisableByMouseBehaviour<T> uiStyle = getUIStyle();
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

        #endregion
    }
}

