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

using System;
using System.Collections;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UI.CustomElement
{
    public class DialogueBoxElement : GenericAndCustomElement
    {
        #region Fields

        public new class UxmlFactory : UxmlFactory<DialogueBoxElement, UxmlTraits> { }

        private Action<bool> choiceCallback;
        /// <summary>
        /// Action to be performed when the user interact with the dialogue box.
        /// </summary>
        public Action<bool> ChoiceCallback => choiceCallback;

        /// <summary>
        /// Title of this dialogue box.
        /// </summary>
        private Label title_L;
        /// <summary>
        /// Message of this dialogue box.
        /// </summary>
        private Label message_L;
        /// <summary>
        /// Button on the left.
        /// </summary>
        private Button optionA_B;
        /// <summary>
        /// Button on the right.
        /// </summary>
        private Button optionB_B;

        /// <summary>
        /// Text of the title
        /// </summary>
        private string titleText;
        /// <summary>
        /// Text of the message
        /// </summary>
        private string messageText;

        #endregion

        /// <summary>
        /// Initialize the dialogue box the first time (UI binding and button settings).
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Cursor.CursorHandler.SetMovement(this, Cursor.CursorHandler.CursorMovement.Free);
            this.style.position = Position.Absolute;

            title_L = this.Q<Label>("dialogue-box-title");
            message_L = this.Q<Label>("dialogue-box-message");

            optionA_B = this.Q<Button>("dialogue-box-btn1");
            optionB_B = this.Q<Button>("dialogue-box-btn2");

            optionA_B.clickable.clicked += () =>
            {
                Menu.DialogueBox_UIController.Close(true);
            };
            optionB_B.clickable.clicked += () =>
            {
                Menu.DialogueBox_UIController.Close(false);
            };
        }

        /// <summary>
        /// Sets up the dialogue box for two choices.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="marginForTitleBar"></param>
        public void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback, bool marginForTitleBar = false)
        {
            Setup(title, message);

            optionB_B.style.display = DisplayStyle.Flex;
            optionA_B.text = optionA;
            optionB_B.text = optionB;

            if (marginForTitleBar)
            {
                this.style.marginTop = 40;
            }

            this.choiceCallback = (b) =>
            {
                Cursor.CursorHandler.UnSetMovement(this);
                choiceCallback(b);
            };
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
            Setup(title, message);

            optionB_B.style.display = DisplayStyle.None;
            optionA_B.text = optionA;

            if (marginForTitleBar)
            {
                this.style.marginTop = 40;
            }

            this.choiceCallback = (b) =>
            {
                Cursor.CursorHandler.UnSetMovement(this);
                choiceCallback();
            };
        }

        /// <summary>
        /// Sets ups elements which do not depends on the number of choices.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void Setup(string title, string message)
        {
            Initialize();

            titleText = title;
            messageText = message;
        }

        public override void OnApplyUserPreferences()
        {
            if (!Menu.DialogueBox_UIController.Displayed) return;

            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(title_L, "title", titleText);
            UserPreferences.UserPreferences.TextAndIconPref.ApplyTextPref(message_L, "corps", messageText);
        }
    }
}