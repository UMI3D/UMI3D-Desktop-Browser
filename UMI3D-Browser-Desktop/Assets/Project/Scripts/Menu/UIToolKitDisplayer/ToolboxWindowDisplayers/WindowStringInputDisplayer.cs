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
using System;
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowStringInputDisplayer
    {
        private TextFieldWithLabel_E m_displayerElement { get; set; } = null;
    }

    public partial class WindowStringInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/textFieldInputDisplayer";
            m_displayerElement = new TextFieldWithLabel_E(UXMLPath);

            string textFieldStylePath = "UI/Style/Displayers/InputTextField";
            StyleKeys textFieldKeys = new StyleKeys(null, "", null);
            m_displayerElement.SetTextField(textFieldStylePath, textFieldKeys);

            string textInputStyle = "UI/Style/Displayers/InputTextField_Input";
            StyleKeys textInputKeys = new StyleKeys("", null, null);
            m_displayerElement.Element.SetTextInputStyle(textInputStyle, textInputKeys);

            Displayer.AddDisplayer(m_displayerElement.Root);
        }
    }

    public partial class WindowStringInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is TextInputMenuItem textMenu)
            {
                string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
                StyleKeys labelKeys = new StyleKeys("", "", null);
                m_displayerElement.SetLabel(labelStylePath, labelKeys);
                m_displayerElement.Label.value = textMenu.ToString();

                m_displayerElement.Element.value = textMenu.GetValue();
                m_displayerElement.Element.OnValueChanged += (_, newValue) =>
                {
                    textMenu.NotifyValueChange(newValue);
                };
            }
            else
                throw new System.Exception("MenuItem must be a TextInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is TextInputMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            m_displayerElement.Reset();
        }
    }
}