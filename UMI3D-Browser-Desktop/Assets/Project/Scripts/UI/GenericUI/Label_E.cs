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
using umi3DBrowser.UICustomStyle;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    public partial class Label_E
    {
        
    }

    public partial class Label_E
    {
        //private TextElement m_label;
        protected string m_text { get; set; }
    }

    public partial class Label_E
    {
        public Label_E(VisualElement visual, string styleResourcePath, FormatAndStyleKeys formatAndStyleKeys) : 
            base(visual, styleResourcePath, formatAndStyleKeys)
        {
            //m_label.style
        }
    }

    public partial class Label_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
        }

        
    }
}
