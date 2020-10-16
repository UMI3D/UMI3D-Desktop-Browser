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
    string defaultCursorClassName = "cursor-cross";
    [SerializeField]
    string hoverCursorClassName = "circle-cursor";
    [SerializeField]
    string followCursorClassName = "circle-follow";
    [SerializeField]
    string clickedCursorClassName = "circle-follow";

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
    }

    void HideAndClearCursors()
    {
        cursorCenter.style.display = DisplayStyle.None;
        cursorSettings.style.display = DisplayStyle.None;

        cursorCenter.ClearClassList();
        cursorSettings.ClearClassList();
    }

    public void DisplayCursor(bool display, CursorState state)
    {
        cursorCenter.ClearClassList();
        cursorCenter.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;

		switch (state)
		{
			case CursorState.Default:
                cursorCenter.AddToClassList(defaultCursorClassName);
                break;
			case CursorState.Hover:
                cursorCenter.AddToClassList(hoverCursorClassName);
                break;
			case CursorState.Clicked:
                cursorCenter.AddToClassList(clickedCursorClassName);
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
