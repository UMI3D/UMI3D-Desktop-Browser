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
using GLTFast.FakeSchema;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using static BrowserDesktop.Cursor.CursorHandler;

public class CursorDisplayer : Singleton<CursorDisplayer>
{
    public PanelRenderer panelRenderer;

    [Header("Class names")]
    [SerializeField]
    Sprite defaultCursor = null;
    [SerializeField]
    Sprite hoverCursor = null;
    [SerializeField]
    Sprite followCursor = null;
    [SerializeField]
    Sprite clickedCursor = null;

    [SerializeField]
    Sprite settingsCursor = null;

    VisualElement cursorContainer;
    VisualElement cursorCenter;
    VisualElement cursorSettings;

    void Start()
    {
        Debug.Assert(panelRenderer != null);
        var root = panelRenderer.visualTree;

        cursorContainer = root.Q<VisualElement>("cursor-container");
        cursorCenter = root.Q<VisualElement>("cursor-center");
        cursorSettings = root.Q<VisualElement>("cursor-settings");
        cursorSettings.style.backgroundImage = new StyleBackground(settingsCursor.texture);
    }

    public void DisplayCursor(bool display, CursorState state)
    {
        cursorCenter.ClearClassList();
        cursorCenter.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

		switch (state)
		{
			case CursorState.Default:
                cursorCenter.style.backgroundImage = new StyleBackground(defaultCursor.texture);
                break;
			case CursorState.Hover:
                cursorCenter.style.backgroundImage = new StyleBackground(hoverCursor.texture);
                break;
			case CursorState.Clicked:
                cursorCenter.style.backgroundImage = new StyleBackground(clickedCursor.texture);
                break;
            case CursorState.FollowCursor:
                cursorCenter.style.backgroundImage = new StyleBackground(followCursor.texture);
                break;
			default:
				break;
		}
	}

    public void DisplaySettingsCursor(bool display)
    {
        cursorSettings.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public bool IsSettingsCursorDisplayed()
    {
        return cursorSettings.resolvedStyle.display == DisplayStyle.Flex;
    }
}
