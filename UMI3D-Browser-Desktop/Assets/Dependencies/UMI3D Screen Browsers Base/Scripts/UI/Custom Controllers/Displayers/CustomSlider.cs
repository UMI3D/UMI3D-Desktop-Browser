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
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomSlider : Slider, ICustomElement
{
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomSlider;

            custom.Set
            (
                m_category.GetValueFromBag(bag, cc), 
                m_size.GetValueFromBag(bag, cc), 
                m_direction.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetTextPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/text";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/slider";
    public virtual string StyleSheetTextFieldPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/textField";
    public virtual string USSCustomClassName => "slider";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassDirection(ElemnetDirection direction) => $"{USSCustomClassName}-{direction}".ToLower();
    public virtual string USSCustomClassLabel => $"{USSCustomClassName}-label";
    public virtual string USSCustomClassTextField => $"{USSCustomClassName}-text__field";

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

    public CustomTextfield SampleTextfield;
    public TextField TextField;
    public CustomText SampleTextLabel;

    public override bool showInputField
    { 
        get => base.showInputField;
        set 
        {
            base.showInputField = value;
            UpdateTextFieldStyle();
        } 
    }

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected ElemnetDirection m_direction;
    protected bool m_hasBeenInitialized;

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
        DirectionDisplayer = direction;

        UpdateTextFieldStyle();
    }
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
            this.AddStyleSheetFromPath(StyleSheetTextFieldPath);
            labelElement.AddStyleSheetFromPath(StyleSheetTextPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        UpdateLabelStyle();
    }

    public void UpdateTextFieldStyle()
    {
        if (!showInputField) return;

        if (TextField == null) TextField = this.Q<TextField>();
        TextField.ClearAndCopyStyleClasses(SampleTextfield);
        TextField.AddToClassList(USSCustomClassTextField);
    }

    public void UpdateLabelStyle()
    {
        labelElement.ClearAndCopyStyleClasses(SampleTextLabel);
        labelElement.AddToClassList(USSCustomClassLabel);
    }
}
