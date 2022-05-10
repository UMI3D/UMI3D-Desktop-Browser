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
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    /// <summary>
    /// A ToolboxItem is a button which has an Icon and a Label.
    /// </summary>
    public partial class ToolboxItem_E
    {
        public Label_E Label { get; protected set; } = null;
        public Button_E Button { get; protected set; } = null;

        private static string m_menuBarStyle => "UI/Style/MenuBar/MenuBar_ToolboxItem";
        private static string m_windowStyle => "UI/Style/ToolboxWindow/ToolboxWindow_ToolboxItem";

        public void Toggle(bool value)
            => Button?.Toggle(value);

        public void SetIcon(Texture2D icon)
        {
            Button.UpdateRootKeys(null);
            Button.Root.style.backgroundImage = icon;
        }

        /// <summary>
        /// A ToolboxItem can be a toolbox or a tool. This item will be set has tool if [isTool] is set to true, else toolbox.
        /// </summary>
        /// <param name="isTool"></param>
        public void SetItemStatus(bool isTool)
        {
            StyleKeys onKeys = new StyleKeys(null, (isTool) ? "placeholderToolActive" : "placeholderToolboxActive", null);
            StyleKeys offKeys = new StyleKeys(null, (isTool) ? "placeholderToolEnable" : "placeholderToolboxEnable", null);
            Button.AddStateKeys(Button, "ToolboxItem_Icon", onKeys, offKeys);
        }

        public static ToolboxItem_E NewItem(string on, string iconOffKey, string itemName, bool isOn = false)
        {
            var item = new ToolboxItem_E();

            return item;
        }

        public static ToolboxItem_E NewPlaceHolder()
        {
            var item = new ToolboxItem_E();

            return item;
        }
    }

    public partial class ToolboxItem_E : IClickableElement
    {
        public event Action Clicked;

        public void ResetClickedEvent()
            => Clicked = null;
        public void OnClicked()
            => Clicked?.Invoke();
        
    }

    public partial class ToolboxItem_E : View_E
    {
        public ToolboxItem_E(bool isInMenuBar = true) :
            this("placeholderToolboxActive", "placeholderToolboxEnable", "", false, isInMenuBar)
        { }
        public ToolboxItem_E(string iconKey, string itemName) :
            this(iconKey, null, itemName, true, true)
        { }
        private ToolboxItem_E(string iconOnKey, string iconOffKey, string itemName, bool isOn = false, bool isInMenuBar = true) :
            base("UI/UXML/Toolbox/toolboxItem", (isInMenuBar) ? m_menuBarStyle : m_windowStyle, null)
        {
            StyleKeys buttonOnKeys = new StyleKeys(null, iconOnKey, null);
            if (iconOffKey != null)
            {
                StyleKeys buttonOffKeys = new StyleKeys(null, iconOffKey, null);
                Button.AddStateKeys(Button, "ToolboxItem_Icon", buttonOnKeys, buttonOffKeys);
                Button.Toggle(isOn);
            }
            else
                Button.UpdateRootStyleAndKeysAndManipulator("ToolboxItem_Icon", buttonOnKeys);

            Name = itemName;
            Label.value = itemName;
        }

        private ToolboxItem_E(string partialStyle) :
            base ("UI/UXML/Toolbox/toolboxItem", partialStyle, null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            Label = new Label_E(QR<Label>(), "CorpsToolboxItem", StyleKeys.DefaultText);
            Button = new Button_E(Root.Q<Button>(), "ToolboxItem_Icon", null);
            Button.Clicked += OnClicked;
        }
    }
}