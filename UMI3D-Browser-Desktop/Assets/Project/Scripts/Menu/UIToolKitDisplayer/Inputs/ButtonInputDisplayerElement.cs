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

using umi3d.cdk.menu.view;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class ButtonInputDisplayerElement : AbstractButtonInputDisplayer, IDisplayerElement
    {
        public VisualTreeAsset buttonTreeAsset;

        VisualElement container;
        /// <summary>
        /// Button
        /// </summary>
        Button button;

        public override void Clear()
        {
            base.Clear();
            container.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            container.style.display = DisplayStyle.Flex;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return container;
        }

        public override void Hide()
        {
            container.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (container == null)
            {
                container = buttonTreeAsset.CloneTree();
                container.name = gameObject.name;
                button = container.Q<Button>();
                button.clickable.clicked += NotifyPress;
            }
        }

        private void OnDestroy()
        {
            container?.RemoveFromHierarchy();
        }
    }
}