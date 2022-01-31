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
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    public UIDocument UIDoc;

    void Start()
    {
        var item = new ToolboxItem_E("Toolbox", "Toolbox" );
        //item.AddTo(UIDoc.rootVisualElement);

        var item1 = new ToolboxItem_E("Toolbox", "Toolbox");
        var item2 = new ToolboxItem_E("Toolbox", "Toolbox");
        var item3 = new ToolboxItem_E("Toolbox", "Toolbox");
        var item4 = new ToolboxItem_E("Toolbox", "Toolbox");
        var item5 = new ToolboxItem_E("Toolbox", "Toolbox");

        var toolbox0 = new Toolbox_E("test", true, item, item1);
        var toolbox1 = new Toolbox_E("test1", true, item2, item3, item4);
        var toolbox2 = new Toolbox_E("test2", true, item5);
        //toolbox.AddTo(UIDoc.rootVisualElement);

        MenuBar_E
            .Instance
            .AddTo(UIDoc.rootVisualElement);

        MenuBar_E.Instance.AddToolbox(toolbox0, toolbox1, toolbox2);
    }

    void Update()
    {
        
    }
}
