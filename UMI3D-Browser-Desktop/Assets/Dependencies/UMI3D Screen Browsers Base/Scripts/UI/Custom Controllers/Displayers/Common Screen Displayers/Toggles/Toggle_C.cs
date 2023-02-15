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
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Toggle_C : Toggle
    {
        public new class UxmlFactory : UxmlFactory<Toggle_C, UxmlTraits> { }

        public new class UxmlTraits : Toggle.UxmlTraits
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
            UxmlEnumAttributeDescription<ElemnetDirection> m_direction = new UxmlEnumAttributeDescription<ElemnetDirection>
            {
                name = "direction",
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
                base.Init(ve, bag, cc);
                var custom = ve as Toggle_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.Direction = m_direction.GetValueFromBag(bag, cc);
                custom.LocaliseLabel = m_localiseLabel.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks> Use <see cref="LocaliseLabel"/> instead. </remarks>
        public new string label { get => base.label; set => base.label = value; }
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
        public virtual ElemnetDirection Direction
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

        public virtual string StyleSheetPath_MainTheme => "USS/displayer";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/toggle";

        /// <summary>
        /// Element main Uss class.
        /// </summary>
        public virtual string UssCustomClass_Emc => "toggle";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{UssCustomClass_Emc}-{direction}".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}__label";

        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }
        public bool IsAttachedToPanel { get; protected set; }

        public Text_C SampleTextLabel = new Text_C();

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected ElemnetDirection m_direction;

        public Toggle_C()
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
        /// <remarks>This methode is called by the base class <see cref="Toggle_C"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
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
            Direction = ElemnetDirection.Leading;
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

        protected virtual void UpdateLabelStyle()
        {
            labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
            labelElement.AddToClassList(USSCustomClassLabel);
        }

        #endregion
    }
}
