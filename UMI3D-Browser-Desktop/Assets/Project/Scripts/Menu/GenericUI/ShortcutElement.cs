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

public class ShortcutElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ShortcutElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    public static float IconsWidth = 0;

    public void Setup(string shortcutName, Sprite[] shortcutIcons, VisualTreeAsset shortcutIconTreeAsset)
    {
        this.Q<Label>("shortcut-name").text = shortcutName;
        VisualElement shortcutIcons_VE = this.Q<VisualElement>("shortcut-icons");

        for (int i = 0; i < shortcutIcons.Length; ++i)
        {
            if (i != 0 && i < shortcutIcons.Length)
            {
                var plus = new Label("+");
                shortcutIcons_VE.Add(plus);
            }

            var icon = shortcutIconTreeAsset.CloneTree().Q<ShortcutIconElement>();
            //icon.style.width = 15;
            //icon.style.height = 15;
            //icon.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
            //icon.style.backgroundImage = new StyleBackground(shortcutIcons[i]);
            icon.Setup(shortcutIcons[i]);
            Debug.Log("icons name = " + shortcutIcons[i].name);
            shortcutIcons_VE.Add(icon);
        }
    }

    public void RemoveShortcut()
    {
        this.RemoveFromHierarchy();
    }
}
