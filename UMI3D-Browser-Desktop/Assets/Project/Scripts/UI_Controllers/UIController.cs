/*
Copyright 2019 - 2021 Inetum

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
using BrowserDesktop.UI;
using System.Collections.Generic;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UIControllers
{
    public class UIController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private string uIControllerName;

        [SerializeField]
        [Tooltip("UIDocument where the custom element will be displayed.")]
        private UIDocument uiDocument;
        public UIDocument UIDoc
        {
            get
            {
                Debug.Assert(uiDocument != null, "UIDocument null when Binding visual.");
                return uiDocument;
            }
        }
        [SerializeField]
        [Tooltip("Visual tree asset of the custom UIElement (if any)")]
        private VisualTreeAsset visualTA;
        public VisualTreeAsset VisualTA
        {
            get
            {
                Debug.Assert(visualTA != null, "Visual Tree Asset null when clonning.");
                return visualTA;
            }
        }

        protected static Dictionary<string, UIController> uIControllers = new Dictionary<string, UIController>();

        #endregion

        protected virtual void Awake()
        {
            Debug.Assert(!uIControllers.ContainsKey(tag));
            Debug.Assert(!string.IsNullOrEmpty(uIControllerName));
            uIControllers.Add(uIControllerName, this);
        }

        public static UIController GetUIController(string key)
        {
            Debug.Assert(uIControllers.ContainsKey(key));
            return uIControllers[key];
        }

        public VisualElement BindVisual(string name)
        {
            VisualElement result = UIDoc.rootVisualElement.Q<VisualElement>(name);
            return (result != null) ? result : throw new System.Exception($"[{name}] not found in scene.");
        }
    }
}

