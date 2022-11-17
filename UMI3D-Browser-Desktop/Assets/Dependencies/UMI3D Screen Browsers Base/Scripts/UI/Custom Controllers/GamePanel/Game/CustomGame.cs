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

public class CustomGame : VisualElement, ICustomElement, IGameView
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller",
            defaultValue = ControllerEnum.MouseAndKeyboard
        };

        protected UxmlBoolAttributeDescription m_leftHand = new UxmlBoolAttributeDescription
        {
            name = "left-hand",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomGame;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_leftHand.GetValueFromBag(bag, cc)
                );
        }
    }

    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            m_controller = value;
            TopArea.Controller = value;
            LeadingArea.Controller = value;
            TrailingArea.Controller = value;
            if (value == ControllerEnum.MouseAndKeyboard) Add(BottomArea);
            else BottomArea.RemoveFromHierarchy();
        }
    }

    public bool LeftHand
    {
        get => m_leftHand;
        set
        {
            m_leftHand = value;
            LeadingArea.LeftHand = value;
            TrailingArea.LeftHand = value;
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/game";
    public virtual string USSCustomClassName => "game";

    public System.Action<object> SetMovement;
    public System.Action<object> UnSetMovement;
    public CustomCursor Cursor;
    public CustomLeadingArea LeadingArea;
    public CustomTrailingArea TrailingArea;
    public CustomTopArea TopArea;
    public CustomBottomArea BottomArea;

    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;
    public static System.Action LeftHandModeUpdated;
    protected bool m_leftHand;

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

        TopArea.InformationArea.NotificationTitleClicked += () =>
        {
            if (!BottomArea.DisplayNotifUsersArea) BottomArea.DisplayNotifUsersArea = true;
            TrailingArea.NotifAndUserArea.AreaPanel = CustomNotifAndUsersArea.NotificationsOrUsers.Notifications;
            TrailingArea.NotifAndUserArea.notificationCenter.Filter = NotificationFilter.New;
        };
        BottomArea.NotifUsersValueChanged = value => TrailingArea.DisplayNotifUsersArea = value;

        Add(Cursor);
        Add(LeadingArea);
        Add(TrailingArea);
        Add(TopArea);

        LeftHandModeUpdated = () => LeftHand = !LeftHand;
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, false);

    public virtual void Set(ControllerEnum controller, bool leftHand)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        LeftHand = leftHand;
    }

    public void TransitionIn(VisualElement persistentVisual)
        => Transition(persistentVisual, false);

    public void TransitionOut(VisualElement persistentVisual)
        => Transition(persistentVisual, true);

    protected virtual void Transition(VisualElement persistentVisual, bool revert)
    {
        this.AddAnimation
        (
            persistentVisual,
            () => style.opacity = 0,
            () => style.opacity = 1,
            "opacity",
            0.5f,
            revert: revert,
            callback: revert ? RemoveFromHierarchy : null
        );
    }
}
