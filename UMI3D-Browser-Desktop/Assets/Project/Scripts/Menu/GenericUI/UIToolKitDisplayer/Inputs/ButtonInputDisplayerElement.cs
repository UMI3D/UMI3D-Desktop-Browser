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
        /// <summary>
        /// Button
        /// </summary>
        Button button;

        public override void Clear()
        {
            base.Clear();
            button.clickable.clicked -= NotifyPress;
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            button.style.display = DisplayStyle.Flex;
            button.clickable.clicked += NotifyPress;
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return button;
        }

        public override void Hide()
        {
            button.clickable.clicked -= NotifyPress;
            button.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (button == null)
                button = new Button();
        }
    }
}