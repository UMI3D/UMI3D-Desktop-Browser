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
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class SliderWithLabelAndField_E
    {
        public FloatField_E Field { get; protected set; } = null;
        //protected FloatField m_floatField { get; set; } = null;
        protected TextField m_floatField { get; set; } = null;
    }

    public partial class SliderWithLabelAndField_E
    {
        public SliderWithLabelAndField_E(string visualresourcePath) :
            this(visualresourcePath, null, null)
        { }
        public SliderWithLabelAndField_E(string visualresourcePath, string styleResourcePath, StyleKeys keys) :
            base(visualresourcePath, styleResourcePath, keys)
        { }

        public void SetFloatField(string styleResourcePath, StyleKeys keys)
        {
            if (Field == null)
                Field = new FloatField_E(m_floatField, styleResourcePath, keys);
            else
                throw new System.NotImplementedException();
            Field.OnValueChanged += (_, newValue) =>
            {
                //To be changed when floatField will be use in runtime.
                if (float.TryParse(newValue, out float f))
                {
                    Debug.Log($"on value changed field, new value = [{newValue}]; f = [{f}]");
                    RefreshSlider(f);
                }
            };
        }

        public float Clamp(float value)
            => Mathf.Clamp(value, m_slider.lowValue, m_slider.highValue);

        protected void RefreshSlider(float newValue)
        {
            if (Element == null) return;
            newValue = Mathf.Clamp(newValue, m_slider.lowValue, m_slider.highValue);
            Element.SetValueWithoutNotify(newValue);
            RefreshField(newValue);
        }

        protected void RefreshField(float newValue)
        {
            if (Field == null) return;
            Debug.Log($"refresh field, newvalue = [{newValue}];]");
            Field.SetValueWithoutNotify(newValue.ToString());
        }
    }

    public partial class SliderWithLabelAndField_E : SliderWithLabel_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            //m_floatField = Root.Q<FloatField>();
            m_floatField = Root.Q<TextField>();
        }

        public override void SetSlider(string styleResourcePath, StyleKeys keys)
        {
            base.SetSlider(styleResourcePath, keys);
            Element.OnValueChanged += (_, newValue) => RefreshField(newValue);
        }
    }
}