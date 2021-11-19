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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI
{
    public abstract class AbstractGenericAndCustomElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        /// <summary>
        /// True if this UIElement has been initialized.
        /// </summary>
        protected bool initialized = false;
        /// <summary>
        /// True if this UIElement is displayed.
        /// </summary>
        protected bool displayed = false;

        public AbstractGenericAndCustomElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
        }

        ~AbstractGenericAndCustomElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
        }

        protected virtual void Initialize()
        {
            if (initialized) return;
            else initialized = true;
        }

        /// <summary>
        /// Add this UiElement as a child of [partent].
        /// </summary>
        /// <param name="parent">the parent of this UIElement.</param>
        public virtual void AddTo(VisualElement parent)
        {
            if (!initialized) throw new System.Exception($"VisualElement Added without being setup.");
            ReadyToDisplay();
            parent.Add(this);
        }

        /// <summary>
        /// To be used in Custom Element that are already added to the UIDocument.
        /// </summary>
        protected virtual void ReadyToDisplay()
        {
            displayed = true;
            OnApplyUserPreferences();
        }

        /// <summary>
        /// Remove the UIElement from the hierarchy
        /// </summary>
        public virtual void Remove()
        {
            if (!displayed) return;
            this.RemoveFromHierarchy();
        }

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public abstract void OnApplyUserPreferences();


    }
}