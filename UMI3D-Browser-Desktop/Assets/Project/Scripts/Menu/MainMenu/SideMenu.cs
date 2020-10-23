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
using System;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class SideMenu : Singleton<SideMenu>
    {
        #region Fields

        private bool isDisplayed = false;
        static public bool IsDisplayed { get { return Exists ? Instance.isDisplayed : false; } }

        public PanelRenderer panelRenderer;

        [Header("Side menu general settings")]
        VisualElement sideMenuScreen;

        Label sessionTime;
        VisualElement sessionInfo;

        VisualElement sideMenu;
        Button hangUpBtn;

        DateTime startOfSession;

        [Header("Toolbox menu")]
        public MenuDisplayManager toolBoxMenuDisplayManager;
        public MenuAsset ToolboxMenu;

        [Header("Icons")]
        public Texture2D webcamOn;
        public Texture2D webcamOff;
        public Texture2D microOn;
        public Texture2D microOff;
        public Texture2D displayOn;
        public Texture2D displayOff;

        Button sideMenuWebcamBtn;
        Button sideMenuMicroBtn;
        Button sideMenuDisplayOthersBtn;

        VisualElement webcamIndicator;
        VisualElement microIndicator;
        VisualElement displayOthersIndicator;

        #endregion

        #region Methods 

        private void Start()
        {
            BindUI();

            sideMenuScreen.style.display = DisplayStyle.None;

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                sideMenuScreen.style.display = DisplayStyle.Flex;
                startOfSession = DateTime.Now;
            });

            InteractionMapper.Instance.toolboxMenu = ToolboxMenu.menu;

        }

        private void Update()
        {
            var time = DateTime.Now - startOfSession;
            sessionTime.text = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");
        }
       
        #region UI Bindings

        private void BindUI()
        {
            VisualElement root = panelRenderer.visualTree;

            BindSideMenu(root);
            BindSessionInfo(root);
            BindIndicators(root);
        }

        private void BindSideMenu(VisualElement root)
        {
            sideMenuScreen = root.Q<VisualElement>("right-side-menu");
            sideMenu = root.Q<VisualElement>("side-menu");

            hangUpBtn = root.Q<Button>("tool-hang-up");
            hangUpBtn.clickable.clicked += ConnectionMenu.Instance.Leave;
        }

        private void BindSessionInfo(VisualElement root)
        {
            sessionInfo = root.Q<VisualElement>("session-info");
            sessionTime = sessionInfo.Q<Label>("session-time");
        }

        private void BindIndicators(VisualElement root)
        {
            sideMenuWebcamBtn = root.Q<Button>("tool-webcam");
            sideMenuMicroBtn = root.Q<Button>("tool-microphone");
            sideMenuMicroBtn.clickable.clicked += ActivateDeactivateMicrophone.Instance.ToggleMicrophoneStatus;
            sideMenuDisplayOthersBtn = root.Q<Button>("tool-other-people");

            webcamIndicator = root.Q<VisualElement>("top-tool-webcam");
            microIndicator = root.Q<VisualElement>("top-tool-microphone");
            displayOthersIndicator = root.Q<VisualElement>("top-tool-display-others");
        }

        #endregion

        /// <summary>
        /// Displays the side menu.
        /// </summary>
        /// <param name="display"></param>
        static public void Display(bool display = true)
        {
            if (Exists) Instance._Display(display);
        }

        void _Display(bool display = true)
        {
            if (display)
            {
                toolBoxMenuDisplayManager.Display(true);

                sideMenu.experimental.animation.Start(sideMenu.resolvedStyle.width,0, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
                sessionInfo.experimental.animation.Start(-20, 0, 100, (elt, val) =>
                {
                    elt.style.right = val;
                });

                //To remove 
                var viewport = panelRenderer.visualTree.Q<VisualElement>("game-menu");
                viewport.style.display = DisplayStyle.Flex;
                CircularMenu.Instance.HideMenu();
            } else
            {
                toolBoxMenuDisplayManager.Hide(true);

                sideMenu.experimental.animation.Start(0, sideMenu.resolvedStyle.width, 100, (elt, val) =>
                {
                    elt.style.left = val;
                });
                sessionInfo.experimental.animation.Start(0, -20, 100, (elt, val) =>
                {
                    elt.style.right = val;
                });

                //To remove 
                var viewport = panelRenderer.visualTree.Q<VisualElement>("game-menu");
                viewport.style.backgroundColor = new Color(0, 0, 0, 0);
                viewport.style.display = DisplayStyle.None;
                CircularMenu.Instance.ShowMenu();
            }

            isDisplayed = display;
            CursorHandler.SetMovement(this, display ? CursorHandler.CursorMovement.Free : CursorHandler.CursorMovement.Center);
            
        }

        #region Events

        public void OnMicrophoneStatusChanged(bool status)
        {
            if (!status)
            {
                microIndicator.style.backgroundImage = new StyleBackground(microOn);
                sideMenuMicroBtn.style.backgroundImage = new StyleBackground(microOn);
            } else
            {
                microIndicator.style.backgroundImage = new StyleBackground(microOff);
                sideMenuMicroBtn.style.backgroundImage = new StyleBackground(microOff);
            }
        }

        void WebcamStatusChanged(bool status)
        {
            if (status)
            {
                webcamIndicator.style.backgroundImage = new StyleBackground(microOn);
                sideMenuWebcamBtn.style.backgroundImage = new StyleBackground(microOn);
            }
            else
            {
                webcamIndicator.style.backgroundImage = new StyleBackground(microOff);
                sideMenuWebcamBtn.style.backgroundImage = new StyleBackground(microOff);
            }
        }

        void DisplayOthersStatusChanged(bool status)
        {
            if (status)
            {
                displayOthersIndicator.style.backgroundImage = new StyleBackground(microOn);
                sideMenuDisplayOthersBtn.style.backgroundImage = new StyleBackground(microOn);
            }
            else
            {
                displayOthersIndicator.style.backgroundImage = new StyleBackground(microOff);
                sideMenuMicroBtn.style.backgroundImage = new StyleBackground(microOff);
            }
        }

        #endregion

        #endregion
    }
}