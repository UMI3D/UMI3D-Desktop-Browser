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

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UserPreferences
{
    [CreateAssetMenu(fileName = "IconPreferences", menuName = "ScriptableObjects/UserPreferences/IconPreferences")]
    public class IconPreferences_SO : ScriptableObject
    {
        [System.Serializable]
        public class IconPref
        {
            [Tooltip("Name of this icon preference.")]
            [SerializeField]
            private string iconPrefName;
            /// <summary>
            /// Name of this icon preference.
            /// </summary>
            public string IconPrefName => iconPrefName;
            
            [Space]
            [Tooltip("If true this icon width and height will be resized")]
            [SerializeField]
            private bool ResizedWhenZoomed;
            [Tooltip("Width of the icon when zoom is set to 100%. If value < 0 the width will be set within the code ; if value == 0 there won't be any resizement.")]
            [SerializeField]
            private float width;
            [Tooltip("height of the icon when zoom is set to 100%. If value < 0 the width will be set within the code ; if value == 0 there won't be any resizement.")]
            [SerializeField]
            private float height;

            public void SetIcon(VisualElement icon, string iconClass, float width, float height)
            {
                icon.ClearClassList();
                if (!string.IsNullOrEmpty(iconClass))
                    icon.AddToClassList(iconClass);

                if (this.width > 0f)
                    icon.style.width = (ResizedWhenZoomed) ? this.width * UserPreferences.GlobalPref.ZoomCoef : this.width;
                else if (this.width < 0f && width > 0f)
                    icon.style.width = (ResizedWhenZoomed) ? width * UserPreferences.GlobalPref.ZoomCoef : width;
                else if (this.width < 0f)
                    throw new System.Exception($"Width not set in {this.iconPrefName}");
                if (this.height > 0f)
                    icon.style.height = (ResizedWhenZoomed) ? this.height * UserPreferences.GlobalPref.ZoomCoef : this.height;
                else if (this.height < 0f && height > 0f)
                    icon.style.height = (ResizedWhenZoomed) ? height * UserPreferences.GlobalPref.ZoomCoef : height;
                else if (this.height < 0f)
                    throw new System.Exception($"height not set in {this.iconPrefName}");
            }
        }

        [SerializeField]
        [Tooltip("List of icon preferences.")]
        private IconPref[] iconPrefs;

        public IconPreferences_SO(IconPreferences_SO iconPreferences)
        {
            //TODO Copy properties.
        }

        public IEnumerator ApplyPref(VisualElement icon, string iconPrefName, string iconClass, float width, float height)
        {
            foreach (IconPref iconPref in iconPrefs)
            {
                if (iconPref.IconPrefName.ToLowerInvariant() == iconPrefName.ToLowerInvariant())
                {
                    iconPref.SetIcon(icon, iconClass, width, height);
                    yield break;
                }
            }
            Debug.LogError($"IconPrefName = [{iconPrefName}] not recognized.");
        }

        [ContextMenu("Apply User Pref")]
        private void ApplyUserPref()
        {
            UserPreferences.Instance.OnApplyUserPreferences.Invoke();
        }


    }
}