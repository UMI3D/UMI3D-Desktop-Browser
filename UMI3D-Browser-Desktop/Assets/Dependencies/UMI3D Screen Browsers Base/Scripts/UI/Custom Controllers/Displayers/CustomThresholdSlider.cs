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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomThresholdSlider : CustomSlider
{
    public new class UxmlTraits : CustomSlider.UxmlTraits
    {
        protected UxmlFloatAttributeDescription m_contentValue = new UxmlFloatAttributeDescription
        {
            name= "content-value",
            defaultValue = 0f,
        };

        protected UxmlBoolAttributeDescription m_isSaturated = new UxmlBoolAttributeDescription
        {
            name = "is-saturated",
            defaultValue = false
        };

        protected UxmlColorAttributeDescription m_colorBeforeThreshold = new UxmlColorAttributeDescription
        {
            name = "color-before-threshold",
            defaultValue = new Color(255, 128, 0)
        };

        protected UxmlColorAttributeDescription m_colorAfterThreshold = new UxmlColorAttributeDescription
        {
            name = "color-after-threshold",
            defaultValue = Color.green
        };

        protected UxmlColorAttributeDescription m_colorSaturation = new UxmlColorAttributeDescription
        {
            name = "color-saturation",
            defaultValue = Color.red
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomThresholdSlider;

            custom.Set
            (
                m_category.GetValueFromBag(bag, cc), 
                m_size.GetValueFromBag(bag, cc), 
                m_direction.GetValueFromBag(bag, cc),
                m_contentValue.GetValueFromBag(bag, cc),
                m_isSaturated.GetValueFromBag(bag, cc),
                m_colorBeforeThreshold.GetValueFromBag(bag, cc),
                m_colorAfterThreshold.GetValueFromBag(bag, cc),
                m_colorSaturation.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual float ContentValue
    {
        get => m_contentValue;
        set
        {
            value = Mathf.Clamp(value, lowValue, highValue);
            m_contentValue = value;

            if (IsSaturated) Content.style.backgroundColor = ColorSaturation;
            else Content.style.backgroundColor = value <= this.value ? ColorBeforeThreshold : ColorAfterThreshold;

            var left = (value - lowValue) * 100 / (highValue - lowValue);
            Content.style.right = Length.Percent(100 - left);
        }
    }

    public virtual bool IsSaturated
    {
        get => m_isSaturated;
        set
        {
            m_isSaturated = value;
            if (value) Content.style.backgroundColor = ColorSaturation;
            else Content.style.backgroundColor = ContentValue <= this.value ? ColorBeforeThreshold : ColorAfterThreshold;
        }
    }

    public virtual Color ColorBeforeThreshold
    {
        get => m_colorBeforeThreshold;
        set => m_colorBeforeThreshold = value;
    }

    public virtual Color ColorAfterThreshold
    {
        get => m_colorAfterThreshold;
        set => m_colorAfterThreshold = value;
    }

    public virtual Color ColorSaturation
    {
        get => m_colorSaturation;
        set => m_colorSaturation = value;
    }

    public virtual string USSCustomClassThreshold => $"{USSCustomClassName}-threshold";
    public virtual string USSCustomClassContent => $"{USSCustomClassName}-content";

    public VisualElement Content = new VisualElement { name = "content" };

    protected float m_contentValue;
    protected bool m_isSaturated;
    protected Color m_colorBeforeThreshold;
    protected Color m_colorAfterThreshold;
    protected Color m_colorSaturation;

    public virtual void Set(ElementCategory category, ElementSize size, ElemnetDirection direction, float contentValue, bool isSaturated, Color colorBeforeThreshold, Color colorAfterThreshold, Color colorSaturation)
    {
        base.Set(category, size, direction);
        ColorBeforeThreshold = colorBeforeThreshold;
        ColorAfterThreshold = colorAfterThreshold;
        ColorSaturation = colorSaturation;
        IsSaturated = isSaturated;
        ContentValue = contentValue;
    }

    public override void InitElement()
    {
        base.InitElement();
        AddToClassList(USSCustomClassThreshold);
        Content.AddToClassList(USSCustomClassContent);

        this.Q("unity-drag-container").Insert(1, Content);
    }
}
