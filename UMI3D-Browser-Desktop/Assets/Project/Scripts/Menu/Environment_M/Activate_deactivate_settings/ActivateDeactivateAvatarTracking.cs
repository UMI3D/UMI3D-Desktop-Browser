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
using DesktopBrowser.UIControllers;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.userCapture;
using UnityEngine;

namespace BrowserDesktop.Menu.Environment.Settings
{
    public class ActivateDeactivateAvatarTracking : umi3d.common.Singleton<ActivateDeactivateAvatarTracking>
    {
        bool isEnvironmentLoaded = false;
        private MenuBar_UIController menuBar;

        private void Start()
        {
            umi3d.cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
                isEnvironmentLoaded = true;
            });

            menuBar = UIController.GetUIController("menuBar") as MenuBar_UIController;
            menuBar.OnAvatarTrackingChanged(UMI3DClientUserTracking.Instance.SendTracking);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAvatar)) && !BrowserDesktop.Menu.TextInputDisplayerElement.isTyping)
            {
                ToggleTrackingStatus();
            }
        }

        public void ToggleTrackingStatus()
        {
            if (!isEnvironmentLoaded)
                return;

            UMI3DClientUserTracking.Instance.setTrackingSending(!UMI3DClientUserTracking.Instance.SendTracking);
            menuBar.OnAvatarTrackingChanged(UMI3DClientUserTracking.Instance.SendTracking);
        }
    }
}

