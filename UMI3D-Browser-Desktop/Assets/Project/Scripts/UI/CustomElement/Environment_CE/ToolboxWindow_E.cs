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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class ToolboxWindow_E
    {
        public static ToolboxWindow_E Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ToolboxWindow_E();
                }
                return m_instance;
            }
        }
        public UnityEvent UnPinedButtonPressed { get; private set; } = new UnityEvent();

        private const string m_partialStylePath = "UI/Style/ToolboxWindow";
        private static ToolboxWindow_E m_instance;
        private ScrollView_E m_scrollView;
        private bool m_isDisplayed { get; set; } = false;
    }

    public partial class ToolboxWindow_E
    {
        private ToolboxWindow_E() : 
            base("UI/UXML/ToolboxWindow/toolboxWindow", 
                $"{m_partialStylePath}/ToolboxWindow_window", 
                new StyleKeys("", null)) { }
    }

    public partial class ToolboxWindow_E
    {
        public void Display()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }

        private void OnCloseButtonPressed()
            => Hide();
    }

    public partial class ToolboxWindow_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            AddVisualStyle(Root.Q("icon"), 
                $"{m_partialStylePath}/ToolboxWindow_Icon", 
                new StyleKeys("", ""));
            AddVisualStyle(Root.Q<Label>("windowName"), 
                $"{m_partialStylePath}/ToolboxWindow_Name", 
                new StyleKeys("Toolbox", "", "", ""));
            new Button_GE(Root.Q("closeButton"), 
                $"{m_partialStylePath}/ToolboxWindow_closeButton", 
                new StyleKeys("", ""))
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            }.SetIcon(Root.Q("closeButton"), 
            $"{m_partialStylePath}/ToolboxWindow_closeButton", 
            new StyleKeys("", null));

            m_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"), null, null)
                .SetVerticalDraggerContainerStyle($"{m_partialStylePath}/ToolboxWindow_DraggerContainer", 
                new StyleKeys("", null))
                .SetVerticalDraggerStyle($"{m_partialStylePath}/ToolboxWindow_Dragger",
                new StyleKeys("", ""));
            //m_scrollView.Scroll_View.style.alignItems = Align.Center;

            new Button_GE(Root.Q("unpinnedButton"),
                $"{m_partialStylePath}/ToolboxWindow_UnpinnedButton",
                new StyleKeys("", null))
            {
                OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            };
            //.SetIcon(Root.Q("unpinnedButton"),
            //$"{m_partialStylePath}/ToolboxWindow_UnpinnedButton",
            //new StyleKeys("", null));

            var tool = new ToolboxItem_E("toolbox", "test", false);
            var tool1 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox = new Toolbox_E("Test", false, tool, tool1);
            var item = new ToolboxWindowItem_E(toolbox);


            var tool2 = new ToolboxItem_E("toolbox", "test", false);
            var tool3 = new ToolboxItem_E("toolbox", "test", false);
            var tool4 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox1 = new Toolbox_E("Test", false, tool2, tool3);
            var item1 = new ToolboxWindowItem_E(toolbox1);

            var tool5 = new ToolboxItem_E("toolbox", "test", false);
            var tool6 = new ToolboxItem_E("toolbox", "test", false);
            var tool7 = new ToolboxItem_E("toolbox", "test", false);
            var tool8 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox2 = new Toolbox_E("Test", false, tool5, tool6, tool7, tool8);
            var item2 = new ToolboxWindowItem_E(toolbox2);

            var tool9 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox3 = new Toolbox_E("Test", false, tool9);
            var item3 = new ToolboxWindowItem_E(toolbox3);

            var tool10 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox4 = new Toolbox_E("Test", false, tool10);
            var item4 = new ToolboxWindowItem_E(toolbox4);

            var tool11 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox5 = new Toolbox_E("Test", false, tool11);
            var item5 = new ToolboxWindowItem_E(toolbox5);

            var tool12 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox6 = new Toolbox_E("Test", false, tool12);
            var item6 = new ToolboxWindowItem_E(toolbox6);

            var tool13 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox7 = new Toolbox_E("Test", false, tool13);
            var item7 = new ToolboxWindowItem_E(toolbox7);

            var tool14 = new ToolboxItem_E("toolbox", "test", false);
            var toolbox8 = new Toolbox_E("Test", false, tool14);
            var item8 = new ToolboxWindowItem_E(toolbox8);

            //m_scrollView.Adds(toolbox, toolbox1, toolbox2, toolbox3, toolbox4, toolbox5, toolbox6, toolbox7, toolbox8);
            m_scrollView.Adds(item, item1, item2, item3, item4, item5, item6, item7, item8);

            PopUpManipulator manipulator = new PopUpManipulator();
            UnregisterVisualCallback(Root);
            Root.AddManipulator(manipulator);
        }
    }
}