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
            base.InitAndBindUI();
            string UXMLPath = "UI/UXML/Displayers/buttonInputDisplayer";
            m_input = new Visual_E(UXMLPath, null, null);

            string dropdownStyle = "UI/Style/Displayers/InputDropdown";
            StyleKeys dropdownKeys = new StyleKeys("", "", "");
            m_dropdown = new Dropdown_E(m_input.Root.Q<Button>(), dropdownStyle, dropdownKeys);

            string dropdownMenuStyle = "UI/Style/Displayers/DropdownMenu";
            StyleKeys dropdownMenuKeys = new StyleKeys(null, "", null);
            m_dropdown.SetMenuStyle(dropdownMenuStyle, dropdownMenuKeys);

            string dropdownMenuLabelStyle = "UI/Style/Displayers/DropdownMenuItemLabel";
            StyleKeys dropdownMenuLabelKeys = new StyleKeys("", null, null);
            m_dropdown.SetMenuLabel(dropdownMenuLabelStyle, dropdownMenuLabelKeys);

            string labelStylePath = "UI/Style/Displayers/DisplayerLabel";
            StyleKeys labelKeys = new StyleKeys("", "", null);
            m_label = new Label_E(m_input.Root.Q<Label>(), labelStylePath, labelKeys);
            m_label.value = menu.Name;

            m_input.Add(m_label);
            m_input.Add(m_dropdown);

            Displayer.AddDisplayer(m_input.Root);
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
                m_dropdown.SetOptions(dropdownMenu.options);
                m_dropdown.SetDefaultValue(dropdownMenu.GetValue());
                m_dropdown.ValueChanged = dropdownMenu.NotifyValueChange;
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
        }
    }
}