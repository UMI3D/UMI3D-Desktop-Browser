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

using DesktopBrowser.UI.CustomElement;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrowserDesktop.UI
{
    public abstract partial class AbstractGenericAndCustomElement : ICustomElement
    {
        public VisualElement Root { get; protected set; } = null;
        public bool AttachedToHierarchy { get; protected set; } = false;
        public bool Displayed { get; protected set; } = false;
        public DisplayStyle RootDisplayStyle
        {
            get => Root.resolvedStyle.display;
            set => Root.style.display = value;
        }
        public Visibility RootVisibility
        {
            get => Root.resolvedStyle.visibility;
            set => Root.style.visibility = value;
        }

        public virtual void AddTo(VisualElement parent)
        {
            if (!initialized) 
                throw new System.Exception($"VisualElement Added without being Initialized.");
            ReadyToDisplay();
            //parent.Add(this);
            parent.Add(Root);
        }

        
        public virtual void Remove()
        {
            if (!Displayed) return;
            else Displayed = false;
            this.RemoveFromHierarchy();
        }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        /// <summary>
        /// True if this UIElement has been initialized.
        /// </summary>
        protected bool initialized = false;
        public bool Initiated { get; protected set; } = false;
        /// <summary>
        /// True if this UIElement is displayed.
        /// </summary>
        

        
        public Rect RootLayout { get => Root.layout; }
    }

    public abstract partial class AbstractGenericAndCustomElement
    {
        public AbstractGenericAndCustomElement() : base() { }

        public AbstractGenericAndCustomElement(VisualTreeAsset visualTA) : this()
        {
            Init(visualTA);
        }

        public AbstractGenericAndCustomElement(VisualElement root) : this()
        {
            Init(root);
        }

        ~AbstractGenericAndCustomElement()
        {
            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.RemoveListener(OnApplyUserPreferences);
        }

        /// <summary>
        /// Clone and add the visualTreeAsset to this and Initialize.
        /// </summary>
        /// <param name="visualTA"></param>
        public void Init(VisualTreeAsset visualTA)
        {
            if (Initiated) return;
            else Initiated = true;
            Root = visualTA.CloneTree();
            this.Add(Root);
            Initialize();
        }

        public void Init(VisualElement root)
        {
            if (Initiated) return;
            else Initiated = true;
            this.Root = root;
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (initialized) return;
            else initialized = true;

            UserPreferences.UserPreferences.Instance.OnApplyUserPreferences.AddListener(OnApplyUserPreferences);
        }

        public virtual void Reset()
        {
            this.Root = null;
        }

        
        

        /// <summary>
        /// To be used in Custom Element that are already added to the UIDocument.
        /// </summary>
        protected virtual void ReadyToDisplay()
        {
            Displayed = true;
            OnApplyUserPreferences();
        }

        

        /// <summary>
        /// Apply user preferences when needed.
        /// </summary>
        public abstract void OnApplyUserPreferences();
    }

    public abstract partial class AbstractGenericAndCustomElement : VisualElement
    {
        /// <summary>
        /// To be recognized by UI Builder
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        /// <summary>
        /// Get Root.layout
        /// </summary>
        public new Rect layout { get => RootLayout; }
    }
}