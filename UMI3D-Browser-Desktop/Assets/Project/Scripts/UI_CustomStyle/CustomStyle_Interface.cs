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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Browser.UICustomStyle
{
    public interface ICustomStyleValue<K,V>
    {
        public K Keyword { get; set; }
        public V Value { get; set; }
    }
    public interface IUIDisplay
    {
        //public DisplayStyle Display { get; }
        public CustomStylePercentFloat Opacity { get; }
        //public Visibility Visibility { get; }
    }

    public interface IUIPosition
    {
        public float Bottom { get; }
        public float Left { get; }
        public Position Position { get; }
        public float Rght { get; }
        public float Top { get; }
    }

    public interface IUISize
    {
        public CustomStylePXAndPercentFloat MaxHeight { get; }
        public CustomStylePXAndPercentFloat MaxWidth { get; }
        public CustomStylePXAndPercentFloat MinHeight { get; }
        public CustomStylePXAndPercentFloat MinWidth { get; }
        public CustomStylePXAndPercentFloat Height { get; }
        public CustomStylePXAndPercentFloat Width { get; }
    }

    public interface IUIMarginAndPadding
    {
        public float MarginBottom { get; }
        public float MarginLeft { get; }
        public float MarginRight { get; }
        public float MarginTop { get; }
        public float PaddingBottom { get; }
        public float PaddingLeft { get; }
        public float PaddingRight { get; }
        public float PaddingTop { get; }
    }

    public interface IUIText
    {
        public Color Color { get; }
        public float LetterSpacing { get; }
        public float FontSize { get; }
        public TextOverflow TextOverflow { get; }
        public Font UnityFont { get; }
        public FontDefinition UnityFontDefinition { get; }
        public FontStyle UnityFontStyleAndWeight { get; }
        public float UnityParagraphSpacing { get; }
        public TextAnchor UnityTextAlign { get; }
        public TextOverflowPosition UnityTextOverflowPosition { get; }
        public Color UnityTextOutlineColor { get; }
        public float UnityTextOutlineWidth { get; }
        public WhiteSpace WhiteSpace { get; }
        public float WordSpacing { get; }
    }

    public interface IBackground
    {
        public CustomStyleColor BackgroundColor { get; }
        public CustomStyleImage BackgroundImage { get; }
        public CustomStyleColor BackgroundImageTintColor { get; }
    }

    public interface IBackgrounds
    {
        public string Key { get; }
        public CustomStyleBackground BackgroundDefault { get; }
        public CustomStyleBackground BackgroundMouseOver { get; }
        public CustomStyleBackground BackgroundMousePressed { get; }
    }

    public interface IUIBackground
    {
        public CustomBackgrounds GetCustomBackgrounds(string key);
        public ScaleMode UnityBackgroundScaleMode { get; }
        public int SliceBottom { get; }
        public int SliceLeft { get; }
        public int SliceRight { get; }
        public int SliceTop { get; }
    }

    public interface IBorderColor
    {
        public Color BorderBottomColor { get; }
        public Color BorderLeftColor { get; }
        public Color BorderRightColor { get; }
        public Color BorderTopColor { get; }
    }

    public interface IBorderWidth
    {
        public float BorderBottomWidth { get; }
        public float BorderLeftWidth { get; }
        public float BorderRightWidth { get; }
        public float BorderTopWidth { get; }
    }

    public interface IBorderRadius
    {
        public float BorderBottomLeftRadius { get; }
        public float BorderBottomRightRadius { get; }
        public float BorderTopLeftRadius { get; }
        public float BorderTopRightRadius { get; }
    }

    public interface IUIBorder : IBorderColor, IBorderWidth, IBorderRadius
    {
    }
}
