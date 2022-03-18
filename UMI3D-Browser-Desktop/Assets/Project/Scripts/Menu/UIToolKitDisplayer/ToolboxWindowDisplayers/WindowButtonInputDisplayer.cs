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
    public partial class WindowButtonInputDisplayer
    {
        private ButtonWithLabel_E m_displayerElement { get; set; } = null;
    }

    public partial class WindowButtonInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_displayerElement = new ButtonWithLabel_E(UXMLPath);

            string buttonStylePath = "UI/Style/Displayers/InputButton";
            StyleKeys buttonKeys = new StyleKeys("", "", null);
            m_displayerElement.SetButton(buttonStylePath, buttonKeys, null);
            m_displayerElement.Element.Text = menu.Name;

            Displayer.AddDisplayer(m_displayerElement.Root);
        }
    }

    public partial class WindowButtonInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is ButtonMenuItem buttonMenu)
            {
                m_displayerElement.OnClicked = () => { buttonMenu.NotifyValueChange(!buttonMenu.GetValue()); };

                string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
                StyleKeys labelKeys = new StyleKeys("", "", null);
                m_displayerElement.SetLabel(labelStylePath, labelKeys);
                m_displayerElement.Label.value = buttonMenu.ToString();
            }
            else
                throw new System.Exception("MenuItem must be a ButtonMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
            => (menu is ButtonMenuItem) ? 2 : 0;

        public override void Clear()
        {
            base.Clear();
            m_displayerElement.Reset();
        }
    }
}