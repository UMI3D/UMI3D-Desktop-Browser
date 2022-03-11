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

using inetum.unityUtils;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class ManipulationDisplayerManager : SingleBehaviour<ManipulationDisplayerManager>
    {
        public UIDocument uiDocument;
        public VisualTreeAsset manipualtionTreeAsset;

        public string activeClassName;

        [SerializeField]
        private string containerTagName = null;

        VisualElement container;

        [SerializeField]
        private int manipualtionItemWidth = 90;

        [SerializeField]
        private int manipualtionItemSpaceBetween = 5;


        static int nbManipualtionDisplayed = 0;
        public static int NbManipualtionsEventsDisplayed
        {
            get
            {
                return nbManipualtionDisplayed;
            } 
            set
            {
                if (value >= 0)
                {
                    nbManipualtionDisplayed = value;
                }
            }
        }


        private void Start()
        {
            Debug.Assert(manipualtionTreeAsset != null);
            Debug.Assert(uiDocument != null);
            Debug.Assert(!string.IsNullOrEmpty(activeClassName));
            Debug.Assert(!string.IsNullOrEmpty(containerTagName));

            container = uiDocument.rootVisualElement.Q<VisualElement>(containerTagName);
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

        void LateUpdate()
        {
            if (nbManipualtionDisplayed > 0 && container.resolvedStyle.display == DisplayStyle.None)
            {
                container.style.display = DisplayStyle.Flex;
                container.style.width = manipualtionItemWidth * nbManipualtionDisplayed + (nbManipualtionDisplayed + 1) * manipualtionItemSpaceBetween;
                container.style.paddingBottom = manipualtionItemSpaceBetween;
                container.style.paddingTop = manipualtionItemSpaceBetween;
                container.style.paddingRight = manipualtionItemSpaceBetween;
                container.style.paddingLeft = manipualtionItemSpaceBetween;

            } else if (nbManipualtionDisplayed == 0 && container.resolvedStyle.display == DisplayStyle.Flex)
            {
                container.style.display = DisplayStyle.None;
            }
        }
    }
}