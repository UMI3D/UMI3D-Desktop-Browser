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
using System.Threading.Tasks;
using umi3d.baseBrowser.utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Text_C : Label, IPanelBindable, ITransitionable
    {
        public new class UxmlFactory : UxmlFactory<Text_C, UxmlTraits> { }

        public new class UxmlTraits : Label.UxmlTraits
        {
            UxmlEnumAttributeDescription<ElementAlignment> m_textAlignment = new UxmlEnumAttributeDescription<ElementAlignment>
            {
                name = "text-alignment",
                defaultValue = ElementAlignment.Leading
            };

            UxmlEnumAttributeDescription<TextStyle> m_style = new UxmlEnumAttributeDescription<TextStyle>
            {
                name = "text-style",
                defaultValue = TextStyle.Body
            };

            UxmlEnumAttributeDescription<TextColor> m_color = new UxmlEnumAttributeDescription<TextColor>
            {
                name = "color",
                defaultValue = TextColor.White
            };

            UxmlLocaliseAttributeDescription m_localisedText = new UxmlLocaliseAttributeDescription
            {
                name = "localised-text"
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
                var custom = ve as Text_C;

                custom.TextAlignment = m_textAlignment.GetValueFromBag(bag, cc);
                custom.LocalisedText = m_localisedText.GetValueFromBag(bag, cc);
                custom.TextStyle = m_style.GetValueFromBag(bag, cc);
                custom.Color = m_color.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Alignment of the text
        /// </summary>
        public ElementAlignment TextAlignment
        {
            get => m_textAlignment;
            set => m_textAlignment.Value = value;
        }
        protected readonly Source<ElementAlignment> m_textAlignment = ElementAlignment.Leading;

        public virtual LocalisationAttribute LocalisedText
        {
            get => m_localisation;
            set
            {
                m_localisation = value;
                if (IsAttachedToPanel) OnLanguageChanged();
                else ChangedLanguage();
                if (!Application.isPlaying) text = value.DefaultText;
            }
        }

        public virtual TextStyle TextStyle
        {
            get => m_style;
            set
            {
                RemoveFromClassList(USSCustomClassStyle(m_style));
                AddToClassList(USSCustomClassStyle(value));
                m_style = value;
            }
        }
        
        public virtual TextColor Color
        {
            get => m_color;
            set
            {
                RemoveFromClassList(USSCustomClassColor(m_color));
                AddToClassList(USSCustomClassColor(value));
                m_color = value;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>!! Use <see cref="LocalisedText"/> instead</remarks>
        public override string text { get => base.text; set => base.text = value; }

        /// <summary>
        /// Style sheet dedicated to the main theme.
        /// </summary>
        public virtual string StyleSheetPath_MainTheme => $"USS/displayer";
        /// <summary>
        /// Style sheet dedicated to the main style of this element.
        /// </summary>
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/text";

        /// <summary>
        /// Element main Uss class.
        /// </summary>
        public virtual string UssCustomClass_Emc => "text";
        public virtual string USSCustomClassAlignment(ElementAlignment alignment) => $"{UssCustomClass_Emc}-alignment-{alignment}".ToLower();
        public virtual string USSCustomClassStyle(TextStyle style) => $"{UssCustomClass_Emc}-{style}".ToLower();
        public virtual string USSCustomClassColor(TextColor color) => $"{UssCustomClass_Emc}-{color}".ToLower();

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }

        protected TextStyle m_style;
        protected TextColor m_color;

        public Text_C()
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
            RemoveFromClassList("unity-label");
            AddToClassList(UssCustomClass_Emc);
        }

        /// <summary>
        /// Initialize this element.
        /// </summary>
        protected virtual void InitElement()
        {
            m_textAlignment.ValueChanged += e =>
            {
                this.SwitchStyleclasses
                (
                    USSCustomClassAlignment(e.previousValue),
                    USSCustomClassAlignment(e.newValue)
                );
            };
        }

        /// <summary>
        /// Set the properties.
        /// </summary>
        /// <remarks>This methode is in the range of <see cref="IsSet"/> equals to true.</remarks>
        protected virtual void SetProperties()
        {
            TextStyle = TextStyle.Body;
            Color = TextColor.White;
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

            LanguageChanged += ChangedLanguage;
            ChangedLanguage();

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
            LanguageChanged -= ChangedLanguage;
        }

        #endregion

        /// <summary>
        /// Method called when a custom style sheet is resolved.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void CustomStyleResolved(CustomStyleResolvedEvent evt)
        {
        }

        /// <summary>
        /// Method called when this element geometry changed.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void GeometryChanged(GeometryChangedEvent evt)
        {
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
                this.TriggerAnimationCallback(property);
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

        #region Localisation

        public LocalisationAttribute m_localisation;
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
            text = m_localisation.DefaultText;

            if (m_localisation.CanBeLocalised)
            {
                while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

                text = LocalisationManager.Instance.GetTranslation(m_localisation);
            }
        }

        #endregion
    }
}
