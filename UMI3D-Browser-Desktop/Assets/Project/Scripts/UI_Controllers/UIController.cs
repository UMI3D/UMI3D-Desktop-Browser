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
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UIControllers
{
    [System.Serializable]
    public struct GlobalUIController<VE>
        where VE : VisualElement
    {
        [SerializeField]
        [Tooltip("UIDocument where the custom element will be displayed.")]
        private UIDocument uiDocument;

        [SerializeField]
        [Tooltip("Visual tree asset of the custom UIElement (if any)")]
        private VisualTreeAsset visualTA;

        public VE CloneVisual()
        {
            Debug.Assert(visualTA != null, "Visual Tree Asset null when clonning.");
            return visualTA.CloneTree().Q<VE>();
        }

        public VE BingVisual(string name)
        {
            Debug.Assert(uiDocument != null, "UIDocument null when Binding visual.");
            return uiDocument.
                rootVisualElement.
                    Q<VisualElement>(name).
                    Q<VE>();
        }
    }
}

