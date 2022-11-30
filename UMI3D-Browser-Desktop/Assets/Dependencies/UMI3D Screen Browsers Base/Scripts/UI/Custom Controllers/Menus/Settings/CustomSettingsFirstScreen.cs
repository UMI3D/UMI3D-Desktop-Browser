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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomSettingsFirstScreen : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomSettingsFirstScreen;

            custom.Set();
        }
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/settings";
    public override string USSCustomClassName => "setting-first-screen";

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

    public CustomScrollView ScrollView;
    public CustomButtonGroup AppSettingsButtonGroup;
    public CustomButtonGroup GameSettingsButtonGroup;

    public override void InitElement()
    {
        base.InitElement();

        ShortScreenTitle = "Settings";

        AppSettingsButtonGroup.Options = $"{SettingsScreensEnum.General}, {SettingsScreensEnum.Graphics}";
        GameSettingsButtonGroup.Options = $"{SettingsScreensEnum.Controls}, {SettingsScreensEnum.Audio}, {SettingsScreensEnum.Notifications}";

        Add(ScrollView);
        ScrollView.Add(AppSettingsButtonGroup);
        ScrollView.Add(GameSettingsButtonGroup);
    }

    public override void Set() => Set("Settings");
}
