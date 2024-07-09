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
using umi3d.browserRuntime;
using umi3d.commonScreen.Container;
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
            UnityEngine.Debug.Log($"DialogueBoxManager argument received");

            //Dialoguebox_C dialoguebox = new();
            //dialoguebox.Type = DialogueboxType.Default;
            //dialoguebox.Title = new LocalisationAttribute("Server error", "ErrorStrings", "ServerError");
            //dialoguebox.Message = message;
            //dialoguebox.ChoiceAText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
            //dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
            //dialoguebox.Enqueue(root);
        }

        static void DeeplinkingArgumentsReceived(Notification notification)
        {
            UnityEngine.Debug.Log($"DialogueBoxManager argument received");
        }
    }
}
