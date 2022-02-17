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
        public void Adds(params Visual_E[] items)
        {
            m_scrollView.Adds(items);
        }
    }

    public partial class ToolboxWindow_E
    {
        private void OnCloseButtonPressed()
            => Hide();
    }

    public partial class ToolboxWindow_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();

            string iconStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Icon";
            StyleKeys iconKeys = new StyleKeys(null, "", "");
            AddVisualStyle(Root.Q("icon"), iconStyle, iconKeys);

            string windowNameStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Name";
            StyleKeys windowNameKeys = new StyleKeys("", "", "");
            new Label_E(Root.Q<Label>("windowName"), windowNameStyle, windowNameKeys);

            Button closeButton = Root.Q<Button>("closeButton");
            string closeButtonStyle = "UI/Style/ToolboxWindow/ToolboxWindow_closeButton";
            StyleKeys closeButtonKeys = new StyleKeys("", "");
            new Button_E(closeButton, closeButtonStyle, closeButtonKeys)
            {
                OnClicked = () => { OnCloseButtonPressed(); }
            };

            m_scrollView = new ScrollView_E(Root.Q("scrollViewContainer"), null, null)
                .SetVerticalDraggerContainerStyle($"{m_partialStylePath}/ToolboxWindow_DraggerContainer", 
                new StyleKeys("", null))
                .SetVerticalDraggerStyle($"{m_partialStylePath}/ToolboxWindow_Dragger",
                new StyleKeys("", ""));
            //m_scrollView.Scroll_View.style.alignItems = Align.Center;


            Button unpinnedButton = Root.Q<Button>("unpinnedButton");
            string unpinnedButtonStyle = "UI/Style/ToolboxWindow/ToolboxWindow_UnpinnedButton";
            StyleKeys unpinnedButtonKeys = new StyleKeys("", null);
            new Button_E(unpinnedButton, unpinnedButtonStyle, unpinnedButtonKeys)
            {
                OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            };

            UpdateVisualManipulator(Root, new PopUpManipulator(true));
        }
    }
}