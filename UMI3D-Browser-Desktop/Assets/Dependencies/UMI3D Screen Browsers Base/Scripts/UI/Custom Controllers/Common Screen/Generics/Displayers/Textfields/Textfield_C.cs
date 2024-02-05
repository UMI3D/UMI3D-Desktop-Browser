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
using umi3d.baseBrowser.inputs.interactions;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Textfield_C : TextField, IPanelBindable, ITransitionable
    {
        public new class UxmlFactory : UxmlFactory<Textfield_C, UxmlTraits> { }

        public new class UxmlTraits : TextField.UxmlTraits
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
            UxmlBoolAttributeDescription m_maskToggle = new UxmlBoolAttributeDescription
            {
                name = "display-mask-toggle",
                defaultValue = false
            };
            UxmlBoolAttributeDescription m_submitButton = new UxmlBoolAttributeDescription
            {
                name = "display-submit-button",
                defaultValue = false
            };
            protected UxmlLocaliseAttributeDescription m_localiseLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localise-label"
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Textfield_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Direction = m_direction.GetValueFromBag(bag, cc);
                custom.DisplayMaskToggle = m_maskToggle.GetValueFromBag(bag, cc);
                custom.DisplaySubmitButton = m_submitButton.GetValueFromBag(bag, cc);
                custom.LocaliseLabel = m_localiseLabel.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocaliseLabel"/> instead. </remarks>
        public new string label { get => base.label; set => base.label = value; }
        public ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                m_category = value;
                MaskToggle.Category = value;
            }
        }
        public ElementSize Size
        {
            get => m_size;
            set
            {
                RemoveFromClassList(USSCustomClassSize(m_size));
                AddToClassList(USSCustomClassSize(value));
                m_size = value;
                MaskToggle.Size = value;
                SubmitButton.Height = value;
            }
        }
        public ElementDirection Direction
        {
            get => m_direction;
            set
            {
                RemoveFromClassList(USSCustomClassDirection(m_direction));
                AddToClassList(USSCustomClassDirection(value));
                m_direction = value;
            }
        }
        public bool DisplayMaskToggle
        {
            get => m_displayMaskToggle;
            set
            {
                if (value) Add(MaskToggle);
                else MaskToggle.RemoveFromHierarchy();
                MaskToggle.value = isPasswordField;
                m_displayMaskToggle = value;
            }
        }
        public bool DisplaySubmitButton
        {
            get => m_submitButton;
            set
            {
                if (value) Add(SubmitButton);
                else SubmitButton.RemoveFromHierarchy();
                m_submitButton = value;
            }
        }
        public LocalisationAttribute LocaliseLabel
        {
            get => SampleTextLabel.LocalisedText;
            set
            {
                SampleTextLabel.LocalisedText = value;
                ChangedLanguage();
            }
        }
        public override string value 
        { 
            get => base.value; 
            set
            {
                var ce = ChangeEvent<string>.GetPooled(base.value, value);
                base.value = value;
                ValueChanged?.Invoke(ce);
            }
        }

        public event System.Action<ChangeEvent<string>> ValueChanged;

        public virtual string StyleSheetPath_MainTheme => "USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/textField";

        /// <summary>
        /// Element main Uss class.
        /// </summary>
        public virtual string UssCustomClass_Emc => "textfield";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassDirection(ElementDirection direction) => $"{UssCustomClass_Emc}-{direction}".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}__label";
        public virtual string USSCustomClassToggle => $"{UssCustomClass_Emc}__toggle";
        public virtual string USSCustomClassButton => $"{UssCustomClass_Emc}__button";

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }

        public Text_C SampleTextLabel = new Text_C();
        public Toggle_C MaskToggle = new Toggle_C();
        public Button_C SubmitButton = new Button_C { name = "submit" };
        public static bool HasFocused;

        public TooltipManipulator TooltipManipulator = new TooltipManipulator();

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected ElementDirection m_direction;
        protected bool m_displayMaskToggle;
        protected bool m_submitButton;
        protected List<string> m_textInputOriginalClassStyle;
        protected TextInputBase TextInput;

        public void HideTextInput()
        {
            TextInput.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

        public Textfield_C()
        {
            this.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
            this.RegisterCallback<CustomStyleResolvedEvent>(CustomStyleResolved);
            this.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
            this.RegisterCallback<TransitionRunEvent>(TransitionRun);
            this.RegisterCallback<TransitionStartEvent>(TransitionStarted);
            this.RegisterCallback<TransitionEndEvent>(TransitionEnded);
            this.RegisterCallback<TransitionCancelEvent>(TransitionCanceled);
            this.AddManipulator(TooltipManipulator);
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
            MaskToggle.AddToClassList(USSCustomClassToggle);
            SubmitButton.AddToClassList(USSCustomClassButton);
        }

        /// <summary>
        /// Initialise this element.
        /// </summary>
        /// <remarks>This methode is called by the base class <see cref="TextField"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
        protected virtual void InitElement()
        {
            MaskToggle.RegisterValueChangedCallback((value) => isPasswordField = value.newValue);

            TextInput = this.Q<TextInputBase>("unity-text-input");
            m_textInputOriginalClassStyle = new List<string>(TextInput.GetClasses());
            TextInput.RegisterCallback<FocusInEvent>(focus =>
            {
                HasFocused = true;
                BaseKeyInteraction.IsEditingTextField = true;
            });
            TextInput.RegisterCallback<FocusOutEvent>(focus =>
            {
                HasFocused = false;
                BaseKeyInteraction.IsEditingTextField = false;
            });

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
            DisplayMaskToggle = false;
            DisplaySubmitButton = false;
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

            LanguageChanged += ChangedLanguage;
            OnLanguageChanged();

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
            LanguageChanged -= ChangedLanguage;
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

        public static event System.Action LanguageChanged;

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
        /// Change language of the text.
        /// </summary>
        public void ChangedLanguage() => _ = _ChangedLanguage();

        protected virtual async Task _ChangedLanguage()
        {
            label = SampleTextLabel.LocalisedText.DefaultText;

            if (SampleTextLabel.LocalisedText.CanBeLocalised)
            {
                while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

                label = LocalisationManager.Instance.GetTranslation(SampleTextLabel.LocalisedText);
            }
        }

        protected virtual void UpdateLabelStyle()
        {
            labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
            labelElement.AddToClassList(USSCustomClassLabel);

            TextInput.ClearClassList();
            foreach (var style in m_textInputOriginalClassStyle)
                TextInput.AddToClassList(style);
            TextInput.CopyStyleClasses(SampleTextLabel);
        }

        #endregion
    }
}
