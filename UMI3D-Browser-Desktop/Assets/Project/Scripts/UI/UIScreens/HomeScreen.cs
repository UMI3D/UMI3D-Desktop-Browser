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
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeScreen
{
    VisualElement rootDocument;
    VisualElement root;

    Button backMenuBnt;
    Button nextMenuBnt;
    System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox;
    System.Action resizeElements;

    //Direct connect
    /// <summary>
    /// Dropdown button to display or hide the new connexion url.
    /// </summary>
    Button connectionDropdownBtn;
    TextField urlInput;
    /// <summary>
    /// Arrow button to connect to the new server.
    /// </summary>
    Button connectBtn;
    Toggle rememberServerToggle;
    ServerPreferences.ServerData currentServerConnectionData;
    System.Action<ServerPreferences.ServerData, bool> connect;

    //Saved Servers
    VisualTreeAsset savedServerTA;
    SliderElement savedServersSlider;
    List<ServerPreferences.ServerData> savedServers;

    public HomeScreen
        (
            VisualElement rootDocument, 
            VisualTreeAsset savedServerTA, 
            ServerPreferences.ServerData currentServerConnectionData, 
            System.Action displayAdvConnectionScreen, 
            System.Action displayLibrariesManagerScreen, 
            System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox,
            System.Action resizeElements,
            System.Action<ServerPreferences.ServerData, bool> connect
        )
    {
        this.rootDocument = rootDocument;
        backMenuBnt = rootDocument.Q<Button>("backMenuBtn");
        nextMenuBnt = rootDocument.Q<Button>("nextMenuBtn");

        root = rootDocument.Q<VisualElement>("home-screen");

        this.displayDialogueBox = displayDialogueBox;
        this.resizeElements = resizeElements;

        //Saved Servers
        this.savedServerTA = savedServerTA;
        savedServers = ServerPreferences.GetRegisteredServerData();
        savedServersSlider = new SliderElement();
        savedServersSlider.SetUp(root.Q<VisualElement>("slider"));

        //Direct connect
        connectionDropdownBtn = root.Q<Button>("newConnection");
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("inputs-url-container"));
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("icon-close"));
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("icon-open"));
        urlInput = root.Q<TextField>("url-input");
        rememberServerToggle = root.Q<Toggle>("toggleRemember");
        root.Q<Button>("url-enter-btn").clickable.clicked += () => SetCurrentConnectionDataAndConnect();
        this.currentServerConnectionData = currentServerConnectionData;
        this.connect = connect;

        root.Q<Button>("advanced-connection-btn").clickable.clicked += () =>
        {
            Hide();
            displayAdvConnectionScreen();
        };
        root.Q<Button>("manage-library-btn").clickable.clicked += () =>
        {
            Hide();
            displayLibrariesManagerScreen();
        };
    }

    public void SetCurrentConnectionDataAndConnect()
    {
        string serverUrl = urlInput.value;
        if (string.IsNullOrEmpty(serverUrl)) return;
        serverUrl = serverUrl.Trim();

        if (currentServerConnectionData != null)
        {
            currentServerConnectionData.serverUrl = serverUrl;
            currentServerConnectionData.serverName = null;
            currentServerConnectionData.serverIcon = null;
        }
        else currentServerConnectionData = new ServerPreferences.ServerData() { serverUrl = serverUrl };

        if (rememberServerToggle.value) ServerPreferences.AddRegisterdeServerData(currentServerConnectionData);

        connect(currentServerConnectionData, true);
    }

    /// <summary>
    /// Display the Home Screen.
    /// </summary>
    public void Display()
    {
        resizeElements();

        //Display and hide UI
        backMenuBnt.style.display = DisplayStyle.None;
        nextMenuBnt.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.Flex;

        DisplaySavedServers();

        urlInput.value = currentServerConnectionData.serverName;
    }

    public void Hide() => root.style.display = DisplayStyle.None;

    private void DisplaySavedServers()
    {
        savedServersSlider.ClearItems();
        bool isEmpty = true;
        foreach (ServerPreferences.ServerData data in savedServers)
        {
            isEmpty = false;
            var item = savedServerTA.CloneTree().Q<VisualElement>("saved-server-item");
            if (data.serverIcon != null)
            {
                try
                {
                    byte[] imageBytes = System.Convert.FromBase64String(data.serverIcon);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    item.style.backgroundImage = tex;
                }
                catch { }
            }
            item.Q<Label>("name").text = data.serverName ?? data.serverUrl;
            item.Q<Button>("delete").clickable.clicked += () => displayDialogueBox(data.serverName, "Delete this server from registered ?", "YES", "NO", (b) =>
            {
                if (b)
                {
                    savedServers.Remove(savedServers.Find(d => d.serverName == data.serverName));
                    ServerPreferences.StoreRegisteredServerData(savedServers);
                    savedServersSlider.RemoveElement(item);
                }
            });
            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 1)
                {
                    this.currentServerConnectionData.serverName = data.serverName;
                    this.currentServerConnectionData.serverUrl = data.serverUrl;
                    this.currentServerConnectionData.serverIcon = data.serverIcon;
                    connect(data, true);
                }
            });
            savedServersSlider.AddElement(item);
        }
        if (isEmpty) root.Q<VisualElement>("saved-servers").style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Toggle display element between DisplayStyle.Flex and DisplayStyle.None
    /// </summary>
    /// <param name="visualElement"></param>
    private void ToggleDisplayElement(VisualElement visualElement)
    {
        visualElement.style.display = DisplayStyle.Flex == visualElement.resolvedStyle.display ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
