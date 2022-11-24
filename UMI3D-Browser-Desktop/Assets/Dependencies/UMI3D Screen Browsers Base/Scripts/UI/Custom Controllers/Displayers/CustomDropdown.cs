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

public abstract class CustomDropdown : DropdownField, ICustomElement
{
    public new class UxmlTraits : DropdownField.UxmlTraits
    {
        UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory> { name = "category", defaultValue = ElementCategory.Menu };
        UxmlEnumAttributeDescription<ElementSize> m_size = new UxmlEnumAttributeDescription<ElementSize> { name = "size", defaultValue = ElementSize.Medium };
        UxmlEnumAttributeDescription<ElemnetDirection> m_direction = new UxmlEnumAttributeDescription<ElemnetDirection> { name = "direction", defaultValue = ElemnetDirection.Leading };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomDropdown;

            custom.Set(m_category.GetValueFromBag(bag, cc), m_size.GetValueFromBag(bag, cc), m_direction.GetValueFromBag(bag, cc));
        }
    }

    public virtual string StyleSheetDisplayerPath => "USS/displayer";
    public virtual string StyleSheetTextPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/text";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/dropDown";
    public virtual string USSCustomClassName => "dropdown";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{USSCustomClassName}-{direction}".ToLower();
    public virtual string USSCustomClassLabel => $"{USSCustomClassName}__label";
    public virtual string USSClassText => $"unity-base-popup-field__text";

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

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected ElemnetDirection m_direction;
    protected bool m_showLabel;
    protected bool m_hasBeenInitialized;

    public CustomText SampleTextLabel;

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, ElemnetDirection.Leading);
    public virtual void Set(ElementCategory category, ElementSize size, ElemnetDirection direction)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Size = size;
        Direction = direction;
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

        UpdateLabelStyle();
    }

    public void UpdateLabelStyle()
    {
        labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
        labelElement.AddToClassList(USSCustomClassLabel);

        textElement.ClearAndCopyStyleClasses(SampleTextLabel);
        textElement.AddToClassList(USSClassText);
    }
}
