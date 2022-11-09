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
using System.Linq;
using umi3d.baseBrowser.ui.viewController;
using UnityEngine.UIElements;

public abstract class CustomSegmentedPicker : VisualElement, ICustomElement
{
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
            name = "optionSeparator",
            defaultValue = ","
        };
        protected UxmlStringAttributeDescription m_options = new UxmlStringAttributeDescription
        {
            name = "options",
            defaultValue = null
        };
        protected UxmlStringAttributeDescription m_value = new UxmlStringAttributeDescription
        {
            name = "value",
            defaultValue = null
        };
        protected UxmlStringAttributeDescription m_label = new UxmlStringAttributeDescription
        {
            name = "label",
            defaultValue = null
        };
        protected UxmlEnumAttributeDescription<ElemnetDirection> m_labelDirection = new UxmlEnumAttributeDescription<ElemnetDirection>
        {
            name = "label-direction",
            defaultValue = ElemnetDirection.Leading
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomSegmentedPicker;
            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_optionSeparator.GetValueFromBag(bag, cc),
                    m_options.GetValueFromBag(bag, cc),
                    m_value.GetValueFromBag(bag, cc),
                    m_label.GetValueFromBag(bag, cc),
                    m_labelDirection.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/segmentedPicker";
    public virtual string USSCustomClassName => "segmented-picker";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{USSCustomClassName}-{direction}-direction".ToLower();
    public virtual string USSCustomClassLabel => $"{USSCustomClassName}__label";
    public virtual string USSCustomClassInput => $"{USSCustomClassName}__input";
    public virtual string USSCustomClassValue => $"{USSCustomClassName}__value";
    public virtual string USSCustomClassSeparator => $"{USSCustomClassName}__separator";
    public virtual string USSCustomClassSeparatorBox => $"{USSCustomClassName}__separator-box";
    public virtual string USSCustomClassSelectedValueBox => $"{USSCustomClassName}__selected-value-box";
    public virtual string USSCustomClassSelectedValue => $"{USSCustomClassName}__selected-value";

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
    public virtual string OptionSeparator
    {
        get => m_optionSeparator;
        set
        {
            m_optionSeparator = value;
            Options = m_optionsString;
        }
    }
    public virtual string Options
    {
        get => m_optionsString;
        set
        {
            m_optionsString = value;
            m_options.Clear();
            PickeValues.ForEach(value => value?.RemoveFromHierarchy());
            PickeValues.Clear();
            Separators.ForEach(value => value?.RemoveFromHierarchy());
            Separators.Clear();

            if (value == null) return;

            var values = value.Split(OptionSeparator);
            for (int i = 0; i < values.Length; i++) values[i] = values[i].Trim();
            m_options.AddRange(values);
            SelectedValueBox.style.width = m_textWidth;
            for (int i = 0; i < m_options.Count; ++i)
            {
                var option = m_options[i];

                var text = CreateText();
                text.AddToClassList(USSCustomClassValue);
                var optionString = OptionToString(option);
                text.name = optionString;
                text.text = optionString;
                text.style.width = m_textWidth;
                PickeValues.Add(text);
                var touchManip = new TouchManipulator2(null, 0, 0);
                touchManip.clicked += () => Value = option;
                text.AddManipulator(touchManip);
                Input.Add(text);

                if (i == m_options.Count - 1) break;
                var separator = new VisualElement { name = "separator" };
                separator.AddToClassList(USSCustomClassSeparator);
                Separators.Add(separator);
                SeparatorBox.Add(separator);
                setSeparator(separator, i);
            }

            SetValueWithoutNotify(m_value);
        }
    }
    public virtual string Value
    {
        get => m_value;
        set
        {
            SetValueWithoutNotify(value);
            ValueChanged?.Invoke(value);
        }
    }
    public virtual string Label
    {
        get => LabelVisual.text;
        set
        {
            if (string.IsNullOrEmpty(value)) LabelVisual.RemoveFromHierarchy();
            else Insert(0, LabelVisual);
            LabelVisual.text = value;
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

    public virtual void SetValueWithoutNotify(string newValue)
    {
        m_value = newValue;

        if (!m_options.Contains(newValue)) return;

        var index = m_options.IndexOf(newValue);
        SelectedValueBox.AddAnimation
        (
            this,
            () => SelectedValueBox.style.left = SelectedValueBox.style.left,
            () => SelectedValueBox.style.left = Length.Percent(m_textWidth.value * index),
            "left",
            0.5f
        );
    }

    public event Action<string> ValueChanged;
    public CustomText LabelVisual;
    public VisualElement Input = new VisualElement { name = "Input" };
    public List<CustomText> PickeValues = new List<CustomText>();
    public List<VisualElement> Separators = new List<VisualElement>();
    public VisualElement SeparatorBox = new VisualElement { name = "separator-box" };
    public VisualElement SelectedValueBox = new VisualElement { name = "selected-value-box" };
    public VisualElement SelectedValue = new VisualElement { name = "selected-value" };

    protected bool m_hasBeenInitialized;
    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected string m_optionSeparator;
    protected List<string> m_options = new List<string>();
    protected string m_optionsString;
    protected string m_value;
    protected ElemnetDirection m_labelDirection;
    protected Length m_textWidth => Length.Percent(100f / (float)m_options.Count);
    protected Length m_separatorLength = float.NaN;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        LabelVisual.AddToClassList(USSCustomClassLabel);
        Input.AddToClassList(USSCustomClassInput);
        SelectedValueBox.AddToClassList(USSCustomClassSelectedValueBox);
        SelectedValue.AddToClassList(USSCustomClassSelectedValue);
        SeparatorBox.AddToClassList(USSCustomClassSeparatorBox);

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
            for (int i = 0; i < m_options.Count - 1; ++i) setSeparator(Separators[i], i);
        });
    }

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, ",", null, null, null, ElemnetDirection.Leading);

    public virtual void Set(ElementCategory category, ElementSize size, string optionSeparator, string options, string value, string label, ElemnetDirection labelDirection)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Size = size;
        OptionSeparator = optionSeparator;
        Options = options;
        Value = value;
        Label = label;
        LabelDirection = labelDirection;
    }
    protected void setSeparator(VisualElement ve, int index)
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

    protected abstract CustomText CreateText();

    protected virtual string OptionToString(string option) => option;
}


