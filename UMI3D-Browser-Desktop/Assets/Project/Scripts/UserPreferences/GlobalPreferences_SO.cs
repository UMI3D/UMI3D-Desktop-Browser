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
    [CreateAssetMenu(fileName = "GlobalPreferences", menuName = "ScriptableObjects/UserPreferences/GlobalPreferences")]
    public class GlobalPreferences_SO : ScriptableObject
    {
        [HideInInspector]
        public UnityEvent ApplyCustomStyle = new UnityEvent();

        [Range(minZoom, maxZoom)]
        [Tooltip("Zoom percentage")]
        [SerializeField]
        private int zoomPercentage = 100;
        private const int minZoom = 20;
        private const int maxZoom = 200;

        [Tooltip("TODO")]
        [SerializeField]
        private Theme_SO currentTheme;
        public Theme_SO CurrentTheme
        {
            get => currentTheme;
            set => currentTheme = value;
        }

        public int Zoom
        {
            get => zoomPercentage;
            set
            {
                if (value < minZoom || value > maxZoom) Debug.LogError("Zoom value behond limit");
                else zoomPercentage = value;
            }
        }
        public float ZoomCoef
            => (float)zoomPercentage / 100f;

        public float ApplyZoomCoef(float value)
            => ZoomCoef * value;

        [ContextMenu("Apply Custom Style")]
        private void ApplyCustomStyleInInspector() => ApplyCustomStyle.Invoke();
    }
}