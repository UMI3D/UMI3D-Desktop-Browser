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
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.notificationKeys;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class Launcher_C : BaseMenuContainer_C<LauncherScreens>
    {
        public new class UxmlFactory : UxmlFactory<Launcher_C, UxmlTraits> { }

        public virtual string StyleSheetLauncherPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/launcher";
        public virtual string USSCustomClassLauncher => "launcher";

        public HomeScreen_C Home = new HomeScreen_C { name = "home-screen" };
        public LibraryScreen_C Libraries = new LibraryScreen_C();
        public TipsScreen_C Tips = new TipsScreen_C();
        public SettingsContainer_C Settings = new SettingsContainer_C { name = "settings" };

        public ButtonGroup_C<LauncherScreens> NavigationButtons = new ButtonGroup_C<LauncherScreens> { name = "navigation-buttons" };

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetLauncherPath);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassLauncher);
        }

        protected override void InitElement()
        {
            base.InitElement();
            NavigationButtons.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Home", "LauncherScreen", "Home"),
                new LocalisationAttribute("Settings", "LauncherScreen", "Settings"),
                new LocalisationAttribute("Libraries", "LauncherScreen", "Libraries"),
                new LocalisationAttribute("Tips", "LauncherScreen", "Tips"),
            };
            NavigationButtons.ValueEnumChanged += value => CurrentScreen = value;
            NavigationButtons.EnumValue = LauncherScreens.Home;

            Home.BackButtonCkicked = () => RemoveScreenFromStack();
            Libraries.BackButtonCkicked = () => RemoveScreenFromStack();
            Tips.BackButtonCkicked = () => RemoveScreenFromStack();
            Settings.BackButtonCkicked = () => RemoveScreenFromStack();

            Libraries.AllowDeletion = true;
            Libraries.WrongLibraryPathFound += (pathes) =>
            {
                string title = new LocalisationAttribute("Some libraries are not found", "LibrariesScreen", "Library_WrongPath_Title").Value;
                string message = new LocalisationAttribute("Here is the list of the path that are not found:", "LibrariesScreen", "Library_WrongPath_Description").Value;
                message += "\n";
                message += "\"" + string.Join("\"\n\"", pathes);
                string buttonA = new LocalisationAttribute("Show me", "LibrariesScreen", "Library_WrongPath_ShowMe").Value;
                Action<int> callback = index =>
                {
                    AddScreenToStack = LauncherScreens.Libraries;
                };
                NotificationHub.Default.Notify(
                    this,
                    DialogueBoxNotificationKey.NewDialogueBox,
                    new()
                    {
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Priority, false },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Size, ElementSize.Small },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueboxType.Default },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, title },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, message },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsText, new[] { buttonA } },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsType, new[] { ButtonType.Default } },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Callback, callback },
                    }
                );
            };

            Navigation_ScrollView.Add(NavigationButtons);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            CurrentScreen = LauncherScreens.Home;
        }

        #region Implementation

        protected override void RemoveAllScreen()
        {
            Home.RemoveFromHierarchy();
            Libraries.RemoveFromHierarchy();
            Tips.RemoveFromHierarchy();
            Settings.RemoveFromHierarchy();
        }

        protected override void GetScreen(LauncherScreens screenEnum, out BaseMenuScreen_C screen)
        {
            switch (screenEnum)
            {
                case LauncherScreens.Home:
                    screen = Home;
                    break;
                case LauncherScreens.Libraries:
                    screen = Libraries;
                    break;
                case LauncherScreens.Tips:
                    screen = Tips;
                    break;
                case LauncherScreens.Settings:
                    screen = Settings;
                    break;
                default:
                    screen = null;
                    break;
            }
        }

        protected override void GetScreenAndButton(LauncherScreens screenEnum, out BaseMenuScreen_C screen, out Button_C button)
        {
            GetScreen(screenEnum, out screen);
            button = null;
        }

        public Action<bool> Connect
        {
            set
            {
                Home.connect = value;
            }
        }

        public umi3d.baseBrowser.preferences.ServerPreferences.ServerData CurrentServer
        {
            set
            {
                Home.currentServer = value;
            }
        }
        public List<umi3d.baseBrowser.preferences.ServerPreferences.ServerData> SavedServers
        {
            set
            {
                Home.savedServers = value;
                Home.InitSavedServer();
            }
        }
        public umi3d.baseBrowser.preferences.ServerPreferences.Data CurrentConnectionData
        {
            set
            {
                currentConnectionData = value;
            }
        }
        public void InitLibraries() => Libraries.InitLibraries();
        public void InitTips() => Tips.InitTips();

        protected umi3d.baseBrowser.preferences.ServerPreferences.Data currentConnectionData;

        /// <summary>
        /// Gets the url and port written by users and stores them.
        /// </summary>
        public void UpdataCurrentConnectionData(string ip, string port)
        {
            currentConnectionData.environmentName = null;
            currentConnectionData.ip = ip;
            currentConnectionData.port = port;
        }

        #endregion
    }
}
