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

public class CustomTrailingArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
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
                    m_displayObjectMenu.GetValueFromBag(bag, cc),
                    m_displayEmoteWindow.GetValueFromBag(bag, cc)
                );
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

    public umi3d.baseBrowser.ui.viewController.CameraManipulator camTouchManipulator;

    protected bool m_hasBeenInitialized;
    protected bool m_displayObjectMenu;
    protected bool m_displayEmoteWindow;

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

        camTouchManipulator = new umi3d.baseBrowser.ui.viewController.CameraManipulator(this);
        CameraLayer.AddManipulator(camTouchManipulator);

        ObjectMenu.name = "object-menu";
        ObjectMenu.Category = ElementCategory.Game;
        ObjectMenu.Title = "Object Menu";

        Add(CameraLayer);
        Add(ButtonsArea);
    }

    public virtual void Set() => Set(false, false);

    public virtual void Set(bool displayObjectMenu, bool displayEmoteWindow)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        DisplayObjectMenu = displayObjectMenu;
        DisplayEmoteWindow = displayEmoteWindow;
    }
}
