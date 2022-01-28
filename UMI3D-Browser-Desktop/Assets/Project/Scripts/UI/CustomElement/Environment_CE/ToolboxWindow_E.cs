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
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class ToolboxWindow_E
    {
        public UnityEvent UnPinedButtonPressed { get; set; }

        private const string m_partialStylePath = "UI/Style/ToolboxWindow";
        private ScrollView_E m_scrollView;
    }

    public partial class ToolboxWindow_E
    {
        public ToolboxWindow_E(VisualElement parent) : 
            base(parent, 
                "UI/UXML/ToolboxWindow/ToolboxWindow", 
                $"{m_partialStylePath}/toolboxWindow-window", 
                new FormatAndStyleKeys()) { }
    }

    public partial class ToolboxWindow_E
    {
        private void OnCloseButtonPressed()
        {
            //Collapse evrything
            //hide the window
            Root.style.display = DisplayStyle.None;
        }
    }

    public partial class ToolboxWindow_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            AddVisualStyle(Root.Q("icon"), 
                $"{m_partialStylePath}/Icon", 
                new FormatAndStyleKeys(null, null, "", null));
            AddVisualStyle(Root.Q<Label>("windowName"), 
                $"{m_partialStylePath}/WindowName", 
                new FormatAndStyleKeys("Toolbox", "", "", null));
            new Button_GE(Root.Q("closeButton"), $"{m_partialStylePath}/closeButton", null)
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            }.SetIcon(Root.Q("closeButton"), 
            $"{m_partialStylePath}/closeButton", 
            new FormatAndStyleKeys(null, null, "", null), 
            new FormatAndStyleKeys(null, null, "", null));

            m_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"), null, null)
                .SetVerticalDraggerContainerStyle($"{m_partialStylePath}/DraggerContainer", 
                new FormatAndStyleKeys(null, null, "", null))
                .SetVerticalDraggerStyle($"{m_partialStylePath}/Dragger",
                new FormatAndStyleKeys(null, null, "", null));

            new Button_GE(Root.Q("unpinnedButton"), $"{m_partialStylePath}/UnpinnedButton", null)
            {
                OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            }.SetIcon(Root.Q("unpinnedButton"),
            $"{m_partialStylePath}/UnpinnedButton", 
            new FormatAndStyleKeys(null, null, "", null),
            new FormatAndStyleKeys(null, null, "", null));
        }
    }
}