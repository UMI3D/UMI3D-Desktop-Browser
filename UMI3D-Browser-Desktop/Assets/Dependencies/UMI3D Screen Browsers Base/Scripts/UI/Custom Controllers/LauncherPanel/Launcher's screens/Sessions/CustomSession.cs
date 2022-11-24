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

public abstract class CustomSession : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_title = new UxmlStringAttributeDescription
        {
            name = "title",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_participantCount = new UxmlStringAttributeDescription
        {
            name = "participants-count",
            defaultValue = null
        };
        UxmlStringAttributeDescription m_maxParticipant = new UxmlStringAttributeDescription
        {
            name = "max-participants",
            defaultValue = null
        };
        UxmlBoolAttributeDescription m_selected = new UxmlBoolAttributeDescription
        {
            name = "is-selected",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomSession;

            custom.Set
                (
                    m_title.GetValueFromBag(bag, cc),
                    m_participantCount.GetValueFromBag(bag, cc),
                    m_maxParticipant.GetValueFromBag(bag, cc),
                    m_selected.GetValueFromBag(bag, cc)
                 );
        }
    }

    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/session";
    public virtual string USSCustomClassName => "session";
    public virtual string USSCustomClassBackground => $"{USSCustomClassName}__background-button";
    public virtual string USSCustomClassTitle => $"{USSCustomClassName}__title";
    public virtual string USSCustomClassCount => $"{USSCustomClassName}__count";
    public virtual string USSCustomClassSelected => $"{USSCustomClassName}__selected";

    public event System.Action Clicked;
    public CustomButton Background__Button;
    public CustomText SessionTitle;
    public CustomText Count;

    protected bool m_hasBeenInitialized;
    protected string m_participantsCount;
    protected string m_maxParticipants;
    protected bool m_isSelected;

    public virtual string Title
    {
        get => SessionTitle.text;
        set
        {
            if (string.IsNullOrEmpty(value)) SessionTitle.RemoveFromHierarchy();
            else
            {
                Background__Button.Insert(0, SessionTitle);
                SessionTitle.text = value;
            }
        }
    }
    public virtual string ParticipantsCount
    {
        get => m_participantsCount;
        set
        {
            m_participantsCount = value;
            Count.text = $"{m_participantsCount}/{m_maxParticipants}";
        }
    }
    public virtual string MaxParticipants
    {
        get => m_maxParticipants;
        set
        {
            m_maxParticipants = value;
            Count.text = $"{m_participantsCount}/{m_maxParticipants}";
        }
    }
    public virtual bool IsSelected
    {
        get => m_isSelected;
        set
        {
            if (value != m_isSelected)
            {
                Clicked?.Invoke();
                m_isSelected = value;
            }
            if (value) AddToClassList(USSCustomClassSelected);
            else RemoveFromClassList(USSCustomClassSelected);
        }
    }

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        Background__Button.AddToClassList(USSCustomClassBackground);
        SessionTitle.AddToClassList(USSCustomClassTitle);
        Count.AddToClassList(USSCustomClassCount);

        Add(Background__Button);
        Background__Button.Add(SessionTitle);
        Background__Button.Add(Count);

        Background__Button.Type = ButtonType.Invisible;

        Background__Button.clicked += () => m_isSelected = !m_isSelected;
    }

    public virtual void Set() => Set(null, null, null, false);

    public virtual void Set(string title, string participantsCount, string maxParticipants, bool selected)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Title = title;
        ParticipantsCount = participantsCount;
        MaxParticipants = maxParticipants;
        IsSelected = selected;
    }
}
