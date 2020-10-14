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
    public class DropDownInputDisplayerElement : AbstractDropDownInputDisplayer, IDisplayerElement
    {

        Label label; //TO REPLACE

        public override void Clear()
        {
            base.Clear();
            label.RemoveFromHierarchy();
            //TODO
            //dropdown.onValueChanged.RemoveAllListeners();
        }

        public override void Display(bool forceUpdate = false)
        {
            // TODO
            /*dropdown.gameObject.SetActive(true);
            dropdown.ClearOptions();
            dropdown.AddOptions(menuItem.options);
            dropdown.value = menuItem.options.IndexOf(GetValue());
            dropdown.onValueChanged.AddListener((i) => NotifyValueChange(menuItem.options[i]));
            label.text = menuItem.ToString();*/

            InitAndBindUI();

            label.style.display = DisplayStyle.Flex;
            label.text = menuItem.ToString();
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return label;
        }

        public override void Hide()
        {
            // TODO
            /*dropdown.onValueChanged.RemoveAllListeners();
            dropdown.gameObject.SetActive(false);*/
            label.style.display = DisplayStyle.None;
        }

        public void InitAndBindUI()
        {
            if (label == null)
                label = new Label();
        }
    }
}