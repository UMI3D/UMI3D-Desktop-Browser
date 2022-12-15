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
using UnityEngine.UIElements;

public class CustomLeadingArea : VisualElement, ICustomElement
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
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomLeadingArea;

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
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    JoystickArea.RemoveFromHierarchy();
                    break;
                case ControllerEnum.Touch:
                    Add(JoystickArea);
                    break;
                case ControllerEnum.GameController:
                    JoystickArea.RemoveFromHierarchy();
                    break;
                default:
                    break;
            }
        }
    }

    public bool LeftHand
    {
        get => m_leftHand;
        set
        {
            m_leftHand = value;
            JoystickArea.LeftHand = value;
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
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/leadingArea";
    public virtual string USSCustomClassName => "leading__area";
    public virtual string USSCustomClassNameReverse => "leading__area-reverse";

    public CustomJoystickArea JoystickArea;

    public TouchManipulator2 LeadingAreaManipulator = new TouchManipulator2(null, 0, 0);

    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;
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

        this.AddManipulator(LeadingAreaManipulator);
        //TODO improve camera navigation with double click.
    }

    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, m_leftHand);

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
}
