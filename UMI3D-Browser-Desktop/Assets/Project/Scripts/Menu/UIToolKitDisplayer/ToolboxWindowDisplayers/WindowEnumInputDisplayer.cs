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
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine.UIElements;

namespace umi3d.DesktopBrowser.menu.Displayer
{
    public partial class WindowEnumInputDisplayer
    {
        private DropdownWithLabel_E m_displayer { get; set; } = null;
        private Dropdown_E m_dropdown 
        {
            get
            {
                if (m_displayer != null)
                    return m_displayer.ButtonE as Dropdown_E;
                else
                    return null;
            }
        }
        private List<string> m_options => ((DropDownInputMenuItem) menu).options;
        private string m_currentValue => ((DropDownInputMenuItem)menu).GetValue();
    }

    public partial class WindowEnumInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_displayer = new DropdownWithLabel_E(UXMLPath, null, null);

            string dropdownStyle = "UI/Style/Displayers/DropdownInput";
            StyleKeys dropdownKeys = new StyleKeys(null, "", "", "");
            m_displayer.SetButton(dropdownStyle, dropdownKeys, null);

            //Dropdown_E dropdown = new Dropdown_E(dropdownStyle, dropdownKeys);
            //m_displayer.SetButton(dropdown);

            string labelStylePath = "UI/Style/Displayers/ButtonInputLabel";
            StyleKeys labelKeys = new StyleKeys("Enum", "", "", null);
            m_displayer.SetLabel(labelStylePath, labelKeys);

            Displayer.AddDisplayer(m_displayer.Root);
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
                m_dropdown.SetsOptions(dropdownMenu.options);
                m_dropdown.SetDefaultValue(dropdownMenu.GetValue());
                m_dropdown.OnValueChanged = dropdownMenu.NotifyValueChange;
                //m_dropdown = () => { buttonMenu.NotifyValueChange(!buttonMenu.GetValue()); };
            }
            else
                throw new System.Exception("MenuItem must be a ButtonInput");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is DropDownInputMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            m_dropdown.Reset();
            m_displayer.Reset();
        }
    }
}