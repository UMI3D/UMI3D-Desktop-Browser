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
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowEnumInputDisplayer
    {
        private Dropdown_E m_dropdown { get; set; } = null;
    }

    public partial class WindowEnumInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            Displayer = new View_E(UXMLPath, s_displayerStyle, null);

            base.InitAndBindUI();
        }
    }

    public partial class WindowEnumInputDisplayer : AbstractWindowInputDisplayer
    {
        public override void SetMenuItem(AbstractMenuItem menu)
        {
            base.SetMenuItem(menu);
            InitAndBindUI();
            if (menu is DropDownInputMenuItem dropdownMenu)
            {
                m_dropdown = new Dropdown_E(Displayer.QR<Button>(), "Displayer", StyleKeys.Text_Bg("enum"));
                m_dropdown.SetMenuStyle("EnumBox", StyleKeys.DefaultBackgroundAndBorder);
                m_dropdown.SetMenuLabel("CorpsDropdown", StyleKeys.DefaultText);
                Displayer.Add(m_dropdown);

                m_dropdown.SetOptions(dropdownMenu.options);
                m_dropdown.SetDefaultValue(dropdownMenu.GetValue());
                m_dropdown.ValueChanged = dropdownMenu.NotifyValueChange;
            }
            else
                throw new System.Exception("MenuItem must be a DropDownInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
            => (menu is DropDownInputMenuItem) ? 2 : 0;

        public override void Clear()
        {
            base.Clear();
        }
    }
}