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
    public partial class ToolboxItem_E : Button_GE
    {
        private const string m_partialStylePath = "UI/Style";

        private VisualElement m_icon = null;
        private Label m_label = null;
    }

    public partial class ToolboxItem_E : Button_GE
    {
        public ToolboxItem_E(bool isInMenuBar = true) :
             base("UI/UXML/Toolbox/toolboxItem",
                (isInMenuBar) ? $"{m_partialStylePath}/MenuBar/MenuBar_ToolboxItem" 
                 : $"{m_partialStylePath}/ToolboxWindow/ToolboxWindow_ToolboxItem",
                null)
        {
            SetIcon(m_icon,
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Icon",
                new StyleKeys("placeholder", null));
            SetLabel(m_label,
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Label",
                new StyleKeys("", "", null, null));
        }
        public ToolboxItem_E(string iconKey, string itemName, bool isInMenuBar = true) :
            base("UI/UXML/Toolbox/toolboxItem", 
                (isInMenuBar) ? $"{m_partialStylePath}/MenuBar/MenuBar_ToolboxItem" 
                : $"{m_partialStylePath}/ToolboxWindow/ToolboxWindow_ToolboxItem", 
                null)
        {
            SetIcon(m_icon, 
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Icon", 
                new StyleKeys(iconKey, null));
            SetLabel(m_label,
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Label",
                new StyleKeys(itemName, "", null, null));
        }
        public ToolboxItem_E(string iconOnKey, string iconOffKey, string itemName, bool isOn = false) : 
            base("UI/UXML/Toolbox/toolboxItem", 
                $"{m_partialStylePath}/MenuBar/MenuBar_ToolboxItem", 
                null, 
                isOn) 
        {
            SetIcon(m_icon, 
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Icon", 
                new StyleKeys(iconOnKey, null), 
                new StyleKeys(iconOffKey, null));
            SetLabel(m_label,
                $"{m_partialStylePath}/Toolbox/ToolboxItem_Label", 
                new StyleKeys(itemName, "", null, null));
        }

        public void SetIcon(Texture2D icon)
        {
            UpdateVisualKeys(m_icon, new StyleKeys(null, null));
            m_icon.style.backgroundImage = icon;
        }

        public void SetLabel(string text)
        {
            UpdateVisualKeys(m_label, new StyleKeys(text, "", null, null));
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
            m_icon = Root.Q<VisualElement>("icon");
            m_label = Root.Q<Label>("label");
        }

        public override void Display()
        {
            Debug.Log($"display toolbox item [{m_label.text}]");
            Root.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            Debug.Log($"Hide toolbox item [{m_label.text}]");
            Root.style.display = DisplayStyle.None;
        }
    }
}