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
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace umi3d.commonScreen.Displayer
{
    public class SegmentedPicker_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<SegmentedPicker_C, UxmlTraits> { }

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
            protected UxmlLocaliseAttributeDescription m_localisedLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localised-label"
            };
            protected UxmlEnumAttributeDescription<ElemnetDirection> m_labelDirection = new UxmlEnumAttributeDescription<ElemnetDirection>
            {
                name = "label-direction",
                defaultValue = ElemnetDirection.Leading
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
                var custom = ve as SegmentedPicker_C;

                custom.Category = m_category.GetValueFromBag(bag, cc);
                custom.Size = m_size.GetValueFromBag(bag, cc);
                custom.OptionSeparator = m_optionSeparator.GetValueFromBag(bag, cc)[0];
                custom.LocalisedOptions = m_localisedOptions.GetValueFromBag(bag, cc);
                custom.LocalisedValue = m_localisedValue.GetValueFromBag(bag, cc);
                custom.LocalisedLabel = m_localisedLabel.GetValueFromBag(bag, cc);
                custom.LabelDirection = m_labelDirection.GetValueFromBag(bag, cc);
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
        public virtual LocalisationAttribute LocalisedLabel
        {
            get => LabelVisual.LocaliseText;
            set
            {
                if (value.IsEmpty) LabelVisual.RemoveFromHierarchy();
                else Insert(0, LabelVisual);
                LabelVisual.LocaliseText = value;
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

        public override string StyleSheetPath_MainTheme => $"USS/displayer";
        /// <summary>
        /// Style sheet dedicated to the main style of this element.
        /// </summary>
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/segmentedPicker";

        public override string UssCustomClass_Emc => "segmented-picker";
        public virtual string USSCustomClassCategory(ElementCategory category) => $"{UssCustomClass_Emc}-{category}".ToLower();
        public virtual string USSCustomClassSize(ElementSize size) => $"{UssCustomClass_Emc}-{size}".ToLower();
        public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{UssCustomClass_Emc}-{direction}-direction".ToLower();
        public virtual string USSCustomClassLabel => $"{UssCustomClass_Emc}__label";
        public virtual string USSCustomClassInput => $"{UssCustomClass_Emc}__input";
        public virtual string USSCustomClassValue => $"{UssCustomClass_Emc}__value";
        public virtual string USSCustomClassSeparator => $"{UssCustomClass_Emc}__separator";
        public virtual string USSCustomClassSeparatorBox => $"{UssCustomClass_Emc}__separator-box";
        public virtual string USSCustomClassSelectedValueBox => $"{UssCustomClass_Emc}__selected-value-box";
        public virtual string USSCustomClassSelectedValue => $"{UssCustomClass_Emc}__selected-value";

        public Text_C LabelVisual = new Text_C { name = "label" };
        public VisualElement Input = new VisualElement { name = "Input" };
        public List<Text_C> PickeValues = new List<Text_C>();
        public List<VisualElement> Separators = new List<VisualElement>();
        public VisualElement SeparatorBox = new VisualElement { name = "separator-box" };
        public VisualElement SelectedValueBox = new VisualElement { name = "selected-value-box" };
        public VisualElement SelectedValue = new VisualElement { name = "selected-value" };

        protected ElementCategory m_category;
        protected ElementSize m_size;
        protected List<string> m_options = new List<string>();
        protected ElemnetDirection m_labelDirection;
        protected Length m_textWidth => Length.Percent(100f / (float)m_options.Count);
        protected Length m_separatorLength = float.NaN;

        public SegmentedPicker_C() { }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            LabelVisual.AddToClassList(USSCustomClassLabel);
            Input.AddToClassList(USSCustomClassInput);
            SelectedValueBox.AddToClassList(USSCustomClassSelectedValueBox);
            SelectedValue.AddToClassList(USSCustomClassSelectedValue);
            SeparatorBox.AddToClassList(USSCustomClassSeparatorBox);
        }

        protected override void InitElement()
        {
            base.InitElement();
            this.RegisterCallback<CustomStyleResolvedEvent>((evt) =>
            {
                this.TryGetCustomStyle("--size-separator-picker", out var length);
                m_separatorLength = length;
            });

            SelectedValueBox.style.left = 0f;

            Add(Input);
            Input.Add(SelectedValueBox);
            SelectedValueBox.Add(SelectedValue);
            Input.Add(SeparatorBox);

            SeparatorBox.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                for (int i = 0; i < m_options.Count - 1; ++i) SetSeparator(Separators[i], i);
            });
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Category = ElementCategory.Menu;
            Size = ElementSize.Medium;
            OptionSeparator = ',';
            LocalisedOptions = (string)null;
            LocalisedValue = null;
            LocalisedLabel = null;
            LabelDirection = ElemnetDirection.Leading;
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

        #region Implementation

        public event Action<string> ValueChanged;

        public virtual void SetValueWithoutNotify(LocalisationAttribute newValue)
        {
            m_value = newValue;

            if (!m_options.Contains(newValue.DefaultText)) return;

            var index = m_options.IndexOf(newValue.DefaultText);
            SelectedValueBox.AddAnimation
            (
                this,
                () => SelectedValueBox.style.left = SelectedValueBox.style.left,
                () => SelectedValueBox.style.left = Length.Percent(m_textWidth.value * index),
                "left",
                0.5f
            );
        }

        protected void SetSeparator(VisualElement ve, int index)
        {
            ve.WaitUntil
            (
                () => !float.IsNaN(m_separatorLength.value) && !float.IsNaN(SeparatorBox.layout.width),
                () =>
                {
                    float halfWidthInPercent;
                    switch (m_separatorLength.unit)
                    {
                        case LengthUnit.Pixel:
                            halfWidthInPercent = m_separatorLength.value / 2f * 100 / Input.layout.width;
                            break;
                        case LengthUnit.Percent:
                            halfWidthInPercent = m_separatorLength.value / 2f;
                            break;
                        default:
                            halfWidthInPercent = 0f;
                            break;
                    }
                    ve.style.left = Length.Percent(m_textWidth.value * (index + 1) - halfWidthInPercent * (2 * index + 1));
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
            PickeValues.ForEach(value => value?.RemoveFromHierarchy());
            PickeValues.Clear();
            Separators.ForEach(value => value?.RemoveFromHierarchy());
            Separators.Clear();

            var defaultOptions = m_localisationOptions.GetDefaultOptions();
            if (defaultOptions == null) return;
            m_options.AddRange(defaultOptions);
            SelectedValueBox.style.width = m_textWidth;
            for (int i = 0; i < m_options.Count; ++i)
            {
                var text = new Text_C();
                text.AddToClassList(USSCustomClassValue);
                text.name = m_localisationOptions[i].DefaultText;
                text.LocaliseText = m_localisationOptions[i];
                text.style.width = m_textWidth;
                PickeValues.Add(text);
                var touchManip = new TouchManipulator2(null, 0, 0);
                touchManip.clicked += () => LocalisedValue = text.LocaliseText;
                text.AddManipulator(touchManip);
                Input.Add(text);

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

    public class SegmentedPicker_C<PickerEnum> : SegmentedPicker_C
    where PickerEnum : struct, Enum
    {
        public override LocalisationAttribute LocalisedValue
        {
            get => m_value;
            set
            {
                if (!TryGetEnumValue(value, out PickerEnum result)) return;
                base.LocalisedValue = value;
                ValueEnumChanged?.Invoke(result);
            }
        }

        public virtual PickerEnum? ValueEnum
        {
            get
            {
                if (!TryGetEnumValue(m_value, out PickerEnum result)) return null;
                else return result;
            }
            set => LocalisedValue = TranslateEnum(value.Value);
        }

        public SegmentedPicker_C() { }

        protected override void SetProperties()
        {
            base.SetProperties();
            var optionList = new List<string>(Enum.GetNames(typeof(PickerEnum)));
            LocalisedOptions = optionList;
        }

        #region Implementation

        public event Action<PickerEnum> ValueEnumChanged;

        /// <summary>
        /// Raised <see cref="ValueEnumChanged"/>.
        /// </summary>
        /// <param name="newValue"></param>
        public void OnValueEnumChanged(PickerEnum newValue) => ValueEnumChanged?.Invoke(newValue);

        public virtual void SetValueEnumWithoutNotify(PickerEnum value) => SetValueWithoutNotify(TranslateEnum(value));

        protected virtual LocalisationAttribute TranslateEnum(PickerEnum enumValue)
            => m_localisationOptions.Options.Find(option => option.DefaultText == enumValue.ToString());

        protected virtual bool TryGetEnumValue(LocalisationAttribute value, out PickerEnum result)
            => Enum.TryParse<PickerEnum>(value.DefaultText, out result);

        #endregion
    }
}
