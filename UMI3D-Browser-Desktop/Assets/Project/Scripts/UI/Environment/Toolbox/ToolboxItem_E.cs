/*
Copyright 2019 Gfi Informatique

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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    /// <summary>
    /// A ToolboxItem is a button which has an Icon and a Label.
    /// </summary>
    public partial class ToolboxItem_E
    {
        public string ItemName { get; private set; } = null;

        private const string m_partialStylePath = "UI/Style";
        private const string m_uxmlPath = "UI/UXML/Toolbox/toolboxItem";
        private const string m_menuBarStyle = "UI/Style/MenuBar/MenuBar_ToolboxItem";
        private const string m_windowStyle = "UI/Style/ToolboxWindow/ToolboxWindow_ToolboxItem";
    }

    public partial class ToolboxItem_E : ButtonWithLabel_E
    {
        public ToolboxItem_E(bool isInMenuBar = true) :
            this("placeholderToolboxActive", "placeholderToolboxEnable", "", false, isInMenuBar)
        { }
        public ToolboxItem_E(string iconKey, string itemName, bool isInMenuBar = true) :
            this(iconKey, null, itemName, true, isInMenuBar)
        { }
        public ToolboxItem_E(string iconOnKey, string iconOffKey, string itemName, bool isOn = false) : 
            this(iconOnKey, iconOffKey, itemName, isOn, true)
        { }
        private ToolboxItem_E(string iconOnKey, string iconOffKey, string itemName, bool isOn = false, bool isInMenuBar = true) :
            base(m_uxmlPath, (isInMenuBar) ? m_menuBarStyle : m_windowStyle, null)
        {
            string buttonStyle = $"{m_partialStylePath}/Toolbox/ToolboxItem_Icon";
            StyleKeys buttonOnKeys = new StyleKeys(iconOnKey, null);
            if (iconOffKey != null)
            {
                StyleKeys buttonOffKeys = new StyleKeys(iconOffKey, null);
                SetButton(buttonStyle, buttonOnKeys, buttonOffKeys, isOn, null);
            }
            else
                SetButton(buttonStyle, buttonOnKeys, null);

            string labelStyle = $"{m_partialStylePath}/Toolbox/ToolboxItem_Label";
            StyleKeys labelKeys = new StyleKeys("", null, null);
            SetLabel(labelStyle, labelKeys);
            Label.value = itemName;
        }

        public void SetIcon(Texture2D icon)
        {
            UpdateVisualKeys(m_button, new StyleKeys(null, null));
            m_button.style.backgroundImage = icon;
        }

        public void SetItemStatus(bool isTool)
        {
            StyleKeys onKeys = new StyleKeys((isTool) ? "placeholderToolActive" : "placeholderToolboxActive", null);
            StyleKeys offKeys = new StyleKeys((isTool) ? "placeholderToolEnable" : "placeholderToolboxEnable", null);
            Element.UpdatesStyle(onKeys, offKeys, IsOn);
        }

        #region Setup

        //public ToolboxItem_E SetIcon(string iconOn, string iconOff, bool isOn = false)
        //{
        //    //ItemButton.SetIcon("square-button", iconOn, iconOff, isOn);
        //    return this;
        //}

        //public ToolboxItem_E SetIcon(Sprite icon)
        //{
        //    //ItemButton.SetIcon(icon);
        //    return this;
        //}

        #endregion
    }
    
    public partial class ToolboxItem_E
    {
        protected override void Initialize()
        {
            base.Initialize();
        }

        //public override void Display()
        //{
        //    Debug.Log($"display toolbox item [{m_label.text}]");
        //    Root.style.display = DisplayStyle.Flex;
        //}

        //public override void Hide()
        //{
        //    Debug.Log($"Hide toolbox item [{m_label.text}]");
        //    Root.style.display = DisplayStyle.None;
        //}
    }
}