/*
Copyright 2019 - 2024 Inetum

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
using umi3d.browserRuntime;
using umi3d.browserRuntime.notificationKeys;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.game;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public static class DialogueBoxManager
    {
        public static VisualElement root { get; private set; }

        public static void SetRoot(VisualElement root)
        {
            // TODO: Test for case when dialoguebox is displayer while changing the root.
            DialogueBoxManager.root = root;
        }

        static DialogueBoxManager()
        {
            NotificationHub.Default.Subscribe(
                typeof(DialogueBoxManager).FullName,
                DialogueBoxNotificationKey.NewDialogueBox,
                null,
                DialogueBoxNewDialogueBox
            );

            NotificationHub.Default.Subscribe(
                typeof(DialogueBoxManager).FullName,
                DeeplinkingNotificationKey.ArgumentsReceived,
                null,
                DeeplinkingArgumentsReceived
            );
        }

        static void DialogueBoxNewDialogueBox(Notification notification)
        {
            UnityEngine.Debug.Log($"DialogueBoxManager new dialogue box");

            Dialoguebox_C dialogueBox = new();

            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Size, out ElementSize size))
            {
                dialogueBox.Size = size;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, out DialogueboxType dialogueboxType))
            {
                dialogueBox.Type = dialogueboxType;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, out string title))
            {
                dialogueBox.Title = title;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, out string message))
            {
                dialogueBox.Message = message;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsText, out string[] buttonsText))
            {
                if (buttonsText?.Length >= 1)
                {
                    dialogueBox.ChoiceAText = buttonsText[0];
                }
                if (buttonsText?.Length >= 2)
                {
                    dialogueBox.ChoiceBText = buttonsText[1];
                }
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.ButtonsType, out ButtonType[] buttonsType))
            {
                if (buttonsType?.Length >= 1)
                {
                    dialogueBox.ChoiceA.Type = buttonsType[0];
                }
                if (buttonsType?.Length >= 2)
                {
                    dialogueBox.ChoiceB.Type = buttonsType[1];
                }
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Callback, out Action<int> callback))
            {
                dialogueBox.Callback = callback;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Priority, out bool priority))
            {
                if (priority)
                {
                    dialogueBox.EnqueuePriority(root);
                }
                else
                {
                    dialogueBox.Enqueue(root);
                }
            }
        }

        static void DeeplinkingArgumentsReceived(Notification notification)
        {
            UnityEngine.Debug.Log($"DialogueBoxManager argument received");
        }
    }
}
