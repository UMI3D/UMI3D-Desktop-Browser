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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class Toolbox_E
    {
        
    }

    public partial class Toolbox_E
    {
        public Label ToolboxName { get;  private set; } = null;
    }

    public partial class Toolbox_E
    {
        public Toolbox_E(bool isInMenuBar = true) :
            this(null, isInMenuBar) { }
        public Toolbox_E(string toolboxName, bool isInMenuBar = true, params ToolboxItem_E[] items) : 
            base("UI/UXML/Toolbox/Toolbox", 
                (isInMenuBar) ? "UI/Style/MenuBar/MenuBar_Toolbox" : "UI/Style/ToolboxWindow/ToolboxWindow_Toolbox", 
                new StyleKeys( "", ""))
        {
            AddVisualStyle(ToolboxName, 
                "UI/Style/Toolbox/ToolboxName", 
                new StyleKeys(toolboxName, "", null, null));
            if (toolboxName == null) ToolboxName.style.display = DisplayStyle.None;

            var backward = Root.Q("backward");
            var forward = Root.Q("forward");
            SetHorizontalBackwardButtonStyle(backward.Q("backwardButton"),
                backward,
                "UI/Style/Toolbox/Toolbox_ScrollViewButton",
                new StyleKeys("backward", null));
            SetHorizontalForwarddButtonStyle(forward.Q("forwardButton"),
                forward,
                "UI/Style/Toolbox/Toolbox_ScrollViewButton",
                new StyleKeys("forward", null));
            if (isInMenuBar)
            {
                backward.style.display = DisplayStyle.None;
                forward.style.display = DisplayStyle.None;
            }

            if (items.Length > 0)
                Adds(items);
        }

        public void SetToolboxName(string text)
        {
            if (text == null) ToolboxName.style.display = DisplayStyle.None;
            else ToolboxName.style.display = DisplayStyle.Flex;
            UpdateVisualKeys(ToolboxName, new StyleKeys(text, "", null, null));
        }
    }

    public partial class Toolbox_E : ScrollView_E
    {
        protected override void Initialize()
        {
            VisualElement scrollView = GetVisualRoot("UI/UXML/horizontalScrollView");
            Root.Q("scrollViewContainer").Add(scrollView);
            base.Initialize();
            ToolboxName = Root.Q<Label>();
        }

        public override void Display()
        {
            base.Display();
            Debug.Log($"Display toolbox [{ToolboxName.text}]");
        }

        public override void Hide()
        {
            base.Hide();
            Debug.Log($"Hide toolbox [{ToolboxName.text}]");
        }
    }
}

