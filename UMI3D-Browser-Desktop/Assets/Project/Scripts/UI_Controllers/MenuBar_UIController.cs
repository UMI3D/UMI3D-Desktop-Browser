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
using umi3dDesktopBrowser.uI.viewController;
using UnityEngine.UIElements;

namespace DesktopBrowser.UIControllers
{
    public class MenuBar_UIController : UIController
    {
        private MenuBar_E menuBar;
        private ToolboxItem_E avatar;
        private ToolboxItem_E sound;
        private ToolboxItem_E mic;

        private Action<VisualElement> addSeparator;

        public Action ToggleAvatarTracking;
        public Action ToggleAudio;
        public Action ToggleMic;

        public bool Initialized { get; private set; } = false;

        /// <summary>
        /// Event called when the status of the avatar tracking changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAvatarTrackingChanged(bool val)
        {
            avatar.Toggle(val);
        }
        /// <summary>
        /// Event called when the status of the audio changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnAudioStatusChanged(bool val)
        {
            sound.Toggle(val);
        }
        /// <summary>
        /// Event called when the status of the microphone changes.
        /// </summary>
        /// <param name="val"></param>
        public void OnMicrophoneStatusChanged(bool val)
        {
            mic.Toggle(val);
        }



    }
}