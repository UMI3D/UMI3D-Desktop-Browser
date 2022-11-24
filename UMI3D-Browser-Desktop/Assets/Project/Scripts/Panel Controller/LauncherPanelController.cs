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

using BrowserDesktop.Controller;
using inetum.unityUtils;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LauncherPanelController : umi3d.baseBrowser.connection.BaseLauncherPanelController
{
    public WindowsManager Windows_Manager;

    #region Keyboard
    [DllImport("user32.dll")]
    private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

    /// <summary>
    /// Sets up the inputs according to the user's keyboard layout.
    /// For now, if the keyboard is a 'fr-FR', go for an azerty configuration otherwise a qwerty config.
    /// </summary>
    void SetUpKeyboardConfiguration()
    {
        StringBuilder name = new StringBuilder(9);

        GetKeyboardLayoutName(name);

        string str = name.ToString();

        if (str == InputLayoutManager.FR_Fr_KeyboardLayout || str == InputLayoutManager.FR_Be_KeyboardLayout)
        {
            InputLayoutManager.SetCurrentInputLayout("AzertyLayout");
        }
        else
        {
            InputLayoutManager.SetCurrentInputLayout("QwertyLayout");
        }
    }

    ///// <summary>
    ///// Allows users to use escape and return keys to navigate through the launcher.
    ///// </summary>
    //private void CheckShortcuts()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return) && !DialogueBox_E.Instance.IsDisplaying)
    //        nextStep?.Invoke();
    //    else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)) && !DialogueBox_E.Instance.IsDisplaying)
    //        previousStep?.Invoke();
    //}
    #endregion

    protected override void Start()
    {
        base.Start();

        Debug.Assert(Windows_Manager != null, "WindowsManager reference is null");
        Launcher.Header.Minimize.clicked += Windows_Manager.Minimize;
        Launcher.Header.Maximize.clicked += Windows_Manager.Maximize;
        Launcher.Header.Close.clicked += Application.Quit;
        Launcher.Settings.Audio.SetMic();

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
            dialogueBox.AddToTheRoot(Launcher);
        };

        SetUpKeyboardConfiguration();
    }
}
