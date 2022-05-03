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
using BrowserDesktop.Menu;
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowFloatRangeInputDisplayer
    {
        private SliderWithLabelAndField_E m_displayerElement { get; set; } = null; 
    }

    public partial class WindowFloatRangeInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/sliderInputDisplayer";
            m_displayerElement = new SliderWithLabelAndField_E(UXMLPath);

            string sliderStylePath = "UI/Style/Displayers/InputSlider";
            //StyleKeys sliderKeys = new StyleKeys("", null);
            m_displayerElement.SetSlider(sliderStylePath, null);
            string floatFieldStylePath = "UI/Style/Displayers/InputFloatField";
            //StyleKeys floatFieldKeys = new StyleKeys("", null);
            m_displayerElement.SetFloatField(floatFieldStylePath, null);

            Displayer.AddDisplayer(m_displayerElement.Root);
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
                string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
                m_displayerElement.SetLabel(labelStylePath, StyleKeys.DefaultTextAndBackground);
                m_displayerElement.Label.value = floatRangeMenu.ToString();

                m_displayerElement.Element.SetSlider(floatRangeMenu.min, floatRangeMenu.max, floatRangeMenu.value);
                m_displayerElement.Field.SetValueWithoutNotify(floatRangeMenu.value.ToString());
                m_displayerElement.Element.OnValueChanged += (_, newValue) 
                    => floatRangeMenu.NotifyValueChange(m_displayerElement.Clamp(newValue));
                m_displayerElement.Field.OnValueChanged += (_, newValue)
                    =>
                {
                    //To be changed when floatField will be use in runtime.
                    if (FloatField_E.TryConvertToFloat(newValue, out float f))
                        floatRangeMenu.NotifyValueChange(m_displayerElement.Clamp(f));
                };
            }
            else
                throw new System.Exception("MenuItem must be a FloatRangeInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is FloatRangeInputMenuItem) ? 2 : 0;
        }
    }
}