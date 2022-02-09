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
        public VisualElement ToolboxesContainer { get; private set; } = null;
        public Toolbox_E FirstToolbox { get; private set; } = null;
    }

    public partial class ToolboxWindowItem_E
    {
        public ToolboxWindowItem_E() :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item",
                null,
                null)
        {
            AddVisualStyle(ToolboxesContainer,
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxContainer",
                new StyleKeys(null, ""));
            FirstToolbox = new Toolbox_E(false);
            FirstToolbox.AddTo(ToolboxesContainer);
        }
        //public ToolboxWindowItem_E(Toolbox_E toolbox) :
        //    base("UI/UXML/ToolboxWindow/toolboxWindow_Item", 
        //        null, 
        //        null)
        //{
        //    SetIcon(PinnButton, 
        //        "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton",
        //        new StyleKeys("PinnedActive", ""),
        //        new StyleKeys("PinnedEnable", ""));
        //    AddVisualStyle(ToolboxesContainer, 
        //        "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxContainer", 
        //        new StyleKeys(null, ""));
        //    toolbox.AddTo(ToolboxesContainer);
        //}

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
            PinnButton = new Button_GE(Root.Q<Button>("pinButton"));
            PinnButton
                .SetIcon(PinnButton.Root,
                "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton",
                new StyleKeys("PinnedActive", ""),
                new StyleKeys("PinnedEnable", ""));
            ToolboxesContainer = Root.Q("toolboxesContainer");
        }

        //public override void Display()
        //{
        //    base.Display();
        //    Debug.Log($"Display window item [{FirstToolbox.ToolboxName.text}]");
        //}
    }
}

