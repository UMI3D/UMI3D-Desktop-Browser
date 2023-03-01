/*
Copyright 2019 - 2022 Inetum

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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Dropdown_C : DropdownField, IPanelBindable, ITransitionable
    {
        public new class UxmlFactory : UxmlFactory<Dropdown_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
            };
            UxmlEnumAttributeDescription<ElementSize> m_size = new UxmlEnumAttributeDescription<ElementSize>
            {
                name = "size",
                defaultValue = ElementSize.Medium
            };
            UxmlEnumAttributeDescription<ElementDirection> m_direction = new UxmlEnumAttributeDescription<ElementDirection>
            {
                name = "direction",
                defaultValue = ElementDirection.Leading
            };
            UxmlLocaliseAttributeDescription m_localisedLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localised-label"
            };
            UxmlLocaliseForOptionsAttributeDescription m_localisedOptions = new UxmlLocaliseForOptionsAttributeDescription
            {
                name = "localised-options"
            };
            UxmlIntAttributeDescription m_localisedIndex = new UxmlIntAttributeDescription
            {
                name = "localised-index",
                defaultValue = -1
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as Dropdown_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Direction = m_direction.GetValueFromBag(bag, cc);
                custom.LocalisedLabel = m_localisedLabel.GetValueFromBag(bag, cc);
                custom.LocalisedOptions = m_localisedOptions.GetValueFromBag(bag, cc);
                custom.LocalisedIndex = m_localisedIndex.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocalisedLabel"/> instead. </remarks>
        public new string label { get => base.label; set => base.label = value; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocalisedOptions"/> instead. </remarks>
        public override List<string> choices { get => base.choices; set => base.choices = value; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocalisedValue"/> instead. </remarks>
        public override string value
        {
            get => base.value;
            set
            {
                if (base.value == value || !choices.Contains(value)) return;
                var ce = ChangeEvent<string>.GetPooled(base.value, value);
                base.value = value;
                ValueChanged?.Invoke(index, ce);
            }
        }
        public virtual ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                m_category = value;
            }
        }
        public virtual ElementSize Size
        {
            get => m_size;
            set
            {
                RemoveFromClassList(USSCustomClassSize(m_size));
                AddToClassList(USSCustomClassSize(value));
                m_size = value;
            }
        }
        public virtual ElementDirection Direction
        {
            get => m_direction;
            set
            {
                RemoveFromClassList(USSCustomClassDirection(m_direction));
                AddToClassList(USSCustomClassDirection(value));
                m_direction = value;
            }
        }
        public virtual LocalisationAttribute LocalisedLabel
        {
            get => m_localisationLabel;
            set
            {
                m_localisationLabel = value;
                if (IsAttachedToPanel) _ = UpdateLabelTranslation();
                if (!Application.isPlaying) label = value.DefaultText;
            }
        }
        public virtual LocalisationForOptionsAttribute LocalisedOptions
        {
            get => m_localisationOptions;
            set
            {
                m_localisationOptions = value;
                if (IsAttachedToPanel) _ = UpdateOptionsTranslation();
                if (!Application.isPlaying) choices = value.GetDefaultOptions();
            }
        }
        public virtual int LocalisedIndex { get => index; set => index = value; }
        public virtual LocalisationAttribute LocalisedValue
        {
            get => LocalisedOptions.Options.Count > index ? LocalisedOptions.Options[index] : null;
            set
            {
                index = LocalisedOptions.Options.FindIndex(local => local.Equals(value));
            }
        }

        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

        public virtual string StyleSheetPath_MainTheme => "USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/dropDown";

        public virtual string UssCustomClass_Emc => "dropdown";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassDirection(ElementDirection direction) => $"{UssCustomClass_Emc}-{direction}".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}__label";
        public virtual string USSClassText => $"unity-base-popup-field__text";

        public virtual string USSCustomClassScrollview(ElementCategory category) => $"{UssCustomClass_Emc}-scrollview-{category}".ToLower();
        public virtual string USSCustomClassItemLabel => $"{UssCustomClass_Emc}-item__label";

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }

        public Text_C SampleTextLabel = new Text_C();
        public Text_C SampleTextItem = new Text_C();

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected ElementDirection m_direction;
        protected bool m_showLabel;

        public Dropdown_C()
        {
            this.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
            this.RegisterCallback<CustomStyleResolvedEvent>(CustomStyleResolved);
            this.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
            this.RegisterCallback<TransitionRunEvent>(TransitionRun);
            this.RegisterCallback<TransitionStartEvent>(TransitionStarted);
            this.RegisterCallback<TransitionEndEvent>(TransitionEnded);
            this.RegisterCallback<TransitionCancelEvent>(TransitionCanceled);
            IsSet = false;
            InstanciateChildren();
            _AttachStyleSheet();
            AttachUssClass();
            InitElement();
            IsSet = true;
            SetProperties();
        }

        /// <summary>
        /// Where to instanciate visual children of this element.
        /// </summary>
        protected virtual void InstanciateChildren()
        {
        }

        /// <summary>
        /// Add style and theme style sheets to this element.
        /// </summary>
        protected virtual void AttachStyleSheet()
        {
            this.AddStyleSheetFromPath(StyleSheetPath_MainTheme);
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
            this.AddStyleSheetFromPath(Text_C.StyleSheetPath_MainStyle);
        }

        private void _AttachStyleSheet()
        {
            try
            {
                AttachStyleSheet();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        /// <summary>
        /// Add Uss custom classes to this element and its children.
        /// </summary>
        protected virtual void AttachUssClass()
        {
            AddToClassList(UssCustomClass_Emc);
        }

        /// <summary>
        /// Initialise this element.
        /// </summary>
        /// <remarks>This methode is called by the base class <see cref="BaseVisual_C"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
        protected virtual void InitElement()
        {
            UpdateLabelStyle();
        }

        /// <summary>
        /// Set the properties.
        /// </summary>
        /// <remarks>This methode is in the range of <see cref="IsSet"/> equals to true.</remarks>
        protected virtual void SetProperties()
        {
            Category = ElementCategory.Menu;
            Size = ElementSize.Medium;
            Direction = ElementDirection.Leading;
        }

        #region Panel Bindable

        /// <summary>
        /// Whether or not this element is attached to a panel.
        /// </summary>
        public bool IsAttachedToPanel { get; protected set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void IPanelBindable.AttachedToPanel(AttachToPanelEvent evt) => AttachedToPanel(evt);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void IPanelBindable.DetachedFromPanel(DetachFromPanelEvent evt) => DetachedFromPanel(evt);

        /// <summary>
        /// Methode called when this element is attached to a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to register to an event you should use this methode.</remarks>
        protected virtual void AttachedToPanel(AttachToPanelEvent evt)
        {
            evt.StopPropagation();
            IsAttachedToPanel = true;
            PropertyChangedEvent += PropertyChanged;

            LanguageChanged += UpdateTranslation;
            UpdateTranslation();

            m_transitionScheduledItem = this.WaitUntil
            (
                () => this.CanBeConsiderAsListeningForTransition(),
                () =>
                {
                    IsListeningForTransition = true;
                    if (this.AreAnimationsWaiting()) this.PlayAllAnimations();
                }
            );
        }

        /// <summary>
        /// Methode called when this element is detached from a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to unregister to an event you should use this methode.</remarks>
        protected virtual void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            evt.StopPropagation();
            IsAttachedToPanel = false;
            IsListeningForTransition = false;

            m_transitionScheduledItem?.Pause();
            m_transitionScheduledItem = null;
            PropertyChangedEvent -= PropertyChanged;
            LanguageChanged -= UpdateTranslation;
        }

        #endregion

        /// <summary>
        /// Method called when a custom style sheet is resolved.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void CustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Method called when this element geometry changed.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void GeometryChanged(GeometryChangedEvent evt)
        {
            evt.StopPropagation();
        }

        #region Transitions

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsListeningForTransition { get; protected set; }

        IVisualElementScheduledItem m_transitionScheduledItem;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void ITransitionable.TransitionRun(TransitionRunEvent evt) => TransitionRun(evt);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void ITransitionable.TransitionStarted(TransitionStartEvent evt) => TransitionStarted(evt);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void ITransitionable.TransitionEnded(TransitionEndEvent evt) => TransitionEnded(evt);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="evt"></param>
        void ITransitionable.TransitionCanceled(TransitionCancelEvent evt) => TransitionCanceled(evt);

        /// <summary>
        /// Method called when a transition is created.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionRun(TransitionRunEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Method called when a transition start. (after transition'delay end)
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionStarted(TransitionStartEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Method called when a transition end properly without being canceled.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionEnded(TransitionEndEvent evt)
        {
            evt.StopPropagation();
            foreach (var property in evt.stylePropertyNames)
            {
                this.TriggerAnimationCallback(property);
            }
        }

        /// <summary>
        /// Method called when a transition is canceled.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionCanceled(TransitionCancelEvent evt)
        {
            evt.StopPropagation();
            foreach (var property in evt.stylePropertyNames)
            {
                this.TriggerAnimationCallcancel(property, evt);
            }
        }

        #endregion

        /// <summary>
        /// Raise the <see cref="PropertyChangedEvent"/> event if this elemnet is attached to a panel, else call <see cref="PropertyChanged(object, object, string)"/>
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="callerName"></param>
        protected void OnPropertyChanged(object oldValue, object newValue, [CallerMemberName] string callerName = "")
        {
            if (IsAttachedToPanel) PropertyChangedEvent?.Invoke(oldValue, newValue, callerName);
            else PropertyChanged(oldValue, newValue, callerName);
        }

        protected virtual void PropertyChanged(object oldValue, object newValue, [CallerMemberName] string callerName = "")
        {
        }

        #region Implementation

        
        public event System.Action<int, ChangeEvent<string>> ValueChanged;

        protected virtual void UpdateLabelStyle()
        {
            labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
            labelElement.AddToClassList(USSCustomClassLabel);

            textElement.ClearAndCopyStyleClasses(SampleTextLabel);
            textElement.AddToClassList(USSClassText);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (evt is not MouseDownEvent mde) return;

            var container = FoundBaseDropdownContainer();
            if (container == null) return;

            InitBaseDropdownContainer(container);
        }

        protected virtual VisualElement FoundBaseDropdownContainer()
        {
            var root = this.FindRoot();
            if (root == null) return null;

            var parent = root.parent;
            if (parent == null) return null;

            return parent.Q(className: "unity-base-dropdown__container-outer");
        }

        protected virtual void InitBaseDropdownContainer(VisualElement container)
        {
            try
            {
                container.AddStyleSheetFromPath(StyleSheetPath_MainTheme);
                container.AddStyleSheetFromPath(Text_C.StyleSheetPath_MainStyle);
                container.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"{e}");
            }

            var scrollview = container.Q(className: "unity-base-dropdown__container-inner");
            scrollview.AddToClassList(USSCustomClassScrollview(m_category));

            var scrollviewContentContainer = container.Q(className: "unity-scroll-view__content-container");
            foreach (var item in scrollviewContentContainer.Children())
            {
                item.Q<Label>().ClearAndCopyStyleClasses(SampleTextItem);
                item.Q<Label>().AddToClassList(USSCustomClassItemLabel);
            }
        }

        #region Localisation

        public static event System.Action LanguageChanged;
        public LocalisationAttribute m_localisationLabel;
        public LocalisationForOptionsAttribute m_localisationOptions;

        /// <summary>
        /// Raise <see cref="LanguageChanged"/> event.
        /// </summary>
        /// <returns></returns>
        public static void OnLanguageChanged()
        {
            if (!Application.isPlaying) return;

            LanguageChanged?.Invoke();
        }

        /// <summary>
        /// Update the translation of its component.
        /// </summary>
        public virtual void UpdateTranslation()
        {
            _ = UpdateLabelTranslation();
            _ = UpdateOptionsTranslation();
        }

        /// <summary>
        /// Update the translation of <see cref="label"/>
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateLabelTranslation()
        {
            label = m_localisationLabel.DefaultText;

            if (m_localisationLabel.CanBeLocalised)
            {
                while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

                label = m_localisationLabel.Value;
            }
        }

        /// <summary>
        /// Update the translation of <see cref="choices"/>
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateOptionsTranslation()
        {
            var currentIndex = index;

            choices = m_localisationOptions.GetDefaultOptions();

            var translatedChoices = await m_localisationOptions.GetTranslatedOptions();
            if (translatedChoices.Item1) choices = translatedChoices.Item2;

            if (currentIndex > -1 && value != choices[currentIndex]) value = choices[currentIndex];
        }

        #endregion

        #endregion
    }
}
