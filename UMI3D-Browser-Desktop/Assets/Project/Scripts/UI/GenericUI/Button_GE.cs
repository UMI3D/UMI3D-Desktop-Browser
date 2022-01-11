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
using DesktopBrowser.UI.CustomElement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.GenericElement
{
    public sealed partial class Button_GE
    {
        private ICustomisableElement m_icon;
    }

    public sealed partial class Button_GE
    {
        public Button_GE(VisualTreeAsset visualTA) : base(visualTA) { }

        public Button_GE(VisualElement root) : base(root) { }

        //public Button_GE(VisualElement root, string customStyleKey) : base(root, customStyleKey) { }
    }

    public sealed partial class Button_GE : AbstractGenericAndCustomElement
    {
        public string Text { get; set; } = "";
        public string TextPref { get; set; } = "";

        //public string IconClassOn { get; private set; } = "";
        //public string IconClassOff { get; private set; } = "";
        public string IconPref { get => m_icon.Key; set => m_icon.Key = value; }



        public Action OnClicked { get; set; } = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); };
        /// <summary>
        /// State of the button.
        /// </summary>
        public bool IsOn { get; private set; } = false;

        private Button button_B;

        
        private string currentClass;
        private IEnumerable<string> USSClassesIconPref = new List<string>();

        

        

        protected override void Initialize()
        {
            base.Initialize();
            button_B = Root.Q<Button>();
            this.button_B.clicked += () => { this.OnClicked(); };
            //m_icon = new Icon_E(button_B, null);
        }

        public override void Reset()
        {
            base.Reset();
            Text = "";
            TextPref = "";

            //IconClassOn = ""; 
            //IconClassOff = "";
            //IconPref = "";
            m_icon.Reset();

            OnClicked = () => { Debug.Log("<color=green>TODO: </color>" + $"ToolboxItem clicked not implemented"); }; 
            IsOn = false;
            button_B = null;
        }

        #region Set and Unset Icon

        public Button_GE SetIcon(string customStyleKey, string iconOnKey, string iconOffKey, bool isOn = false)
        {
            return this;
        }

        public Button_GE SetIcon(string iconOn, string iconOff, bool isOn = false)
        {
            //this.IconClassOn = iconOn;
            //this.IconClassOff = iconOff;
            m_icon.SetValues(iconOn, iconOff);
            this.IsOn = isOn;
            SwitchClass(this.IsOn);
            return this;
        }

        public Button_GE SetIcon(Texture2D icon)
        {
            if (icon != null)
                button_B.style.backgroundImage = Background.FromTexture2D(icon);
            else
                button_B.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        public Button_GE SetIcon(Sprite icon)
        {
            if (icon != null)
                button_B.style.backgroundImage = Background.FromSprite(icon);
            else
                button_B.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        public Button_GE UnSetIcon()
        {
            button_B.style.backgroundImage = StyleKeyword.Auto;
            return this;
        }

        #endregion

        #region To be deleted

        //public Button_GE Setup(System.Action onClicked, bool isOn = true, bool isReadyToDisplay = false)
        //{
        //    Initialize();

        //    this.Text = "";
        //    this.TextPref = "";
        //    //button_B.text = "";

        //    this.IconClassOn = "";
        //    this.IconClassOff = "";
        //    this.currentClass = "";
        //    this.IconPref = "";

        //    this.IsOn = isOn;

        //    OnClicked = onClicked;
        //    button_B.clicked += OnClicked;

        //    if (isReadyToDisplay)
        //        ReadyToDisplay();

        //    return this;
        //}

        //public Button_GE WithBackgroundImage(string classOn, string classOff, string iconPref)
        //{
        //    this.IconClassOn = classOn;
        //    this.IconClassOff = classOff;
        //    this.IconPref = iconPref;

        //    SwitchClass(this.IsOn);

        //    return this;
        //}

        //public Button_GE WithText(string text, string textPref)
        //{
        //    this.Text = text;
        //    this.TextPref = textPref;

        //    UserPreferences.TextAndIconPref.ApplyTextPref(button_B, textPref, text);

        //    return this;
        //}

        #endregion

        /// <summary>
        /// Switch between USS classes when button is on or off.
        /// </summary>
        /// <param name="value">True if the button should be on, false else.</param>
        public void SwitchClass(bool value)
        {
            //if (string.IsNullOrEmpty(IconClassOn) || string.IsNullOrEmpty(IconClassOff))
            //    return;
            if (m_icon.IsEmpty)
                return;
            IsOn = value;
            string theme = "darkTheme"; //TODO to be replace by theme checked.
            if (value)
            {
                //currentClass = $"{theme}-{IconClassOn}-btn";
                m_icon.DeselectLasCurrentValues();
                m_icon.SelectCurrentValues(0);
            }
            else
            {
                //currentClass = $"{theme}-{IconClassOff}-btn";
                m_icon.DeselectLasCurrentValues();
                m_icon.SelectCurrentValues(1);
            }
            
            //UserPreferences.TextAndIconPref.ApplyIconPref(button_B, IconPref, currentClass);
        }

        //private void UpdateUSSClasses()
        //{
        //    button_B.ClearClassList();

        //    button_B.AddToClassList(currentClass);
        //    foreach (string USSClass in USSClassesIconPref)
        //        button_B.AddToClassList(USSClass);
        //}

        public override void Remove()
        {
            base.Remove();

            button_B.clicked -= OnClicked;
        }

        public override void OnApplyUserPreferences()
        {
            if (!Displayed) return;

            if (!string.IsNullOrEmpty(TextPref))
                UserPreferences.TextAndIconPref.ApplyTextPref(button_B, TextPref, Text);
            else
                button_B.text = "";
        }
    }
}