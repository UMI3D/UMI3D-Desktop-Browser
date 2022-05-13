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
using umi3d.baseBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class ToolboxWindowItem_E
    {
        public event Action<bool> PinnedOrUnpinned;

        public Button_E PinButton { get; private set; } = null;
        public Toolbox_E FirstToolbox { get; private set; } = null;

        private VisualElement m_toolboxesContainer { get; set; } = null;
        private VisualElement m_displayersContainer { get; set; } = null;
    
        private void PinUnpin()
        {
            var willBePinned = !PinButton.IsOn;
            PinUnpin(willBePinned);
        }
        public void PinUnpin(bool willBePinned)
        {
            PinnedOrUnpinned?.Invoke(willBePinned);
            PinButton.Toggle(willBePinned);
        }

        public void SetFirstToolboxName(string name)
            => FirstToolbox.SetName(name ?? "");

        public void AddToolboxItemInFirstToolbox(params View_E[] items)
            => FirstToolbox.AddRange(items);

        public void AddToolbox(Toolbox_E toolbox)
            => m_toolboxesContainer.Add(toolbox.Root);

        public void AddDisplayerbox(Displayerbox_E displayerbox)
            => m_displayersContainer.Add(displayerbox.Root);
    }

    public partial class ToolboxWindowItem_E : View_E
    {
        public ToolboxWindowItem_E() :
            base("UI/UXML/ToolboxWindow/toolboxWindow_Item",
                null,
                null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            PinButton = new Button_E(QR<Button>("pinButton"), "Pin", StyleKeys.Bg_Border("on"), StyleKeys.Bg_Border("off"), false);
            PinButton.Clicked += PinUnpin;
            PinButton.AddIconInFront(new Icon_E(), "Square1", StyleKeys.Bg("pin"));

            VisualElement containers = Root.Q("containers");
            string conainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_Container";
            AddVisualStyle(containers, conainerStyle, null);

            m_toolboxesContainer = Root.Q("toolboxesContainer");
            string toolboxesContainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_ToolboxesContainer";
            AddVisualStyle(m_toolboxesContainer, toolboxesContainerStyle, null);

            m_displayersContainer = Root.Q("displayersContainer");
            string displayersContainerStyle = "UI/Style/ToolboxWindow/ToolboxWindow_Item_DisplayersContainer";
            AddVisualStyle(m_displayersContainer, displayersContainerStyle, null);

            FirstToolbox = Toolbox_E.NewWindowToolbox(null);
            AddToolbox(FirstToolbox);
        }

        public override void Reset()
        {
            base.Reset();
            PinnedOrUnpinned = null;
        }
    }
}

