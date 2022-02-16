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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class SliderWithLabel_E
    {
        protected Slider m_slider { get; set; } = null;
    }

    public partial class SliderWithLabel_E
    {
        public SliderWithLabel_E(string visualResourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualResourcePath, styleResourcePath, keys)
        { }

        public virtual void SetSlider(string styleResourcePath, StyleKeys keys)
        {
            if (Element == null)
                Element = new Slider_E(m_slider, styleResourcePath, keys);
            else
                throw new System.NotImplementedException();
        }
    }

    public partial class SliderWithLabel_E : AbstractElementWithLabel_E<Slider_E>
    {
        public override Slider_E Element { get; protected set; } = null;

        protected override void Initialize()
        {
            base.Initialize();
            m_slider = Root.Q<Slider>();
        }
    }
}