public abstract class CustomSegmentedPicker<PickerEnum> : CustomSegmentedPicker
    where PickerEnum : struct, System.Enum
{
    public new class UxmlTraits : CustomSegmentedPicker.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomSegmentedPicker<PickerEnum>;
            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_value.GetValueFromBag(bag, cc),
                    m_label.GetValueFromBag(bag, cc),
                    m_labelDirection.GetValueFromBag(bag, cc)
                );
        }
    }

    public override string OptionSeparator => ",";

    public override string Value
    {
        get => m_value;
        set
        {
            
            if (!Enum.TryParse<PickerEnum>(value, out PickerEnum result)) return;
            base.Value = value;
            ValueEnumChanged?.Invoke(result);
        }
    }

    public virtual PickerEnum? ValueEnum
    {
        get
        {
            if (!Enum.TryParse<PickerEnum>(m_value, out PickerEnum result)) return null;
            else return result;
        }
        set => Value = value.ToString();
    }

    public event Action<PickerEnum> ValueEnumChanged;

    public virtual void SetValueEnumWithoutNotify(PickerEnum value) => SetValueWithoutNotify(value.ToString()); 

    public override void Set() => Set(ElementCategory.Menu, ElementSize.Medium, null, null, ElemnetDirection.Leading);

    public virtual void Set(ElementCategory category, ElementSize size, string value, string label, ElemnetDirection labelDirection)
    {
        var optionList = new List<string>(Enum.GetNames(typeof(PickerEnum)));
        base.Set(category, size, ",", String.Join(",", optionList), value, label, labelDirection);
    }

    protected override string OptionToString(string option)
    {
        if (!Enum.TryParse<PickerEnum>(option, out PickerEnum result)) return option;
        return EnumToString(result);
    }

    protected abstract string EnumToString(PickerEnum enumValue);
}
