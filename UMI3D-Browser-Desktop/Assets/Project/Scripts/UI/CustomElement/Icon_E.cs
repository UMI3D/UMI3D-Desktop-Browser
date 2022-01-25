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
using BrowserDesktop.UI;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public partial class Icon_E
    {
        public Icon_E(VisualElement visual, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys = null) : 
            base(visual, styleResourcePath, formatAndStyleKeys) { }

        public void ChangeBackground(string customStyleBackgroundKey)
        {
            //m_rootBackgroundStyleKey = customStyleBackgroundKey;
        }
    }

    public partial class Icon_E : Visual_E
    {

        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}