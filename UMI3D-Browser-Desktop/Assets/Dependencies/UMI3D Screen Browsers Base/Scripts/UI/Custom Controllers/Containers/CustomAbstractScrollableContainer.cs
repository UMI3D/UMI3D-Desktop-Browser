/*
Copyright 2019 - 2023 Inetum

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

public abstract class CustomAbstractScrollableContainer : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ElementCategory> m_category = new UxmlEnumAttributeDescription<ElementCategory>
        {
            name = "category",
            defaultValue = ElementCategory.Menu
        };
        protected UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription
        {
            name = "title",
            defaultValue = null
        };
        protected UxmlStringAttributeDescription m_iconPath = new UxmlStringAttributeDescription
        {
            name = "icon-path",
            defaultValue = null
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomAbstractScrollableContainer;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_title.GetValueFromBag(bag, cc),
                    m_iconPath.GetValueFromBag(bag, cc)
                 );
        }
    }

    /// <summary>
    /// Category of this element (<see cref="ElementCategory"/>)
    /// </summary>
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

    /// <summary>
    /// Display a title for this container.
    /// </summary>
    public virtual string Title
    {
        get => TitleLabel.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) TitleLabel.RemoveFromHierarchy();
            else
            {
                Insert(0, TitleLabel);
                TitleLabel.text = value;
            }
            m_isSet = true;
        }
    }

    /// <summary>
    /// Display an icon for this container.
    /// </summary>
    public virtual string IconPath
    {
        get => m_iconPath;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value))
            {
                m_iconPath = null;
                IconVisual.style.backgroundImage = StyleKeyword.Null;
                IconVisual.RemoveFromHierarchy();
            }
            else
            {
                m_iconPath = value;
                Insert(1, IconVisual);
                IconVisual.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(value));
            }
            m_isSet = true;
        }
    }

    public virtual string StyleSheetContainerPath => $"USS/container";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/scrollableForm";
    public virtual string USSCustomClassName => "scrollable__form";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}-title";
    public virtual string USSCustomClassIcon => $"{USSCustomClassName}-icon";

    public CustomText TitleLabel;
    public VisualElement IconVisual = new VisualElement { name = "icon" };

    protected ElementCategory m_category;
    protected string m_iconPath;
    protected bool m_hasBeenInitialized;
    protected bool m_isSet = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
        TitleLabel.AddToClassList(USSCustomClassTitle);
        IconVisual.AddToClassList(USSCustomClassIcon);

        TitleLabel.TextStyle = TextStyle.LowTitle;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Set() => Set(ElementCategory.Menu, null, null);
    /// <summary>
    /// Set this UI element
    /// </summary>
    /// <param name="category"></param>
    /// <param name="title"></param>
    /// <param name="iconPath"></param>
    public virtual void Set(ElementCategory category, string title, string iconPath)
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Title = title;
        IconPath = iconPath;

        m_isSet = true;
    }
}
