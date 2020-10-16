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

using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class EventDisplayer : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EventDisplayer, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }


        public void SetUp(string label, string inputName, Sprite icon = null)
        {
            this.Q<Label>("label").text = label;
            if(icon != null)
                this.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(icon.texture);
            this.Q<Label>("input").text = inputName;
        }

        public void Display(bool display)
        {
            style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void Remove()
        {
            RemoveFromHierarchy();
        }
    }
}