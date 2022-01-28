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

using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.Events;

namespace BrowserDesktop.preferences
{
    public partial class UserPreferences : umi3d.common.PersistentSingleton<UserPreferences>
    {
        /// <summary>
        /// Root of the path where preferences are stored.
        /// </summary>
        public const string UserPrefRootPath = "UserPreferences/";

        [Tooltip("")]
        [SerializeField]
        private GlobalPreferences_SO globalPreferences_SO;
        public static GlobalPreferences_SO GlobalPref => (Exists) ? Instance.globalPreferences_SO : null;

        [Tooltip("")]
        [SerializeField]
        private TextAndIconPreferences textAndIconPref = new TextAndIconPreferences();
        public static TextAndIconPreferences TextAndIconPref => (Exists) ? Instance.textAndIconPref : null;

        [HideInInspector]
        public UnityEvent OnApplyUserPreferences = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();

            textAndIconPref.Load();
        }
    }
}