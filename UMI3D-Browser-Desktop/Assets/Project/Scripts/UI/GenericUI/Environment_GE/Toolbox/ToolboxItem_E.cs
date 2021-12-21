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
using BrowserDesktop.UI;
using BrowserDesktop.UserPreferences;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    /// <summary>
    /// A ToolboxItem has a button and a label.
    /// </summary>
    public class ToolboxItem_E : AbstractGenericAndCustomElement
    {
        #region Fields

        public string ItemName { get; set; } = "";
        public string ItemIconOn { get; protected set; } = "";
        public string ItemIconOff { get; protected set; } = "";
        public Action ItemClicked { get; set; } = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); };

        /// <summary>
        /// Button of this ToolboxItem.
        /// </summary>
        private Button button;
        /// <summary>
        /// Button's label (name of this item.
        /// </summary>
        private Label label;

        /// <summary>
        /// State of the button.
        /// </summary>
        private bool isOn = false;
        
        
        private string currentClass;

        #endregion

        public ToolboxItem_E(VisualTreeAsset visualTA, bool isOn = true): base(visualTA) 
        {
            this.isOn = isOn;
            button.clicked += () => { ItemClicked(); };
        }

        protected override void Initialize()
        {
            base.Initialize();
            button = this.Q<Button>("toolboxItem-button");
            label = this.Q<Label>("toolboxItem-label");
        }

        #region Setup

        public ToolboxItem_E SetIcon(string iconOn, string iconOff)
        {
            this.ItemIconOn = iconOn;
            this.ItemIconOff = iconOff;
            SwitchClass(this.isOn);
            return this;
        }

        public ToolboxItem_E SetIcon(Texture2D icon)
        {
            if (icon != null)
                button.style.backgroundImage = Background.FromTexture2D(icon);
            else
                button.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        public ToolboxItem_E SetIcon(Sprite icon)
        {
            button.style.backgroundImage = Background.FromSprite(icon);
            return this;
        }

        #endregion

        /// <summary>
        /// Switch between USS classes when button is on or off.
        /// </summary>
        /// <param name="value">True if the button should be on, false else.</param>
        public void SwitchClass(bool value)
        {
            if (string.IsNullOrEmpty(ItemIconOn) || string.IsNullOrEmpty(ItemIconOff))
                return;
            isOn = value;
            string className = "darkTheme-menuBar-"; //TODO to be replace by theme checked.
            if (value)
            {
                currentClass = className + ItemIconOn + "-btn";
            }
            else
            {
                currentClass = className + ItemIconOff + "-btn";
            }
            button.ClearClassList();
            button.AddToClassList(currentClass);
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public override void OnApplyUserPreferences()
        {
            if (!Displayed)
                return;

            label.style.width = StyleKeyword.Auto;
            UserPreferences.TextAndIconPref.ApplyTextPref(label, "label", ItemName);
            UserPreferences.TextAndIconPref.ApplyIconPref(button, "square-button", iconClass: currentClass);
        }
    }
}