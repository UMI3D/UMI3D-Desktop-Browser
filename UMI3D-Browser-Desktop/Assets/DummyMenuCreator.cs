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

using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using UnityEngine;

public class DummyMenuCreator : MonoBehaviour
{
    public MenuAsset menuAsset;
    public Texture2D icon;
    public umi3d.cdk.menu.view.MenuDisplayManager MenuDisplay;

    [ContextMenu("FillMenuWithGarbage")]
    public void FillMenuWithGarbage()
    {
        Menu rootContainer = new Menu();

        Menu imageMenu;
        CreateMenu(out imageMenu, "image");
        rootContainer.Add(imageMenu);

        MenuItem screenShotMenuItem;
        CreateMenuItem(out screenShotMenuItem, "Screenshot", icon);
        imageMenu.Add(screenShotMenuItem);

        MenuItem importMenuItem;
        CreateMenuItem(out importMenuItem, "Import");
        imageMenu.Add(importMenuItem);

        MenuItem galleryMenuItem;
        CreateMenuItem(out galleryMenuItem, "Gallery");
        imageMenu.Add(galleryMenuItem);



        Menu toolboxMenu;
        CreateMenu(out toolboxMenu, "Toolbox 1");
        rootContainer.Add(toolboxMenu);

        MenuItem tool1;
        CreateMenuItem(out tool1, "Tool1");
        toolboxMenu.Add(tool1);

        MenuItem tool2;
        CreateMenuItem(out tool2, "Tool2");
        toolboxMenu.Add(tool2);



        Menu toolboxMenu2;
        CreateMenu(out toolboxMenu2, "Toolbox 2");
        toolboxMenu.Add(toolboxMenu2);

        MenuItem tool3;
        CreateMenuItem(out tool3, "Tool 3");
        toolboxMenu2.Add(tool3);


        menuAsset.menu = rootContainer;

        MenuDisplay.Display(true);
    }

    private void CreateMenuItem(out MenuItem menuItem, string name, Texture2D icon = null)
    {
        menuItem = new MenuItem()
        {
            Name = name,
            icon2D = icon
        };
    }

    private void CreateMenu(out Menu menu, string name)
    {
        menu = new Menu()
        {
            Name = name
        };
    }
}
