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

public enum SettingsScreensEnum
{
    FirstScreen,
    General,
    Graphics,

    Controls,
    Audio,
    Notifications
}

public class CustomSettingsContainer : CustomMenuScreen<SettingsScreensEnum>
{
    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/settings";
    public override string USSCustomClassName => "setting-container";

    public CustomSettingsFirstScreen FirstScreen;
    public CustomSettingsGeneral General;
    public CustomSettingsResolution Resolution;

    public CustomSettingsController Controller;
    public CustomSettingsAudio Audio;
    public CustomSettingsNotification Notification;

    public override void InitElement()
    {
        base.InitElement();

        FirstScreen.AppSettingsButtonGroup.ValueChanged += value =>
        {
            if (!System.Enum.TryParse<SettingsScreensEnum>(value.ToString(), out var result)) return;

            AddScreenToStack = result;
        };

        FirstScreen.GameSettingsButtonGroup.ValueChanged += value =>
        {
            if (!System.Enum.TryParse<SettingsScreensEnum>(value.ToString(), out var result)) return;

            AddScreenToStack = result;
        };

        General.BackButtonCkicked = () => RemoveScreenFromStack();
        Resolution.BackButtonCkicked = () => RemoveScreenFromStack();
        Controller.BackButtonCkicked = () => RemoveScreenFromStack();
        Audio.BackButtonCkicked = () => RemoveScreenFromStack();
        Notification.BackButtonCkicked = () => RemoveScreenFromStack();
    }

    public override void Set() => Set(null, SettingsScreensEnum.FirstScreen);

    protected override void GetScreen(SettingsScreensEnum screenEnum, out CustomMenuScreen screen)
    {
        switch (screenEnum)
        {
            case SettingsScreensEnum.FirstScreen:
                screen = FirstScreen;
                break;
            case SettingsScreensEnum.General:
                screen = General;
                break;
            case SettingsScreensEnum.Graphics:
                screen = Resolution;
                break;
            case SettingsScreensEnum.Controls:
                screen = Controller;
                break;
            case SettingsScreensEnum.Audio:
                screen = Audio;
                break;
            case SettingsScreensEnum.Notifications:
                screen = Notification;
                break;
            default:
                screen = null;
                break;
        }
    }

    protected override void RemoveAllScreen()
    {
        FirstScreen.RemoveFromHierarchy();
        General.RemoveFromHierarchy();
        Resolution.RemoveFromHierarchy();
        Controller.RemoveFromHierarchy();
        Audio.RemoveFromHierarchy();
        Notification.RemoveFromHierarchy();
    }

    protected override void ResetButton()
    {
        FirstScreen.AppSettingsButtonGroup.SetValueWithoutNotify(null);
        FirstScreen.GameSettingsButtonGroup.SetValueWithoutNotify(null);
    }
}
