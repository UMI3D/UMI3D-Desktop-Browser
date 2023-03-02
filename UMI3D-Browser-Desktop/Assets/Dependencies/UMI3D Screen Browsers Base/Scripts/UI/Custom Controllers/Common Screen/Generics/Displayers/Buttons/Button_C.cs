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
using umi3d.baseBrowser.ui.viewController;
using umi3d.baseBrowser.utils;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Button_C : Button, IPanelBindable, ITransitionable, IDisplayer
    {
        public new class UxmlFactory : UxmlFactory<Button_C, UxmlTraits> { }

        public new class UxmlTraits : Button.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
            };

            protected UxmlEnumAttributeDescription<ElementSize> m_height = new UxmlEnumAttributeDescription<ElementSize>
            {
                name = "height",
                defaultValue = ElementSize.Medium
            };
            protected UxmlEnumAttributeDescription<ElementSize> m_width = new UxmlEnumAttributeDescription<ElementSize>
            {
                name = "width",
                defaultValue = ElementSize.Custom
            };
            protected UxmlEnumAttributeDescription<ElementAlignment> m_labelAndInputDirection = new UxmlEnumAttributeDescription<ElementAlignment>
            {
                name = "label-and-input-direction",
                defaultValue = ElementAlignment.Leading
            };
            protected UxmlEnumAttributeDescription<ElementAlignment> m_labelAlignment = new UxmlEnumAttributeDescription<ElementAlignment>
            {
                name = "label-alignment",
                defaultValue = ElementAlignment.Leading
            };
            protected UxmlLocaliseAttributeDescription m_localisedLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localised-label"
            };

            protected UxmlEnumAttributeDescription<ButtonShape> m_shape = new UxmlEnumAttributeDescription<ButtonShape>
            {
                name = "shape",
                defaultValue = ButtonShape.Square
            };
            protected UxmlEnumAttributeDescription<ButtonType> m_type = new UxmlEnumAttributeDescription<ButtonType>
            {
                name = "type",
                defaultValue = ButtonType.Default
            };
            protected UxmlBoolAttributeDescription m_isToggle = new UxmlBoolAttributeDescription
            {
                name = "is-toggle",
                defaultValue = false
            };
            protected UxmlBoolAttributeDescription m_toogleValue = new UxmlBoolAttributeDescription
            {
                name = "toggle-value",
                defaultValue = false
            };
            
            
            protected UxmlEnumAttributeDescription<ElementAlignment> m_iconAlignment = new UxmlEnumAttributeDescription<ElementAlignment>
            {
                name = "icon-alignment",
                defaultValue = ElementAlignment.Center
            };
            protected UxmlLocaliseAttributeDescription m_localiseText = new UxmlLocaliseAttributeDescription
            {
                name = "localise-text"
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
                var custom = ve as Button_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);

                custom.Height = m_height.GetValueFromBag(bag, cc);
                custom.Width = m_width.GetValueFromBag(bag, cc);
                custom.LabelAndInputDirection = m_labelAndInputDirection.GetValueFromBag(bag, cc);
                custom.LabelAlignment = m_labelAlignment.GetValueFromBag(bag, cc);
                custom.LocalisedLabel = m_localisedLabel.GetValueFromBag(bag, cc);

                custom.Shape = m_shape.GetValueFromBag(bag, cc);
                custom.Type = m_type.GetValueFromBag(bag, cc);
                custom.IsToggle = m_isToggle.GetValueFromBag(bag, cc);
                custom.ToggleValue = m_toogleValue.GetValueFromBag(bag, cc);
                
                custom.IconAlignment = m_iconAlignment.GetValueFromBag(bag, cc);
                custom.LocaliseText = m_localiseText.GetValueFromBag(bag, cc);
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

        #region Displayer properties

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual ElementSize Height
        {
            get => m_height;
            set => m_height.Value = value;
        }
        protected readonly Source<ElementSize> m_height = ElementSize.Medium;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual ElementSize Width
        {
            get => m_width;
            set => m_width.Value = value;
        }
        protected readonly Source<ElementSize> m_width = ElementSize.Custom;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ElementAlignment LabelAndInputDirection
        {
            get => m_labelAndInputDirection;
            set => m_labelAndInputDirection.Value = value;
        }
        protected readonly Source<ElementAlignment> m_labelAndInputDirection = ElementAlignment.Leading;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ElementAlignment LabelAlignment
        {
            get => m_labelAlignment;
            set => m_labelAlignment.Value = value;
        }
        protected readonly Source<ElementAlignment> m_labelAlignment = ElementAlignment.Leading;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual LocalisationAttribute LocalisedLabel
        {
            get => LabelVisual.LocalisedText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) LabelVisual.RemoveFromHierarchy();
                else Insert(0, LabelVisual);
                LabelVisual.LocalisedText = value;
                IsSet = true;
            }
        }

        #endregion

        #region Button properties

        public ButtonShape Shape
        {
            get => m_shape;
            set => m_shape.Value = value;
        }
        protected readonly Source<ButtonShape> m_shape = ButtonShape.Square;

        public ButtonType Type
        {
            get => m_type;
            set => m_type.Value = value;
        }
        protected readonly Source<ButtonType> m_type = ButtonType.Default;

        public bool IsToggle
        {
            get => m_isToggle;
            set => m_isToggle.Value = value;
        }
        protected readonly Source<bool> m_isToggle = false;
        public bool ToggleValue
        {
            get => m_toggleValue;
            set => m_toggleValue.Value = value;
        }
        protected readonly Source<bool> m_toggleValue = false;
        /// <summary>
        /// Bind <see cref="ToggleValue"/> source data.
        /// </summary>
        /// <param name="toggleValue"></param>
        public void BindSourceToggleValue(out Derive<bool> toggleValue) => toggleValue = m_toggleValue;

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocaliseText"/> instead. </remarks>
        public override string text { get => base.text; set => base.text = value; }
       
        
        public ElementAlignment IconAlignment
        {
            get => m_iconAlignment;
            set
            {
                RemoveFromClassList(USSCustomClassAlignment(m_iconAlignment));
                AddToClassList(USSCustomClassAlignment(value));
                m_iconAlignment = value;
            }
        }
        public virtual LocalisationAttribute LocaliseText
        {
            get => TextVisual.LocalisedText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) TextVisual.RemoveFromHierarchy();
                else Body.Insert(1, TextVisual);
                TextVisual.LocalisedText = value;
                IsSet = true;
            }
        }

        public virtual string StyleSheetPath_MainTheme => $"USS/displayer";
        /// <summary>
        /// Style sheet dedicated to the main style of this element.
        /// </summary>
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/button";

        /// <summary>
        /// Element main Uss class.
        /// </summary>
        public virtual string UssCustomClass_Emc => "button";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementDimension dimension, ElementSize size) => $"{UssCustomClass_Emc}-{dimension}-{size}".ToLower();
        public virtual string USSCustomClassShape(ButtonShape shape) => $"{UssCustomClass_Emc}-{shape}".ToLower();
        public virtual string USSCustomClassType(ButtonType type) => $"{UssCustomClass_Emc}-{type}".ToLower();
        public virtual string USSCustomClassDirection(ElementAlignment direction) => $"{UssCustomClass_Emc}-direction-{direction}".ToLower();
        public virtual string USSCustomClassAlignment(ElementAlignment alignment) => $"{UssCustomClass_Emc}-{alignment}-alignment".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}-label";
        public virtual string USSCustomClassBody => $"{UssCustomClass_Emc}-body";
        public virtual string USSCustomClassText => $"{UssCustomClass_Emc}-text";
        public virtual string USSCustomClassContainer => $"{UssCustomClass_Emc}-content__container";
        public virtual string USSCustomClassFront => $"{UssCustomClass_Emc}-front";

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }

        public Text_C LabelVisual = new Text_C { name = "label" };
        public Visual_C Body = new Visual_C { name = "body" };
        public Text_C TextVisual = new Text_C {name = "text"};
        public Visual_C Container = new Visual_C { name = "content-container" };
        public Visual_C Front = new Visual_C { name = "front" };

        protected ElementCategory m_category;
        protected ElementAlignment m_iconAlignment;

        public Button_C()
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
            StartElement();
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
            RemoveFromClassList("unity-button");
            AddToClassList(UssCustomClass_Emc);
            LabelVisual.AddToClassList(USSCustomClassLabel);
            Body.AddToClassList(USSCustomClassBody);
            TextVisual.AddToClassList(USSCustomClassText);
            Container.AddToClassList(USSCustomClassContainer);
            Front.AddToClassList(USSCustomClassFront);
        }

        /// <summary>
        /// Initialise this element.
        /// </summary>
        /// <remarks>This methode is called by the base class <see cref="Button_C"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
        protected virtual void InitElement()
        {
            clickable = TouchManipulator;

            m_height.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassSize(ElementDimension.Height, e.previousValue),
                    USSCustomClassSize(ElementDimension.Height, e.newValue)
                );
                Body.SwitchStyleclasses
                (
                    USSCustomClassSize(ElementDimension.Height, e.previousValue),
                    USSCustomClassSize(ElementDimension.Height, e.newValue)
                );
            };
            m_width.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassSize(ElementDimension.Width, e.previousValue),
                    USSCustomClassSize(ElementDimension.Width, e.newValue)
                );
                Body.SwitchStyleclasses
                (
                    USSCustomClassSize(ElementDimension.Width, e.previousValue),
                    USSCustomClassSize(ElementDimension.Width, e.newValue)
                );
            };
            m_labelAndInputDirection.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassDirection(e.previousValue),
                    USSCustomClassDirection(e.newValue)
                );
                Body.SwitchStyleclasses
                (
                    USSCustomClassDirection(e.previousValue),
                    USSCustomClassDirection(e.newValue)
                );
            };
            m_labelAlignment.ValueChanged += e =>
            {
                LabelVisual.TextAlignment = e.newValue;
            };

            m_shape.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassShape(e.previousValue),
                    USSCustomClassShape(e.newValue)
                );
                Body.SwitchStyleclasses
                (
                    USSCustomClassShape(e.previousValue),
                    USSCustomClassShape(e.newValue)
                );
            };
            m_type.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassType(e.previousValue),
                    USSCustomClassType(e.newValue)
                );
            };
            clicked += () =>
            {
                if (IsToggle) ToggleValue = !ToggleValue;
            };

            Add(Body);
            Body.Add(Container);
            Body.Add(Front);
        }

        /// <summary>
        /// Initialise this element.
        /// </summary>
        /// <remarks>This methode is called by the base class <see cref="Button_C"/>. This methode is in the range of <see cref="IsSet"/> equals to true.</remarks>
        protected virtual void StartElement()
        {
        }

        /// <summary>
        /// Set the properties.
        /// </summary>
        /// <remarks>This methode is in the range of <see cref="IsSet"/> equals to true.</remarks>
        protected virtual void SetProperties()
        {
            Category = ElementCategory.Menu;
            LocalisedLabel = null;
            IconAlignment = ElementAlignment.Center;
            LocaliseText = null;
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
        }

        #endregion

        public override VisualElement contentContainer => IsSet ? Container : this;

        #region Implementation

        #region Manipulator and event

        public TouchManipulator2 TouchManipulator = new TouchManipulator2(null, 0, 0);

        /// <summary>
        /// Event raised when the interaction is up if delay and interval is 0. 
        /// Otherwise it will be raised once when down then repeatedly after [delay] at [interval] rate.
        /// </summary>
        public event System.Action<EventBase> ClickedWithInfo
        {
            add => TouchManipulator.clickedWithEventInfo += value;
            remove => TouchManipulator.clickedWithEventInfo -= value;
        }
        /// <summary>
        /// Event raised when the interaction is down.
        /// </summary>
        public event System.Action<EventBase, UnityEngine.Vector2> ClickedDownWithInfo
        {
            add => TouchManipulator.ClickedDownWithInfo += value;
            remove => TouchManipulator.ClickedDownWithInfo -= value;
        }
        /// <summary>
        /// Event raised when the interaction is up.
        /// </summary>
        public event System.Action<EventBase, UnityEngine.Vector2> ClickedUpWithInfo
        {
            add => TouchManipulator.ClickedUpWithInfo += value;
            remove => TouchManipulator.ClickedUpWithInfo -= value;
        }
        /// <summary>
        /// Event raised when the interaction is long pressed.
        /// </summary>
        public event System.Action<EventBase, UnityEngine.Vector2> ClickedLongWithInfo
        {
            add => TouchManipulator.ClickedLongWithInfo += value;
            remove => TouchManipulator.ClickedLongWithInfo -= value;
        }
        /// <summary>
        /// Event raised when the interaction is clicked and the cursor moved.
        /// </summary>
        public event System.Action<EventBase, UnityEngine.Vector2> MovedWithInfo
        {
            add => TouchManipulator.MovedWithInfo += value;
            remove => TouchManipulator.MovedWithInfo -= value;
        }
        /// <summary>
        /// Event raised when the interaction is up if delay and interval is 0. 
        /// Otherwise it will be raised once when down then repeatedly after [delay] at [interval] rate.
        /// </summary>
        public new event System.Action clicked
        {
            add => TouchManipulator.clicked += value;
            remove => TouchManipulator.clicked -= value;
        }
        /// <summary>
        /// Event raised when the interaction is down.
        /// </summary>
        public event System.Action ClickedDown
        {
            add => TouchManipulator.ClickedDown += value;
            remove => TouchManipulator.ClickedDown -= value;
        }
        /// <summary>
        /// Event raised when the interaction is up.
        /// </summary>
        public event System.Action ClickedUp
        {
            add => TouchManipulator.ClickedUp += value;
            remove => TouchManipulator.ClickedUp -= value;
        }
        /// <summary>
        /// Event raised when the interaction is long pressed.
        /// </summary>
        public event System.Action ClickedLong
        {
            add => TouchManipulator.ClickedLong += value;
            remove => TouchManipulator.ClickedLong -= value;
        }
        /// <summary>
        /// Event raised when the interaction is clicked and the cursor moved.
        /// </summary>
        public event System.Action Moved
        {
            add => TouchManipulator.Moved += value;
            remove => TouchManipulator.Moved -= value;
        }

        #endregion

        #endregion
    }
}
