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

    [ContextMenu("FillMenuWithGarbage")]
    public void FillMenuWithGarbage()
    {
        Menu rootContainer = new Menu();
        Debug.Log($"rootContainer = {rootContainer.Name}");

        Menu imageMenu = new Menu();
        imageMenu.Name = "image";
        MenuItem screenShotMenuItem = new MenuItem()
        {
            Name = "screenshot",
            icon2D = icon
        };
        MenuItem importMenuItem = new MenuItem()
        {
            Name = "import"
        };
        MenuItem galleryMenuItem = new MenuItem()
        {
            Name = "gallery"
        };

        imageMenu.Add(screenShotMenuItem);
        imageMenu.Add(importMenuItem);
        imageMenu.Add(galleryMenuItem);

        Menu toolboxMenu = new Menu();
        toolboxMenu.Name = "toolbox 1";
        MenuItem tool1 = new MenuItem()
        {
            Name = "tool1",
            icon2D = icon
        };
        MenuItem tool2 = new MenuItem()
        {
            Name = "tool2"
        };

        toolboxMenu.Add(tool1);
        toolboxMenu.Add(tool2);

        rootContainer.Add(imageMenu);
        rootContainer.Add(toolboxMenu);


        menuAsset.menu = rootContainer;
    }
}
