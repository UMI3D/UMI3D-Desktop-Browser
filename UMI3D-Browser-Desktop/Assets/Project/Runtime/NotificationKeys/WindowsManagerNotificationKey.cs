/*
Copyright 2019 - 2024 Inetum

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.notificationKeys
{
    public static class WindowsManagerNotificationKey
    {
        /// <summary>
        /// Hide the window.
        /// </summary>
        public const string Hide = "Hide";

        /// <summary>
        /// Hide the window
        /// </summary>
        public const string Minimize = "Minimize";

        /// <summary>
        ///  If the window is hidden, maximize (display) the window in its previous state, else do nothing.
        /// </summary>
        public const string Maximize = "Windowed";

        /// <summary>
        /// Ask to change the full screen mode.
        /// </summary>
        public const string FullScreenModeWillChange = "FullScreenModeWillChange";

        /// <summary>
        /// The full screen mode has changed.
        /// </summary>
        public const string FullScreenModeChanged = "FullScreenModeChanged";

        public class FullScreenModeChangedInfo
        {
            /// <summary>
            /// Mode of the full screen. <see cref="FullScreenMode"/>
            /// </summary>
            public const string Mode = "Mode";
        }
    }
}
