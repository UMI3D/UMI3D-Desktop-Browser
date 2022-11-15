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

public class CustomButtonsArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlBoolAttributeDescription m_displayActionButton = new UxmlBoolAttributeDescription
        {
            name = "is-action-button-displayed",
            defaultValue = false
        };
        protected UxmlBoolAttributeDescription m_displayEmoteButton = new UxmlBoolAttributeDescription
        {
            name = "is-emote-button-displayed",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomButtonsArea;

            custom.Set
                (
                    m_displayActionButton.GetValueFromBag(bag, cc),
                    m_displayEmoteButton.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual bool IsActionButtonDisplayed
    {
        get => m_isActionButtonDisplayed;
        set
        {
            m_isActionButtonDisplayed = value;
            if (value) Add(Action);
            else Action.RemoveFromHierarchy();
        }
    }

    public virtual bool IsEmoteButtonDisplayed
    {
        get => m_isEmoteButtonDisplayed;
        set
        {
            m_isEmoteButtonDisplayed = value;
            if (value) Add(Emote);
            else Emote.RemoveFromHierarchy();
        }
    }

    public bool LeftHand
    {
        get => m_leftHand;
        set
        {
            m_leftHand = value;
            if (value)
            {
            Crouch.RemoveFromClassList(USSCustomClassCrouch);
            Crouch.AddToClassList(USSCustomClassCrouchReverse);
            Jump.RemoveFromClassList(USSCustomClassjump);
            Jump.AddToClassList(USSCustomClassjumpReverse);
            Action.RemoveFromClassList(USSCustomClassAction);
            Action.AddToClassList(USSCustomClassActionReverse);
            Emote.RemoveFromClassList(USSCustomClassEmote);
            Emote.AddToClassList(USSCustomClassEmoteReverse);
        }
        else
        {
            Crouch.RemoveFromClassList(USSCustomClassCrouchReverse);
            Crouch.AddToClassList(USSCustomClassCrouch);
            Jump.RemoveFromClassList(USSCustomClassjumpReverse);
            Jump.AddToClassList(USSCustomClassjump);
            Action.RemoveFromClassList(USSCustomClassActionReverse);
            Action.AddToClassList(USSCustomClassAction);
            Emote.RemoveFromClassList(USSCustomClassEmoteReverse);
            Emote.AddToClassList(USSCustomClassEmote);
        }
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/buttonsArea";
    public virtual string USSCustomClassName => "buttons-area";
    public virtual string USSCustomClassCrouch => $"{USSCustomClassName}__crouch";
    public virtual string USSCustomClassjump => $"{USSCustomClassName}__jump";
    public virtual string USSCustomClassAction => $"{USSCustomClassName}__action";
    public virtual string USSCustomClassEmote => $"{USSCustomClassName}__emote";
    public virtual string USSCustomClassCrouchReverse => $"{USSCustomClassName}__crouch_reverse";
    public virtual string USSCustomClassjumpReverse => $"{USSCustomClassName}__jump_reverse";
    public virtual string USSCustomClassActionReverse => $"{USSCustomClassName}__action_reverse";
    public virtual string USSCustomClassEmoteReverse => $"{USSCustomClassName}__emote_reverse";

    public System.Action MainActionDown;
    public System.Action MainActionUp;
    public CustomButton Crouch;
    public CustomButton Jump;
    public CustomButton Action;
    public CustomButton Emote;

    public System.Action<EventBase, Vector2> ClickedDown;
    public System.Action<EventBase, Vector2> Moved;

    protected bool m_leftHand;

    protected bool m_isActionButtonDisplayed;
    protected bool m_isEmoteButtonDisplayed;
    protected bool m_hasBeenInitialized;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        Crouch.name = "crouch";
        Jump.name = "jump";
        Action.name = "action";
        Emote.name = "emote";

        Crouch.text = "Crouch";
        Jump.text = "Jump";
        Action.text = "Action";
        Emote.text = "Emote";

        Crouch.Category = ElementCategory.Game;
        Jump.Category = ElementCategory.Game;
        Action.Category = ElementCategory.Game;
        Emote.Category = ElementCategory.Game;

        Crouch.Size = ElementSize.Custom;
        Jump.Size = ElementSize.Custom;
        Action.Size = ElementSize.Custom;
        Emote.Size = ElementSize.Custom;

        Crouch.Shape = ButtonShape.Round;
        Jump.Shape = ButtonShape.Round;
        Action.Shape = ButtonShape.Round;
        Emote.Shape = ButtonShape.Round;

        Action.ClickedDown += () => MainActionDown?.Invoke();
        Action.ClickedUp += () => MainActionUp?.Invoke();

        Crouch.ClickedDownWithInfo += (evt, localposition) => ClickedDown?.Invoke(evt, Crouch.LocalToWorld(localposition));
        Jump.ClickedDownWithInfo += (evt, localposition) => ClickedDown?.Invoke(evt, Jump.LocalToWorld(localposition));
        Action.ClickedDownWithInfo += (evt, localposition) => ClickedDown?.Invoke(evt, Action.LocalToWorld(localposition));

        Crouch.MovedWithInfo += (evt, localposition) => Moved?.Invoke(evt, Crouch.LocalToWorld(localposition));
        Jump.MovedWithInfo += (evt, localposition) => Moved?.Invoke(evt, Jump.LocalToWorld(localposition));
        Action.MovedWithInfo += (evt, localposition) => Moved?.Invoke(evt, Action.LocalToWorld(localposition));

        Add(Crouch);
        Add(Jump);
    }

    public virtual void Set() => Set(false, false);

    public virtual void Set(bool isActionButtonDisplayed, bool isEmoteButtonDisplayed)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        IsActionButtonDisplayed = isActionButtonDisplayed;
        IsEmoteButtonDisplayed = isEmoteButtonDisplayed;
    }
}
