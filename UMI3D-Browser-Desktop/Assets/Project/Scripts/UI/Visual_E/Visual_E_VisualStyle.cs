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

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Visual_E
    {
        protected UIElementStyleApplicator m_styleApplicator;
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
            ApplyTextFormat(style_SO, visual);
        }

        protected void ApplySize(CustomStyle_SO style_SO, IStyle style)
        {
            if (style_SO == null) return;
            UISize uiSize = style_SO.Size;
            m_styleApplicator.ApplySize(uiSize.Height, (length) => style.height = length);
            m_styleApplicator.ApplySize(uiSize.Width, (length) => style.width = length);
            m_styleApplicator.ApplySize(uiSize.MinHeight, (length) => style.minHeight = length);
            m_styleApplicator.ApplySize(uiSize.MinWidth, (length) => style.minWidth = length);
            m_styleApplicator.ApplySize(uiSize.MaxHeight, (length) => style.maxHeight = length);
            m_styleApplicator.ApplySize(uiSize.MaxWidth, (length) => style.maxWidth = length);
        }

        protected void ApplyMarginAndPadding(CustomStyle_SO style_SO, IStyle style)
        {
            if (style_SO == null) return;
            UIMarginAndPadding marginAndPadding = style_SO.MarginAndPadding;
            m_styleApplicator.ApplySize(marginAndPadding.Margin,
                (bottom, left, right, top) =>
                {
                    style.marginBottom = bottom;
                    style.marginLeft = left;
                    style.marginRight = right;
                    style.marginTop = top;
                });
            m_styleApplicator.ApplySize(marginAndPadding.Padding,
                (bottom, left, right, top) =>
                {
                    style.paddingBottom = bottom;
                    style.paddingLeft = left;
                    style.paddingRight = right;
                    style.paddingTop = top;
                });
        }

        protected virtual void ApplyTextFormat(CustomStyle_SO style_SO, VisualElement textE)
        {
            if (style_SO == null) return;
            UITextFormat textFormat = style_SO.TextFormat;
            m_styleApplicator.ApplyFontSize(textFormat.FontSize, textE.style);
            m_styleApplicator.ApplySize(textFormat.LetterSpacing,
                (length) => textE.style.letterSpacing = length);
            m_styleApplicator.ApplySize(textFormat.WordSpacing,
                (length) => textE.style.wordSpacing = length);
            m_styleApplicator.ApplySize(textFormat.ParagraphSpacing,
                (length) => textE.style.unityParagraphSpacing = length);
            textE.style.unityTextAlign = textFormat.TextAlign;

            //m_styleApplicator.AppliesTextFormat(textFormat.NumberOfVisibleCharacter,
            //    textFormat.TextAlign, text, textE);
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
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetTextStyle(m_globalPref.CurrentTheme, styleKey), m_styleApplicator.AppliesTextStyle);

        protected void ApplyBackgroundStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetBackground(m_globalPref.CurrentTheme, styleKey), m_styleApplicator.AppliesBackground);

        protected void ApplyBorderStyle(CustomStyle_SO style_SO, string styleKey, IStyle style, MouseBehaviour mouseBehaviour)
            => ApplyAtomStyle(style, mouseBehaviour, () => style_SO.GetBorder(m_globalPref.CurrentTheme, styleKey), m_styleApplicator.AppliesBorder);

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

