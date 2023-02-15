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
using System.Runtime.CompilerServices;
using umi3d.baseBrowser.ui.viewController;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Button_C : Button
    {
        public new class UxmlFactory : UxmlFactory<Button_C, UxmlTraits> { }

        public new class UxmlTraits : Button.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
            {
                name = "category",
                defaultValue = ElementCategory.Menu
            };
            protected UxmlEnumAttributeDescription<ElementSize> m_size = new UxmlEnumAttributeDescription<ElementSize>
            {
                name = "size",
                defaultValue = ElementSize.Medium
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
            protected UxmlLocaliseAttributeDescription m_localiseLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localise-label"
            };
            protected UxmlEnumAttributeDescription<ElemnetDirection> m_labelDirection = new UxmlEnumAttributeDescription<ElemnetDirection>
            {
                name = "label-direction",
                defaultValue = ElemnetDirection.Leading
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
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Shape = m_shape.GetValueFromBag(bag, cc);
                custom.Type = m_type.GetValueFromBag(bag, cc);
                custom.LocaliseLabel = m_localiseLabel.GetValueFromBag(bag, cc);
                custom.LabelDirection = m_labelDirection.GetValueFromBag(bag, cc);
                custom.IconAlignment = m_iconAlignment.GetValueFromBag(bag, cc);
                custom.LocaliseText = m_localiseText.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

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
        public virtual ButtonShape Shape
        {
            get => m_shape;
            set
            {
                RemoveFromClassList(USSCustomClassShape(m_shape));
                AddToClassList(USSCustomClassShape(value));
                m_shape = value;
            }
        }
        public virtual ButtonType Type
        {
            get => m_type;
            set
            {
                RemoveFromClassList(USSCustomClassType(m_type));
                AddToClassList(USSCustomClassType(value));
                m_type = value;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocaliseText"/> instead. </remarks>
        public override string text { get => base.text; set => base.text = value; }
        public virtual LocalisationAttribute LocaliseLabel
        {
            get => LabelVisual.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) LabelVisual.RemoveFromHierarchy();
                else Insert(0, LabelVisual);
                LabelVisual.LocaliseText = value;
                IsSet = true;
            }
        }
        public ElemnetDirection LabelDirection
        {
            get => m_labelDirection;
            set
            {
                RemoveFromClassList(USSCustomClassDirection(m_labelDirection));
                AddToClassList(USSCustomClassDirection(value));
                m_labelDirection = value;
            }
        }
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
            get => TextVisual.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) TextVisual.RemoveFromHierarchy();
                else Body.Insert(1, TextVisual);
                TextVisual.LocaliseText = value;
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
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassShape(ButtonShape shape) => $"{UssCustomClass_Emc}-{shape}".ToLower();
        public virtual string USSCustomClassType(ButtonType type) => $"{UssCustomClass_Emc}-{type}".ToLower();
        public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{UssCustomClass_Emc}-{direction}-direction".ToLower();
        public virtual string USSCustomClassAlignment(ElementAlignment alignment) => $"{UssCustomClass_Emc}-{alignment}-alignment".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}__label";
        public virtual string USSCustomClassBody => $"{UssCustomClass_Emc}__body";
        public virtual string USSCustomClassText => $"{UssCustomClass_Emc}__text";
        public virtual string USSCustomClassContainer => $"{UssCustomClass_Emc}__content-container";
        public virtual string USSCustomClassFront => $"{UssCustomClass_Emc}__front";

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }
        public bool IsAttachedToPanel { get; protected set; }

        public Text_C LabelVisual = new Text_C { name = "label" };
        public VisualElement Body = new VisualElement { name = "body" };
        public Text_C TextVisual = new Text_C {name = "text"};
        public VisualElement Container = new VisualElement { name = "content-container" };
        public VisualElement Front = new VisualElement { name = "front" };

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected ButtonShape m_shape;
        protected ButtonType m_type;
        protected ElemnetDirection m_labelDirection;
        protected ElementAlignment m_iconAlignment;

        

        public Button_C()
        {
            this.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
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
            Size = ElementSize.Medium;
            Shape = ButtonShape.Square;
            Type = ButtonType.Default;
            LocaliseLabel = null;
            LabelDirection = ElemnetDirection.Leading;
            IconAlignment = ElementAlignment.Center;
            LocaliseText = null;
        }

        /// <summary>
        /// Methode called when this element is attached to a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to register to an event you should use this methode.</remarks>
        protected virtual void AttachedToPanel(AttachToPanelEvent evt)
        {
            IsAttachedToPanel = true;
            PropertyChangedEvent += PropertyChanged;
        }

        /// <summary>
        /// Methode called when this element is detached from a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to unregister to an event you should use this methode.</remarks>
        protected virtual void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            PropertyChangedEvent -= PropertyChanged;
            IsAttachedToPanel = false;
        }

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

        public override VisualElement contentContainer => IsSet ? Container : this;

        #region Implementation

        #region Localisation



        #endregion

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
        public event System.Action<EventBase, UnityEngine.Vector2> ClickedLongWithInfo
        {
            add => TouchManipulator.ClickedLongWithInfo += value;
            remove => TouchManipulator.ClickedLongWithInfo -= value;
        }
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
        public event System.Action ClickedLong
        {
            add => TouchManipulator.ClickedLong += value;
            remove => TouchManipulator.ClickedLong -= value;
        }
        public event System.Action Moved
        {
            add => TouchManipulator.Moved += value;
            remove => TouchManipulator.Moved -= value;
        }

        #endregion

        #endregion
    }
}
