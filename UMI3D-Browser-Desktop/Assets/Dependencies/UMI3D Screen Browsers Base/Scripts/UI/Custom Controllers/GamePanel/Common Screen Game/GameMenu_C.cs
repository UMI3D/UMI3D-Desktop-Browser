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
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public sealed class GameMenu_C : CustomGameMenu
    {
        public new class UxmlFactory : UxmlFactory<GameMenu_C, UxmlTraits> { }

        public GameMenu_C() => Set();

        public override void InitElement()
        {
            if (VersionLabel == null) VersionLabel = new Displayer.Text_C();
            if (Navigation_ScrollView == null) Navigation_ScrollView = new Container.ScrollView_C();
            if (Header == null) Header = new commonDesktop.menu.AppHeader_C();

            if (GameData == null) GameData = new GameData_C();
            if (Libraries == null) Libraries = new LibraryScreen_C();
            if (Settings == null) Settings = new SettingsContainer_C();

            if (Resume == null) Resume = new Displayer.Button_C();
            if (Leave == null) Leave = new Displayer.Button_C();
            if (NavigationButtons == null) NavigationButtons = new Container.ButtonGroup_C<GameMenuScreens>();

            base.InitElement();
        }
    }
}
