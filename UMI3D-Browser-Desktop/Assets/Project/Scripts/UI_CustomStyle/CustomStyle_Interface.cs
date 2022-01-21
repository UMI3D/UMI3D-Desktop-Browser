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
    public interface ISquarePosition<T>
    {
        public T BottomLeft { get; }
        public T BottomRight { get; }
        public T TopLeft { get; }
        public T TopRight { get; }
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
        public CustomStyleValue<CustomStyleColorKeyword, Color> BackgroundColor { get; }
        public CustomStyleValue<CustomStyleSimpleKeyword, Sprite> BackgroundImage { get; }
        public CustomStyleValue<CustomStyleColorKeyword, Color> BackgroundImageTintColor { get; }
    }

    public interface ICustomisableByMouseBehaviour<T>
    {
        public string Key { get; }
        public T Default { get; }
        public T MouseOver { get; }
        public T MousePressed { get; }
    }

    public interface IUIBackground
    {
        //public Backgrounds GetBackgroundsByTheme(CustomStyleTheme theme);
        public string Key { get; }
        //public CustomStyleBackground Default { get; }
        //public CustomStyleBackground MouseOver { get; }
        //public CustomStyleBackground MousePressed { get; }
        //public ScaleMode UnityBackgroundScaleMode { get; }
    }

    public interface IUIBorder
    {
        //public CustomStyleCrossPosition<CustomStyleColorKeyword, Color> Color { get; }
        //public CustomStyleCrossPosition<CustomStyleSizeKeyword, float> Width { get; }
        //public CustomStyleSquarePosition<CustomStyleSimpleKeyword, float> Radius { get; }
    }
}
