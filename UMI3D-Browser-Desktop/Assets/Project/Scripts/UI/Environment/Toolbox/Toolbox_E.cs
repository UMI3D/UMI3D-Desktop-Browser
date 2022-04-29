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

namespace umi3dDesktopBrowser.ui.viewController
{
    public enum ToolboxType { Pinned, SubPinned, Popup }
    public partial class Toolbox_E
    {
        public Label_E ToolboxName { get; protected set; } = null;

        protected Label m_name { get; private set; } = null;

        private static string m_toolboxResourcePath => "UI/UXML/Toolbox/Toolbox";
        private static string m_toolboxPinnedStyle => "UI/Style/MenuBar/MenuBar_ToolboxPinned";
        private static string m_toolboxSubPinnedStyle => "UI/Style/MenuBar/MenuBar_ToolboxSubPinned";
        private static string m_toolboxPopupStyle => "UI/Style/ToolboxWindow/ToolboxWindow_Toolbox";
        private static StyleKeys m_toolboxKeys => new StyleKeys(null, "", "");
        private static string GetToolboxType(ToolboxType type)
        {
            switch (type)
            {
                case ToolboxType.Pinned:
                    return m_toolboxPinnedStyle;
                case ToolboxType.SubPinned:
                    return m_toolboxSubPinnedStyle;
                case ToolboxType.Popup:
                    return m_toolboxPopupStyle;
                default:
                    throw new System.Exception();
            }
        }
    }

    public partial class Toolbox_E
    {
        public Toolbox_E(ToolboxType type = ToolboxType.Pinned) :
            this(null, type) { }
        public Toolbox_E(string toolboxName, ToolboxType type = ToolboxType.Pinned, params ToolboxItem_E[] items) : 
            base(m_toolboxResourcePath, GetToolboxType(type), m_toolboxKeys)
        {
            string nameStyle = "UI/Style/Toolbox/ToolboxName";
            StyleKeys nameKeys = new StyleKeys("", null, null);
            ToolboxName = new Label_E(m_name, nameStyle, nameKeys);
            SetToolboxName(toolboxName);

            var backwardLayout = Root.Q("backward");
            var backwardContainer = backwardLayout.Q("backwardButton");
            string buttonStyle = "UI/Style/Toolbox/Toolbox_ScrollViewButton";
            StyleKeys backwardKeys = new StyleKeys(null, "backward", null);
            SetHorizontalBackwardButtonStyle(backwardContainer, backwardLayout, buttonStyle, backwardKeys);

            var forwardLayout = Root.Q("forward");
            var forwardContainer = forwardLayout.Q("forwardButton");
            StyleKeys forwardKeys = new StyleKeys(null, "forward", null);
            SetHorizontalForwarddButtonStyle(forwardContainer, forwardLayout, buttonStyle, forwardKeys);

            if (type == ToolboxType.Pinned)
            {
                backwardLayout.style.display = DisplayStyle.None;
                forwardLayout.style.display = DisplayStyle.None;
            }

            if (items.Length > 0)
                AddRange(items);
        }

        public void SetToolboxName(string text)
        {
            if (text == null) m_name.style.display = DisplayStyle.None;
            else m_name.style.display = DisplayStyle.Flex;
            ToolboxName.value = text;
        }
    }

    public partial class Toolbox_E : ScrollView_E
    {
        protected override void Initialize()
        {
            VisualElement scrollView = GetVisualRoot("UI/UXML/horizontalScrollView");
            Root.Q("scrollViewContainer").Add(scrollView);
            base.Initialize();
            m_name = Root.Q<Label>();
        }
    }
}

