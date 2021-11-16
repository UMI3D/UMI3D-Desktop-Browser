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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrowserDesktop.Menu.Environment.Settings
{
    public class ActivateDeactivateAudio : umi3d.common.Singleton<ActivateDeactivateAudio>
    {
        bool isEnvironmentLoaded = false;
        bool isAudioOn = true;
        public bool AudioOn => isAudioOn;

        void Start()
        {
            umi3d.cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
                isEnvironmentLoaded = true;
            });

            MenuBar_UIController.Instance.OnAudioStatusChanged(isAudioOn);
        }

        void Update()
        {
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.ToggleAudio)) && !BrowserDesktop.Menu.TextInputDisplayerElement.isTyping)
            {
                ToggleAudioStatus();
            }
        }

        public void ToggleAudioStatus()
        {
            if (!isEnvironmentLoaded)
                return;

            isAudioOn = !isAudioOn;
            if (isAudioOn) AudioListener.volume = 1f;
            else AudioListener.volume = 0f;

            MenuBar_UIController.Instance.OnAudioStatusChanged(isAudioOn);
        }
    }
}

