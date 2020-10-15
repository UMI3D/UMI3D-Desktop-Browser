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

using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// Toggle displayer.
    /// </summary>
    public class BooleanInputDisplayerElement : AbstractBooleanInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset booleanInputVisualTreeAsset;

        Toggle toggle;

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            toggle.style.display = DisplayStyle.Flex;
            toggle.value = GetValue();
            toggle.RegisterValueChangedCallback(OnValueChanged);
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return toggle;
        }

        public override void Hide()
        {
            toggle.style.display = DisplayStyle.None;
            toggle.UnregisterValueChangedCallback(OnValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            toggle.UnregisterValueChangedCallback(OnValueChanged);
            toggle.RemoveFromHierarchy();
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is BooleanInputMenuItem) ? 2 : 0;
        }

        private void OnValueChanged(ChangeEvent<bool> e)
        {
            NotifyValueChange(e.newValue);
        }

        public void InitAndBindUI()
        {
            if (toggle == null)
                toggle = booleanInputVisualTreeAsset.CloneTree().Q<Toggle>();
        }

        private void OnDestroy()
        {
            toggle?.RemoveFromHierarchy();
        }
    }
}