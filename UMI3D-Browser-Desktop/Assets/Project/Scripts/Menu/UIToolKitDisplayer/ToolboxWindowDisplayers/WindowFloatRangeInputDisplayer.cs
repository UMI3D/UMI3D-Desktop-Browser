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
using umi3d.baseBrowser.Menu;
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowFloatRangeInputDisplayer
    {
        private Slider_E m_slider { get; set; } = null;
        private FloatField_E m_floatField { get; set; } = null;
    }

    public partial class WindowFloatRangeInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            string UXMLPath = "UI/UXML/Displayers/sliderInputDisplayer";
            Displayer = new View_E(UXMLPath, s_displayerStyle, null);

            base.InitAndBindUI();
        }
    }

    public partial class WindowFloatRangeInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is FloatRangeInputMenuItem floatRangeMenu)
            {
                void RefreshSlider(float newValue)
                {
                    newValue = Mathf.Clamp(newValue, m_slider.LowValue, m_slider.HightValue);
                    m_slider.SetValueWithoutNotify(newValue);
                    RefreshField(newValue);
                }
                void RefreshField(float newValue)
                {
                    m_floatField.SetValueWithoutNotify(newValue.ToString());
                }

                string sliderStylePath = "UI/Style/Displayers/InputSlider";
                m_slider = new Slider_E(Displayer.Root.Q<Slider>(), sliderStylePath, null);
                m_slider.ValueChanged += (_, newValue) =>
                {
                    RefreshField(newValue);
                    floatRangeMenu.NotifyValueChange(newValue);
                };
                m_slider.SetSlider(floatRangeMenu.min, floatRangeMenu.max, floatRangeMenu.value);
                Displayer.Add(m_slider);

                string floatFieldStylePath = "UI/Style/Displayers/InputFloatField";
                m_floatField = new FloatField_E(Displayer.Root.Q<TextField>(), floatFieldStylePath, null);
                m_floatField.ValueChanged += (oldValue, newValue) =>
                {
                    //To be changed when floatField will be use in runtime.
                    if (FloatField_E.TryConvertToFloat(newValue, out float f))
                    {
                        f = Mathf.Clamp(f, m_slider.LowValue, m_slider.HightValue);
                        RefreshSlider(f);
                    }
                    else
                        RefreshField(m_slider.value);
                };
                m_floatField.SetValueWithoutNotify(floatRangeMenu.value.ToString());
                Displayer.Add(m_floatField);
            }
            else
                throw new System.Exception("MenuItem must be a FloatRangeInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
            => (menu is FloatRangeInputMenuItem) ? 2 : 0;
    }
}