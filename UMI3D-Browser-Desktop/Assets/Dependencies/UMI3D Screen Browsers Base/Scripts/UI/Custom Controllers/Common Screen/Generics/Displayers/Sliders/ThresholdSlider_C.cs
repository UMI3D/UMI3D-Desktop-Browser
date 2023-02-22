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

namespace umi3d.commonScreen.Displayer
{
    public class ThresholdSlider_C : Slider_C
    {
        public new class UxmlFactory : UxmlFactory<ThresholdSlider_C, UxmlTraits> { }

        public new class UxmlTraits : Slider_C.UxmlTraits
        {
            protected UxmlFloatAttributeDescription m_contentValue = new UxmlFloatAttributeDescription
            {
                name = "content-value",
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
                var custom = ve as ThresholdSlider_C;

                custom.ContentValue = m_contentValue.GetValueFromBag(bag, cc);
                custom.IsSaturated = m_isSaturated.GetValueFromBag(bag, cc);
                custom.ColorBeforeThreshold = m_colorBeforeThreshold.GetValueFromBag(bag, cc);
                custom.ColorAfterThreshold = m_colorAfterThreshold.GetValueFromBag(bag, cc);
                custom.ColorSaturation = m_colorSaturation.GetValueFromBag(bag, cc);
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

        public virtual string USSCustomClassThreshold => $"{UssCustomClass_Emc}-threshold";
        public virtual string USSCustomClassContent => $"{UssCustomClass_Emc}-content";

        public VisualElement Content = new VisualElement { name = "content" };

        protected float m_contentValue;
        protected bool m_isSaturated;
        protected Color m_colorBeforeThreshold;
        protected Color m_colorAfterThreshold;
        protected Color m_colorSaturation;

        public ThresholdSlider_C() { }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassThreshold);
            Content.AddToClassList(USSCustomClassContent);
        }

        protected override void InitElement()
        {
            base.InitElement();
            
            this.Q("unity-drag-container").Insert(1, Content);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            ColorBeforeThreshold = new Color(255, 128, 0);
            ColorAfterThreshold = Color.green;
            ColorSaturation = Color.red;
            IsSaturated = false;
            ContentValue = 0f;
        }
    }
}
