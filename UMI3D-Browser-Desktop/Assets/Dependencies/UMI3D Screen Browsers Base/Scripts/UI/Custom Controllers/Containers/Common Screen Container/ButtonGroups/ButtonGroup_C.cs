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
using System;
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ButtonGroup_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<ButtonGroup_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
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
            protected UxmlStringAttributeDescription m_optionSeparator = new UxmlStringAttributeDescription
            {
                name = "option-separator",
                defaultValue = ","
            };
            protected UxmlLocaliseForOptionsAttributeDescription m_localisedOptions = new UxmlLocaliseForOptionsAttributeDescription
            {
                name = "localised-options"
            };
            protected UxmlLocaliseAttributeDescription m_localisedValue = new UxmlLocaliseAttributeDescription
            {
                name = "localised-value"
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
                var custom = ve as ButtonGroup_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.OptionSeparator = m_optionSeparator.GetValueFromBag(bag, cc)[0];
                custom.LocalisedOptions = m_localisedOptions.GetValueFromBag(bag, cc);
                custom.LocalisedValue = m_localisedValue.GetValueFromBag(bag, cc);
            }
        }

        public virtual ElementCategory Category
        {
            get => m_category;
            set
            {
                RemoveFromClassList(USSCustomClassCategory(m_category));
                AddToClassList(USSCustomClassCategory(value));
                foreach (var button in Buttons) button.Category = value;
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
                foreach (var button in Buttons) button.Height = value;
                m_size = value;
            }
        }
        public virtual char OptionSeparator
        {
            get => m_localisationOptions.Separator;
            set
            {
                m_localisationOptions.Separator = value;
                if (IsAttachedToPanel) UpdateTranslation();
            }
        }
        public virtual LocalisationForOptionsAttribute LocalisedOptions
        {
            get => m_localisationOptions;
            set
            {
                var currentSeparator = m_localisationOptions.Separator;
                m_localisationOptions = value;
                m_localisationOptions.Separator = currentSeparator;
                if (IsAttachedToPanel) UpdateTranslation();
            }
        }
        public virtual LocalisationAttribute LocalisedValue
        {
            get => m_value;
            set
            {
                SetValueWithoutNotify(value);
                ValueChanged?.Invoke(value);
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/container";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetContainersFolderPath}/buttonGroup";

        public override string UssCustomClass_Emc => "button-group";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassInput => $"{UssCustomClass_Emc}__input";
        public virtual string USSCustomClassSeparator => $"{UssCustomClass_Emc}__separator";
        public virtual string USSCustomClassSeparatorBox => $"{UssCustomClass_Emc}__separator-box";

        public VisualElement Input = new VisualElement { name = "Input" };
        public List<Button_C> Buttons = new List<Button_C>();
        public VisualElement SeparatorBox = new VisualElement { name = "separator-box" };
        public List<VisualElement> Separators = new List<VisualElement>();
        public TouchManipulator2 m_touchManipulator = new TouchManipulator2(null, 0, 0);

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected List<string> m_options = new List<string>();
        
        protected Length m_buttonHeight => Length.Percent(100f / m_buttonCount);
        protected Length m_separatorLength = float.NaN;
        protected virtual float m_buttonCount => (float)m_options.Count;

        public ButtonGroup_C() { }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            Input.AddToClassList(USSCustomClassInput);
            SeparatorBox.AddToClassList(USSCustomClassSeparatorBox);
        }

        protected override void InitElement()
        {
            SeparatorBox.AddManipulator(m_touchManipulator);
            m_touchManipulator.ClickedUpWithInfo += (evt, localPosition) =>
            {
                var localToWorld = SeparatorBox.LocalToWorld(localPosition);
                foreach (var button in Buttons)
                {
                    var worldToLocal = button.WorldToLocal(localToWorld);
                    if (button.ContainsPoint(worldToLocal))
                    {
                        button.TouchManipulator.OnClickedUp();
                        return;
                    }
                }
            };

            SeparatorBox.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                for (int i = 0; i < m_options.Count - 1; ++i) SetSeparator(Separators[i], i);
            });

            Add(Input);
            Add(SeparatorBox);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Category = ElementCategory.Menu;
            Size = ElementSize.Medium;
            OptionSeparator = ',';
            LocalisedOptions = (string)null;
            LocalisedValue = null;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);

            LanguageChanged += UpdateTranslation;
            UpdateTranslation();
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);

            LanguageChanged -= UpdateTranslation;
        }

        protected override void CustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            base.CustomStyleResolved(evt);
            this.TryGetCustomStyle("--size-separator-container", out var length);
            m_separatorLength = length;
        }

        #region Implementation

        public event System.Action<LocalisationAttribute> ValueChanged;

        /// <summary>
        /// Update the value without notify.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValueWithoutNotify(LocalisationAttribute value)
        {
            Button_C previousButton = Buttons.Find((button) => button.name == m_value.DefaultText);
            Button_C nextButton = Buttons.Find((button) => button.name == value.DefaultText);
            m_value = value;

            foreach (var separator in Separators) separator.style.visibility = StyleKeyword.Null;

            if (previousButton != null) previousButton.Type = ButtonType.ButtonGroupEnable;
            if (nextButton != null)
            {
                nextButton.Type = ButtonType.ButtonGroupSelected;
                var index = Buttons.IndexOf(nextButton);
                if (index - 1 >= 0) Separators[index - 1].style.visibility = Visibility.Hidden;
                if (index < Separators.Count) Separators[index].style.visibility = Visibility.Hidden;
            }
        }

        protected void SetSeparator(VisualElement ve, int index)
        {
            ve.WaitUntil
            (
                () => !float.IsNaN(m_separatorLength.value) && !float.IsNaN(SeparatorBox.layout.width),
                () =>
                {
                    float halfHeightInPercent;
                    switch (m_separatorLength.unit)
                    {
                        case LengthUnit.Pixel:
                            halfHeightInPercent = m_separatorLength.value / 2f * 100 / layout.height;
                            break;
                        case LengthUnit.Percent:
                            halfHeightInPercent = m_separatorLength.value / 2f;
                            break;
                        default:
                            halfHeightInPercent = 0f;
                            break;
                    }
                    ve.style.top = Length.Percent(m_buttonHeight.value * (index + 1) - halfHeightInPercent * (2 * index + 1));
                }
            );
        }

        #region Localisation

        public static event System.Action LanguageChanged;
        public LocalisationForOptionsAttribute m_localisationOptions;
        protected LocalisationAttribute m_value;

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
            m_options.Clear();
            Buttons.ForEach(value => value?.RemoveFromHierarchy());
            Buttons.Clear();
            Separators.ForEach(value => value?.RemoveFromHierarchy());
            Separators.Clear();

            var defaultOptions = m_localisationOptions.GetDefaultOptions();
            if (defaultOptions == null) return;
            m_options.AddRange(defaultOptions);
            for (int i = 0; i < m_options.Count; ++i)
            {
                var button = new Button_C();
                button.Type = ButtonType.ButtonGroupEnable;
                button.name = m_localisationOptions[i].DefaultText;
                button.LocaliseText = m_localisationOptions[i];
                button.ClickedUp += () => LocalisedValue = button.LocaliseText;
                Buttons.Add(button);
                Input.Add(button);

                if (i == m_options.Count - 1) break;
                var separator = new VisualElement { name = "separator" };
                separator.AddToClassList(USSCustomClassSeparator);
                Separators.Add(separator);
                SeparatorBox.Add(separator);
                SetSeparator(separator, i);
            }

            SetValueWithoutNotify(m_value);
        }

        #endregion

        #endregion
    }

    public class ButtonGroup_C<ButtonEnum> : ButtonGroup_C
    where ButtonEnum : struct, System.Enum
    {
        public override LocalisationAttribute LocalisedValue
        {
            get => m_value;
            set
            {
                if (!Enum.TryParse<ButtonEnum>(value.DefaultText, out ButtonEnum result)) return;
                base.LocalisedValue = value;
                ValueEnumChanged?.Invoke(result);
            }
        }

        public virtual ButtonEnum EnumValue { set => LocalisedValue = TranslateEnum(value); }

        public ButtonGroup_C() { }

        protected override void SetProperties()
        {
            base.SetProperties();
            var optionList = new List<string>(Enum.GetNames(typeof(ButtonEnum)));
            LocalisedOptions = optionList;
        }

        #region Implementation

        public event Action<ButtonEnum> ValueEnumChanged;

        public virtual void SetValueEnumWithoutNotify(ButtonEnum value) => SetValueWithoutNotify(TranslateEnum(value));

        protected virtual LocalisationAttribute TranslateEnum(ButtonEnum enumValue)
            => m_localisationOptions.Options.Find(option => option.DefaultText == enumValue.ToString());

        #endregion
    }
}
