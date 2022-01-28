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
        public virtual StyleLength GetPxAndPourcentageFloatLength(CustomStyleSize customStyle, float zoomCoef)
        {
            StyleLength lenght = new StyleLength();
            switch (customStyle.Keyword)
            {
                case CustomStyleSizeKeyword.Undefined:
                    lenght.keyword = StyleKeyword.Null;
                    break;
                case CustomStyleSizeKeyword.Default:
                    lenght.keyword = StyleKeyword.Auto;
                    break;
                case CustomStyleSizeKeyword.CustomResizable:
                    lenght = customStyle.Value * zoomCoef;
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
                case CustomStyleSizeKeyword.CustomUnresizabe:
                    float floatLenght = customStyle.Value;
                    lenght = (customStyle.ValueMode == CustomStyleSizeMode.Px) ? floatLenght : Length.Percent(floatLenght);
                    lenght.keyword = StyleKeyword.Undefined;
                    break;
            }
            return lenght;
        }

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
