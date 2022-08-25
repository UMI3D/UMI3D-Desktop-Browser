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
using umi3dDesktopBrowser.ui;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This class is reponsible for connecting users to environments. It implies asking for login/password or parameters if 
/// necessary.
/// </summary>
public class ConnectionMenu : umi3d.baseBrowser.connection.BaseConnectionMenu
{
    protected override void DisplayDialogueBox(string title, string message, string optionA, string optionB, System.Action<bool> callback)
    {
        DialogueBox_E.Instance.Setup(title, message, optionA, optionB, callback);
        DialogueBox_E.Instance.DisplayFrom(document);
    }
    protected override void DisplayDialogueBox(string title, string message, string option, System.Action callback)
    {
        DialogueBox_E.Instance.Setup(title, message, option, callback);
        DialogueBox_E.Instance.DisplayFrom(document);
    }

    protected override void Display()
    {
        base.Display();
        MainView.Instance.Display(false);
    }

    protected override void OnRedirectionAborted()
    {
        base.OnRedirectionAborted();
        MainView.Instance.Display(true);
    }

    protected override void InitUI()
    {
        base.InitUI();
        VisualElement root = document.rootVisualElement;
        root.Q<Label>("version").text = BrowserDesktop.BrowserVersion.Version;
        root.RegisterCallback<GeometryChangedEvent>(ResizeElements);
    }

    protected override void ResizeElements(GeometryChangedEvent e)
    {
        logo.style.height = e.newRect.height * 0.16f;
        logo.style.marginBottom = e.newRect.height * 0.08f;
    }

    protected override void Start()
    {
        base.Start();
        Settingbox_E.Instance.LeaveButton.Clicked += () =>
        {
            DisplayDialogueBox
            (
                "Leave environment",
                "Are you sure ...?",
                "YES", "NO",
                (b) =>
                {
                    if (b) Leave();
                }
            );
        };
    }

    private void Update()
    {
        ManageInputs();
    }

    /// <summary>
    /// Manages the Return input to navigate through the menu.
    /// </summary>
    private void ManageInputs()
    {
        if (!isDisplayed || DialogueBox_E.Instance.IsDisplaying) return;
        else if (Input.GetKeyDown(KeyCode.Return)) nextStep?.Invoke();
    }
}