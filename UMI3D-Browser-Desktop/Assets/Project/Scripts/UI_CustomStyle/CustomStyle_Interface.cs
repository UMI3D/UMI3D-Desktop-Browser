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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3DBrowser.UICustomStyle
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
        public CustomStyleSize MaxHeight { get; }
        public CustomStyleSize MaxWidth { get; }
        public CustomStyleSize MinHeight { get; }
        public CustomStyleSize MinWidth { get; }
        public CustomStyleSize Height { get; }
        public CustomStyleSize Width { get; }
    }

    public interface ICrossPosition<T>
    {
        public T Bottom { get; }
        public T Left { get; }
        public T Right { get; }
        public T Top { get; }
    }

    public interface IUIMarginAndPadding
    {
        //public FloatCrossPosition Margin { get; }
        //public FloatCrossPosition Padding { get; }
    }

    public interface IUIText
    {
        public Color Color { get; }
        public float LetterSpacing { get; }
        public float FontSize { get; }
        //public TextOverflow TextOverflow { get; }
        public Font UnityFont { get; }
        //public FontDefinition UnityFontDefinition { get; }
        public FontStyle UnityFontStyleAndWeight { get; }
        public float UnityParagraphSpacing { get; }
        public TextAnchor UnityTextAlign { get; }
        //public TextOverflowPosition UnityTextOverflowPosition { get; }
        public Color UnityTextOutlineColor { get; }
        public float UnityTextOutlineWidth { get; }
        //public WhiteSpace WhiteSpace { get; }
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
        //public string Key { get; }
        public CustomStyleBackground BackgroundDefault { get; }
        public CustomStyleBackground BackgroundMouseOver { get; }
        public CustomStyleBackground BackgroundMousePressed { get; }
    }

    public interface IUIBackground
    {
        //public BackgroundsByTheme GetCustomBackgrounds(string key, CustomStyleTheme theme);
        public BackgroundsByTheme GetBackgroundsByTheme(CustomStyleTheme theme);
        public ScaleMode UnityBackgroundScaleMode { get; }
        public int SliceBottom { get; }
        public int SliceLeft { get; }
        public int SliceRight { get; }
        public int SliceTop { get; }
    }

    public interface IBorderRadius
    {
        public float BottomLeft { get; }
        public float BottomRight { get; }
        public float TopLeft { get; }
        public float TopRight { get; }
    }

    public interface IUIBorder
    {
        public CustomStyleColorCrossPosition Color { get; }
        public CustomStyleFloatCrossPosition Width { get; }
        public CustomStyleBorderRadius Radius { get; }
    }
}
