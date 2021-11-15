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
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    /// <summary>
    /// This class manages the UI elements which gives information about the current session such as : the name of the environment,
    /// is the microphone working, the session tim, etc.
    /// </summary>
    public class SessionInformationMenu : Singleton<SessionInformationMenu>
    {
        public UIDocument uiDocument;

        VisualElement sessionInfo;

        Label sessionTime;
        //Button microphoneBtn;
        Label environmentName;

        VisualElement topCenterMenu;

        DateTime startOfSession = new DateTime();

        /// <summary>
        /// Binds the UI
        /// </summary>
        void Start()
        {
            UnityEngine.Debug.Assert(uiDocument != null);
            var root = uiDocument.rootVisualElement;

            topCenterMenu = root.Q<VisualElement>("top-center-menu");
            topCenterMenu.style.display = DisplayStyle.None;

            sessionInfo = root.Q<VisualElement>("session-info");
            sessionTime = sessionInfo.Q<Label>("session-time");


            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                startOfSession = DateTime.Now;
                topCenterMenu.style.display = DisplayStyle.Flex;
            });
        }

        private void Update()
        {
            var time = DateTime.Now - startOfSession;
            sessionTime.text = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");
        }

        /// <summary>
        /// Initiates the custom title bar with the name of the environment.
        /// </summary>
        /// <param name="media"></param>
        public void SetEnvironmentName(MediaDto media)
        {
            environmentName = uiDocument.rootVisualElement.Q<Label>("environment-name");
            environmentName.text = media.name;
        }
    }
}