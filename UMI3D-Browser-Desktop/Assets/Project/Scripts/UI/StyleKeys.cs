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
        public string TextStyleKey { get; set; } = null;
        public string BackgroundStyleKey { get; set; } = null;
        public string BorderStyleKey { get; set; } = null;

        public StyleKeys() { }
        public StyleKeys(string textStyle, string backgroundStyle, string borderStyle)
        {
            TextStyleKey = textStyle;
            BackgroundStyleKey = backgroundStyle;
            BorderStyleKey = borderStyle;
        }
    }
}