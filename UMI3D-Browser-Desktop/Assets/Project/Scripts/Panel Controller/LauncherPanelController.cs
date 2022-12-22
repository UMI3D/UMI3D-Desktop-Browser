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
using UnityEngine;

public class LauncherPanelController : umi3d.baseBrowser.connection.BaseLauncherPanelController
{
    public WindowsManager Windows_Manager;

    protected override void Start()
    {
        base.Start();

        Debug.Assert(Windows_Manager != null, "WindowsManager reference is null");
        Launcher.Header.Minimize.clicked += Windows_Manager.Minimize;
        Launcher.Header.Maximize.clicked += Windows_Manager.Maximize;
        Launcher.Header.Close.clicked += Application.Quit;

        Windows_Manager.FullScreenEnabled = value => Launcher.DisplayHeader = value;
        Windows_Manager.DisplayDialogueBoxToQuit = () =>
        {
            var dialogueBox = new umi3d.commonScreen.Displayer.Dialoguebox_C();
            dialogueBox.Size = ElementSize.Small;
            dialogueBox.Type = DialogueboxType.Confirmation;
            dialogueBox.Title = "Close application";
            dialogueBox.Message = "Do you want to close the application?";
            dialogueBox.ChoiceAText = "Cancel";
            dialogueBox.ChoiceA.Type = ButtonType.Default;
            dialogueBox.ChoiceBText = "Close";
            dialogueBox.Callback = index =>
            {
                QuittingManager.ApplicationIsQuitting = index == 1;
                if (index == 1) Application.Quit();
            };
            dialogueBox.EnqueuePriority(Launcher);
        };

        Launcher.Version = BrowserDesktop.BrowserVersion.Version;
    }
}
