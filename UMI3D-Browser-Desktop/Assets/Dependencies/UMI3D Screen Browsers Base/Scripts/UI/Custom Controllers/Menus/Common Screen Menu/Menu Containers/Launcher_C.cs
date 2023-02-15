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
                new LocalisationAttribute("Libraries", "LauncherScreen", "Libraries")
            };
            NavigationButtons.ValueEnumChanged += value => CurrentScreen = value;
            NavigationButtons.EnumValue = LauncherScreens.Home;

            Home.BackButtonCkicked = () => RemoveScreenFromStack();
            Libraries.BackButtonCkicked = () => RemoveScreenFromStack();
            Settings.BackButtonCkicked = () => RemoveScreenFromStack();

            Libraries.AllowDeletion = true;
            Libraries.WrongLibraryPathFound += (pathes) =>
            {
                var dialogueBox = new Dialoguebox_C();
                dialogueBox.Type = DialogueboxType.Default;
                dialogueBox.Title = new LocalisationAttribute("Some libraries are not found", "LibrariesScreen", "Library_WrongPath_Title");
                dialogueBox.Message = new LocalisationAttribute("Here is the list of the path that are not found:", "LibrariesScreen", "Library_WrongPath_Description");
                var pathList = new Text_C();
                pathList.LocaliseText = "\"" + string.Join("\"\n\"", pathes);
                pathList.style.whiteSpace = WhiteSpace.Normal;
                dialogueBox.Add(pathList);
                dialogueBox.ChoiceAText = new LocalisationAttribute("Show me", "LibrariesScreen", "Library_WrongPath_ShowMe");
                dialogueBox.Callback = (index) =>
                {
                    AddScreenToStack = LauncherScreens.Libraries;
                };
                dialogueBox.Enqueue(this);
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
