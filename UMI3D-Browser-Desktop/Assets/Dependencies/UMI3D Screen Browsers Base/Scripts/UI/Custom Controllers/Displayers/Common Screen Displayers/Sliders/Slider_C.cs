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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Slider_C : Slider
    {
        public new class UxmlFactory : UxmlFactory<Slider_C, UxmlTraits> { }

        public new class UxmlTraits : Slider.UxmlTraits
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
            protected UxmlEnumAttributeDescription<ElemnetDirection> m_direction = new UxmlEnumAttributeDescription<ElemnetDirection>
            {
                name = "direction-displayer",
                defaultValue = ElemnetDirection.Leading
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
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as Slider_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.DirectionDisplayer = m_direction.GetValueFromBag(bag, cc);
                custom.LocaliseLabel = m_localiseLabel.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocaliseLabel"/> instead. </remarks>
        public new string label { get => base.label; set => base.label = value; }
        public override bool showInputField
        {
            get => base.showInputField;
            set
            {
                base.showInputField = value;
                UpdateTextFieldStyle();
            }
        }
        public ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                m_category = value;
                SampleTextfield.Category = value;
                UpdateTextFieldStyle();
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
                SampleTextfield.Size = value;
                UpdateTextFieldStyle();
            }
        }
        public ElemnetDirection DirectionDisplayer
        {
            get => m_direction;
            set
            {
                RemoveFromClassList(USSCustomClassDirection(m_direction));
                AddToClassList(USSCustomClassDirection(value));
                m_direction = value;
            }
        }
        public LocalisationAttribute LocaliseLabel
        {
            get => SampleTextLabel.LocaliseText;
            set
            {
                SampleTextLabel.LocaliseText = value;
                ChangedLanguage();
            }
        }

        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

        public virtual string StyleSheetPath_MainTheme => $"USS/displayer";

        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/slider";

        public virtual string UssCustomClass_Emc => "slider";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{UssCustomClass_Emc}-{direction}".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}-label";
        public virtual string USSCustomClassTextField => $"{UssCustomClass_Emc}-text__field";

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }
        public bool IsAttachedToPanel { get; protected set; }

        public Textfield_C SampleTextfield = new Textfield_C();
        public TextField TextField;
        public Text_C SampleTextLabel = new Text_C();

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected ElemnetDirection m_direction;

        public Slider_C()
        {
            this.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
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
            labelElement.AddStyleSheetFromPath(Text_C.StyleSheetPath_MainStyle);
            this.AddStyleSheetFromPath(Textfield_C.StyleSheetPath_MainStyle);
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
        /// <remarks>This methode is called by the base class <see cref="Slider"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
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
            DirectionDisplayer = ElemnetDirection.Leading;
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

            LanguageChanged += ChangedLanguage;
            OnLanguageChanged();
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

            LanguageChanged -= ChangedLanguage;
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
            label = SampleTextLabel.LocaliseText.DefaultText;

            if (SampleTextLabel.LocaliseText.CanBeLocalised)
            {
                while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

                label = LocalisationManager.Instance.GetTranslation(SampleTextLabel.LocaliseText);
            }
        }

        protected virtual void UpdateTextFieldStyle()
        {
            if (!showInputField) return;

            if (TextField == null) TextField = this.Q<TextField>();
            TextField.ClearAndCopyStyleClasses(SampleTextfield);
            TextField.AddToClassList(USSCustomClassTextField);
        }

        protected virtual void UpdateLabelStyle()
        {
            labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
            labelElement.AddToClassList(USSCustomClassLabel);
        }

        #endregion
    }
}
