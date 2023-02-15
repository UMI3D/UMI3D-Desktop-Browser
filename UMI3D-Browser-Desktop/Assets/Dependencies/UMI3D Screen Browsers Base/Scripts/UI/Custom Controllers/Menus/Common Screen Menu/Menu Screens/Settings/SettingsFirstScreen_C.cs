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
using umi3d.commonScreen.Container;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class SettingsFirstScreen_C : BaseMenuScreen_C
    {
        public new class UxmlFactory : UxmlFactory<SettingsFirstScreen_C, UxmlTraits> { }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/settings";
        public override string UssCustomClass_Emc => "setting-first-screen";

        public enum AppSettingsEnum
        {
            None,
            Language,
            Theme,
            Resolution
        }

        public enum GameSettingsEnum
        {
            None,
            Player,
            Controller,
            Audio,
            Notification
        }

        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };
        public ButtonGroup_C AppSettingsButtonGroup = new ButtonGroup_C { name = "app-settings-buttons" };
        public ButtonGroup_C GameSettingsButtonGroup = new ButtonGroup_C { name = "game-settings-buttons" };

        public SettingsFirstScreen_C() { }

        protected override void InitElement()
        {
            base.InitElement();
            ShortScreenTitle = new LocalisationAttribute("Settings", "LauncherScreen", "Settings");

            AppSettingsButtonGroup.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("General", "GenericStrings", "General"),
                new LocalisationAttribute("Graphics", "GenericStrings", "Graphics")
            };
            GameSettingsButtonGroup.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Controls", "GenericStrings", "Controls"),
                new LocalisationAttribute("Audio", "GenericStrings", "Audio"),
                new LocalisationAttribute("Notifications", "GenericStrings", "Notifications")
            };

            Add(ScrollView);
            ScrollView.Add(AppSettingsButtonGroup);
            ScrollView.Add(GameSettingsButtonGroup);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Settings", "LauncherScreen", "Settings");
        }
    }
}
