/*
Copyright 2019 Gfi Informatique

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
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.pc;
using UnityEngine;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class LauncherPanelController : umi3d.baseBrowser.connection.BaseLauncherPanelController
{
    protected override void Start()
    {
        base.Start();

        Launcher.Version = BrowserDesktop.BrowserVersion.Version;
        Launcher.Settings.Controller.Controller = ControllerEnum.MouseAndKeyboard;

        GeneralData data;
        if (TryGetGeneralData(out data))
        {
            if (!data.HasChosenLanguage)
                ShowLanguageSelection();
        } else
        {
            Debug.Log("Not Found");
            ShowLanguageSelection();
        }

        Launcher.DisplayHeader = WindowsManager.IsWindowInFullScreen;
    }

    private void OnEnable()
    {
        NotificationHub.Default.Subscribe(
            this,
            QuittingManagerNotificationKey.RequestToQuit,
            null,
            _ApplicationIsQuitting
        );

        NotificationHub.Default.Subscribe(
            this,
            WindowsManagerNotificationKey.FullScreenModeChanged,
            null,
            FullScreenChanged
        );
    }

    private void OnDisable()
    {
        NotificationHub.Default.Unsubscribe(this, QuittingManagerNotificationKey.RequestToQuit);
        NotificationHub.Default.Unsubscribe(this, WindowsManagerNotificationKey.FullScreenModeChanged);
    }

    private void Update()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE
        WindowsManager.Update();
#endif
    }

    void _ApplicationIsQuitting(Notification notification)
    {
        Action<int> callback = index =>
        {
            NotificationHub.Default.Notify(
                this,
                QuittingManagerNotificationKey.QuittingConfirmation,
                new()
                {
                    {QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation, index == 1 }
                }
            );
        };

        NotificationHub.Default.Notify(
            this,
            DialogueBoxNotificationKey.NewDialogueBox,
            new()
            {
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Priority, true },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Size, ElementSize.Small },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueboxType.Confirmation },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, "Close application" },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, "Do you want to close the application?" },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsText, new[] { "Cancel", "Close" } },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsType, new[] { ButtonType.Default, ButtonType.Danger } },
                { DialogueBoxNotificationKey.NewDialogueBoxInfo.Callback, callback },
            }
        );
    }

    void FullScreenChanged(Notification notification)
    {
        if (!notification.TryGetInfoT(WindowsManagerNotificationKey.FullScreenModeChangedInfo.Mode, out FullScreenMode mode))
        {
            return;
        }

        Launcher.DisplayHeader = mode != FullScreenMode.Windowed;
    }

    private void ShowLanguageSelection()
    {
        Debug.Log("Need new Language");
        Launcher.Main.Add(new LocalisationPopUp_C());
    }
}
