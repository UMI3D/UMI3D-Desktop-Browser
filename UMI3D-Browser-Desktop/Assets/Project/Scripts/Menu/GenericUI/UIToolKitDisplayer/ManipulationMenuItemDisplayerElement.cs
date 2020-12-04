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
            button.clickable.clicked -= NotifyPress;
            button.RemoveFromHierarchy();
        }

        public override void Display(bool forceUpdate = false)
        {
            InitAndBindUI();

            button.style.display = DisplayStyle.Flex;
            button.clickable.clicked += NotifyPress;
        }

        public override void Hide()
        {
            button.style.display = DisplayStyle.None;
            button.clickable.clicked -= NotifyPress;
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
                button = manipulationMenuItemTreeAsset.CloneTree().Q<Button>();
        }

        private void OnDestroy()
        {
            button?.RemoveFromHierarchy();
        }
    }
}