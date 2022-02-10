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
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowButtonInputDisplayer
    {
        private ButtonWithLabel_E m_button { get; set; } = null;
        private Action m_buttonClicked { get; set; } = null;
    }

    public partial class WindowButtonInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_button = new ButtonWithLabel_E(UXMLPath, null, null);

            string buttonStylePath = "UI/Style/Displayers/ButtonInput";
            StyleKeys buttonKeys = new StyleKeys(menu.Name, "", "", null);
            m_button.SetButton(buttonStylePath, buttonKeys, m_buttonClicked);

            string labelStylePath = "UI/Style/Displayers/ButtonInputLabel";
            StyleKeys labelKeys = new StyleKeys("Button", "", "", null);
            m_button.SetLabel(labelStylePath, labelKeys);

            Displayer.AddDisplayer(m_button.Root);
        }
    }

    public partial class WindowButtonInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            if (menu is ButtonMenuItem buttonMenu)
            {
                m_buttonClicked = () => { buttonMenu.NotifyValueChange(!buttonMenu.GetValue()); };
            }
            else
                throw new System.Exception("MenuItem must be a ButtonInput");
            InitAndBindUI();
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is ButtonMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            m_button.Reset();
        }
    }
}