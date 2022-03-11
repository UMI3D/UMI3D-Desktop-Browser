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
        private DropdownWithLabel_E m_displayerElement { get; set; } = null;
        private List<string> m_options => ((DropDownInputMenuItem) menu).options;
        private string m_currentValue => ((DropDownInputMenuItem)menu).GetValue();
    }

    public partial class WindowEnumInputDisplayer : IDisplayerElement
    {
        public override void InitAndBindUI()
        {
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_displayerElement = new DropdownWithLabel_E(UXMLPath, null, null);

            string dropdownStyle = "UI/Style/Displayers/InputDropdown";
            StyleKeys dropdownKeys = new StyleKeys("", "", "");
            m_displayerElement.SetButton(dropdownStyle, dropdownKeys, null);
            string dropdownMenuStyle = "UI/Style/Displayers/DropdownMenu";
            StyleKeys dropdownMenuKeys = new StyleKeys(null, "", null);
            m_displayerElement.SetMenuStyle(dropdownMenuStyle, dropdownMenuKeys);
            string dropdownMenuLabelStyle = "UI/Style/Displayers/DropdownMenuItemLabel";
            StyleKeys dropdownMenuLabelKeys = new StyleKeys("", null, null);
            m_displayerElement.SetMenuItemLabel(dropdownMenuLabelStyle, dropdownMenuLabelKeys);

            Displayer.AddDisplayer(m_displayerElement.Root);
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
                m_displayerElement.Element.SetsOptions(dropdownMenu.options);
                m_displayerElement.Element.SetDefaultValue(dropdownMenu.GetValue());
                m_displayerElement.Element.OnValueChanged = dropdownMenu.NotifyValueChange;

                string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
                StyleKeys labelKeys = new StyleKeys("", "", null);
                m_displayerElement.SetLabel(labelStylePath, labelKeys);
                m_displayerElement.Label.value = dropdownMenu.ToString();
            }
            else
                throw new System.Exception("MenuItem must be a DropDownInputMenuItem");
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is DropDownInputMenuItem) ? 2 : 0;
        }

        public override void Clear()
        {
            base.Clear();
            m_displayerElement.Reset();
        }
    }
}