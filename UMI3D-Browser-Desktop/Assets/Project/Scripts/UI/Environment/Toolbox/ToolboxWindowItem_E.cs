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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.uI.viewController
{
    public partial class ToolboxWindowItem_E
    {
        public Button_GE PinnButton { get; private set; } = null;
        private VisualElement m_pin { get; set; } = null;
        private string m_pinStyle { get; set; } = null;

        private VisualElement m_containers { get; set; } = null;
        private string m_conainerStyle { get; set; } = null;

        public VisualElement ToolboxesContainer { get; private set; } = null;
        private string m_toolboxesContainerStyle { get; set; } = null;

        public Toolbox_E FirstToolbox { get; private set; } = null;
    }

    public partial class ToolboxWindowItem_E
    {
        public ToolboxWindowItem_E() :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item",
                null,
                null)
        {
            
            FirstToolbox = new Toolbox_E(false);
            FirstToolbox.AddTo(ToolboxesContainer);
        }

        public void Pin()
        {
            throw new System.NotImplementedException();
        }

        public void Adds(params Visual_E[] items)
        {
            FirstToolbox.Adds(items);
        }
    }

    public partial class ToolboxWindowItem_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_pin = Root.Q<Button>("pinButton");
            m_pinStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton";
            PinnButton = new Button_GE(m_pin);
            PinnButton.SetIcon(m_pin,
                m_pinStyle,
                new StyleKeys("PinnedActive", ""),
                new StyleKeys("PinnedEnable", ""));

            m_containers = Root.Q("containers");
            m_conainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_Container";
            AddVisualStyle(m_containers, m_conainerStyle, null);

            ToolboxesContainer = Root.Q("toolboxesContainer");
            m_toolboxesContainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxesContainer";
            AddVisualStyle(ToolboxesContainer,
                m_toolboxesContainerStyle,
                new StyleKeys(null, ""));
        }

        //public override void Display()
        //{
        //    base.Display();
        //    Debug.Log($"Display window item [{FirstToolbox.ToolboxName.text}]");
        //}
    }
}

