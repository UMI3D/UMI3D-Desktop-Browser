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
    /// A ToolboxItem is a button which has an Icon and a Label.
    /// </summary>
    public partial class ToolboxItem_E : Button_GE
    {

        public ToolboxItem_E(string iconStyleResourcePath, FormatAndStyleKeys iconOnKeys, FormatAndStyleKeys iconOffKeys, string labelStyleResourcePath, FormatAndStyleKeys labelOnKeys, FormatAndStyleKeys labelOffKeys) : 
            base("UXML/Toolbox/ToolboxItem", null, null) 
        {
            SetIcon(Root.Q<VisualElement>("icon"), iconStyleResourcePath, iconOnKeys, iconOffKeys);
            SetLabel(Root.Q<Label>("label"), labelStyleResourcePath, labelOnKeys, labelOffKeys);
        }

        #region Setup

        //public ToolboxItem_E SetIcon(string iconOn, string iconOff, bool isOn = false)
        //{
        //    //ItemButton.SetIcon("square-button", iconOn, iconOff, isOn);
        //    return this;
        //}

        //public ToolboxItem_E SetIcon(Texture2D icon)
        //{
        //    //ItemButton.SetIcon(icon);
        //    return this;
        //}

        //public ToolboxItem_E SetIcon(Sprite icon)
        //{
        //    //ItemButton.SetIcon(icon);
        //    return this;
        //}

        #endregion

        ///// <summary>
        ///// Apply user preferences when needed.
        ///// </summary>
        //public override void OnApplyUserPreferences()
        //{
        //    if (!Displayed)
        //        return;

        //    label.style.width = StyleKeyword.Auto;
        //    UserPreferences.TextAndIconPref.ApplyTextPref(label, "label", ItemName);
        //}
    }
}