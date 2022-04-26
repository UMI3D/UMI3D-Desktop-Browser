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

public class ThemeColorTest : MonoBehaviour
{
    public UIDocument UIDoc;

    void Start()
    {
        var root = UIDoc.rootVisualElement;

        string style = "UI/Tests/ThemeColorTest";
        var primary = UIDoc.rootVisualElement.Q("primary");
        new Visual_E(primary, style, new StyleKeys(null, "primary", ""));
        var secondary = UIDoc.rootVisualElement.Q("secondary");
        new Visual_E(secondary, style, new StyleKeys(null, "secondary", ""));
        var tertiary = UIDoc.rootVisualElement.Q("tertiary");
        new Visual_E(tertiary, style, new StyleKeys(null, "tertiary", ""));
    }
}
