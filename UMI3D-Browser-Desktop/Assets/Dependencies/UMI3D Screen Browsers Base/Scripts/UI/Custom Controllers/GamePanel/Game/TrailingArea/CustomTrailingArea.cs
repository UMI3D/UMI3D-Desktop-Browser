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
using umi3d.commonMobile.game;
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

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomTrailingArea;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_displayObjectMenu.GetValueFromBag(bag, cc),
                    m_displayEmoteWindow.GetValueFromBag(bag, cc)
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
        get => m_displayEmoteWindow;
        set
        {
            m_displayEmoteWindow = value;
            if (value)
            {
                this.AddIfNotInHierarchy(EmoteWindow);
                EmoteWindow.style.visibility = Visibility.Hidden;
            }
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
                    revert: !m_displayEmoteWindow,
                    callback: m_displayEmoteWindow ? null : EmoteWindow.RemoveFromHierarchy
                );
            });
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/trailingArea";
    public virtual string USSCustomClassName => "trailing-area";
    public virtual string USSCustomClassObjectMenu => $"{USSCustomClassName}__object-menu";
    public virtual string USSCustomClassEmoteWindow => $"{USSCustomClassName}__emote-window";
    public virtual string USSCustomClassCameraLayer => $"{USSCustomClassName}__camera-layer";

    public CustomForm ObjectMenu;
    public CustomEmoteWindow EmoteWindow;
    public CustomButtonsArea ButtonsArea;
    public VisualElement CameraLayer = new VisualElement { name = "camera-layer" };

    public TouchManipulator2 m_touchManipulator = new TouchManipulator2(null, 0, 0);
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

    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;
    protected bool m_displayObjectMenu;
    protected bool m_displayEmoteWindow;
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
        AddToClassList(USSCustomClassName);
        ObjectMenu.AddToClassList(USSCustomClassObjectMenu);
        EmoteWindow.AddToClassList(USSCustomClassEmoteWindow);
        CameraLayer.AddToClassList(USSCustomClassCameraLayer);

        CameraLayer.AddManipulator(m_touchManipulator);
        m_touchManipulator.ClickedDownWithInfo += (evt, localposition) => m_initialDownPosition = localposition;
        m_touchManipulator.MovedWithInfo += (evt, localposition) =>
        {
            m_localPosition = localposition;
            m_direction = localposition - m_initialDownPosition;
            m_direction.x /= worldBound.width;
            m_direction.y /= -worldBound.height;
            m_direction *= 50;
            m_cameraMoved = true;
        };
        m_touchManipulator.ClickedUpWithInfo += (evt, localposition) =>
        {

        };

        ButtonsArea.ClickedDown = (evt, worldPosition) => m_touchManipulator.OnClickedDownWithInf(evt, CameraLayer.WorldToLocal(worldPosition));
        ButtonsArea.Moved = (evt, worldPosition) => m_touchManipulator.OnMovedWithInf(evt, CameraLayer.WorldToLocal(worldPosition));

        ObjectMenu.name = "object-menu";
        ObjectMenu.Category = ElementCategory.Game;
        ObjectMenu.Title = "Object Menu";
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, false, false);

    public virtual void Set(ControllerEnum controller, bool displayObjectMenu, bool displayEmoteWindow)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        DisplayObjectMenu = displayObjectMenu;
        DisplayEmoteWindow = displayEmoteWindow;
    }
}
