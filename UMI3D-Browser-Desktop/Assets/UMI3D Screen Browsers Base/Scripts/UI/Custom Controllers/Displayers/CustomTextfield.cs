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
using UnityEngine.UIElements;

public abstract class CustomTextfield : TextField, ICustomElement
{
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
        UxmlEnumAttributeDescription<ElemnetDirection> m_direction = new UxmlEnumAttributeDescription<ElemnetDirection> 
        { 
            name = "direction", 
            defaultValue = ElemnetDirection.Leading 
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomTextfield;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc), 
                    m_size.GetValueFromBag(bag, cc), 
                    m_direction.GetValueFromBag(bag, cc),
                    m_maskToggle.GetValueFromBag(bag, cc),
                    m_submitButton.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetDisplayerPath => "USS/displayer";
    public virtual string StyleSheetTextPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/text";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/textField";
    public virtual string USSCustomClassName => "textfield";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{USSCustomClassName}-{direction}".ToLower();
    public virtual string USSCustomClassLabel => $"{USSCustomClassName}__label";
    public virtual string USSCustomClassToggle => $"{USSCustomClassName}__toggle";
    public virtual string USSCustomClassButton => $"{USSCustomClassName}__button";

    public static bool IsTyping;
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
            SubmitButton.Size = value;
        }
    }
    public ElemnetDirection Direction
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

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected ElemnetDirection m_direction;
    protected bool m_displayMaskToggle;
    protected bool m_submitButton;
    protected bool m_hasBeenInitialized;
    protected List<string> m_textInputOriginalClassStyle;

    public CustomText SampleTextLabel;
    public CustomToggle MaskToggle;
    public CustomButton SubmitButton;

    protected TextInputBase TextInput;

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, ElemnetDirection.Leading, false, false);
    public virtual void Set(ElementCategory category, ElementSize size, ElemnetDirection direction, bool maskToggle, bool submitButton)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }
        Category = category;
        Size = size;
        Direction = direction;
        DisplayMaskToggle = maskToggle;
        DisplaySubmitButton = submitButton;
    }
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetTextPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        MaskToggle.AddToClassList(USSCustomClassToggle);
        SubmitButton.AddToClassList(USSCustomClassButton);

        MaskToggle.RegisterValueChangedCallback((value) => isPasswordField = value.newValue);

        TextInput = this.Q<TextInputBase>("unity-text-input");
        m_textInputOriginalClassStyle = new List<string>(TextInput.GetClasses());

        UpdateLabelStyle();
    }

    public void UpdateLabelStyle()
    {
        labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
        labelElement.AddToClassList(USSCustomClassLabel);

        TextInput.ClearClassList();
        foreach (var style in m_textInputOriginalClassStyle)
            TextInput.AddToClassList(style);
        TextInput.CopyStyleClasses(SampleTextLabel);
    }
}
