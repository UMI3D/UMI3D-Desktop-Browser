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
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class EventMenu : umi3d.common.Singleton<EventMenu>
    {
        public PanelRenderer panelRenderer;

        [SerializeField]
        string containerTagName;
        VisualElement container;

        VisualElement parentContainer;

        [SerializeField]
        VisualTreeAsset eventDisplayerTreeAsset;

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
            Debug.Assert(panelRenderer.visualTree != null);
            container = panelRenderer.visualTree.Q<VisualElement>(containerTagName);
            parentContainer = panelRenderer.visualTree.Q<VisualElement>("information-pop-up-events");
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
    }
}