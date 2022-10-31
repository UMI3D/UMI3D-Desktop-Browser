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
using System.Runtime.InteropServices;
using System.Text;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;

public class LauncherPanelController : umi3d.baseBrowser.connection.BaseLauncherPanelController
{
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

        SetUpKeyboardConfiguration();
    }
}
