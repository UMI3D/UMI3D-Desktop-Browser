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
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.pc;
using UnityEngine;

/// <summary>
/// This class is reponsible for connecting users to environments. It implies asking for login/password or parameters if 
/// necessary.
/// </summary>
public class GamePanelController : umi3d.baseBrowser.connection.BaseGamePanelController
{
    protected override void Start()
    {
        base.Start();

        GamePanel.DisplayHeader = WindowsManager.IsWindowInFullScreen;
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

        GamePanel.DisplayHeader = mode == FullScreenMode.FullScreenWindow;
    }

    protected override void InitLoader()
    {
        base.InitLoader();

        Loader.Version = BrowserDesktop.BrowserVersion.Version;
    }

    protected override void InitGame()
    {
        base.InitGame();

        Menu.Version = BrowserDesktop.BrowserVersion.Version;
    }

    protected override void InitControls()
    {
        base.InitControls();

        Menu.Settings.Controller.Controller = ControllerEnum.MouseAndKeyboard;
    }
}
