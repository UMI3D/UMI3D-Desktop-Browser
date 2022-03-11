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
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class DialogueBox_UIController : umi3d.common.PersistentSingleton<DialogueBox_UIController>
    {
        [SerializeField]
        [Tooltip("VisualTreeAsset of a dialogue box")]
        private VisualTreeAsset dialogueBox_VTA;

        /// <summary>
        /// The one and only dialogue box to be displayed.
        /// </summary>
        private DialogueBoxElement dialogueBox;

        /// <summary>
        /// True if the Dialogue box is visible.
        /// </summary>
        public static bool Displayed { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dialogueBox_VTA != null, "Dialogue box VTA null in DialogueBox_UIController");
            dialogueBox = dialogueBox_VTA.
                CloneTree().
                Q<DialogueBoxElement>().
                Init(
                    Close,
                    (o) => { Cursor.CursorHandler.SetMovement(o, Cursor.CursorHandler.CursorMovement.Free); },
                    (o) => { Cursor.CursorHandler.UnSetMovement(o); }
                );
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && Displayed) Close(true);
            else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)) && Displayed) Close(false);
        }

        public static DialogueBox_UIController Setup(string title, string message, string optionA, string optionB, System.Action<bool> choiceCallback, bool marginForTitleBar = false)
        {
            if (!Displayed) 
                Instance.dialogueBox.
                    Setup(title, message, optionA, optionB, choiceCallback, marginForTitleBar);
            return Instance;
        }

        public static DialogueBox_UIController Setup(string title, string message, string optionA, System.Action choiceCallback, bool marginForTitleBar = false)
        {
            if (!Displayed) 
                Instance.dialogueBox.
                    Setup(title, message, optionA, choiceCallback, marginForTitleBar);
            return Instance;
        }

        public void DisplayFrom(UIDocument uiDocument)
        {
            if (Displayed) return;
            else Displayed = true;
            //dialogueBox.InsertRootTo(uiDocument.rootVisualElement);
        }

        public static void Close(bool val)
        {
            if (!Displayed) return;
            else Displayed = false;

            Instance.dialogueBox.ChoiceCallback(val);
            //Instance.dialogueBox.Remove();
        }
    }
}