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
using System;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Shortcut_E
    {
        protected VisualElement m_iconbox { get; set; } = null;
        protected Label_E m_title { get; set; } = null;

        private static string m_shortcutUXML = "UI/UXML/Shortcuts/shortcut";
        private static string m_shortcutStyle = "UI/Style/Shortcuts/Shortcut";
        private static StyleKeys m_shortcutKeys = new StyleKeys();
    }

    public partial class Shortcut_E
    {
        public Shortcut_E() :
            base(m_shortcutUXML, m_shortcutStyle, m_shortcutKeys)
        { }

        public void Setup(string title, params Sprite[] icons)
        {

        }
    }

    public partial class Shortcut_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            var title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/Shortcuts/Shortcut_Title";
            StyleKeys titleKeys = new StyleKeys();
            m_title = new Label_E(title, titleStyle, titleKeys);
        }
    }
}
