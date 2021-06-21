﻿/*
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
using System.Collections.Generic;
using UnityEngine;
using umi3d.common;
using umi3d.cdk;
using UnityEngine.UIElements;

public class GameMenu : Singleton<GameMenu>
{
    public UIDocument uiDocument;

    [SerializeField]
    string gameMenuTagName = "game-menu-container";

    VisualElement gameMenu;

    void Start()
    {
        gameMenu = uiDocument.rootVisualElement.Q<VisualElement>(gameMenuTagName);
        Display(false);

        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
        {
            Display(true);
        });
    }

    void Display(bool val)
    {
        gameMenu.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
