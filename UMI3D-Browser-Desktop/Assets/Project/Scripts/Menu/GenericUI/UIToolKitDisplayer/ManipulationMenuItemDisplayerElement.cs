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
using umi3d.cdk.menu.view;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class ManipulationMenuItemDisplayerElement : AbstractManipulationMenuItemDisplayer, IDisplayerElement
    {
        public VisualTreeAsset manipulationMenuItemTreeAsset;

        /// <summary>
        /// Selection event subscribers.
        /// </summary>
        private List<UnityAction<bool>> subscribers = new List<UnityAction<bool>>();

        public bool select = false;

        /// <summary>
        /// Button
        /// </summary>
        Button button;

        public override void Clear()
        {
            base.Clear();
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            button.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
        {
            button.style.display = DisplayStyle.None;
        }

        void NotifyPress()
        {
            ChangeSelectState(!select);
        }

        /// <summary>
        /// Raise selection event.
        /// </summary>
        public override void Select()
        {
            foreach (UnityAction<bool> sub in subscribers)
                sub.Invoke(true);
        }

        public override void Deselect()
        {
            foreach (UnityAction<bool> sub in subscribers)
                sub.Invoke(false);
        }

        /// <summary>
        /// Select if the argument is true, deselect it otherwise.
        /// </summary>
        /// <param name="select"></param>
        public void ChangeSelectState(bool select)
        {
            this.select = select;
            if (select)
                Select();
            else
                Deselect();
        }

        /// <summary>
        /// Subscribe a callback to the selection event.
        /// </summary>
        /// <param name="callback">Callback to raise on selection</param>
        /// <see cref="UnSubscribe(UnityAction)"/>
        public override void Subscribe(UnityAction<bool> callback)
        {
            subscribers.Add(callback);
        }

        /// <summary>
        /// Unsubscribe a callback from the selection event.
        /// </summary>
        /// <param name="callback">Callback to unsubscribe</param>
        /// <see cref="Subscribe(UnityAction)"/>
        public override void UnSubscribe(UnityAction<bool> callback)
        {
            subscribers.Remove(callback);
        }

        public VisualElement GetUXMLContent()
        {
            InitAndBindUI();
            return button;
        }

        public void InitAndBindUI()
        {
            if (button == null)
            {
                button = manipulationMenuItemTreeAsset.CloneTree().Q<Button>();
                button.clickable.clicked += NotifyPress;
            }          
        }

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}