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
    VisualElement root;

    Button backMenuBnt, nextMenuBnt;
    System.Action back, next;

    //Advanced Connection screen
    TextField IP, Port;

    ServerPreferences.Data currentConnectionData;
    System.Action resizeElements;

    public AdvancedConnectionScreen
        (
            VisualElement rootDocument, 
            ref ServerPreferences.Data currentConnectionData,
            System.Action resizeElements,
            System.Action displayHome, 
            System.Action StoreCurrentConnectionDataAndConnect
        )
    {
        this.currentConnectionData = currentConnectionData;
        this.resizeElements = resizeElements;

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

        root = rootDocument.Q<VisualElement>("advancedConnectionScreen");

        IP = root.Q<TextField>("IpInput");
        Port = root.Q<TextField>("PortInput");
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
        IP.value = currentConnectionData.ip ?? "localhost";
        Port.value = currentConnectionData.port ?? "";
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
        currentConnectionData.environmentName = null;
        currentConnectionData.ip = string.IsNullOrEmpty(ip) ? IP.value.Trim() : ip;
        currentConnectionData.port = string.IsNullOrEmpty(port) ? Port.value.Trim() : port;
    }
}
