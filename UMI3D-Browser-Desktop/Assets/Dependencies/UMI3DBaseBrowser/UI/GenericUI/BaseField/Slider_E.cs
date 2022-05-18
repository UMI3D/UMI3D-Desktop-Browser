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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class Slider_E
    {
        public float LowValue => m_slider.lowValue;
        public float HightValue => m_slider.highValue;

        protected Slider m_slider => (Slider)Root;
        protected VisualElement m_tracker { get; set; } = null;
        protected VisualElement m_draggerBorder { get; set; } = null;
        protected VisualElement m_dragger { get; set; } = null;

        public void SetSlider(float start, float end, float initialValue, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
        {
            m_slider.lowValue = start;
            m_slider.highValue = end;
            m_slider.value = initialValue;
            m_slider.direction = direction;
            m_slider.pageSize = pageSize;
        }

        public void SetDragger(string styleResourcePath, StyleKeys keys)
            => AddVisualStyle(m_dragger, styleResourcePath, keys);
        public void SetDraggerBorder()
        {
            Debug.Log("<color=green>TODO: </color>" + $"not imp.");
        }
        public void SetTracker(string styleResourcePath, StyleKeys keys)
            => AddVisualStyle(m_tracker, styleResourcePath, keys);
    }

    public partial class Slider_E : AbstractBaseField_E<float>
    {
        public Slider_E() :
            this(null, null)
        { }
        public Slider_E(string styleResourcePath, StyleKeys keys) :
            this(new Slider(), styleResourcePath, keys)
        { }
        public Slider_E(Slider slider, string styleResourcePath, StyleKeys keys) :
            this(slider, styleResourcePath, keys, 0f, 0f, 0f)
        { }
        public Slider_E(Slider slider, string styleResourcePath, StyleKeys keys, float start, float end, float initialValue, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f) :
            base(slider, styleResourcePath, keys)
        {
            SetSlider(start, end, initialValue, direction, pageSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
            m_tracker = Root.Q("unity-tracker");
            m_dragger = Root.Q("unity-dragger");
        }
    }
}