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

using BrowserDesktop.Cursor;
using inetum.unityUtils;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class PauseMenu : SingleBehaviour<PauseMenu>
    {
        public UIDocument uiDocument;

        VisualElement pauseMenuContainer;
        Button pauseMenuOpenBtn;
        Button openOptionMenuBtn;
        Button leaveEnvironmentBtn;
        Label shortcutsMenu;

        [SerializeField]
        float pauseMenuHeight = 52;

        public VisualTreeAsset dialogueBoxTreeAsset;

        bool isOpen;

        void Start()
        {
            Debug.Assert(uiDocument != null);
            var root = uiDocument.rootVisualElement;
            var pauseMenu = root.Q<VisualElement>("pause-menu");
            shortcutsMenu = root.Q<Label>("shortcuts-menu");
            pauseMenuContainer = pauseMenu.Q<VisualElement>("pause-menu-container");

            openOptionMenuBtn = pauseMenu.Q<Button>("open-option-menu-btn");
            openOptionMenuBtn.clickable.clicked += () =>
            {
                Debug.Log("TODO");
            };

            leaveEnvironmentBtn = pauseMenu.Q<Button>("leave-environment-btn");
            leaveEnvironmentBtn.clickable.clicked += () =>
            {
                DialogueBoxElement dialogueBox = dialogueBoxTreeAsset.CloneTree().Q<DialogueBoxElement>();
                dialogueBox.Setup("Leave environment", "Are you sure ...?", "YES", "NO", (b) =>
                {
                    if (b)
                        ConnectionMenu.Instance.Leave();
                });

               uiDocument.rootVisualElement.Add(dialogueBox);
            };

            pauseMenuOpenBtn = pauseMenu.Q<Button>("pause-menu-open-btn");
            pauseMenuOpenBtn.clickable.clicked += () =>
            {
                Display(!isOpen);
            };

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                Display(false);
            });
        }

        void Display(bool value)
        {
            isOpen = value;
            CursorHandler.SetMovement(this, value ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);

            if (value)
            {
                leaveEnvironmentBtn.style.display = DisplayStyle.Flex;
                //openOptionMenuBtn.style.display = DisplayStyle.Flex;
                pauseMenuContainer.experimental.animation.Start(0, pauseMenuHeight, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
                pauseMenuOpenBtn.style.marginTop = 0;
                shortcutsMenu.text = "Escape : close pause menu";
            } else
            {
                leaveEnvironmentBtn.style.display = DisplayStyle.None;
                //openOptionMenuBtn.style.display = DisplayStyle.None;
                pauseMenuContainer.experimental.animation.Start(pauseMenuHeight, 0, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
                pauseMenuOpenBtn.style.marginTop = 1;
                shortcutsMenu.text = "Escape : open pause menu";
            }
        }

        public static void ToggleDisplay()
        {
            if (Exists)
            {
                Instance.Display(!Instance.isOpen);
            }
        }
    }
}