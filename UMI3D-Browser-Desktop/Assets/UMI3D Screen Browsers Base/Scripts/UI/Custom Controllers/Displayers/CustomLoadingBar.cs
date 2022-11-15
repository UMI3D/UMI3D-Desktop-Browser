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

public abstract class CustomLoadingBar : ProgressBar, ICustomElement
{
    public new class UxmlTraits : ProgressBar.UxmlTraits
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
        UxmlStringAttributeDescription m_message = new UxmlStringAttributeDescription
        {
            name = "message",
            defaultValue = null
        };
        UxmlFloatAttributeDescription m_value = new UxmlFloatAttributeDescription
        {
            name = "value",
            defaultValue = 0f
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomLoadingBar;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_message.GetValueFromBag(bag, cc), 
                    m_value.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetTextPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/text";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/loadingBar";
    public virtual string USSCustomClassName => "loadingbar";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassMessage => $"{USSCustomClassName}__message".ToLower();

    public override float value 
    { 
        get => base.value;
        set
        {
            base.value = value;
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
            switch (value)
            {
                case ElementSize.Small:
                    MessageLabel.TextStyle = TextStyle.Caption;
                    break;
                case ElementSize.Medium:
                    MessageLabel.TextStyle = TextStyle.Body;
                    break;
                case ElementSize.Large:
                    MessageLabel.TextStyle = TextStyle.Body;
                    break;
                default:
                    break;
            }
        }
    }
    public virtual string Message
    {
        get => MessageLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) MessageLabel.RemoveFromHierarchy();
            else
            {
                Add(MessageLabel);
                MessageLabel.text = value;
            }
        }
    }

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected bool m_hasBeenInitialized;

    public CustomText SampleTitleLabel;
    public Label TitleLabel;
    public CustomText MessageLabel;

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

        MessageLabel.AddToClassList(USSCustomClassMessage);
    }

    public virtual void Set()
        => Set(ElementCategory.Menu, ElementSize.Medium, null, 0f);

    public virtual void Set(ElementCategory category, ElementSize size, string message, float value)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Size = size;
        Message = message;
        this.value = value;

        SampleTitleLabel.TextStyle = TextStyle.Subtitle;
        UpdateTitleStyle();
    }

    protected void UpdateTitleStyle()
    {
        if (TitleLabel == null) TitleLabel = this.Q<Label>(className: titleUssClassName);
        TitleLabel.ClearAndCopyStyleClasses(SampleTitleLabel);
        TitleLabel.AddToClassList(titleUssClassName);
    }

    //System.Collections.IEnumerator SetValueNextFrame()
    //{
    //    yield return new UnityEngine.WaitForEndOfFrame();
    //    value = (value);
    //}
}
