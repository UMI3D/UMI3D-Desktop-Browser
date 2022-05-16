/*
Copyright 2019 - 2022 Inetum

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
namespace umi3d.baseBrowser.ui.viewController
{
    public class StyleKeys
    {
        public static StyleKeys Default => s_default;
        public static StyleKeys DefaultText => s_defaultText;
        public static StyleKeys DefaultBackground => s_defaultBackground;
        public static StyleKeys DefaultBorder => s_defaultBorder;
        public static StyleKeys DefaultTextAndBackground => s_defaultTextAndBackground;
        public static StyleKeys DefaultTextAndBorder => s_defaultTextAndBorder;
        public static StyleKeys Default_Bg_Border => s_defaultBackgroundAndBorder;

        private static StyleKeys s_default = new StyleKeys("", "", "");
        private static StyleKeys s_defaultText = new StyleKeys("", null, null);
        private static StyleKeys s_defaultBackground = new StyleKeys(null, "", null);
        private static StyleKeys s_defaultBorder = new StyleKeys(null, null, "");
        private static StyleKeys s_defaultTextAndBackground = new StyleKeys("", "", null);
        private static StyleKeys s_defaultTextAndBorder = new StyleKeys("", null, "");
        private static StyleKeys s_defaultBackgroundAndBorder = new StyleKeys(null, "", "");

        public string TextStyleKey { get; private set; } = null;
        public string BackgroundStyleKey { get; private set; } = null;
        public string BorderStyleKey { get; private set; } = null;

        public static StyleKeys Text(string text) => new StyleKeys(text, null, null);
        public static StyleKeys Bg(string bg) => new StyleKeys(null, bg, null);
        public static StyleKeys Border(string border) => new StyleKeys(null, null, border);
        public static StyleKeys Text_Bg(string value) => new StyleKeys(value, value, null);
        public static StyleKeys Text_Bg(string text, string bg) => new StyleKeys(text, bg, null);
        public static StyleKeys Text_Border(string value) => new StyleKeys(value, null, value);
        public static StyleKeys Text_Border(string text, string border) => new StyleKeys(text, null, border);
        public static StyleKeys Bg_Border(string value) => new StyleKeys(null, value, value);
        public static StyleKeys Bg_Border(string bg, string border) => new StyleKeys(null, bg, border);
        public static StyleKeys Text_Bg_Border(string value) => new StyleKeys(value, value, value);

        public StyleKeys(string textStyle, string backgroundStyle, string borderStyle)
        {
            TextStyleKey = textStyle;
            BackgroundStyleKey = backgroundStyle;
            BorderStyleKey = borderStyle;
        }
    }
}