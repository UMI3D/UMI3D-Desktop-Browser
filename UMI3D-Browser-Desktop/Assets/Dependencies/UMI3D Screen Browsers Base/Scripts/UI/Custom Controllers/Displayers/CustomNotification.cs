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
using umi3d.common;
using UnityEngine.UIElements;

public abstract class CustomNotification : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
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
        UxmlEnumAttributeDescription<NotificationType> m_type = new UxmlEnumAttributeDescription<NotificationType> 
        { 
            name = "type", 
            defaultValue = NotificationType.Default 
        };
        UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription 
        { 
            name = "title", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_author = new UxmlStringAttributeDescription 
        { 
            name = "author", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_timestamp = new UxmlStringAttributeDescription 
        { 
            name = "timestamp", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_message = new UxmlStringAttributeDescription 
        { 
            name = "message", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_choiceALabel = new UxmlStringAttributeDescription
        {
            name = "choice-a-label",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_choiceBLabel = new UxmlStringAttributeDescription
        {
            name = "choice-b-label",
            defaultValue = null
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomNotification;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_type.GetValueFromBag(bag, cc),
                    m_title.GetValueFromBag(bag, cc),
                    m_author.GetValueFromBag(bag, cc),
                    m_timestamp.GetValueFromBag(bag, cc),
                    m_message.GetValueFromBag(bag, cc),
                    m_choiceALabel.GetValueFromBag(bag, cc),
                    m_choiceBLabel.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/notification";
    public virtual string USSCustomClassName => "notification";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassType(NotificationType type) => $"{USSCustomClassName}-{type}".ToLower();
    public virtual string USSCustomClassHeader => $"{USSCustomClassName}__header";
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}__title";
    public virtual string USSCustomClassAuthor => $"{USSCustomClassName}__author";
    public virtual string USSCustomClassTimestamp => $"{USSCustomClassName}__timestamp";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}__main";
    public virtual string USSCustomClassMessage => $"{USSCustomClassName}__message";
    public virtual string USSCustomClassChoicesContainer => $"{USSCustomClassName}__choices-container";
    public virtual string USSCustomClassChoice => $"{USSCustomClassName}__choice";

    public virtual ElementCategory Category
    {
        get => m_category;
        set
        {
            RemoveFromClassList(USSCustomClassCategory(m_category));
            AddToClassList(USSCustomClassCategory(value));
            m_category = value;
            ChoiceA.Category = value;
            ChoiceB.Category = value;
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
            TitleLabel.TextStyle = TextStyle.Subtitle;
            TimestampLabel.TextStyle = TextStyle.Caption;
            AuthorLabel.TextStyle = TextStyle.Caption;
        }
    }
    public virtual NotificationType Type
    {
        get => m_type;
        set
        {
            RemoveFromClassList(USSCustomClassType(m_type));
            AddToClassList(USSCustomClassType(value));
            m_type = value;
            switch (value)
            {
                case NotificationType.Default:
                    ChoicesContainer.Insert(0, ChoiceA);
                    ChoiceB.RemoveFromHierarchy();
                    ChoiceA.Type = ButtonType.Default;
                    break;
                case NotificationType.Temporary:
                    ChoiceA.RemoveFromHierarchy();
                    ChoiceB.RemoveFromHierarchy();
                    break;
                case NotificationType.Confirmation:
                    ChoicesContainer.Insert(0, ChoiceA);
                    ChoiceA.Type = ButtonType.Primary;
                    ChoicesContainer.Insert(1, ChoiceB);
                    ChoiceB.Type = ButtonType.Danger;
                    break;
                default:
                    break;
            }
        }
    }
    public virtual string Title
    {
        get => TitleLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) TitleLabel.RemoveFromHierarchy();
            else
            {
                Header.Insert(0, TitleLabel);
                TitleLabel.text = value;
            }
        }
    }
    public virtual string Author
    {
        get => AuthorLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) AuthorLabel.RemoveFromHierarchy();
            else
            {
                Header.Insert(0, AuthorLabel);
                if (Header.Contains(TitleLabel)) AuthorLabel.PlaceInFront(TitleLabel);
                AuthorLabel.text = value;
            }
        }
    }
    public virtual string Timestamp
    {
        get => TimestampLabel.text;
        set
        {
            if (string.IsNullOrEmpty(value)) TimestampLabel.RemoveFromHierarchy();
            else
            {
                Header.Insert(0, TimestampLabel);
                if (Header.Contains(AuthorLabel)) TimestampLabel.PlaceInFront(AuthorLabel);
                else if (Header.Contains(TitleLabel)) TimestampLabel.PlaceInFront(TitleLabel);
                TimestampLabel.text = value;
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
                Main.Insert(0, MessageLabel);
                MessageLabel.text = value;
            }
        }
    }
    public virtual string ChoiceALabel
    {
        get => ChoiceA.text;
        set => ChoiceA.text = value;
    }
    public virtual string ChoiceBLabel
    {
        get => ChoiceB.text;
        set => ChoiceB.text = value;
    }
    public NotificationDto DTO;

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected NotificationType m_type;
    protected bool m_hasBeenInitialized;

    public VisualElement Header = new VisualElement { name = "header" };
    public CustomText TitleLabel;
    public CustomText AuthorLabel;
    public CustomText TimestampLabel;
    public VisualElement Main = new VisualElement { name = "main" };
    public CustomText MessageLabel;
    public VisualElement ChoicesContainer = new VisualElement { name = "choices-container" };
    /// <summary>
    /// Primary button.
    /// </summary>
    public CustomButton ChoiceA;
    /// <summary>
    /// Danger button.
    /// </summary>
    public CustomButton ChoiceB;

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, NotificationType.Default, null, null, null, null, null, null);
    public virtual void Set(ElementCategory category, ElementSize size, NotificationType type, string title, string author, string timestamp, string message, string choiceALabel, string choiceBLabel)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Category = category;
        Size = size;
        Type = type;
        Title = title;
        Author = author;
        Timestamp = timestamp;
        Message = message;
        ChoiceALabel = choiceALabel;
        ChoiceBLabel = choiceBLabel;
    }

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
        Header.AddToClassList(USSCustomClassHeader);
        TitleLabel.AddToClassList("text-low-title");
        TitleLabel.AddToClassList(USSCustomClassTitle);
        AuthorLabel.AddToClassList("text-body");
        AuthorLabel.AddToClassList(USSCustomClassAuthor);
        TimestampLabel.AddToClassList("text-body");
        TimestampLabel.AddToClassList(USSCustomClassTimestamp);
        Main.AddToClassList(USSCustomClassMain);
        MessageLabel.AddToClassList("text-body");
        MessageLabel.AddToClassList(USSCustomClassMessage);
        ChoicesContainer.AddToClassList(USSCustomClassChoicesContainer);
        ChoiceA.AddToClassList(USSCustomClassChoice);
        ChoiceB.AddToClassList(USSCustomClassChoice);
        ChoiceA.AddToClassList($"{USSCustomClassName}__choice-a");
        ChoiceB.AddToClassList($"{USSCustomClassName}__choice-b");

        TitleLabel.name = "title";
        AuthorLabel.name = "author";
        TimestampLabel.name = "timestamp";

        Add(Header);
        Add(Main);
        Add(ChoicesContainer);

        ChoiceA.Size = ElementSize.Small;
        ChoiceB.Size = ElementSize.Small;

        ChoiceA.clicked += () =>
        {
            Callback?.Invoke(0);
            this.RemoveFromHierarchy();
        };

        ChoiceB.clicked += () =>
        {
            Callback?.Invoke(1);
            this.RemoveFromHierarchy();
        };
    }

    public Action<int> Callback;
}
