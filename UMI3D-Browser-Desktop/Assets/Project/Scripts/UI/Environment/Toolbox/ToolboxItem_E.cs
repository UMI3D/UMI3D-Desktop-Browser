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
        public enum ItemType { Undefine, Tool, Toolbox }

        public Label_E Label { get; protected set; } = null;
        public Button_E Button { get; protected set; } = null;
        public Icon_E Icon { get; protected set; } = null;

        public void Toggle(bool value)
            => Button?.Toggle(value);

        public void SetName(string itemName)
        {
            Name = itemName;
            Label.value = itemName;
        }

        public void SetIcon(Texture2D icon)
        {
            Button.UpdateRootKeys(null);
            Button.Root.style.backgroundImage = icon;
            Icon.Hide();
        }
        public void SetIcon(ItemType type)
        {
            switch (type)
            {
                case ItemType.Undefine:
                    break;
                case ItemType.Tool:
                    Button.UpdateStateKeys(Icon, StyleKeys.Bg("placeholderToolActive"), StyleKeys.Bg("placeholderToolEnable"));
                    break;
                case ItemType.Toolbox:
                    Button.UpdateStateKeys(Icon, StyleKeys.Bg("placeholderToolboxActive"), StyleKeys.Bg("placeholderToolboxEnable"));
                    break;
                default:
                    break;
            }
        }

        public static ToolboxItem_E NewMenuItem(string itemName, ItemType type = ItemType.Undefine)
        {
            var item = new ToolboxItem_E("ItemMenu");
            item.Button.UpdateStateKeys(item.Button, StyleKeys.Default_Bg_Border, StyleKeys.Default_Bg_Border);
            item.SetName(itemName);
            //item.SetIcon(type);
            
            return item;
        }

        public static ToolboxItem_E NewWindowItem(string itemName, ItemType type = ItemType.Undefine)
        {
            var item = new ToolboxItem_E("ItemWindow");
            item.Button.UpdateStateKeys(item.Button, StyleKeys.DefaultBackground, StyleKeys.DefaultBackground);
            item.SetName(itemName);
            item.SetIcon(type);

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

    public partial class ToolboxItem_E : Box_E
    {
        private ToolboxItem_E(string partialStyle) :
            base ("UI/UXML/Toolbox/toolboxItem", partialStyle, null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            Label = new Label_E(QR<Label>(), "CorpsToolboxItem", StyleKeys.DefaultText);
            Button = new Button_E(Root.Q<Button>(), "Square_m", null);
            Button.Clicked += OnClicked;

            Icon = new Icon_E();
            Button.AddIconInFront(Icon, "Square2", null, null);
            Button.Root.style.alignContent = Align.Center;
            Button.Root.style.alignItems = Align.Center;
        }
    }
}