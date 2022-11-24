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
using UnityEngine.UIElements;

public abstract class CustomDialoguebox : VisualElement, ICustomElement
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
        UxmlEnumAttributeDescription<DialogueboxType> m_type = new UxmlEnumAttributeDescription<DialogueboxType> 
        { 
            name = "type", 
            defaultValue = DialogueboxType.Default 
        };
        UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription 
        { 
            name = "title", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_message = new UxmlStringAttributeDescription 
        { 
            name = "message", 
            defaultValue = null 
        };
        UxmlStringAttributeDescription m_choiceA = new UxmlStringAttributeDescription
        {
            name = "choice-a-text",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_choiceB = new UxmlStringAttributeDescription
        {
            name = "choice-b-text",
            defaultValue = null
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomDialoguebox;

            custom.Set
                (
                    m_category.GetValueFromBag(bag, cc),
                    m_size.GetValueFromBag(bag, cc),
                    m_type.GetValueFromBag(bag, cc),
                    m_title.GetValueFromBag(bag, cc),
                    m_message.GetValueFromBag(bag, cc),
                    m_choiceA.GetValueFromBag(bag, cc),
                    m_choiceB.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/dialogueBox";
    public virtual string USSCustomClassName => "dialoguebox";
    public virtual string USSCustomClassCategory(ElementCategory category) => $"{USSCustomClassName}-{category}".ToLower();
    public virtual string USSCustomClassSize(ElementSize size) => $"{USSCustomClassName}-{size}".ToLower();
    public virtual string USSCustomClassType(DialogueboxType type) => $"{USSCustomClassName}-{type}".ToLower();

    public virtual string USSCustomClassBody => $"{USSCustomClassName}__body".ToLower();
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}__title".ToLower();
    public virtual string USSCustomClassMain => $"{USSCustomClassName}__main".ToLower();
    public virtual string USSCustomClassMessage => $"{USSCustomClassName}__message".ToLower();
    public virtual string USSCustomClassContainer => $"{USSCustomClassName}__container".ToLower();
    public virtual string USSCustomClassChoiceContainer => $"{USSCustomClassName}__choices-container".ToLower();
    public virtual string USSCustomClassChoice => $"{USSCustomClassName}__choice".ToLower();
    public virtual string USSCustomClassChoiceA => $"{USSCustomClassName}__choice-a".ToLower();
    public virtual string USSCustomClassChoiceB => $"{USSCustomClassName}__choice-b".ToLower();

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
            Container.Category = value;
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
            ChoiceA.Size = value;
            ChoiceB.Size = value;
            switch (value)
            {
                case ElementSize.Small:
                    TitleLabel.TextStyle = TextStyle.Subtitle;
                    MessageLabel.TextStyle = TextStyle.Caption;
                    break;
                case ElementSize.Medium:
                    TitleLabel.TextStyle = TextStyle.LowTitle;
                    MessageLabel.TextStyle = TextStyle.Body;
                    break;
                case ElementSize.Large:
                    TitleLabel.TextStyle = TextStyle.Title;
                    MessageLabel.TextStyle = TextStyle.Body;
                    break;
                default:
                    break;
            }
        }
    }
    public virtual DialogueboxType Type
    {
        get => m_type;
        set
        {
            RemoveFromClassList(USSCustomClassType(m_type));
            AddToClassList(USSCustomClassType(value));
            m_type = value;
            switch (value)
            {
                case DialogueboxType.Default:
                    ChoicesContainer.Insert(0, ChoiceA);
                    ChoiceB.RemoveFromHierarchy();
                    ChoiceA.Type = ButtonType.Default;
                    break;
                case DialogueboxType.Confirmation:
                    ChoicesContainer.Insert(0, ChoiceA);
                    ChoiceA.Type = ButtonType.Primary;
                    ChoicesContainer.Insert(1, ChoiceB);
                    ChoiceB.Type = ButtonType.Danger;
                    break;
                case DialogueboxType.Error:
                    ChoicesContainer.Insert(0, ChoiceA);
                    ChoicesContainer.Insert(1, ChoiceB);
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
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) TitleLabel.RemoveFromHierarchy();
            else
            {
                Body.Insert(0, TitleLabel);
                TitleLabel.text = value;
            }
            m_isSet = true;
        }
    }
    public virtual string Message
    {
        get => MessageLabel.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) MessageLabel.RemoveFromHierarchy();
            else
            {
                Container.Insert(0, MessageLabel);
                MessageLabel.text = value;
            }
            m_isSet = true;
        }
    }
    public virtual string ChoiceAText
    {
        get => ChoiceA.text;
        set => ChoiceA.text = value;
    }
    public virtual string ChoiceBText
    {
        get => ChoiceB.text;
        set => ChoiceB.text = value;
    }

    protected ElementCategory m_category;
    protected ElementSize m_size;
    protected DialogueboxType m_type;
    protected bool m_hasBeenInitialised;
    protected bool m_isSet;

    public VisualElement Body = new VisualElement { name = "body" };
    public CustomText TitleLabel;
    public VisualElement Main = new VisualElement { name = "main" };
    public CustomText MessageLabel;
    public CustomScrollView Container;
    public VisualElement ChoicesContainer = new VisualElement { name = "choice-container" };
    public CustomButton ChoiceA;
    public CustomButton ChoiceB;

    public virtual void Set() => Set(ElementCategory.Menu, ElementSize.Medium, DialogueboxType.Default, null, null, null, null);
    public virtual void Set(ElementCategory category, ElementSize size, DialogueboxType type, string title, string message, string choiceA, string choiceB)
    {
        m_isSet = false;
        if (!m_hasBeenInitialised)
        {
            InitElement();
            m_hasBeenInitialised = true;
        }

        Category = category;
        Size = size;
        Type = type;
        Title = title;
        Message = message;
        ChoiceAText = choiceA;
        ChoiceBText = choiceB;
        m_isSet = true;
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
        Body.AddToClassList(USSCustomClassBody);
        TitleLabel.AddToClassList(USSCustomClassTitle);
        Main.AddToClassList(USSCustomClassMain);
        MessageLabel.AddToClassList(USSCustomClassMessage);
        Container.AddToClassList(USSCustomClassContainer);
        ChoicesContainer.AddToClassList(USSCustomClassChoiceContainer);
        ChoiceA.AddToClassList(USSCustomClassChoice);
        ChoiceB.AddToClassList(USSCustomClassChoice);
        ChoiceA.AddToClassList(USSCustomClassChoiceA);
        ChoiceB.AddToClassList(USSCustomClassChoiceB);

        TitleLabel.name = "title";
        MessageLabel.name = "message";
        ChoiceA.name = "choice-a";
        ChoiceB.name = "choice-b";

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

        Add(Body);
        Body.Add(Main);
        Main.Add(Container);
        Body.Add(ChoicesContainer);
    }

    public virtual void AddToTheRoot(VisualElement element)
    {
        var root = element.FindRoot();
        root.Add(this);
    }

    public Action<int> Callback;

    public override VisualElement contentContainer => m_isSet ? Container.contentContainer : this;
}
