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

using umi3d.baseBrowser.Menu;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common.interaction;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// Toggle displayer.
    /// </summary>
    public class LocalInfoRequestInputDisplayerElement : AbstractLocalInfoRequestInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset booleanInputVisualTreeAsset;

        VisualElement container;
        Toggle toggleRead;
        Toggle toggleWrite;

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            container.style.display = DisplayStyle.Flex;
            var value = GetValue();
            toggleRead.value = value.read;
            toggleWrite.value = value.write;
            /*   if (!string.IsNullOrEmpty(menuItem.ToString()))
                   //toggle.label = menuItem.ToString();
                   container.name = menuItem.ToString();*/
            toggleRead.RegisterValueChangedCallback(OnReadValueChanged);
            toggleWrite.RegisterValueChangedCallback(OnWriteValueChanged);
            
            toggleRead.style.display = toggleRead.value ? DisplayStyle.Flex : DisplayStyle.None; // Read access not required
            toggleWrite.style.display = toggleWrite.value ? DisplayStyle.Flex : DisplayStyle.None; // Write access not required

        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return container;
        }

        public override void Hide()
        {
            container.style.display = DisplayStyle.None;
            toggleRead.UnregisterValueChangedCallback(OnReadValueChanged);
            toggleWrite.UnregisterValueChangedCallback(OnWriteValueChanged);
        }

        public override void Clear()
        {
            base.Clear();
            toggleRead.UnregisterValueChangedCallback(OnReadValueChanged);
            toggleWrite.UnregisterValueChangedCallback(OnWriteValueChanged);
            container.RemoveFromHierarchy();
        }

        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is LocalInfoRequestInputMenuItem) ? 2 : 0;
        }

        private void OnReadValueChanged(ChangeEvent<bool> e)
        {
            NotifyValueChange(new LocalInfoRequestParameterValue(e.newValue, toggleWrite.value));
        }

        private void OnWriteValueChanged(ChangeEvent<bool> e)
        {
            NotifyValueChange(new LocalInfoRequestParameterValue(toggleRead.value, e.newValue));
        }

        public void InitAndBindUI()
        {
            if (container == null)
            {
                container = booleanInputVisualTreeAsset.CloneTree();
                container.name = gameObject.name;
                toggleRead = container.Q<Toggle>("toggleReadAccess");
                toggleWrite = container.Q<Toggle>("toggleWriteAccess");

                container.Q<Label>("ServerRequest").text = "Server "+ ((LocalInfoRequestParameterDto)menuItem.dto).serverName + " requests acces to local data : " + ((LocalInfoRequestParameterDto)menuItem.dto).key;
                container.Q<Label>("localReason").text = ((LocalInfoRequestParameterDto)menuItem.dto).reason;
            }
        }

        private void OnDestroy()
        {
            container?.RemoveFromHierarchy();
        }
    }
}
