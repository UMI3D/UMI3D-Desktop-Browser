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
using DesktopBrowser.UI.GenericElement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public partial class ToolboxWindow_E
    {
        public UnityEvent UnPinedButtonPressed { get; set; }

        private IScrollable scrollViewData;
    }

    public partial class ToolboxWindow_E
    {
        public ToolboxWindow_E(VisualElement root) : base(root) { }

        private void OnCloseButtonPressed()
        {
            //Collapse evrything
            //hide the window
            Root.style.display = DisplayStyle.None;
        }
        private Icon_E icon;
    }

    public partial class ToolboxWindow_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();

            scrollViewData = new AbstractScrollView_E(Root.Q("scrollView"))
            {
            }.InitFromSrollViewToProperties();

            icon = new Icon_E(Root.Q("icon"), "toolboxWindow-icon");

            new Button_GE(Root.Q("closeButton"))
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            }.SetIcon("toolboxWindow-closeButton", "", "");

            new Button_GE(Root.Q("unpinnedButton"))
            {
                OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            }.SetIcon("toolboxWindow-unpinnedButton", "", "");
        }

        public override bool GetCustomStyle()
        {
            return GetCustomStyle("toolboxWindow-window");
        }

        public override void OnApplyUserPreferences()
        {
            
        }
    }
}