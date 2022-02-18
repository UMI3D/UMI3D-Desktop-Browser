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
using umi3dDesktopBrowser.uI.viewController;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowToggleInputDisplayer
    {
        private ButtonWithLabel_E m_displayerElement { get; set; } = null;
    }

    public partial class WindowToggleInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_displayerElement = new ButtonWithLabel_E(UXMLPath, null, null);

            Displayer.AddDisplayer(m_displayerElement.Root);
        }
    }

    public partial class WindowToggleInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is BooleanInputMenuItem toggleMenu)
            {
                string toggleStylePath = "UI/Style/Displayers/InputToggle";
                StyleKeys toggleOnKeys = new StyleKeys(null, "toggleOn", null);
                StyleKeys toggleOffKeys = new StyleKeys(null, "toggleOff", null);
                m_displayerElement.SetButton(toggleStylePath, toggleOnKeys, toggleOffKeys, toggleMenu.GetValue(), null);
                m_displayerElement.Element.Text = null;

                m_displayerElement.OnClicked = () => 
                {
                    bool newValue = !m_displayerElement.IsOn;
                    m_displayerElement.Toggle(newValue);
                    toggleMenu.NotifyValueChange(newValue);
                };

                string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
                StyleKeys labelKeys = new StyleKeys("", "", null);
                m_displayerElement.SetLabel(labelStylePath, labelKeys);
                m_displayerElement.Label.value = toggleMenu.ToString();
            }
            else
                throw new System.Exception("MenuItem must be a BooleanInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is BooleanInputMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            m_displayerElement.Reset();
        }
    }
}