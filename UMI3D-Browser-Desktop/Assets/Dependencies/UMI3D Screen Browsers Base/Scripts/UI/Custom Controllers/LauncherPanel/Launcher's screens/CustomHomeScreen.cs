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
using System;
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomHomeScreen : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        UxmlBoolAttributeDescription m_displaySavedServer = new UxmlBoolAttributeDescription
        {
            name = "display-saved-servers",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomHomeScreen;

            custom.Set
                (
                    m_displaySavedServer.GetValueFromBag(bag, cc)
                 );
        }
    }
    
    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/homeScreen";
    public override string USSCustomClassName => "home-screen";
    
    public virtual string USSCustomClassSavedServer__Box => $"{USSCustomClassName}__saved-servers__box";
    public virtual string USSCustomClassSavedServer__Header => $"{USSCustomClassName}__saved-servers__header";
    public virtual string USSCustomClassSavedServer__Title => $"{USSCustomClassName}__saved-servers__title";
    public virtual string USSCustomClassSavedServer__Edit => $"{USSCustomClassName}__saved-servers__edit";
    public virtual string USSCustomClassSavedServer__ScrollView => $"{USSCustomClassName}__saved-servers__scroll-view";

    public virtual string USSCustomClassDirectConnect__Box => $"{USSCustomClassName}__direct-connect__box";
    public virtual string USSCustomClassDirectConnect__Title => $"{USSCustomClassName}__direct-connect__title";
    public virtual string USSCustomClassDirectConnect__TextField => $"{USSCustomClassName}__direct-connect__text-field";
    public virtual string USSCustomClassDirectConnect__Toggle => $"{USSCustomClassName}__direct-connect__toggle";

    public virtual string USSCustomClassButton_Box => $"{USSCustomClassName}__button__box";
    public virtual string USSCustomClassButton_Switch => $"{USSCustomClassName}__button__switch";

    public virtual bool DisplaySavedServers
    {
        get => m_displaySavedServers;
        set
        {
            m_displaySavedServers = value;
            if (m_displaySavedServers)
            {
                Insert(1, SavedServers__Box);
                DirectConnect__Box.RemoveFromHierarchy();
                Button_SwitchBoxes.text = "Connect to a new server";
            }
            else
            {
                Insert(1, DirectConnect__Box);
                SavedServers__Box.RemoveFromHierarchy();
                Button_SwitchBoxes.text = "Display saved servers";
            }
        }
    }

    public override string ShortScreenTitle  => "Home";
    public VisualElement SavedServers__Box = new VisualElement();
    public VisualElement SavedServers__Header = new VisualElement();
    public CustomText SavedServers__Title;
    public CustomButton SavedServers__Edit;
    public CustomScrollView SavedServers__ScrollView;
    public CustomServerButton SavedServers_NewServer;

    public VisualElement DirectConnect__Box = new VisualElement();
    public CustomText DirectConnect__Title;
    public CustomTextfield DirectConnect__TextField;
    public CustomToggle DirectConnect__Toggle;

    public VisualElement Button__Box = new VisualElement();
    public CustomButton Button_SwitchBoxes;
    

    protected bool m_displaySavedServers;
    protected bool m_isEditingSavedServers = false;

    public override void InitElement()
    {
        base.InitElement();

        SavedServers__Box.AddToClassList(USSCustomClassSavedServer__Box);
        SavedServers__Header.AddToClassList(USSCustomClassSavedServer__Header);
        SavedServers__Title.AddToClassList(USSCustomClassSavedServer__Title);
        SavedServers__Edit.AddToClassList(USSCustomClassSavedServer__Edit);
        SavedServers__ScrollView.AddToClassList(USSCustomClassSavedServer__ScrollView);

        DirectConnect__Box.AddToClassList(USSCustomClassDirectConnect__Box);
        DirectConnect__Title.AddToClassList(USSCustomClassDirectConnect__Title);
        DirectConnect__TextField.AddToClassList(USSCustomClassDirectConnect__TextField);
        DirectConnect__Toggle.AddToClassList(USSCustomClassDirectConnect__Toggle);

        Button__Box.AddToClassList(USSCustomClassButton_Box);
        Button_SwitchBoxes.AddToClassList(USSCustomClassButton_Switch);

        SavedServers__Title.text = "Saved Servers";
        SavedServers__Title.TextStyle = TextStyle.Subtitle;
        SavedServers__Edit.text = "Edit";
        SavedServers__Edit.Size = ElementSize.Small;
        SavedServers__Edit.clicked += () =>
        {
            m_isEditingSavedServers = !m_isEditingSavedServers;
            if (m_isEditingSavedServers)
            {
                SavedServers__Edit.Type = ButtonType.Primary;
                SavedServers__ScrollView.Insert(0, SavedServers_NewServer);
            }
            else
            {
                SavedServers__Edit.Type = ButtonType.Default;
                SavedServers_NewServer.RemoveFromHierarchy();
            }
        };
        SavedServers__ScrollView.mode = ScrollViewMode.Horizontal;
        SavedServers_NewServer.Label = "New";
        SavedServers_NewServer.clicked += () 
            => AddNewSavedServer(SavedServers_NewServer, new ServerPreferences.ServerData(), (index, data) =>
            {
                if (index != 0) return;
                savedServers.Add(data);
            });
        
        DirectConnect__Title.text = "Server URL";
        DirectConnect__Title.TextStyle = TextStyle.Subtitle;
        DirectConnect__TextField.Size = ElementSize.Large;
        DirectConnect__TextField.DisplaySubmitButton = true;
        DirectConnect__TextField.SubmitButton.Type = ButtonType.Primary;
        DirectConnect__TextField.SubmitButton.text = "Connect";
        DirectConnect__TextField.SubmitButton.clicked += () => SetCurrentServerAndConnect();
        DirectConnect__Toggle.label = "Remember this server";
        DirectConnect__Toggle.Direction = ElemnetDirection.Trailing;

        Button_SwitchBoxes.clicked += () => DisplaySavedServers = !DisplaySavedServers;

        SavedServers__Box.Insert(0, SavedServers__Header);
        SavedServers__Header.Insert(0, SavedServers__Title);
        SavedServers__Header.Insert(1, SavedServers__Edit);
        SavedServers__Box.Insert(1, SavedServers__ScrollView);

        DirectConnect__Box.Insert(0, DirectConnect__Title);
        DirectConnect__Box.Insert(1, DirectConnect__TextField);
        DirectConnect__Box.Insert(2, DirectConnect__Toggle);

        Add(Button__Box);
        Button__Box.Add(Button_SwitchBoxes);

        Button_Back.RemoveFromHierarchy();
    }

    public override void Set() => Set(false);

    public virtual void Set(bool displaySavedServer)
    {
        Set("Home");

        DisplaySavedServers = displaySavedServer;
    }

    #region Implementation

    public Action<bool> connect;
    public ServerPreferences.ServerData currentServer;
    public List<ServerPreferences.ServerData> savedServers;

    public void Display()
    {
        ElementExtensions.Display(this);
        DirectConnect__TextField.value = currentServer.serverUrl ?? "Enter a server url here";
        m_isEditingSavedServers = false;
    }

    public void SetCurrentServerAndConnect()
    {
        string serverUrl = DirectConnect__TextField.value;
        if (string.IsNullOrEmpty(serverUrl)) return;

        TryToConnect(new ServerPreferences.ServerData { serverUrl = serverUrl.Trim() }, DirectConnect__Toggle.value);
    }

    public void InitSavedServer()
    {
        bool isEmpty = true;
        foreach (ServerPreferences.ServerData data in savedServers)
        {
            isEmpty = false;
            CustomServerButton item = CreateServerItem();
            SetServerItem(item, data);
            item.ClickedLong += () =>
            {
                if (!m_isEditingSavedServers) UpdateSavedServer(item, item.Data, true, null);
                else return;
            };

            item.clicked += () =>
            {
                if (!m_isEditingSavedServers) TryToConnect(item.Data);
                else UpdateSavedServer(item, item.Data, true, null);
            };
            SavedServers__ScrollView.Add(item);
        }
        Button_SwitchBoxes.style.display = isEmpty ? DisplayStyle.None : DisplayStyle.Flex;
        DisplaySavedServers = !isEmpty;
    }

    protected void SetServerItem(CustomServerButton item, ServerPreferences.ServerData data)
    {
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
        item.Label = data.serverName ?? data.serverUrl;
        item.TouchManipulator.LongPressDelay = 1000;
        item.Data = data;
    }

    protected void TryToConnect(ServerPreferences.ServerData data, bool saveServer = false)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            var dialogueBox = CreateDialogueBox();
            dialogueBox.Title = "No internet connection";
            dialogueBox.Message = $"Connect your device to load {data.serverName}";
            dialogueBox.Type = DialogueboxType.Confirmation;
            dialogueBox.ChoiceAText = "Cancel";
            dialogueBox.ChoiceBText = "Retry";
            dialogueBox.ChoiceA.Type = ButtonType.Default;
            dialogueBox.ChoiceB.Type = ButtonType.Default;
            dialogueBox.Callback = index =>
            {
                if (index == 1) TryToConnect(data, saveServer);
            };
            dialogueBox.Enqueue(this);
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            var dialogueBox = CreateDialogueBox();
            dialogueBox.Title = "You are connected to internet via carrier data network";
            dialogueBox.Message = $"You are trying to load {data.serverName} via carrier data network. \nLoading environment use a lot of data, if you can you should connect via wifi or cable and retry";
            dialogueBox.Type = DialogueboxType.Confirmation;
            dialogueBox.ChoiceAText = "Connect anyway";
            dialogueBox.ChoiceBText = "Retry";
            dialogueBox.ChoiceA.Type = ButtonType.Default;
            dialogueBox.ChoiceB.Type = ButtonType.Default;
            dialogueBox.Callback = index =>
            {
                if (index == 1) TryToConnect(data, saveServer);
                else Connect(data, saveServer);
            };
            dialogueBox.Enqueue(this);
        }
        else Connect(data, saveServer);
    }

    protected void Connect(ServerPreferences.ServerData data, bool saveServer = false)
    {
        this.currentServer.serverName = data.serverName;
        this.currentServer.serverUrl = data.serverUrl;
        this.currentServer.serverIcon = data.serverIcon;
        connect(saveServer);
    }

    protected void DeleteSavedServer(VisualElement item, ServerPreferences.ServerData data, Action<int> callback)
    {
        var dialogueBox = CreateDialogueBox();
        dialogueBox.Set
        (
            ElementCategory.Menu,
            ElementSize.Medium,
            DialogueboxType.Confirmation,
            data.serverName,
            "Delete this server from registered ?",
            "Cancel",
            "Delete"
            );
        dialogueBox.ChoiceA.Type = ButtonType.Default;
        dialogueBox.Callback = (b) =>
        {
            callback?.Invoke(b);
            if (b != 1) return;
            savedServers.Remove(savedServers.Find(d => d.serverName == data.serverName));
            ServerPreferences.StoreRegisteredServerData(savedServers);
            SavedServers__ScrollView.Remove(item);
        };
        dialogueBox.Enqueue(item);
    }

    protected void UpdateSavedServer(CustomServerButton item, ServerPreferences.ServerData data, bool displayDeleteButton, Action<int, ServerPreferences.ServerData> callback)
    {
        var dialogueBox = CreateDialogueBox();
        dialogueBox.Set
        (
            ElementCategory.Menu,
            ElementSize.Large,
            DialogueboxType.Confirmation,
            data.serverName,
            "Update saved server",
            "Save",
            "Cancel"
            );
        dialogueBox.ChoiceB.Type = ButtonType.Default;

        var name = CreateTextField();
        var url = CreateTextField();

        name.label = "Name";
        name.value = data.serverName;
        url.label = "URL";
        url.value = data.serverUrl;

        dialogueBox.Add(name);
        dialogueBox.Add(url);

        if (displayDeleteButton)
        {
            var delete = CreateButton();
            delete.Type = ButtonType.Danger;
            delete.text = "Delete";
            delete.clicked += () =>
            {
                dialogueBox.RemoveFromHierarchy();
                DeleteSavedServer(item, data, (index) =>
                {
                    if (index == 0) dialogueBox.Enqueue(item);
                });
            };
            dialogueBox.Add(delete); 
        }

        dialogueBox.Callback = (b) =>
        {
            callback?.Invoke(b, data);
            if (b != 0) return;

            data.serverName = name.value;
            data.serverUrl = url.value;
            SetServerItem(item, data);

            ServerPreferences.StoreRegisteredServerData(savedServers);
        };
        dialogueBox.Enqueue(item);
    }

    protected void AddNewSavedServer(CustomServerButton item, ServerPreferences.ServerData data, Action<int, ServerPreferences.ServerData> callback)
    {
        var dialogueBox = CreateDialogueBox();
        dialogueBox.Set
        (
            ElementCategory.Menu,
            ElementSize.Large,
            DialogueboxType.Confirmation,
            data.serverName,
            "New server",
            "Save",
            "Cancel"
            );
        dialogueBox.ChoiceB.Type = ButtonType.Default;

        var name = CreateTextField();
        var url = CreateTextField();

        name.label = "Name";
        name.value = data.serverName;
        url.label = "URL";
        url.value = data.serverUrl;

        dialogueBox.Add(name);
        dialogueBox.Add(url);

        dialogueBox.Callback = (b) =>
        {
            callback?.Invoke(b, data);
            if (b != 0) return;

            data.serverName = name.value;
            data.serverUrl = url.value;

            var newServer = CreateServerItem();
            SetServerItem(newServer, data);
            newServer.ClickedLong += () =>
            {
                if (!m_isEditingSavedServers) UpdateSavedServer(newServer, newServer.Data, true, null);
                else return;
            };

            newServer.clicked += () =>
            {
                if (!m_isEditingSavedServers) TryToConnect(newServer.Data);
                else UpdateSavedServer(newServer, newServer.Data, true, null);
            };
            SavedServers__ScrollView.Insert(1, newServer);

            ServerPreferences.StoreRegisteredServerData(savedServers);
        };
        dialogueBox.Enqueue(item);
    }

    protected abstract CustomServerButton CreateServerItem();

    protected abstract CustomDialoguebox CreateDialogueBox();

    protected abstract CustomTextfield CreateTextField();

    protected abstract CustomButton CreateButton();

    #endregion
}
