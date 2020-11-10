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

using umi3d.cdk;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using UnityEngine;
using BrowserDesktop.Cursor;
using BrowserDesktop.Menu;

namespace BrowserMenu
{
    public class PauseMenu : Singleton<PauseMenu>
    {
        public PanelRenderer panelRenderer;

        VisualElement pauseMenuContainer;
        Button pauseMenuOpenBtn;
        Button openOptionMenuBtn;
        Button leaveEnvironmentBtn;

        [SerializeField]
        float pauseMenuHeight = 52;

        [SerializeField]
        private VisualTreeAsset dialogueBoxTreeAsset;

        bool isOpen;

        void Start()
        {
            var pauseMenu = panelRenderer.visualTree.Q<VisualElement>("pause-menu");
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

                panelRenderer.visualTree.Add(dialogueBox);
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
            CursorHandler.SetMovement(SideMenu.Instance, value ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
            if (value)
            {
                leaveEnvironmentBtn.style.display = DisplayStyle.Flex;
                openOptionMenuBtn.style.display = DisplayStyle.Flex;
                pauseMenuContainer.experimental.animation.Start(2, pauseMenuHeight, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
                pauseMenuOpenBtn.style.marginTop = 0;
            } else
            {
                leaveEnvironmentBtn.style.display = DisplayStyle.None;
                openOptionMenuBtn.style.display = DisplayStyle.None;
                pauseMenuContainer.experimental.animation.Start(pauseMenuHeight, 2, 100, (elt, val) =>
                {
                    elt.style.height = val;
                });
                pauseMenuOpenBtn.style.marginTop = 1;
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