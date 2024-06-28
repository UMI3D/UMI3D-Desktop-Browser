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
using System;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.UIElements;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.Container;
using System.Net;
using umi3d.common;

namespace umi3d.commonScreen.menu
{
    public class HomeScreen_C : BaseMenuScreen_C
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;
        public new class UxmlFactory : UxmlFactory<HomeScreen_C, UxmlTraits> { }

        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
        {
            UxmlBoolAttributeDescription m_displaySavedServer = new UxmlBoolAttributeDescription
            {
                name = "display-saved-servers",
                defaultValue = false
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as HomeScreen_C;

                custom.DisplaySavedServers = m_displaySavedServer.GetValueFromBag(bag, cc);
            }
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/homeScreen";
        public override string UssCustomClass_Emc => "home-screen";

        public virtual string USSCustomClassSavedServer__Box => $"{UssCustomClass_Emc}__saved-servers__box";
        public virtual string USSCustomClassSavedServer__Header => $"{UssCustomClass_Emc}__saved-servers__header";
        public virtual string USSCustomClassSavedServer__Title => $"{UssCustomClass_Emc}__saved-servers__title";
        public virtual string USSCustomClassSavedServer__Edit => $"{UssCustomClass_Emc}__saved-servers__edit";
        public virtual string USSCustomClassSavedServer__ScrollView => $"{UssCustomClass_Emc}__saved-servers__scroll-view";

        public virtual string USSCustomClassDirectConnect__Box => $"{UssCustomClass_Emc}__direct-connect__box";
        public virtual string USSCustomClassDirectConnect__Title => $"{UssCustomClass_Emc}__direct-connect__title";
        public virtual string USSCustomClassDirectConnect__TextField => $"{UssCustomClass_Emc}__direct-connect__text-field";
        public virtual string USSCustomClassDirectConnect__Toggle => $"{UssCustomClass_Emc}__direct-connect__toggle";

        public virtual string USSCustomClassButton_Box => $"{UssCustomClass_Emc}__button__box";
        public virtual string USSCustomClassButton_Switch => $"{UssCustomClass_Emc}__button__switch";

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
                    Button_SwitchBoxes.LocaliseText = new LocalisationAttribute("Connect to a new server", "LauncherScreen", "ConnectNewServer");
                }
                else
                {
                    Insert(1, DirectConnect__Box);
                    SavedServers__Box.RemoveFromHierarchy();
                    Button_SwitchBoxes.LocaliseText = new LocalisationAttribute("Display saved servers", "LauncherScreen", "DisplaySavedServer");
                }
            }
        }

        public override LocalisationAttribute ShortScreenTitle => new LocalisationAttribute("Home", "LauncherScreen", "Home");
        public VisualElement SavedServers__Box = new VisualElement();
        public VisualElement SavedServers__Header = new VisualElement();
        public Text_C SavedServers__Title = new Text_C { name = "title" };
        public Button_C SavedServers__Edit = new Button_C();
        public ScrollView_C SavedServers__ScrollView = new ScrollView_C();
        public ServerButton_C SavedServers_NewServer = new ServerButton_C();

        public VisualElement DirectConnect__Box = new VisualElement();
        public Text_C DirectConnect__Title = new Text_C {name = "title"};
        public Textfield_C DirectConnect__TextField = new Textfield_C();
        public Toggle_C DirectConnect__Toggle = new Toggle_C();

        public VisualElement Button__Box = new VisualElement();
        public Button_C Button_SwitchBoxes = new Button_C();

        protected bool m_displaySavedServers;
        protected bool m_isEditingSavedServers = false;

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
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
        }

        protected override void InitElement()
        {
            base.InitElement();
            SavedServers__Title.LocalisedText = new LocalisationAttribute("Saved Servers", "LauncherScreen", "SavedServers");
            SavedServers__Title.TextStyle = TextStyle.Subtitle;
            SavedServers__Edit.LocaliseText = new LocalisationAttribute("Edit", "GenericStrings", "Edit");
            SavedServers__Edit.Height = ElementSize.Small;
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
            SavedServers_NewServer.LocalisedLabel = new LocalisationAttribute("New", "GenericStrings", "New");
            SavedServers_NewServer.clicked += ()
                => AddNewSavedServer(SavedServers_NewServer, new ServerPreferences.ServerData(), (index, data) =>
                {
                    if (index != 0) return;
                    savedServers.Add(data);
                });

            DirectConnect__Title.LocalisedText = new LocalisationAttribute("Server URL", "LauncherScreen", "ServerURL");
            DirectConnect__Title.TextStyle = TextStyle.Subtitle;
            DirectConnect__TextField.Size = ElementSize.Large;
            DirectConnect__TextField.DisplaySubmitButton = true;
            DirectConnect__TextField.SubmitButton.Type = ButtonType.Primary;
            DirectConnect__TextField.SubmitButton.LocaliseText = new LocalisationAttribute("Connect", "GenericStrings", "Connect");
            DirectConnect__TextField.SubmitButton.clicked += () => SetCurrentServerAndConnect();
            DirectConnect__Toggle.LocaliseLabel = new LocalisationAttribute("Remember this server", "LauncherScreen", "RememberServer");
            DirectConnect__Toggle.value = true;
            DirectConnect__Toggle.Direction = ElementDirection.Trailing;

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

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Home", "LauncherScreen", "Home");
            DisplaySavedServers = false;
        }

        #region Implementation

        public Action<bool> connect;
        public ServerPreferences.ServerData currentServer;
        public List<ServerPreferences.ServerData> savedServers;

        /// <summary>
        /// Display this Element
        /// </summary>
        public void Display()
        {
            ElementExtensions.Display(this);
            DirectConnect__TextField.value = currentServer.serverUrl ?? "Enter a server url here";
            m_isEditingSavedServers = false;
        }

        /// <summary>
        /// Register the current server and try to connect.
        /// </summary>
        public void SetCurrentServerAndConnect()
        {
            string serverUrl = DirectConnect__TextField.value;
            if (string.IsNullOrEmpty(serverUrl)) return;

            // Get local address if localhost is enterd
            serverUrl.Replace("localhost", GetLocalIPAddress());

            TryToConnect(new ServerPreferences.ServerData { serverUrl = serverUrl.Trim() }, DirectConnect__Toggle.value);
        }

        /// <summary>
        /// Initialized the saved server.
        /// </summary>
        public void InitSavedServer()
        {
            bool isEmpty = true;
            foreach (ServerPreferences.ServerData data in savedServers)
            {
                isEmpty = false;
                ServerButton_C item = new ServerButton_C();
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
                    item.Blur();
                };
                SavedServers__ScrollView.Add(item);
            }
            Button_SwitchBoxes.style.display = isEmpty ? DisplayStyle.None : DisplayStyle.Flex;
            DisplaySavedServers = !isEmpty;
        }

        protected void SetServerItem(ServerButton_C item, ServerPreferences.ServerData data)
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
            item.LocalisedLabel = data.serverName ?? data.serverUrl;
            item.TouchManipulator.LongPressDelay = 1000;
            item.Data = data;
        }

        protected void TryToConnect(ServerPreferences.ServerData data, bool saveServer = false)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable && data.serverUrl.StartsWith(GetLocalIPAddress()))
            {
                var dialogueBox = new Dialoguebox_C();
                dialogueBox.Title = new LocalisationAttribute("No internet connection", "LauncherScreen", "NoConnection_Title");
                dialogueBox.Message = new LocalisationAttribute
                (
                    $"Connect your device to load {data.serverName}",
                    "LauncherScreen", 
                    "NoConnection_Description", 
                    new string[1] { data.serverName }
                );
                dialogueBox.Type = DialogueboxType.Confirmation;
                dialogueBox.ChoiceAText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel");
                dialogueBox.ChoiceBText = new LocalisationAttribute("Retry", "GenericStrings", "Retry");
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
                var dialogueBox = new Dialoguebox_C();
                dialogueBox.Title = new LocalisationAttribute
                (
                    "You are connected to internet via carrier data network", 
                    "LauncherScreen", 
                    "WeakConnection_Title"
                );
                dialogueBox.Message = new LocalisationAttribute
                (
                    $"You are trying to load {data.serverName} via carrier data network. \nLoading environment use a lot of data, if you can you should connect via wifi or cable and retry",
                    "LauncherScreen", 
                    "WeakConnection_Description", 
                    new string[1] { data.serverName }
                );
                dialogueBox.Type = DialogueboxType.Confirmation;
                dialogueBox.ChoiceAText = new LocalisationAttribute("Connect anyway", "LauncherScreen", "WeakConnection_ConnectAnyway");
                dialogueBox.ChoiceBText = new LocalisationAttribute("Retry", "GenericStrings", "Retry");
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

        private static string GetLocalIPAddress()
        {
            IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ip.ToString().EndsWith(".1"))
                {
                    return ip.ToString();
                }
            }
            //if offline. 
            UMI3DLogger.LogWarning("No public IP found. This computer seems to be offline.", scope);
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
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
            var dialogueBox = new Dialoguebox_C
            {
                Category = ElementCategory.Menu,
                Size = ElementSize.Medium,
                Type = DialogueboxType.Confirmation,
                Title = data.serverName,
                Message = new LocalisationAttribute($"Delete this server from registered ?", "LauncherScreen", "DeleteServer_Description"),
                ChoiceAText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel"),
                ChoiceBText = new LocalisationAttribute("Delete", "GenericStrings", "Delete")
            };
            dialogueBox.ChoiceA.Type = ButtonType.Default;
            dialogueBox.Callback = (b) =>
            {
                callback?.Invoke(b);
                if (b != 1) return;
                savedServers.Remove(savedServers.Find(d => d.serverUrl == data.serverUrl));
                ServerPreferences.StoreRegisteredServerData(savedServers);
                SavedServers__ScrollView.Remove(item);
            };
            dialogueBox.Enqueue(item);
        }

        protected void UpdateSavedServer(ServerButton_C item, ServerPreferences.ServerData data, bool displayDeleteButton, Action<int, ServerPreferences.ServerData> callback)
        {
            var dialogueBox = new Dialoguebox_C
            {
                Category = ElementCategory.Menu,
                Size = ElementSize.Medium,
                Type = DialogueboxType.Confirmation,
                Title = data.serverName,
                Message = new LocalisationAttribute($"Update saved server", "LauncherScreen", "UpdateServer_Description"),
                ChoiceAText = new LocalisationAttribute("Save", "GenericStrings", "Save"),
                ChoiceBText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel")
            };
            dialogueBox.ChoiceB.Type = ButtonType.Default;

            var name = new Textfield_C();
            var url = new Textfield_C();

            name.LocaliseLabel = new LocalisationAttribute("Name", "GenericStrings", "Name");
            name.value = data.serverName;
            url.LocaliseLabel = "URL";
            url.value = data.serverUrl;

            dialogueBox.Add(name);
            dialogueBox.Add(url);

            if (displayDeleteButton)
            {
                var delete = new Button_C();
                delete.Type = ButtonType.Danger;
                delete.LocaliseText = new LocalisationAttribute("Delete", "GenericStrings", "Delete");
                delete.clicked += () =>
                {
                    dialogueBox.RemoveFromHierarchy();
                    dialogueBox.DisplayNextDialogueBox();
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

        protected void AddNewSavedServer(ServerButton_C item, ServerPreferences.ServerData data, Action<int, ServerPreferences.ServerData> callback)
        {
            var dialogueBox = new Dialoguebox_C
            {
                Category = ElementCategory.Menu,
                Size = ElementSize.Large,
                Type = DialogueboxType.Confirmation,
                Title = data.serverName,
                Message = new LocalisationAttribute($"New server", "LauncherScreen", "NewServer"),
                ChoiceAText = new LocalisationAttribute("Save", "GenericStrings", "Save"),
                ChoiceBText = new LocalisationAttribute("Cancel", "GenericStrings", "Cancel")
            };
            dialogueBox.ChoiceB.Type = ButtonType.Default;

            var name = new Textfield_C();
            var url = new Textfield_C();

            name.LocaliseLabel = new LocalisationAttribute("Name", "GenericStrings", "Name");
            name.value = data.serverName;
            url.LocaliseLabel = "URL";
            url.value = data.serverUrl;

            dialogueBox.Add(name);
            dialogueBox.Add(url);

            dialogueBox.Callback = (b) =>
            {
                callback?.Invoke(b, data);
                if (b != 0) return;

                data.serverName = name.value;
                data.serverUrl = url.value;

                var newServer = new ServerButton_C();
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

        #endregion
    }
}
