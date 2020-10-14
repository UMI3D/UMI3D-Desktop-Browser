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
using System.Collections.Generic;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class ManipulationDisplayerManager : umi3d.common.Singleton<ManipulationDisplayerManager>
    {
        public PanelRenderer panelRenderer;
        public VisualTreeAsset manipualtionTreeAsset;

        public string activeClassName;

        [SerializeField]
        private string containerTagName;

        VisualElement container;

        private void Start()
        {
            Debug.Assert(manipualtionTreeAsset != null);
            Debug.Assert(panelRenderer != null);
            Debug.Assert(!string.IsNullOrEmpty(activeClassName));
            Debug.Assert(!string.IsNullOrEmpty(containerTagName));

            container = panelRenderer.visualTree.Q<VisualElement>(containerTagName);
        }

        public static ManipulationElement CreateDisplayer()
        {
            if (Exists)
            {
                var manipulationElement = Instance.manipualtionTreeAsset.CloneTree().Q<ManipulationElement>();
                Instance.container.Add(manipulationElement);

                return manipulationElement;
            }
            return null;
        }
    }
}