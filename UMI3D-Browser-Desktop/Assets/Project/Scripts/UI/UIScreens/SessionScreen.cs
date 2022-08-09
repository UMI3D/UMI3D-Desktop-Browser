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
using BeardedManStudios.Forge.Networking;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;

public class SessionScreen
{
    public System.Action Submit;

    VisualElement rootDocument;
    VisualElement root;

    Button backMenuBnt, nextMenuBnt;

    //Session screen
    VisualTreeAsset sessionEntry;
    ScrollView sessionList;
    /// <summary>
    /// The item selected by a click with the mouse.
    /// </summary>
    private VisualElement selectedItem = null;
    public List<MasterServerResponse.Server> serverResponses = new List<MasterServerResponse.Server>();
    string ip, port;
    bool isSessionSelected;

    public SessionScreen
        (
            VisualElement rootDocument, 
            VisualTreeAsset sessionEntry,
            LaucherOnMasterServer masterServer,
            System.Action displayHome, 
            System.Action<string, string> UpdataCurrentConnectionData, 
            System.Action StoreCurrentConnectionDataAndConnect
        )
    {
        void ConnectWithIPAndPort()
        {
            UpdataCurrentConnectionData(ip, port);
            StoreCurrentConnectionDataAndConnect();
        }
        void ConnectWithPin()
        {
            masterServer.SendDataSession
                (
                    root.Q<TextField>("pinInput").value,
                    (ser) =>
                    {
                        serverResponses.Add(ser);
                        UpdateSessionList();
                    }
                );
        }
        Submit = () =>
        {
            if (isSessionSelected) ConnectWithIPAndPort();
            else ConnectWithPin();
        };

        this.rootDocument = rootDocument;
        backMenuBnt = rootDocument.Q<Button>("backMenuBtn");
        nextMenuBnt = rootDocument.Q<Button>("nextMenuBtn");
        backMenuBnt.clickable.clicked += () =>
        {
            Hide();
            displayHome();
        };
        nextMenuBnt.clickable.clicked += () =>
        {
            Hide();
            ConnectWithIPAndPort();
        };

        root = rootDocument.Q<VisualElement>("sessionScreen");

        this.sessionEntry = sessionEntry;
        sessionList = root.Q<ScrollView>("sessionsList");
        root.Q<Button>("pin-enter-btn").clickable.clicked += () => ConnectWithPin();
    }

    /// <summary>
    /// Display the sessions screen.
    /// </summary>
    public void Display()
    {
        //Display or hide UI
        backMenuBnt.style.display = DisplayStyle.Flex;
        nextMenuBnt.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide() => root.style.display = DisplayStyle.None;

    private void UpdateSessionList()
    {
        sessionList.Clear();

        foreach (MasterServerResponse.Server session in serverResponses)
        {
            VisualElement item = sessionEntry.CloneTree().Q<VisualElement>("session-entry");
            sessionList.Add(item);
            item.Q<Label>("server-name").text = session.Name;
            item.Q<Label>("users-count").text = session.PlayerCount.ToString();

            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 1) SelectSession(item, session.Address, session.Port);
            });
            item.RegisterCallback<MouseEnterEvent>(e =>
            {
                if (!item.ClassListContains("orange-background"))
                    foreach (var label in item.Q<VisualElement>("server-entry-btn").Children()) label.AddToClassList("orange-text");
            }
            );
            item.RegisterCallback<MouseLeaveEvent>(e =>
            {
                foreach (var label in item.Q<VisualElement>("server-entry-btn").Children()) label.RemoveFromClassList("orange-text");
            }
           );
        }

        serverResponses.Clear();
    }

    private void SelectSession(VisualElement itemSelected, string ip, ushort port)
    {
        isSessionSelected = true;
        //TODO color the element
        if (selectedItem != null)
        {
            selectedItem.RemoveFromClassList("orange-background");
            selectedItem.RemoveFromClassList("black-txt");
            foreach (var label in selectedItem.Q<VisualElement>("server-entry-btn").Children())
            {
                label.AddToClassList("orange.text");
                label.RemoveFromClassList("black-txt");
            }
        }

        this.ip = ip;
        this.port = port.ToString();
        nextMenuBnt.style.display = DisplayStyle.Flex;

        //Color
        itemSelected.AddToClassList("orange-background");
        itemSelected.AddToClassList("black-txt");
        foreach (var label in itemSelected.Q<VisualElement>("server-entry-btn").Children())
        {
            label.RemoveFromClassList("orange.text");
            label.AddToClassList("black-txt");
        }
        selectedItem = itemSelected;
    }
}
