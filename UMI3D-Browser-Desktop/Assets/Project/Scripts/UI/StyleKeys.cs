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
namespace umi3dDesktopBrowser.ui.viewController
{
    public class StyleKeys
    {
        public static StyleKeys Default => s_default;
        public static StyleKeys DefaultText => s_defaultText;
        public static StyleKeys DefaultBackground => s_defaultBackground;
        public static StyleKeys DefaultBorder => s_defaultBorder;
        public static StyleKeys DefaultTextAndBackground => s_defaultTextAndBackground;
        public static StyleKeys DefaultTextAndBorder => s_defaultTextAndBorder;
        public static StyleKeys DefaultBackgroundAndBorder => s_defaultBackgroundAndBorder;

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

        public StyleKeys(string textStyle, string backgroundStyle, string borderStyle)
        {
            TextStyleKey = textStyle;
            BackgroundStyleKey = backgroundStyle;
            BorderStyleKey = borderStyle;
        }
    }
}