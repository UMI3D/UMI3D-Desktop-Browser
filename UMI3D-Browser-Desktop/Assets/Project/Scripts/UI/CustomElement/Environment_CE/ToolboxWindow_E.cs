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
using DesktopBrowser.UI.GenericElement;
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
        public ToolboxWindow_E(VisualElement parent) : base(parent, "UI/UXML/ToolboxWindow/ToolboxWindow", null, "") { }

        private void OnCloseButtonPressed()
        {
            //Collapse evrything
            //hide the window
            Root.style.display = DisplayStyle.None;
        }
    }

    public partial class ToolboxWindow_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();

            new Icon_E(Root.Q("icon"), "toolboxWindow-icon");
            //TODO Add label.
            new Button_GE(Root.Q("closeButton"))
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            }.SetIcon(Root.Q("closeButton"),"toolboxWindow-closeButton", "", "");

            //scrollViewData = new AbstractScrollView_E(Root.Q("scrollViewContainer"), "UI/UXML/ToolboxWindow/ToolboxWindow-ScrollView", null)
            //{
            //}.InitFromSrollViewToProperties();
            scrollViewData = new AbstractScrollView_E(Root.Q("scrollViewContainer"), null)
            {
            }
            .SetVerticalDraggerContainerStyle("ToolboxWindow-ScrollView", "DraggerContainer")
            .SetVerticalDraggerStyle("ToolboxWindow-ScrollView", "Dragger");

            new Button_GE(Root.Q("unpinnedButton"))
            {
                OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            }.SetIcon(Root.Q("unpinnedButton"),"toolboxWindow-unpinnedButton", "", "");
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