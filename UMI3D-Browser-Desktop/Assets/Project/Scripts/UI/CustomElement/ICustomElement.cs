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
        /// True if this has been initialized.
        /// </summary>
        public bool Initialized { get; }
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
        /// Layout of this visual.
        /// </summary>
        public Rect RootLayout { get; }

        /// <summary>
        /// Clone and add the visualTreeAsset to this and Initialize.
        /// </summary>
        /// <param name="visualTA"></param>
        public void Init(VisualTreeAsset visualTA);
        /// <summary>
        /// Add root to this and Initialize.
        /// </summary>
        /// <param name="root"></param>
        public void Init(VisualElement root);
        /// <summary>
        /// Reset this.
        /// </summary>
        public void Reset();
        /// <summary>
        /// Add this UiElement as a child of [partent].
        /// </summary>
        /// <param name="parent">the parent of this UIElement.</param>
        public void AddTo(VisualElement parent);
        /// <summary>
        /// Remove the UIElement from the hierarchy
        /// </summary>
        public void Remove();
        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public void OnApplyUserPreferences();
    }

    public interface ICustomisableElement
    {
        public string Key { get; set; }
        public IList<string> Values { get; }
        public IList<string> CurrentValues { get; set; }
        public bool IsEmpty { get; }

        public ICustomisableElement SetValues(params string[] values);
        public void SelectCurrentValues(params int[] indexes);
        public void DeselectCurrentValues(params string[] values);
        public void DeselectAllCurrentValues();
        public void DeselectLasCurrentValues();
        public void Reset();
    }
}
