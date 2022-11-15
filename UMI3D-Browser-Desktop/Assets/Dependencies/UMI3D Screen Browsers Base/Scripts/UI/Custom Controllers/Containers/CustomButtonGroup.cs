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
using UnityEngine.UIElements;

public abstract class CustomButtonGroup : VisualElement, ICustomElement
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomButtonGroup;

            custom.Set
            (
                m_category.GetValueFromBag(bag, cc),
                m_size.GetValueFromBag(bag, cc),
                m_optionSeparator.GetValueFromBag(bag, cc),
                m_options.GetValueFromBag(bag, cc),
                m_value.GetValueFromBag(bag, cc)
            );
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
            foreach (var button in Buttons) button.Size = value;
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
            Buttons.ForEach(value => value?.RemoveFromHierarchy());
            Buttons.Clear();
            Separators.ForEach(value => value?.RemoveFromHierarchy());
            Separators.Clear();

            if (value == null) return;

            var values = value.Split(OptionSeparator);
            for (int i = 0; i < values.Length; i++) values[i] = values[i].Trim();
            m_options.AddRange(values);
            for (int i = 0; i < m_options.Count; ++i)
            {
                var option = m_options[i];

                var button = CreateButton();
                button.Type = ButtonType.ButtonGroupEnable;
                var optionString = OptionToString(option);
                button.name = optionString;
                button.text = optionString;
                button.ClickedUp += () => Value = button.name;
                Buttons.Add(button);
                Input.Add(button);

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

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/buttonGroup";
    public virtual string USSCustomClassName => "button-group";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassInput => $"{USSCustomClassName}__input";
    public virtual string USSCustomClassSeparator => $"{USSCustomClassName}__separator";
    public virtual string USSCustomClassSeparatorBox => $"{USSCustomClassName}__separator-box";

    public event System.Action<string> ValueChanged;
    public VisualElement Input = new VisualElement { name = "Input" };
    public List<CustomButton> Buttons = new List<CustomButton>();
    public VisualElement SeparatorBox = new VisualElement { name = "separator-box" };
    public List<VisualElement> Separators = new List<VisualElement>();
    public TouchManipulator2 m_touchManipulator = new TouchManipulator2(null, 0, 0);

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected string m_optionSeparator;
    protected List<string> m_options = new List<string>();
    protected string m_optionsString;
    protected string m_value;

    protected bool m_hasBeenInitialised;
    protected Length m_buttonHeight => Length.Percent(100f / m_buttonCount);
    protected Length m_separatorLength = float.NaN;
    protected virtual float m_buttonCount => (float)m_options.Count;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetContainerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        Input.AddToClassList(USSCustomClassInput);
        SeparatorBox.AddToClassList(USSCustomClassSeparatorBox);

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

        this.RegisterCallback<CustomStyleResolvedEvent>((evt) =>
        {
            this.TryGetCustomStyle("--size-separator-container", out var length);
            m_separatorLength = length;
        });
        
        SeparatorBox.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            for (int i = 0; i < m_options.Count - 1; ++i) setSeparator(Separators[i], i);
        });

        Add(Input);
        Add(SeparatorBox);
    }

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, ",", null, null);

    public virtual void Set(ElementCategory category, ElementSize size, string optionSeparator, string options, string value)
    {
        if (!m_hasBeenInitialised)
        {
            InitElement();
            m_hasBeenInitialised = true;
        }

        Category = category;
        Size = size;
        OptionSeparator = optionSeparator;
        Options = options;
        Value = value;
    }

    public virtual void SetValueWithoutNotify(string value)
    {
        CustomButton previousButton = Buttons.Find((button) => button.name == m_value);
        CustomButton nextButton = Buttons.Find((button) => button.name == value);
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

    protected void setSeparator(VisualElement ve, int index)
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

    protected abstract CustomButton CreateButton();

    protected virtual string OptionToString(string option) => option;
}

public abstract class CustomButtonGroup<ButtonEnum> : CustomButtonGroup
     where ButtonEnum : struct, System.Enum
{
    public new class UxmlTraits : CustomButtonGroup.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomButtonGroup<ButtonEnum>;

            custom.Set
            (
                m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_value.GetValueFromBag(bag, cc)
            );
        }
    }

    public override string OptionSeparator => ",";

    public override string Value
    {
        get => m_value;
        set
        {
            base.Value = value;
            if (!Enum.TryParse<ButtonEnum>(m_value, out ButtonEnum result)) return;
            ValueEnumChanged?.Invoke(result);
        }
    }

    public virtual ButtonEnum EnumValue { set => Value = EnumToString(value); }

    public event System.Action<ButtonEnum> ValueEnumChanged;

    public override void Set() => Set(ElementCategory.Menu, ElementSize.Medium, null);

    public virtual void Set(ElementCategory category, ElementSize size, string value)
    {
        var optionList = new List<string>(Enum.GetNames(typeof(ButtonEnum)));
        base.Set(category, size, ",", String.Join(",", optionList), value);
    }

    protected override string OptionToString(string option)
    {
        if (!Enum.TryParse<ButtonEnum>(option, out ButtonEnum result)) return option;
        return EnumToString(result);
    }

    protected abstract string EnumToString(ButtonEnum enumValue);
}
