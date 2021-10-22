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
    Label shortcutName_L;

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
                IconsAreaWidth = 0;
        }
    }

    private static float iconsAreaWidth = 0;
    public static float IconsAreaWidth
    {
        get => iconsAreaWidth;
        private set
        {
            if (value == 0) //Should only happen when there is no shortcup to display.
            {
                iconsAreaWidth = 0;
            }
            else if (iconsAreaWidth < value) //The icons area width has increased.
            {
                iconsAreaWidth = value;
            }
        }
    }

    private static float shortcutNameWidth = 0;
    public static float ShortcutNameWidth
    {
        get => shortcutNameWidth;
        private set
        {
            if (value == 0) //Should only happen when there is no shortcup to display.
            {
                shortcutNameWidth = 0;
            }
            else if (shortcutNameWidth < value) //The shortcut name width has increased.
            {
                shortcutNameWidth = value;
            }
        }
    }

    private bool isShortcutDisplay = false;

    public void Setup(string shortcutName, Sprite[] shortcutIcons, VisualTreeAsset shortcutIconTreeAsset, BrowserDesktop.Menu.Shortcuts shortcutsClass)
    {
        isShortcutDisplay = true;
        ++ShortcutsCount;

        shortcutName_L = this.Q<Label>("shortcut-name");
        shortcutName_L.text = shortcutName;
        shortcutName_L.AddToClassList("label-shortcut");

        iconsArea_VE = this.Q<VisualElement>("shortcut-icons");

        for (int i = 0; i < shortcutIcons.Length; ++i)
        {
            if (i != 0 && i < shortcutIcons.Length)
            {
                //Object Pooling for the plus label.
                Label plus;
                if (shortcutsClass.ShortcutPlusLabelWaitedList.Count == 0)
                {
                    plus = new Label("+");
                }
                else
                {
                    plus = shortcutsClass.ShortcutPlusLabelWaitedList[shortcutsClass.ShortcutPlusLabelWaitedList.Count - 1];
                    shortcutsClass.ShortcutPlusLabelWaitedList.RemoveAt(shortcutsClass.ShortcutPlusLabelWaitedList.Count - 1);
                }
                shortcutsClass.ShortcutPlusLabelDisplayList.Add(plus);

                plus.AddToClassList("label-shortcut");
                plus.AddToClassList("label-shortcut-plus");
                iconsArea_VE.Add(plus);
            }

            //Object Pooling for Icons.
            ShortcutIconElement icon;
            if (shortcutsClass.ShortcutIconsWaitedList.Count == 0)
            {
                icon = shortcutIconTreeAsset.CloneTree().Q<ShortcutIconElement>();
            }
            else
            {
                icon = shortcutsClass.ShortcutIconsWaitedList[shortcutsClass.ShortcutIconsWaitedList.Count - 1];
                shortcutsClass.ShortcutIconsWaitedList.RemoveAt(shortcutsClass.ShortcutIconsWaitedList.Count - 1);
            }
            shortcutsClass.ShortcutIconsDisplayedList.Add(icon);

            icon.Setup(shortcutIcons[i]);
            iconsArea_VE.Add(icon);
        }
    }

    public void ComputeShortcutWidth()
    {
        IconsAreaWidth = iconsArea_VE.resolvedStyle.width;
        ShortcutNameWidth = shortcutName_L.resolvedStyle.width;

        Debug.Log("icons width = " + IconsAreaWidth + ", Icons area width = " + iconsArea_VE.resolvedStyle.width);
    }

    public void ResizeShortcutWidth()
    {
        iconsArea_VE.style.width = IconsAreaWidth;
        //shortcutName_L.style.width = shortcutNameWidth;
        shortcutName_L.style.width = IconsAreaWidth;

        Debug.Log("resizement");
    }

    public void RemoveShortcut(BrowserDesktop.Menu.Shortcuts shortcutsClass)
    {
        if (isShortcutDisplay)
        {
            isShortcutDisplay = false;
            --ShortcutsCount;

            iconsArea_VE.style.width = StyleKeyword.Auto; //Unset the icons area width.
            shortcutName_L.style.width = StyleKeyword.Auto; //Unset the shortcut name width.

            //shortcutsClass.OnResizeIconsArea.RemoveListener(ResizeIconsArea);
            this.RemoveFromHierarchy();
        }
    }
}
