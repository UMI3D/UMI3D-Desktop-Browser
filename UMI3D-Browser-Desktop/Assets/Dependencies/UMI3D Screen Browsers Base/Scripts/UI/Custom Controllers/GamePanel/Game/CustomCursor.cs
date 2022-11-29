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

public class CustomCursor : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<ElementPseudoState> m_state = new UxmlEnumAttributeDescription<ElementPseudoState>
        {
            name = "state",
            defaultValue = ElementPseudoState.Enabled
        };
        protected UxmlStringAttributeDescription m_action = new UxmlStringAttributeDescription
        {
            name = "action",
            defaultValue = null
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomCursor;

            custom.Set
                (
                    m_state.GetValueFromBag(bag, cc),
                    m_action.GetValueFromBag(bag, cc)
                );
        }
    }

    public virtual ElementPseudoState State
    {
        get => m_state;
        set
        {
            RemoveFromClassList(USSCustomClassState(m_state));
            AddToClassList(USSCustomClassState(value));
            m_state = value;
            if (value == ElementPseudoState.Disabled) this.SetEnabled(false);
            else this.SetEnabled(true);
        }
    }
    public virtual string Action
    {
        get => ActionText.text;
        set
        {
            ActionText.text = value;
            if (string.IsNullOrEmpty(value)) ActionText.RemoveFromHierarchy();
            else
            {
                Add(ActionText);
                ActionText.schedule.Execute(() =>
                {
                    var halfWidth = ActionText.layout.width / 2f;
                    ActionText.style.right = halfWidth + 40f;
                });
            }
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/cursor";
    public virtual string USSCustomClassName => "cursor";
    public virtual string USSCustomClassTarget => $"{USSCustomClassName}-target";
    public virtual string USSCustomClassAction => $"{USSCustomClassName}-action";
    public virtual string USSCustomClassState(ElementPseudoState state) => $"{USSCustomClassName}-{state}".ToLower();

    public VisualElement Target = new VisualElement { name = "target" };
    public CustomText ActionText;

    protected ElementPseudoState m_state;
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
        Target.AddToClassList(USSCustomClassTarget);
        ActionText.AddToClassList(USSCustomClassAction);

        Add(Target);
    }

    public virtual void Set() => Set(ElementPseudoState.Enabled, null);

    public virtual void Set(ElementPseudoState state, string action)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        State = state;
        Action = action;
    }
}
