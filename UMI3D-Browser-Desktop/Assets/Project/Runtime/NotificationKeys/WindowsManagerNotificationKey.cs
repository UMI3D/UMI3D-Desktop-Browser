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
