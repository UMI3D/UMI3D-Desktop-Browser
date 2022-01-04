using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public interface ICustomElement
    {
        /// <summary>
        /// Visual root of this custom element.
        /// </summary>
        public VisualElement Root { get; }
        /// <summary>
        /// True if this visual is attached to a hierarchy of visualElement, else false.
        /// </summary>
        public bool AttachedToHierarchy { get; }
        /// <summary>
        /// True if this visual is displayed, else false.
        /// </summary>
        public bool Displayed { get; }
        public DisplayStyle RootDisplayStyle { get; set; }
        public Visibility RootVisibility { get; set; }

        /// <summary>
        /// Add this UiElement as a child of [partent].
        /// </summary>
        /// <param name="parent">the parent of this UIElement.</param>
        public void AddTo(VisualElement parent);
        /// <summary>
        /// Remove the UIElement from the hierarchy
        /// </summary>
        public void Remove();
    }

    public interface ICustomisableElement
    {
        public string Key { get; set; }
        public IList<string> Values { get; }
        public string CurrentValue { get; set; }
        public bool IsEmpty { get; }

        public ICustomisableElement SetValues(params string[] values);
        public void SwitchValue(int index);
        public void Reset();
    }
}
