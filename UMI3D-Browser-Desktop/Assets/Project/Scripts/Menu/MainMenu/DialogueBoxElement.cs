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


using BrowserDesktop.UI;
using System;
using System.Collections;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueBoxElement : GenericAndCustomElement
{
    public new class UxmlFactory : UxmlFactory<DialogueBoxElement, UxmlTraits> { }

    /// <summary>
    /// Return true if the dialogue box is displayed
    /// </summary>
    public static bool IsADialogueBoxDislayed => currentDialogueBox != null;

    private static DialogueBoxElement currentDialogueBox;
    private Action<bool> choiceCallback;
    

    /// <summary>
    /// Sets up the dialogue box for two choices.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="optionA"></param>
    /// <param name="optionB"></param>
    /// <param name="choiceCallback"></param>
    /// <param name="marginForTitleBar"></param>
    public void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback, bool marginForTitleBar = false) {
        (Button optionABtn, Button optionBBtn) = Setup(title, message);

        optionABtn.text = optionA;
        optionABtn.clickable.clicked += () =>
        {
            CloseDialogueBox(true);
        };

        optionBBtn.text = optionB;
        optionBBtn.clickable.clicked += () =>
        {
            CloseDialogueBox(false);
        };

        if (marginForTitleBar)
        {
            this.style.marginTop = 40;
        }

        this.choiceCallback = (b) =>
        {
            BrowserDesktop.Cursor.CursorHandler.UnSetMovement(this);
            choiceCallback(b);
        };
        currentDialogueBox = this;
        
    }

    /// <summary>
    /// Sets up the dialogue box for one choice.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="optionA"></param>
    /// <param name="optionB"></param>
    /// <param name="choiceCallback"></param>
    /// <param name="marginForTitleBar"></param>
    public void Setup(string title, string message, string optionA, Action choiceCallback, bool marginForTitleBar = false)
    {
        (Button optionABtn, Button optionBBtn) = Setup(title, message);

        optionBBtn.style.display = DisplayStyle.None;

        optionABtn.text = optionA;
        optionABtn.clickable.clicked += () =>
        {
            CloseDialogueBox(true);
        };

        if (marginForTitleBar)
        {
            this.style.marginTop = 40;
        }

        this.choiceCallback = (b) =>
        {
            BrowserDesktop.Cursor.CursorHandler.UnSetMovement(this);
            choiceCallback();
        };
        currentDialogueBox = this;
        
    }

    /// <summary>
    /// Sets ups elements which do not depends on the number of choices.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private (Button, Button) Setup(string title, string message)
    {
        BrowserDesktop.Cursor.CursorHandler.SetMovement(this, BrowserDesktop.Cursor.CursorHandler.CursorMovement.Free);
        this.style.position = Position.Absolute;

        this.Q<Label>("dialogue-box-title").text = title;
        this.Q<Label>("dialogue-box-message").text = message;

        Button optionABtn = this.Q<Button>("dialogue-box-btn1");
        Button optionBBtn = this.Q<Button>("dialogue-box-btn2");

        return (optionABtn, optionBBtn);
    }

    public static void CloseDialogueBox(bool choice)
    {

        Debug.Log($"Dialogue box == null : {currentDialogueBox == null}");
        currentDialogueBox.RemoveFromHierarchy();
        currentDialogueBox.choiceCallback.Invoke(choice);
        currentDialogueBox = null;
    }

    public override void OnApplyUserPreferences()
    {
        //throw new NotImplementedException();
    }
}
