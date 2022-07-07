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
using UnityEngine;
using UnityEngine.UIElements;

public class CursorDisplayer : umi3d.baseBrowser.Controller.BaseCursorDisplayer
{
    [SerializeField]
    Sprite settingsCursor = null;

    private static VisualElement cursorSettings;

    protected override void Awake()
    {
        base.Awake();
        var root = document.rootVisualElement;
        cursorSettings = root.Q<VisualElement>("cursor-settings");
        cursorSettings.style.backgroundImage = new StyleBackground(settingsCursor.texture);
        DisplaySettingsCursor(false);
    }

    public static void DisplaySettingsCursor(bool display)
    {
        if (!Exists) return;
        cursorSettings.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public bool IsSettingsCursorDisplayed => cursorSettings.resolvedStyle.display == DisplayStyle.Flex;
}
