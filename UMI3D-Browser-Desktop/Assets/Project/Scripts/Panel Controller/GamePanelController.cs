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
using UnityEngine;

/// <summary>
/// This class is reponsible for connecting users to environments. It implies asking for login/password or parameters if 
/// necessary.
/// </summary>
public class GamePanelController : umi3d.baseBrowser.connection.BaseGamePanelController
{
    [Header("Windows Manager")]
    public WindowsManager Windows_Manager;

    protected override void Start()
    {
        base.Start();

        Debug.Assert(Windows_Manager != null, "WindowsManager reference null");
        Loader.Minimize.clicked += Windows_Manager.Minimize;
        Loader.Maximize.clicked += Windows_Manager.Maximize;
        Loader.Close.clicked += Application.Quit;
        Menu.Minimize.clicked += Windows_Manager.Minimize;
        Menu.Maximize.clicked += Windows_Manager.Maximize;
        Menu.Close.clicked += Application.Quit;

        Windows_Manager.FullScreenEnabled = value =>
        {
            Loader.DisplayHeader = value;
            Menu.DisplayHeader = value;
        };
        Windows_Manager.DisplayDialogueBoxToQuit = () =>
        {
            var dialogueBox = new umi3d.commonScreen.Displayer.Dialoguebox_C();
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
            dialogueBox.AddToTheRoot(Game);
        };
    }

    public override CustomDialoguebox CreateDialogueBox()
        => new umi3d.commonScreen.Displayer.Dialoguebox_C();
}
