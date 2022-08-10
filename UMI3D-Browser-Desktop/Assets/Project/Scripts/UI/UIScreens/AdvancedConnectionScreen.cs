/*
Copyright 2019 - 2022 Inetum

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
using umi3d.baseBrowser.preferences;
using UnityEngine.UIElements;

public class AdvancedConnectionScreen
{
    VisualElement rootDocument;
    VisualElement root;

    Button backMenuBnt, nextMenuBnt;
    System.Action back, next, resizeElements;

    //Advanced Connection screen
    TextField PortInput;
    TextField IpInput;
    ServerPreferences.Data currentConnectionData;

    public AdvancedConnectionScreen
        (
            VisualElement rootDocument, 
            ref ServerPreferences.Data currentConnectionData,
            System.Action resizeElements,
            System.Action displayHome, 
            System.Action StoreCurrentConnectionDataAndConnect
        )
    {
        this.rootDocument = rootDocument;
        backMenuBnt = rootDocument.Q<Button>("backMenuBtn");
        nextMenuBnt = rootDocument.Q<Button>("nextMenuBtn");
        back = () =>
        {
            Hide();
            displayHome();
        };
        next = () =>
        {
            Hide();
            UpdataCurrentConnectionData();
            StoreCurrentConnectionDataAndConnect();
        };

        this.resizeElements = resizeElements;

        root = rootDocument.Q<VisualElement>("advancedConnectionScreen");

        IpInput = root.Q<TextField>("IpInput");
        PortInput = root.Q<TextField>("PortInput");
        this.currentConnectionData = currentConnectionData;
    }

    /// <summary>
    /// Display the advanced connection screen and hide the other screens.
    /// </summary>
    public void Display()
    {
        resizeElements();

        //Display or hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.Flex;
        root.style.display = DisplayStyle.Flex;
        backMenuBnt.clickable.clicked += back;
        nextMenuBnt.clickable.clicked += next;

        //Update Ip and Port input
        IpInput.value = currentConnectionData.ip ?? "localhost";
        PortInput.value = currentConnectionData.port ?? "";
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
        backMenuBnt.clickable.clicked -= back;
        nextMenuBnt.clickable.clicked -= next;
    }

    /// <summary>
    /// Gets the url and port written by users and stores them.
    /// </summary>
    public void UpdataCurrentConnectionData(string ip = "", string port = "")
    {
        currentConnectionData.ip = string.IsNullOrEmpty(ip) ? IpInput.value.Trim() : ip;
        currentConnectionData.port = string.IsNullOrEmpty(port) ? PortInput.value.Trim() : port;
    }
}
