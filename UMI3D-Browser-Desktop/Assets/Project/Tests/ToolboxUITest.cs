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
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolboxUITest : MonoBehaviour
{
    public UIDocument UIDoc;
    public MenuDisplayManager DisplayManager;

    private void Start()
    {
        //ListViewTest();
        SDCTest();
    }

    #region SDC

    public class ServerData
    {
        public string name;
    }

    public class SDC_Server : CustomScrollableDataCollection<ServerData>
    {
        public SDC_Server() => Set();

        public override void InitElement()
        {
            if (ScrollView == null) ScrollView = new umi3d.commonScreen.Container.ScrollView_C();

            base.InitElement();
        }
    }

    private void SDCTest()
    {
        var data = new List<ServerData>()
        {
            new ServerData {name = "name a"},
            new ServerData {name = "name ab"},
            new ServerData {name = "name abc"},
            new ServerData {name = "name abcd"},
            new ServerData {name = "name abcde"},
            new ServerData {name = "name abcdef"},
        };

        var sdc = new SDC_Server();
        sdc.Mode = ScrollViewMode.Horizontal;
        sdc.MakeItem = () => new umi3d.commonScreen.menu.ServerButton_C();
        sdc.BindItem = (datum, item) => (item as CustomServerButton).Label = datum.name;
        sdc.Size = 200f;
        sdc.IsReorderable = true;

        foreach (var item in data)
        {
            sdc.AddDatum(item);
        }

        Button reorder = new Button();
        reorder.text = "Reorderable";
        reorder.clicked += () => sdc.IsReorderable = !sdc.IsReorderable;

        Button mode = new Button();
        mode.text = "mode";
        mode.clicked += () => sdc.Mode = sdc.Mode == ScrollViewMode.Vertical ? ScrollViewMode.Horizontal : ScrollViewMode.Vertical;

        UIDoc.rootVisualElement.Add(reorder);
        UIDoc.rootVisualElement.Add(mode);
        UIDoc.rootVisualElement.Add(sdc);
    }


    #endregion

    private void ListViewTest()
    {
        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 1000;
        var items = new List<string>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(i.ToString());

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 30;

        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Multiple;

        //listView.onItemsChosen += objects => Debug.Log(objects);
        //listView.onSelectionChange += objects => Debug.Log(objects);

        listView.reorderable = true;
        listView.reorderMode = ListViewReorderMode.Animated;

        //listView.showAddRemoveFooter = true;
        //listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
        listView.showBorder = true;

        listView.showFoldoutHeader = true;


        listView.style.flexGrow = 1.0f;

        UIDoc.rootVisualElement.Add(listView);
    }

    #region Toolboxes tests

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

    private void StartToolboxTest()
    {
        Close();

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
        DropDownInputMenuItem item2 = new DropDownInputMenuItem { Name = "Enum Item2", options = new List<string>() { "un", "deux", "trois" } };
        item2.NotifyValueChange("un");
        subBoxTwoOne.Add(item2);
        MenuItem item3 = new BooleanInputMenuItem { Name = "Toggle Item3" };
        subBoxTwoOne.Add(item3);
        MenuItem item4 = new FloatRangeInputMenuItem { Name = "Slider Item4", min = 0f, max = 50f, value = 0f };
        subBoxTwoOne.Add(item4);
        MenuItem item5 = new TextInputMenuItem { Name = "Text Item5" };
        subBoxTwoOne.Add(item5);

        DisplayManager.menuAsset.menu = rootContainer;

        Open();
    }

    #endregion*
}
