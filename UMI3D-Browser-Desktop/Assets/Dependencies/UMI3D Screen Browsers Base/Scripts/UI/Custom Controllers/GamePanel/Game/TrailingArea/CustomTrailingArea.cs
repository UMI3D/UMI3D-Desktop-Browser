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
using umi3d.cdk.menu;
using umi3d.commonMobile.game;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomTrailingArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller",
            defaultValue = ControllerEnum.MouseAndKeyboard
        };
        protected UxmlBoolAttributeDescription m_displayNotifAndUsersArea = new UxmlBoolAttributeDescription
        {
            name = "display-notif-users-area",
            defaultValue = false
        };
        protected UxmlBoolAttributeDescription m_displayObjectMenu = new UxmlBoolAttributeDescription
        {
            name = "display-object-menu",
            defaultValue = false
        };
        protected UxmlBoolAttributeDescription m_displayEmoteWindow = new UxmlBoolAttributeDescription
        {
            name = "display-emote-window",
            defaultValue = false
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
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomTrailingArea;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_displayNotifAndUsersArea.GetValueFromBag(bag, cc),
                    m_displayObjectMenu.GetValueFromBag(bag, cc),
                    m_displayEmoteWindow.GetValueFromBag(bag, cc),
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
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    CameraLayer.RemoveFromHierarchy();
                    ButtonsArea.RemoveFromHierarchy();
                    break;
                case ControllerEnum.Touch:
                    Add(CameraLayer);
                    Add(ButtonsArea);
                    break;
                case ControllerEnum.GameController:
                    CameraLayer.RemoveFromHierarchy();
                    ButtonsArea.RemoveFromHierarchy();
                    break;
                default:
                    break;
            }
        }
    }

    public virtual bool DisplayNotifUsersArea
    {
        get
        {
            if (Application.isPlaying) return CustomGame.S_displayNotifUserArea;
            else return m_displayNotifAndUserArea;
        }
        set
        {
            if (!Application.isPlaying) m_displayNotifAndUserArea = value;
            if (value)
            {
                this.AddIfNotInHierarchy(NotifAndUserArea);
                NotifAndUserArea.style.visibility = Visibility.Hidden;
                NotifAndUserArea.notificationCenter.UpdateFilter();
                NotifAndUserArea.style.opacity = StyleKeyword.Null;
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
                    () => NotifAndUserArea.style.width = Length.Percent(0),
                    () => NotifAndUserArea.style.width = Length.Percent(60),
                    "width",
                    0.5f,
                    revert: !value,
                    callback: value ? null : NotifAndUserArea.RemoveFromHierarchy
                );
            });
        }
    }

    public virtual bool DisplayObjectMenu
    {
        get => m_displayObjectMenu;
        set
        {
            m_displayObjectMenu = value;
            if (value)
            {
                this.AddIfNotInHierarchy(ObjectMenu);
                ObjectMenu.style.visibility = Visibility.Hidden;
            }

            if
            (
                !value
                && ObjectMenu.FindRoot() == null
            ) return;

            ObjectMenu.schedule.Execute(() =>
            {
                ObjectMenu.style.visibility = StyleKeyword.Null;
                ObjectMenu.AddAnimation
                (
                    this,
                    () => ObjectMenu.style.width = Length.Percent(0),
                    () => ObjectMenu.style.width = Length.Percent(70),
                    "width",
                    0.5f,
                    revert: !m_displayObjectMenu,
                    callback: m_displayObjectMenu ? null : ObjectMenu.RemoveFromHierarchy
                );
            });
        }
    }

    public virtual bool DisplayEmoteWindow
    {
        get
        {
            if (Application.isPlaying) return CustomGame.S_displayEmoteWindow;
            else return m_displayEmoteWindow;
        }
        set
        {
            if (!Application.isPlaying) m_displayEmoteWindow = value;
            if (value)
            {
                this.AddIfNotInHierarchy(EmoteWindow);
                EmoteWindow.style.visibility = Visibility.Hidden;
                EmoteWindow.UpdateFilter();
            }

            if
            (
                !value
                && EmoteWindow.FindRoot() == null
            ) return;

            EmoteWindow.schedule.Execute(() =>
            {
                EmoteWindow.style.visibility = StyleKeyword.Null;
                EmoteWindow.AddAnimation
                (
                    this,
                    () => EmoteWindow.style.width = Length.Percent(0),
                    () => EmoteWindow.style.width = Length.Percent(70),
                    "width",
                    0.5f,
                    revert: !value,
                    callback: value ? null : EmoteWindow.RemoveFromHierarchy
                );
            });
        }
    }

    public bool LeftHand
    {
        get => m_leftHand;
        set
        {
            m_leftHand = value;
            ButtonsArea.LeftHand = value;
            if (value)
            {
                RemoveFromClassList(USSCustomClassName);
                AddToClassList(USSCustomClassNameReverse);
            }
            else
            {
                RemoveFromClassList(USSCustomClassNameReverse);
                AddToClassList(USSCustomClassName);
            }
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/trailingArea";
    public virtual string USSCustomClassName => "trailing__area";
    public virtual string USSCustomClassNameReverse => "trailing__area-reverse";
    public virtual string USSCustomClassObjectMenu => $"{USSCustomClassName}-object__menu";
    public virtual string USSCustomClassEmoteWindow => $"{USSCustomClassName}-emote__window";
    public virtual string USSCustomClassToolsWindow => $"{USSCustomClassName}-tools__window";
    public virtual string USSCustomClassCameraLayer => $"{USSCustomClassName}-camera__layer";

    public ToolsWindow_C ToolsWindow = new ToolsWindow_C { name = "tools-window" };
    public CustomNotifAndUsersArea NotifAndUserArea;
    public CustomForm ObjectMenu;
    public CustomEmoteWindow EmoteWindow;
    public CustomButtonsArea ButtonsArea;
    public VisualElement CameraLayer = new VisualElement { name = "camera-layer" };
    public MenuAsset GlobalToolsMenu;

    public TouchManipulator2 TrailingAreaManipulator = new TouchManipulator2(null, 0, 0);
    public TouchManipulator2 CameraManipulator = new TouchManipulator2(null, 0, 0);
    /// <summary>
    /// Direction of the swipe.
    /// </summary>
    public Vector2 Direction
    {
        get
        {
            if (!m_cameraMoved) return Vector2.zero;
            m_cameraMoved = false;
            m_initialDownPosition = m_localPosition;
            return m_direction;
        }
    }

    public static System.Action LeftHandModeUpdated;
    protected bool m_leftHand;
    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;
    protected bool m_displayObjectMenu;
    protected bool m_displayEmoteWindow;
    protected bool m_displayNotifAndUserArea;
    protected Vector2 m_initialDownPosition;
    protected Vector2 m_localPosition;
    protected bool m_cameraMoved;
    protected Vector2 m_direction;

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

        ObjectMenu.AddToClassList(USSCustomClassObjectMenu);
        EmoteWindow.AddToClassList(USSCustomClassEmoteWindow);
        ToolsWindow.AddToClassList(USSCustomClassToolsWindow);
        CameraLayer.AddToClassList(USSCustomClassCameraLayer);

        //this.AddManipulator(TrailingAreaManipulator);
        //TODO improve camera navigation with double click.

        CameraLayer.AddManipulator(CameraManipulator);
        CameraManipulator.ClickedDownWithInfo += (evt, localposition) => m_initialDownPosition = localposition;
        CameraManipulator.MovedWithInfo += (evt, localposition) =>
        {
            m_localPosition = localposition;
            m_direction = localposition - m_initialDownPosition;
            m_direction.x /= worldBound.width;
            m_direction.y /= -worldBound.height;
            m_direction *= 50;
            m_cameraMoved = true;
        };

        ButtonsArea.ClickedDown = (evt, worldPosition) => CameraManipulator.OnClickedDownWithInf(evt, CameraLayer.WorldToLocal(worldPosition));
        ButtonsArea.Moved = (evt, worldPosition) => CameraManipulator.OnMovedWithInf(evt, CameraLayer.WorldToLocal(worldPosition));

        ObjectMenu.name = "object-menu";
        ObjectMenu.Category = ElementCategory.Game;
        ObjectMenu.Title = "Contextual Menu";

        GlobalToolsMenu = Resources.Load<MenuAsset>("Scriptables/GamePanel/GlobalToolsMenu");
        ToolsWindow.Title = "Toolbox";
        ToolsWindow.Category = ElementCategory.Game;
        ToolsWindow.AddRoot(GlobalToolsMenu.menu);
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, false, false, false, m_leftHand);

    public virtual void Set(ControllerEnum controller, bool displayNotifUsersArea, bool displayObjectMenu, bool displayEmoteWindow, bool leftHand)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        DisplayNotifUsersArea = displayNotifUsersArea;
        DisplayObjectMenu = displayObjectMenu;
        DisplayEmoteWindow = displayEmoteWindow;
        LeftHand = leftHand;
    }
}
