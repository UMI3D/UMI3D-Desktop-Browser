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
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;
using BrowserDesktop.Cursor;

public class TestUI : MonoBehaviour
{
    public UIDocument UIDoc;
    public MenuDisplayManager DisplayManager;

    void Start()
    {
        Close();

        var mainView = UIDoc.rootVisualElement.Q("mainView");

        MenuBar_E
            .Instance
            .InsertRootTo(UIDoc.rootVisualElement.Q("top"));

        //ToolboxWindow_E
        //    .Instance
        //    .InsertRootTo(UIDoc.rootVisualElement.Q("mainView"));
        var message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam vel ullamcorper lectus. Donec tincidunt purus sit amet elit pretium imperdiet. Proin a tempor dui, ac luctus orci. Proin a tempor dui, ac luctus orci.Proin a tempor dui, ac luctus orci.Proin a tempor dui, ac luctus orci.Proin a tempor dui, ac luctus orci.Proin a tempor dui, ac luctus orci.Proin a tempor dui, ac luctus orci.";
        DialogueBox_E
            .SetCursorMovementActions
            (
                    (o) => { CursorHandler.SetMovement(o, CursorHandler.CursorMovement.Free); },
                    (o) => { CursorHandler.UnSetMovement(o); }
            );
        DialogueBox_E
            .Setup("Test", message, "optionA", "optionB", (val) => { Debug.Log($"pressed [{val}]"); });
        DialogueBox_E
            .DisplayFrom(UIDoc);

        ///FAKE DATA

        Menu rootContainer = new Menu();

        Menu boxOne = new Menu { Name = "Toolbox1" };
        rootContainer.Add(boxOne);

        Menu subBoxOneOne = new Menu { Name = "Poire1.1" };
        boxOne.Add(subBoxOneOne);
        Menu subBoxOneTwo = new Menu { Name = "Fraise1.2" };
        boxOne.Add(subBoxOneTwo);

        Menu subBoxOneTwoOne = new Menu { Name = "Chat1.2.1" };
        subBoxOneTwo.Add(subBoxOneTwoOne);
        Menu subBoxOneTwoTwo = new Menu { Name = "Chien1.2.2" };
        subBoxOneTwo.Add(subBoxOneTwoTwo);

        Menu subBoxOneTwoTwoOne = new Menu { Name = "Mac1.2.2.1" };
        subBoxOneTwoTwo.Add(subBoxOneTwoTwoOne);



        Menu boxTwo = new Menu { Name = "Toolbox 2" };
        rootContainer.Add(boxTwo);

        Menu subBoxTwoOne = new Menu { Name = "BSubToolBox2.1" };
        boxTwo.Add(subBoxTwoOne);

        MenuItem item1 = new ButtonMenuItem { Name = "Button Item1" };
        subBoxTwoOne.Add(item1);
        DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois"} };
        item2.NotifyValueChange("un");
        subBoxTwoOne.Add(item2);
        MenuItem item3 = new BooleanInputMenuItem { Name = "Toggle Item3" };
        subBoxTwoOne.Add(item3);
        MenuItem item4 = new FloatRangeInputMenuItem { Name = "Slider Item4", min = 0f, max = 50f, value = 0f };
        subBoxTwoOne.Add(item4);
        MenuItem item5 = new TextInputMenuItem { Name = "Text Item5"};
        subBoxTwoOne.Add(item5);

        DisplayManager.menuAsset.menu = rootContainer;

        Open();
    }

    [ContextMenu("Open player menu")]
    public void Open()
    {
        DisplayManager.Display(true);
    }

    /// <summary>
    /// Closes the menu with all pinned items.
    /// </summary>
    public void Close()
    {
        DisplayManager.Hide(true);
    }
}
