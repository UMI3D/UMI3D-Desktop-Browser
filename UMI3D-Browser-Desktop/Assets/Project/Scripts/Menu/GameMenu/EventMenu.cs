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
    public class EventMenu : SingleBehaviour<EventMenu>
    {
        public UIDocument uiDocument;

        [SerializeField]
        string containerTagName = null;
        VisualElement container;

        VisualElement parentContainer;

        [SerializeField]
        VisualTreeAsset eventDisplayerTreeAsset = null;

        private int nbEventsDisplayed;
        public static int NbEventsDIsplayed
        {
            get
            {
                if (Exists) return Instance.nbEventsDisplayed; else return 0;
            } 
            set
            {
                if (Exists)
                {
                    if (value >= 0)
                        Instance.nbEventsDisplayed = value;
                }
            }
        }

        void Start()
        {
            Debug.Assert(uiDocument != null);
            container = uiDocument.rootVisualElement.Q<VisualElement>(containerTagName);
            parentContainer = uiDocument.rootVisualElement.Q<VisualElement>("information-pop-up-events");
            Debug.Assert(container != null);
        }

        static public EventDisplayer CreateDisplayer()
        {
            if (Exists && Instance.container != null)
            {
                var displayer = Instance.eventDisplayerTreeAsset.CloneTree().Q<EventDisplayer>();
                Instance.container.Add(displayer);
                return displayer;
            }
            return null;
        }

        public static void Expand(bool val)
        {
            if (Exists)
            {
                Instance.parentContainer.style.flexGrow = val ? 1 : 0;
            }
        }
    }
}