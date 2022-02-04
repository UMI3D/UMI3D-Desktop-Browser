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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class ToolboxWindowItem_E
    {
        public VisualElement Container => m_container;
        public Toolbox_E Toolbox => m_toolbox;

        private VisualElement m_container;
        private Toolbox_E m_toolbox;
    }

    public partial class ToolboxWindowItem_E : Button_GE
    {
        public ToolboxWindowItem_E() :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item",
                null,
                null)
        {
            SetIcon(Root.Q("pinButton"),
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton",
                new StyleKeys("PinnedActive", ""),
                new StyleKeys("PinnedEnable", ""));
            m_container = Root.Q("toolboxContainer");
            AddVisualStyle(m_container,
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxContainer",
                new StyleKeys(null, ""));
            m_toolbox = new Toolbox_E(false);
            m_toolbox.AddTo(m_container);
        }
        public ToolboxWindowItem_E(Toolbox_E toolbox) :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item", 
                null, 
                null)
        {
            SetIcon(Root.Q("pinButton"), 
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton",
                new StyleKeys("PinnedActive", ""),
                new StyleKeys("PinnedEnable", ""));
            VisualElement container = Root.Q("toolboxContainer");
            AddVisualStyle(container, 
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxContainer", 
                new StyleKeys(null, ""));
            toolbox.AddTo(container);
        }

        public void Adds(params Visual_E[] items)
        {
            m_toolbox.Adds(items);
        }

        public void Display()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}

