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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using BrowserDesktop.preferences;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class DisplayerContainer_E
    {
        public DisplayerContainer_E() :
            base(new VisualElement(), 
                "UI/Style/Displayers/DisplayerContainer", 
                new StyleKeys(null, "", ""))
        { }

        public void Adds(params Displayer_E[] displayers)
        {
            foreach (Displayer_E displayer in displayers)
                Root.Add(displayer.Root);
        }
    }

    public partial class DisplayerContainer_E : Visual_E
    {
        
    }
}