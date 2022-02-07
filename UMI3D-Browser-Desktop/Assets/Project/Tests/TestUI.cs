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
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    public UIDocument UIDoc;
    public MenuDisplayManager DisplayManager;

    void Start()
    {
        Close();

        //var item = new ToolboxItem_E("Toolbox", "Toolbox" );

        //var item1 = new ToolboxItem_E("Toolbox", "Toolbox");
        //var item2 = new ToolboxItem_E("Toolbox", "Toolbox");
        //var item3 = new ToolboxItem_E("Toolbox", "Toolbox");
        //var item4 = new ToolboxItem_E("Toolbox", "Toolbox");
        //var item5 = new ToolboxItem_E("Toolbox", "Toolbox");

        //var toolbox0 = new Toolbox_E("test", true, item, item1);
        //var toolbox1 = new Toolbox_E("test1", true, item2, item3, item4);
        //var toolbox2 = new Toolbox_E("test2", true, item5);
        //MenuBar_E.Instance.AddToolbox(toolbox0, toolbox1, toolbox2);

        MenuBar_E
            .Instance
            .AddTo(UIDoc.rootVisualElement.Q("top"));

        ToolboxWindow_E
            .Instance
            .AddTo(UIDoc.rootVisualElement.Q("mainView"));

        ///FAKE DATA

        Menu rootContainer = new Menu();

        Menu toolboxMenu = new Menu { Name = "Toolbox 1" };
        rootContainer.Add(toolboxMenu);

        Menu subToolBox = new Menu { Name = "SubToolBox" };
        toolboxMenu.Add(subToolBox);

        Menu tool3 = new Menu { Name = "Fraise" };
        toolboxMenu.Add(tool3);

        Menu charcuterieToolBox = new Menu { Name = "Charcuterie Toolbox" };
        tool3.Add(charcuterieToolBox);

        Menu toolboxMenu2 = new Menu { Name = "Toolbox 2" };
        rootContainer.Add(toolboxMenu2);

        DisplayManager.menuAsset.menu = rootContainer;
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
