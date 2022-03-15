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
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ToolboxWindowItem_E
    {
        public event Action OnPinned;

        public ButtonWithIcon_E PinnButton { get; private set; } = null;
        public Toolbox_E FirstToolbox { get; private set; } = null;

        private VisualElement m_toolboxesContainer { get; set; } = null;
        private VisualElement m_displayersContainer { get; set; } = null;
    }

    public partial class ToolboxWindowItem_E
    {
        public void Pin()
            => OnPinned?.Invoke();

        public void SetFirstToolboxName(string name)
            => FirstToolbox.SetToolboxName(name ?? "");

        public void AddToolboxItemInFirstToolbox(params Visual_E[] items)
            => FirstToolbox.Adds(items);

        public void AddToolbox(Toolbox_E toolbox)
            => m_toolboxesContainer.Add(toolbox.Root);

        public void AddDisplayerbox(Displayerbox_E displayerbox)
            => m_displayersContainer.Add(displayerbox.Root);
    }

    public partial class ToolboxWindowItem_E : Visual_E
    {
        public ToolboxWindowItem_E() :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item",
                null,
                null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            Button pin = Root.Q<Button>("pinButton");
            PinnButton = new ButtonWithIcon_E(pin);
            string pinButtonStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButton";
            StyleKeys pinActiveKeys = new StyleKeys(null, "PinnedActive", "PinnedActive");
            StyleKeys pinEnableKeys = new StyleKeys(null, "PinnedEnable", "PinnedEnable");
            PinnButton.SetButton(pinButtonStyle, pinActiveKeys, pinEnableKeys, false, Pin);
            string pinIconStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_PinButtonIcon";
            StyleKeys pinIconKeys = new StyleKeys(null, "", null);
            PinnButton.SetIcon(pinIconStyle, pinIconKeys);
            PinnButton.ApplyButtonStyleWithIcon();

            VisualElement containers = Root.Q("containers");
            string conainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_Container";
            AddVisualStyle(containers, conainerStyle, null);

            m_toolboxesContainer = Root.Q("toolboxesContainer");
            string toolboxesContainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxesContainer";
            //StyleKeys toolboxesContainerKeys = new StyleKeys(null, "");
            AddVisualStyle(m_toolboxesContainer, toolboxesContainerStyle, null);

            m_displayersContainer = Root.Q("displayersContainer");
            string displayersContainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_DisplayersContainer";
            //StyleKeys displayersContainerKeys = new StyleKeys(null, "");
            AddVisualStyle(m_displayersContainer, displayersContainerStyle, null);

            FirstToolbox = new Toolbox_E(false);
            AddToolbox(FirstToolbox);
        }

        public override void Reset()
        {
            base.Reset();
            OnPinned = null;
        }
    }
}

