/*
Copyright 2019 - 2023 Inetum

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
using umi3d.baseBrowser.inputs.interactions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

public abstract class CustomInteractableMappingRow : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_actionName = new UxmlStringAttributeDescription
        {
            name = "action-name",
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
            var custom = ve as CustomInteractableMappingRow;

            custom.Set
                (
                    m_actionName.GetValueFromBag(bag, cc)
                );
        }
    }

    /// <summary>
    /// Name of the action.
    /// </summary>
    public virtual string ActionName
    {
        get => ActionNameText.text;
        set => ActionNameText.text = value;
    }

    public virtual string StyleSheetDisplayerPath => "USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/interactableMappingRow";
    public virtual string USSCustomClassName => "interactable__mapping__row";
    public virtual string USSCustomClassActionName => $"{USSCustomClassName}-action__name";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}-main";

    public CustomText ActionNameText;
    public VisualElement Main = new VisualElement { name = "main" };

    protected bool m_hasBeenInitialized;
    protected bool m_isSet = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetDisplayerPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        ActionNameText.AddToClassList(USSCustomClassActionName);
        Main.AddToClassList(USSCustomClassMain);

        Add(ActionNameText);
        Add(Main);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Set() => Set(null);

    /// <summary>
    /// Set this UI element
    /// </summary>
    /// <param name="actionName"></param>
    public virtual void Set(string actionName)
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        ActionName = actionName;

        m_isSet = true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override VisualElement contentContainer => m_isSet ? Main : this;

    #region Implementation

    /// <summary>
    /// List of the children corresponding of the mapping (CustomMapping and CustomText).
    /// </summary>
    public List<VisualElement> MappingChildren = new List<VisualElement>();

    /// <summary>
    /// Add the mapping for this interaction.
    /// </summary>
    /// <param name="name">Name of the action</param>
    /// <param name="controller"></param>
    /// <param name="action"></param>
    public void AddMapping(string name, ControllerEnum controller, InputAction action)
    {
        ActionName = name;

        List<(ControllerInputEnum, string)> mappings = null;
        switch (controller)
        {
            case ControllerEnum.MouseAndKeyboard:
                mappings = action.GetFirstMappingFromController(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
                foreach (var mapping in mappings)
                {
                    var map = CreateMapping();
                    if (mapping.Item1 == ControllerInputEnum.Keyboard)
                    {
                        map.MappingName = mapping.Item2;
                        map.Type = MappingType.Keyboard;
                    }
                    else
                    {
                        //TODO for manipulation
                    }

                    Add(map);
                    MappingChildren.Add(map);
                }
                break;
            case ControllerEnum.Touch:
                //TODO
                break;
            case ControllerEnum.GameController:
                mappings = action.GetFirstMappingFromController(ControllerInputEnum.Gamepad);
                //TODO
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Remove all mapping.
    /// </summary>
    public void RemoveAllMapping()
    {
        for (int i = 0; i < MappingChildren.Count; i++)
        {
            var child = MappingChildren[i];
            child.RemoveFromHierarchy();
            if (child is not CustomMapping mapping) return;
            RemoveMapping(mapping);
        }
    }

    protected abstract CustomMapping CreateMapping();
    protected abstract void RemoveMapping(CustomMapping mapping);

    #endregion
}
