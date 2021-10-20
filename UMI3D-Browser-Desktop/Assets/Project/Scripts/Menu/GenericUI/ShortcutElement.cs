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

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ShortcutElement : VisualElement
{
    VisualElement iconsArea_VE;

    public new class UxmlFactory : UxmlFactory<ShortcutElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    private static int shortcutsCount = 0;
    public static int ShortcutsCount
    {
        get => shortcutsCount;
        private set
        {
            if (shortcutsCount - value == 1 || value - shortcutsCount == 1) //the equivalent of --ShortcutsCount or ++Shortcut
            {
                shortcutsCount = value;
            }

            if (shortcutsCount == 0)
                IconsWidth = 0;
        }
    }

    private static float iconsWidth = 0;
    public static float IconsWidth
    {
        get => iconsWidth;
        private set
        {
            if (value == 0) //Should only happen when there is no shortcup to display.
            {
                iconsWidth = 0;
            }
            else if (iconsWidth < value) //The icons area width has increased.
            {
                iconsWidth = value;
                Debug.Log("resizement in Icons width");
                OnIconsAreaResizement.Invoke();
            }
            else //This icons area is smaller
            {
                Debug.Log("resizement in Icons width");
                OnIconsAreaResizement.Invoke();
            }
            
        }
    }

    private bool isShortcutDisplay = false;

    private static UnityEvent OnIconsAreaResizement = new UnityEvent();

    public ShortcutElement()
    {
        OnIconsAreaResizement.AddListener(IconsAreaResizement);
    }

    public void Setup(string shortcutName, Sprite[] shortcutIcons, VisualTreeAsset shortcutIconTreeAsset)
    {
        isShortcutDisplay = true;
        ++ShortcutsCount;

        this.Q<Label>("shortcut-name").text = shortcutName;
        iconsArea_VE = this.Q<VisualElement>("shortcut-icons");

        for (int i = 0; i < shortcutIcons.Length; ++i)
        {
            if (i != 0 && i < shortcutIcons.Length)
            {
                var plus = new Label("+");
                iconsArea_VE.Add(plus);
            }

            var icon = shortcutIconTreeAsset.CloneTree().Q<ShortcutIconElement>();
            icon.Setup(shortcutIcons[i]);
            //Debug.Log("icons name = " + shortcutIcons[i].name);
            iconsArea_VE.Add(icon);
        }

        umi3d.cdk.UMI3DEnvironmentLoader.Instance.StartCoroutine(GetIconsAreaWidth());
    }

    IEnumerator GetIconsAreaWidth()
    {
        yield return null;
        IconsWidth = iconsArea_VE.resolvedStyle.width;
        Debug.Log("icons width = " + IconsWidth);
        Debug.Log("Icons area width = " + iconsArea_VE.resolvedStyle.width);
    }

    public void IconsAreaResizement()
    {
        //TODO
        iconsArea_VE.style.width = IconsWidth;
        Debug.Log("resizement");
    }

    public void RemoveShortcut()
    {
        if (isShortcutDisplay)
        {
            isShortcutDisplay = false;
            this.RemoveFromHierarchy();
            --ShortcutsCount;
            Debug.Log("remove shortcut : " + shortcutsCount);
        }
    }
}
