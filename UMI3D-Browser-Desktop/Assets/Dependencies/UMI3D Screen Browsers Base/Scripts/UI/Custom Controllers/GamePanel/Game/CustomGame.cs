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
using umi3d.baseBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.InputSystem;
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

        protected UxmlBoolAttributeDescription m_displayNotifUserArea = new UxmlBoolAttributeDescription
        {
            name = "display-notif-users-area",
            defaultValue = false
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomGame;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_displayNotifUserArea.GetValueFromBag(bag, cc),
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
            NotifAndUserArea.Controller = value;
            if (value == ControllerEnum.MouseAndKeyboard) Add(BottomArea);
            else BottomArea.RemoveFromHierarchy();
        }
    }

    public virtual bool DisplayNotifUsersArea
    {
        get => S_displayNotifUserArea;
        set
        {
            if (S_displayNotifUserArea == value) return;
            S_displayNotifUserArea = value;
            switch (m_controller)
            {
                case ControllerEnum.MouseAndKeyboard:
                    DisplayMouseAndKeyboardNotifAndUsers(value);
                    break;
                case ControllerEnum.Touch:
                    DisplayTouchNotifAndUsers(value);
                    break;
                case ControllerEnum.GameController:
                    break;
                default:
                    break;
            }
        }
    }

    public virtual bool DisplayEmoteWindow
    {
        get => S_displayEmoteWindow;
        set
        {
            if (S_displayEmoteWindow == value) return;
            S_displayEmoteWindow = value;
            switch (m_controller)
            {
                case ControllerEnum.MouseAndKeyboard:
                    DisplayMouseAndKeyboardEmoteWindow(value);
                    break;
                case ControllerEnum.Touch:
                    DisplayTouchEmoteWindow(value);
                    break;
                case ControllerEnum.GameController:
                    break;
                default:
                    break;
            }
        }
    }

    public static bool LeftHand
    {
        get => m_leftHand;
        set
        {
            m_leftHand = value;
            LeftHandModeUpdated?.Invoke(value);
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/game";
    public virtual string USSCustomClassName => "game";
    public virtual string USSCustomClassLeadingAndTrailingBox => "game-leading__trailing__box";

    /// <summary>
    /// Event raised if the if the user clicke on the trailing and leading area but not in the objectMenu.
    /// Take in parameter the world position of the click.
    /// </summary>
    public event System.Action<Vector2> LeadingAndTrailingAreaClicked;

    public CustomCursor Cursor;
    public CustomTopArea TopArea;
    public CustomLeadingArea LeadingArea;
    public CustomTrailingArea TrailingArea;
    public CustomBottomArea BottomArea;
    public VisualElement LeadingAndTrailingBox = new VisualElement { name = "leading-trailing-area" };

    public CustomNotifAndUsersArea NotifAndUserArea;

    public static bool S_displayNotifUserArea;
    public static bool S_displayEmoteWindow;

    public TouchManipulator2 TouchManipulator = new TouchManipulator2(null, 0, 0);

    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;
    public static System.Action<bool> LeftHandModeUpdated;
    protected static bool m_leftHand;

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
        LeadingAndTrailingBox.AddToClassList(USSCustomClassLeadingAndTrailingBox);

        TopArea.InformationArea.NotificationTitleClicked += () =>
        {
            DisplayNotifUsersArea = true;
            NotifAndUserArea.AreaPanel = CustomNotifAndUsersArea.NotificationsOrUsers.Notifications;
            NotifAndUserArea.notificationCenter.Filter = NotificationFilter.New;
        };
        BottomArea.NotifUsersValueChanged = value => DisplayNotifUsersArea = value;
        BottomArea.EmoteClicked += () => DisplayEmoteWindow = !DisplayEmoteWindow;
        TopArea.InformationArea.ExpandUpdate += value => DisplayNotifUsersArea = value;
        TrailingArea.ButtonsArea.Emote.clicked += () => DisplayEmoteWindow = !DisplayEmoteWindow;

        LeftHandModeUpdated = (value) =>
        {
            LeadingArea.LeftHand = value;
            TrailingArea.LeftHand = value;
        };
        LeftHandModeUpdated(m_leftHand);

        this.AddManipulator(TouchManipulator);
        TouchManipulator.ClickedDownWithInfo += (evnt, localPosition) =>
        {
            var worldPosition = this.LocalToWorld(localPosition);

            //if (IsLeadingAndtrailingClicked(worldPosition)) LeadingAndTrailingAreaClicked?.Invoke(worldPosition);
        };

        TrailingArea.ToolsWindow.Pinned = (isPinned, menu) =>
        {
            if (isPinned) LeadingArea.PinnedToolsArea.AddMenu(menu);
            else LeadingArea.PinnedToolsArea.RemoveMenu(menu);
        };

        Add(Cursor);
        Add(TopArea);
        Add(LeadingAndTrailingBox);
        LeadingAndTrailingBox.Add(LeadingArea);
        LeadingAndTrailingBox.Add(TrailingArea);
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, S_displayNotifUserArea, m_leftHand);

    public virtual void Set(ControllerEnum controller, bool displayNotifUserArea, bool leftHand)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        DisplayNotifUsersArea = displayNotifUserArea;
        LeftHand = leftHand;
    }

    public bool IsLeadingAndtrailingClicked(Vector2 worldPosition)
    {
        var leadingAndTrailingLocal = LeadingAndTrailingBox.WorldToLocal(worldPosition);
        if (!LeadingAndTrailingBox.ContainsPoint(leadingAndTrailingLocal)) return false;

        var objectMenuLocal = TrailingArea.ObjectMenu.WorldToLocal(worldPosition);
        if (TrailingArea.ObjectMenu.ContainsPoint(objectMenuLocal)) return false;
        
        return true;
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

    protected void DisplayMouseAndKeyboardNotifAndUsers(bool value)
    {
        BottomArea.DisplayNotifUsersArea = value;
        TrailingArea.DisplayNotifUsersArea = value;
    }

    protected void DisplayTouchNotifAndUsers(bool value)
    {
        if (value)
        {
            this.AddIfNotInHierarchy(NotifAndUserArea);
            NotifAndUserArea.style.visibility = Visibility.Hidden;
            NotifAndUserArea.notificationCenter.UpdateFilter();
            NotifAndUserArea.style.width = StyleKeyword.Null;
        }
        else NotifAndUserArea.notificationCenter.ResetNewNotificationFilter();

        if
        (
            !value
            && NotifAndUserArea.FindRoot() == null
        ) return;

        NotifAndUserArea.schedule.Execute(() =>
        {
            NotifAndUserArea.style.visibility = StyleKeyword.Null;
            NotifAndUserArea.AddAnimation
            (
                this,
                () => NotifAndUserArea.style.opacity = 0f,
                () => NotifAndUserArea.style.opacity = 1f,
                "opacity",
                0.5f,
                revert: !value,
                callback: value ? null : NotifAndUserArea.RemoveFromHierarchy
            );
        });
    }

    protected void DisplayMouseAndKeyboardEmoteWindow(bool value)
    {
        BottomArea.ButtonSelected = value ? CustomBottomArea.BottomBarButton.Emote : CustomBottomArea.BottomBarButton.None;
    }

    protected void DisplayTouchEmoteWindow(bool value)
    {
        TrailingArea.DisplayEmoteWindow = value;
    }
}
