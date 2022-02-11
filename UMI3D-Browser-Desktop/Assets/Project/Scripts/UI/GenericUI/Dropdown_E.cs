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
using umi3DBrowser.UICustomStyle;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Dropdown_E
    {
        protected Button_E m_enumField { get; set; } = null;
        protected List<string> m_items { get; set; } = null;
        protected string m_currentValue { get; set; } = null;
    }

    public partial class Dropdown_E
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

        public void AddItem(string item)
        {
            m_items.Add(item);
        }

        public void SetDefaultValue(int index)
            => SetDefaultValue(m_items[index]);
        public void SetDefaultValue(string value)
        {
            if (!m_items.Contains(value))
                throw new Exception($"items doesn't contain [{value}]");
            SelectItem(value);
        }
    }

    public partial class Dropdown_E
    {
        protected void ShowMenu()
        {
            var menu = new GenericDropdownMenu();
            foreach (string item in m_items)
            {
                bool isSelected = item == m_currentValue;
                menu.AddItem(item, isSelected, () => SelectItem(item));
            }
            Debug.Log($"world bound = [{Root.worldBound}]; layout = [{Root.layout}]; local bound = [{Root.localBound}]");
            menu.contentContainer.style.backgroundColor = Color.black;
            menu.DropDown(Root.worldBound, Root, true);
        }

        protected void SelectItem(string item)
        {
            m_currentValue = item;
            StyleKeys newKeys = new StyleKeys(item, 
                m_currentKeys.TextStyleKey, 
                m_currentKeys.BackgroundStyleKey, 
                m_currentKeys.BorderStyleKey);
            UpdatesStyle(newKeys);
        }
    }

    public partial class Dropdown_E : Button_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_clicked += ShowMenu;
        }

        public override void Reset()
        {
            base.Reset();
            m_clicked -= ShowMenu;
        }
    }
}