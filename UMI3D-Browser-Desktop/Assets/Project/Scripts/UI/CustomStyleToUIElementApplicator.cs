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
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI
{
    public class CustomStyleToUIElementApplicator
    {
        public virtual StyleLength GetPxAndPourcentageFloatLength(CustomStyleSize customStyle, float zoomCoef)
        {
            StyleLength lenght = new StyleLength();
            float floatLenght = -1;
            switch (customStyle.Keyword)
            {
                case CustomStyleSizeKeyword.Default:
                    lenght.keyword = StyleKeyword.Null;
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    floatLenght = customStyle.Value * zoomCoef;
                    lenght = floatLenght;
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    floatLenght = customStyle.Value;
                    lenght = (customStyle.ValueMode == CustomStyleSizeMode.Px) ? floatLenght : Length.Percent(floatLenght);
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
            }
            return lenght;
        }

        public virtual void ApplyBackgroundToVisual(IStyle style, CustomStyleBackground customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    ApplyBackgroundColorToVisual(style, customStyle.Value.BackgroundColor);
                    ApplyImageToVisual(style, customStyle.Value.BackgroundImage);
                    ApplyImageTintColorToVisual(style, customStyle.Value.BackgroundImageTintColor);
                    break;
            }
        }

        public virtual void ApplyBackgroundColorToVisual(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Custom:
                    style.backgroundColor = customStyle.Value;
                    break;
            }
        }

        public virtual void ApplyImageTintColorToVisual(IStyle style, CustomStyleValue<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Custom:
                    style.unityBackgroundImageTintColor = customStyle.Value;
                    break;
            }
        }

        public virtual void ApplyImageToVisual(IStyle style, CustomStyleValue<CustomStyleSimpleKeyword, Sprite> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    style.backgroundImage = customStyle.Value.texture;
                    break;
            }
        }

        public virtual void ApplyBorderColorToVisual(IStyle style, CustomStyleCrossPosition<CustomStyleColorKeyword, Color> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleColorKeyword.Undefined:
                    break;
                case CustomStyleColorKeyword.Custom:
                    CrossPosition<Color> borderColor = customStyle.Value;
                    style.borderTopColor = borderColor.Top;
                    style.borderLeftColor = borderColor.Left;
                    style.borderRightColor = borderColor.Right;
                    style.borderBottomColor = borderColor.Bottom;
                    break;
            }
        }

        public virtual void ApplyBorderWidthToVisual(IStyle style, CustomStyleCrossPosition<CustomStyleSizeKeyword, float> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    CrossPosition<float> borderWidth = customStyle.Value;
                    style.borderTopWidth = borderWidth.Top;
                    style.borderLeftWidth = borderWidth.Left;
                    style.borderRightWidth = borderWidth.Right;
                    style.borderBottomWidth = borderWidth.Bottom;
                    break;
            }
        }

        public virtual void ApplyBorderRadiusToVisual(IStyle style, CustomStyleSquarePosition<CustomStyleSimpleKeyword, float> customStyle)
        {
            switch (customStyle.Keyword)
            {
                case CustomStyleSimpleKeyword.Undefined:
                    break;
                case CustomStyleSimpleKeyword.Custom:
                    SquarePosition<float> borderRadius = customStyle.Value;
                    style.borderTopLeftRadius = borderRadius.TopLeft;
                    style.borderTopRightRadius = borderRadius.TopRight;
                    style.borderBottomLeftRadius = borderRadius.BottomLeft;
                    style.borderBottomRightRadius = borderRadius.BottomRight;
                    break;
            }
        }
    }
}
