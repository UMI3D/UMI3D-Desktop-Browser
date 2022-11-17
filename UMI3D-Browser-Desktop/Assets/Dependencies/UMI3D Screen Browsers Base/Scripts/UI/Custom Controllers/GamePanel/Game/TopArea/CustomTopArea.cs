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
using System.Collections.Generic;
using umi3d.commonMobile.game;
using UnityEngine.UIElements;

public class CustomTopArea : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller",
            defaultValue = ControllerEnum.MouseAndKeyboard
        };
        protected UxmlBoolAttributeDescription m_displayHeader = new UxmlBoolAttributeDescription
        {
            name = "display-header",
            defaultValue = false,
        };
        UxmlBoolAttributeDescription m_isExpanded = new UxmlBoolAttributeDescription
        {
            name = "is-expanded",
            defaultValue = false,
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomTopArea;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_displayHeader.GetValueFromBag(bag, cc),
                    m_isExpanded.GetValueFromBag(bag, cc)
                );
        }
    }

    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            m_controller = value;
            InformationArea.Controller = value;
        }
    }

    public virtual bool DisplayHeader
    {
        get => m_displayHeader;
        set
        {
            m_displayHeader = value;
            if (value) Insert(0, AppHeader);
            else AppHeader.RemoveFromHierarchy();
        }
    }

    public virtual bool IsExpanded
    {
        get => m_isExplanded;
        set
        {
            m_isExplanded = value;
            this.AddAnimation
            (
                this,
#if UNITY_STANDALONE
                () => style.height = Length.Percent(0),
#else
                () => style.height = Length.Percent(20),
#endif
                () => style.height = Length.Percent(100),
                "height",
                1,
                revert: !m_isExplanded
            );
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/topArea";
    public virtual string USSCustomClassName => "top-area";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}-main";
    public virtual string USSCustomClassMenu => $"{USSCustomClassName}__menu";
    public virtual string USSCustomClassButton => $"{USSCustomClassName}__button";

    public CustomAppHeader AppHeader;
    public VisualElement Main = new VisualElement { name = "main" };
    public CustomInformationArea InformationArea;
    public CustomButton Menu;

    protected ControllerEnum m_controller;
    protected bool m_displayHeader;
    protected bool m_isExplanded;
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
        Main.AddToClassList(USSCustomClassMain);
        Menu.AddToClassList(USSCustomClassMenu);
        Menu.AddToClassList(USSCustomClassButton);

        Menu.name = "menu";
        Menu.Category = ElementCategory.Game;

        InformationArea.ExpandUpdate += (value) => IsExpanded = value;

        Add(Main);
        Main.Add(InformationArea);
        Main.Add(Menu);
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, false, false);

    public virtual void Set(ControllerEnum controller, bool displayHeader, bool isExpand)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        DisplayHeader = displayHeader;
        IsExpanded = isExpand;
    }
}