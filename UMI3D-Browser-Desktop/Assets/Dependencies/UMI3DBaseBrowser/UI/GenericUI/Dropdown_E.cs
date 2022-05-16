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
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class Dropdown_E
    {
        public Action<string> ValueChanged { get; set; } = null;

        protected Box_E m_menu { get; set; } = null;

        protected List<string> m_items { get; set; } = null;
        protected string m_currentValue { get; set; } = null;
        protected string m_menuStyle { get; set; } = null;
        protected StyleKeys m_menuKeys { get; set; } = null;
        protected string m_menuItemStyle { get; set; } = null;
        protected StyleKeys m_menuItemKeys { get; set; } = null;
        protected string m_menuCheckmarkStyle { get; set; } = null;
        protected StyleKeys m_menuCheckmarkKeys { get; set; } = null;
        protected string m_menuLabelStyle { get; set; } = null;
        protected StyleKeys m_menuLabelKeys { get; set; } = null;
    
        public void SetMenuStyle(string styleResourcePath, StyleKeys keys)
        {
            m_menuStyle = styleResourcePath;
            m_menuKeys = keys;
        }
        public void SetMenuItemStyle(string styleResourcePath, StyleKeys keys)
        {
            m_menuItemStyle = styleResourcePath;
            m_menuItemKeys = keys;
        }
        public void SetMenuCheckmark(string styleResourcePath, StyleKeys keys)
        {
            m_menuCheckmarkStyle = styleResourcePath;
            m_menuCheckmarkKeys = keys;
        }
        public void SetMenuLabel(string styleResourcePath, StyleKeys keys)
        {
            m_menuLabelStyle = styleResourcePath;
            m_menuLabelKeys = keys;
        }

        public void AddItem(string item)
        {
            m_items.Add(item);
        }
        public void SetOptions(List<string> options)
        {
            m_items = options;
        }

        public void SetDefaultValue(int index)
            => SetDefaultValue(m_items[index]);
        public void SetDefaultValue(string value)
        {
            if (!m_items.Contains(value))
                throw new Exception($"items doesn't contain [{value}]");
            SelectItem(value);
        }

        protected void ShowMenu()
        {
            var menu = new GenericDropdownMenu();
            var menubox = new Box_E(menu.contentContainer, m_menuStyle, m_menuKeys);
            m_items.ForEach((item) =>
            {
                menu.AddItem(item, item == m_currentValue, () => SelectItem(item));
            });
            foreach (VisualElement row in menu.contentContainer.Children())
            {
                var item = new DropdownItem_E(row, m_menuItemStyle, m_menuItemKeys);
                item.SetCheckmark(m_menuCheckmarkStyle, m_menuCheckmarkKeys);
                item.SetLabel(m_menuLabelStyle, m_menuLabelKeys);
            }
            menu.DropDown(Root.worldBound, Root, true);
            MakeDropDownAbsolute(menu);
        }

        protected void MakeDropDownAbsolute(GenericDropdownMenu menu)
        {
            var parent = menu.contentContainer.parent;
            for (int i = 0; i < 3; ++i)
                parent = parent.parent;

            parent.style.position = Position.Absolute;
        }

        protected void SelectItem(string item)
        {
            m_currentValue = item;
            Text = item;
            ValueChanged?.Invoke(item);
        }
    }

    public partial class Dropdown_E : Button_E
    {
        public Dropdown_E(string styleResourcePath, StyleKeys keys) :
            this(styleResourcePath, keys, new List<string>())
        { }
        public Dropdown_E(string styleResourcePath, StyleKeys keys, List<string> items) :
            this(new Button(), styleResourcePath, keys, items)
        { }
        public Dropdown_E(Button button, string styleResourcePath, StyleKeys keys) :
            this(button, styleResourcePath, keys, new List<string>())
        { }
        public Dropdown_E(Button button, string styleResourcePath, StyleKeys keys, List<string> items) :
            base(button, styleResourcePath, keys)
        {
            m_items = items;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Clicked += ShowMenu;
        }
    }
}