/*
Copyright 2019 Gfi Informatique

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

//using BrowserDesktop.Menu.Environment.Settings;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Displayer_E
    {
        //private VisualElement m_displayer { get; set; } = null;
        private VisualElement m_input { get; set; } = null;
        private VisualElement m_separator { get; set; } = null;
    }

    public partial class Displayer_E
    {
        public Displayer_E() : 
            base("UI/UXML/Displayers/displayer", 
                "UI/Style/Displayers/Displayer", 
                null) { }

        public void AddDisplayer(VisualElement displayer)
        {
            m_input.Add(displayer);
        }
    }

    public partial class Displayer_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_input = Root.Q("input");
            string inputStyle = "UI/Style/Displayers/Displayer_Input";
            AddVisualStyle(m_input, inputStyle, null);

            m_separator = Root.Q("separator");
            string separatorStyle = "UI/Style/Displayers/DisplayerSeparator";
            StyleKeys separatorKeys = new StyleKeys(null, "", null);
            AddVisualStyle(m_separator, separatorStyle, separatorKeys);
        }
    }
}