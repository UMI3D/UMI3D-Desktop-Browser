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
    public class ToolboxWindowItem_E : Button_GE
    {
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
    }
}

