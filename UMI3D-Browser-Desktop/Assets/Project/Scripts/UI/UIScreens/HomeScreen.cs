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
    VisualElement root;

    Button backMenuBnt, nextMenuBnt;

    //Direct connect
    TextField urlInput;
    Toggle rememberServerToggle;
    
    //Saved Servers
    VisualElement savedServersBox;
    SliderElement savedServersSlider;
    
    VisualTreeAsset savedServerTA;
    ServerPreferences.ServerData currentServer;
    List<ServerPreferences.ServerData> savedServers;
    System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox;
    System.Action resizeElements;
    System.Action<bool> connect;

    public HomeScreen
        (
            VisualElement rootDocument, 
            VisualTreeAsset savedServerTA, 
            ServerPreferences.ServerData currentServer,
            List<ServerPreferences.ServerData> savedServers,
            System.Action displayAdvConnectionScreen, 
            System.Action displayLibrariesManagerScreen, 
            System.Action<string, string, string, string, System.Action<bool>> displayDialogueBox,
            System.Action resizeElements,
            System.Action<bool> connect
        )
    {
        this.savedServerTA = savedServerTA;
        this.currentServer = currentServer;
        this.savedServers = savedServers;
        this.displayDialogueBox = displayDialogueBox;
        this.resizeElements = resizeElements;
        this.connect = connect;

        backMenuBnt = rootDocument.Q<Button>("backMenuBtn");
        nextMenuBnt = rootDocument.Q<Button>("nextMenuBtn");

        root = rootDocument.Q<VisualElement>("home-screen");

        //Saved Servers
        savedServersBox = root.Q<VisualElement>("saved-servers");
        savedServersSlider = new SliderElement();
        savedServersSlider.SetUp(root.Q<VisualElement>("slider"));

        //Direct connect
        /// <summary>
        /// Dropdown button to display or hide the new connexion url.
        /// </summary>
        Button connectionDropdownBtn = root.Q<Button>("newConnection");
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("inputs-url-container"));
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("icon-close"));
        connectionDropdownBtn.clickable.clicked += () => ToggleDisplayElement(root.Q<VisualElement>("icon-open"));
        urlInput = root.Q<TextField>("url-input");
        rememberServerToggle = root.Q<Toggle>("toggleRemember");
        root.Q<Button>("url-enter-btn").clickable.clicked += () => SetCurrentServerAndConnect();
        
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

    public void SetCurrentServerAndConnect()
    {
        string serverUrl = urlInput.value;
        if (string.IsNullOrEmpty(serverUrl)) return;

        currentServer.serverUrl = serverUrl.Trim();
        currentServer.serverName = null;
        currentServer.serverIcon = null;

        connect(rememberServerToggle.value);
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

        urlInput.value = currentServer.serverUrl ?? "Test.com";
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
            item.Q<Button>("delete").clickable.clicked += () => displayDialogueBox
            (
                data.serverName, 
                "Delete this server from registered ?", 
                "YES", 
                "NO", 
                (b) =>
                {
                    if (!b) return;
                    savedServers.Remove(savedServers.Find(d => d.serverName == data.serverName));
                    ServerPreferences.StoreRegisteredServerData(savedServers);
                    savedServersSlider.RemoveElement(item);
                    DisplaySavedServers();
                }
            );
            item.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.clickCount == 1)
                {
                    this.currentServer.serverName = data.serverName;
                    this.currentServer.serverUrl = data.serverUrl;
                    this.currentServer.serverIcon = data.serverIcon;
                    connect(false);
                }
            });
            savedServersSlider.AddElement(item);
        }
        savedServersBox.style.display = isEmpty ? DisplayStyle.None : DisplayStyle.Flex;
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
