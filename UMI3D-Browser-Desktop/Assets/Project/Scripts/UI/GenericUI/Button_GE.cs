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
using BrowserDesktop.UI;
using BrowserDesktop.UserPreferences;
using DesktopBrowser.UI.CustomElement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    /// <summary>
    /// A button is a clickable area. It can have an icon or/and a label.
    /// </summary>
    public partial class Button_GE
    {
        public string Text { get; set; } = "";
        public string TextPref { get; set; } = "";

        public Action OnClicked { get; set; } = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); };
        /// <summary>
        /// State of the button.
        /// </summary>
        public bool IsOn { get; private set; } = false;
    }

    public partial class Button_GE
    {
        private Button m_button;
        private Icon_E m_icon;
        private Label_E m_label;
        private string m_iconOnKey = null;
        private string m_iconOffKey = null;
    }

    public partial class Button_GE
    {
        public Button_GE(VisualTreeAsset visualTA) : base(visualTA) { }
        //public Button_GE(VisualElement root) : base(root) { }

        public Button_GE(VisualElement root, string customStyleKey = null) : base(root, customStyleKey) { }

        public Button_GE SetIcon(VisualElement icon, string customStyleKey, string iconOnKey, string iconOffKey, bool isOn = false)
        {
            Debug.Assert(icon != null, "visual element null");
            m_iconOnKey = iconOnKey;
            m_iconOffKey = iconOffKey;
            CustomStyleBackgroundKey = isOn ? m_iconOnKey : m_iconOffKey;
            m_icon = new Icon_E(icon, customStyleKey, CustomStyleBackgroundKey);
            return this;
        }

        public Button_GE SetLabel(TextElement label, string customStyleKey)
        {
            Debug.Assert(label != null, "visual element null");
            return this;
        }

        public void Toggle(bool value)
        {
            IsOn = value;
            m_icon?.ChangeBackground((IsOn) ? m_iconOnKey : m_iconOffKey);
            //TODO m_label?.change
        }
    }

    public partial class Button_GE : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_button = Root.Q<Button>();
            this.m_button.clicked += () => { this.OnClicked(); };
        }

        public override void Reset()
        {
            base.Reset();
            Text = "";
            TextPref = "";

            m_icon.Reset();

            OnClicked = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); }; 
            IsOn = false;
            m_button = null;
        }

        #region Set and Unset Icon

        public Button_GE SetIcon(Texture2D icon)
        {
            if (icon != null)
                m_button.style.backgroundImage = Background.FromTexture2D(icon);
            else
                m_button.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        public Button_GE SetIcon(Sprite icon)
        {
            if (icon != null)
                m_button.style.backgroundImage = Background.FromSprite(icon);
            else
                m_button.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        public Button_GE UnSetIcon()
        {
            m_button.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        #endregion


        public override void Remove()
        {
            base.Remove();
            m_button.clicked -= OnClicked;
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed) return;

            if (!string.IsNullOrEmpty(TextPref))
                UserPreferences.TextAndIconPref.ApplyTextPref(m_button, TextPref, Text);
            else
                m_button.text = "";
        }
    }
}