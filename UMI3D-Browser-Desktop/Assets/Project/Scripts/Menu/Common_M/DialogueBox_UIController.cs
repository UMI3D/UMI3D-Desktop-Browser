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
using BrowserDesktop.UI.CustomElement;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class DialogueBox_UIController : umi3d.common.PersistentSingleton<DialogueBox_UIController>
    {
        [Tooltip("VisualTreeAsset of a dialogue box")]
        [SerializeField]
        private VisualTreeAsset dialogueBox_VTA;

        private DialogueBoxElement dialogueBox;

        private bool isDisplayed = false;
        public static bool Displayed => Instance.isDisplayed;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dialogueBox_VTA != null, "Dialogue box VTA null in DialogueBox_UIController");
            dialogueBox = dialogueBox_VTA.CloneTree().Q<DialogueBoxElement>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && isDisplayed) Close(true);
            else if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.MainMenuToggle)) && isDisplayed) Close(false);
        }

        public static DialogueBox_UIController Setup(string title, string message, string optionA, string optionB, System.Action<bool> choiceCallback, bool marginForTitleBar = false)
        {
            if (!Instance.isDisplayed) Instance.dialogueBox.Setup(title, message, optionA, optionB, choiceCallback, marginForTitleBar);
            return Instance;
        }

        public static DialogueBox_UIController Setup(string title, string message, string optionA, System.Action choiceCallback, bool marginForTitleBar = false)
        {
            if (!Instance.isDisplayed) Instance.dialogueBox.Setup(title, message, optionA, choiceCallback, marginForTitleBar);
            return Instance;
        }

        public void DisplayFrom(UIDocument uiDocument)
        {
            if (isDisplayed) return;

            isDisplayed = true;
            uiDocument.rootVisualElement.Add(dialogueBox);
        }

        public static void Close(bool val)
        {
            if (!Instance.isDisplayed) return;

            Instance.isDisplayed = false;

            Instance.dialogueBox.ChoiceCallback(val);
            Instance.dialogueBox.RemoveFromHierarchy();
        }
    }
}