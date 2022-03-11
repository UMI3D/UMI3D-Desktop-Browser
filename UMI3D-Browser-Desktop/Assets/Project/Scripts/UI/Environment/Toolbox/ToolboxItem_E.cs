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

namespace umi3dDesktopBrowser.ui.viewController
{
    /// <summary>
    /// A ToolboxItem is a button which has an Icon and a Label.
    /// </summary>
    public partial class ToolboxItem_E
    {
        private static string m_uxmlPath => "UI/UXML/Toolbox/toolboxItem";
        private static string m_menuBarStyle => "UI/Style/MenuBar/MenuBar_ToolboxItem";
        private static string m_windowStyle => "UI/Style/ToolboxWindow/ToolboxWindow_ToolboxItem";
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
            string buttonStyle = "UI/Style/Toolbox/ToolboxItem_Icon";
            StyleKeys buttonOnKeys = new StyleKeys(null, iconOnKey, null);
            if (iconOffKey != null)
            {
                StyleKeys buttonOffKeys = new StyleKeys(null, iconOffKey, null);
                SetButton(buttonStyle, buttonOnKeys, buttonOffKeys, isOn, null);
            }
            else
                SetButton(buttonStyle, buttonOnKeys, null);

            string labelStyle = "UI/Style/Toolbox/ToolboxItem_Label";
            StyleKeys labelKeys = new StyleKeys("", null, null);
            SetLabel(labelStyle, labelKeys);
            Label.value = itemName;
        }

        public void SetIcon(Texture2D icon)
        {
            UpdateVisualKeys(m_button, null);
            m_button.style.backgroundImage = icon;
        }

        /// <summary>
        /// A ToolboxItem can be a toolbox or a tool. This item will be set has tool if [isTool] is set to true, else toolbox.
        /// </summary>
        /// <param name="isTool"></param>
        public void SetItemStatus(bool isTool)
        {
            StyleKeys onKeys = new StyleKeys(null, (isTool) ? "placeholderToolActive" : "placeholderToolboxActive", null);
            StyleKeys offKeys = new StyleKeys(null, (isTool) ? "placeholderToolEnable" : "placeholderToolboxEnable", null);
            Element.UpdatesStyle(onKeys, offKeys, IsOn);
        }
    }
    
    public partial class ToolboxItem_E
    { }
}