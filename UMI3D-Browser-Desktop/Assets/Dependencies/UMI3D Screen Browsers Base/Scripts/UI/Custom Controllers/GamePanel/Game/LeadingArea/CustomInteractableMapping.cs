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

public abstract class CustomInteractableMapping : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller",
            defaultValue = ControllerEnum.MouseAndKeyboard
        };

        protected UxmlStringAttributeDescription m_interactableName = new UxmlStringAttributeDescription
        {
            name = "interactable-name",
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
            var custom = ve as CustomInteractableMapping;

            custom.Set
                (
                    m_controller.GetValueFromBag(bag, cc),
                    m_interactableName.GetValueFromBag(bag, cc)
                );
        }
    }

    /// <summary>
    /// The current controller use with this browser.
    /// </summary>
    public virtual ControllerEnum Controller
    {
        get => m_controller;
        set => m_controller = value;
    }
    /// <summary>
    /// Name of the interactable currently hovered (by default : "Interactable mapping"). This text will be displayed at the top.
    /// </summary>
    public virtual string InteractableName
    {
        get => InteractableNameText.text;
        set => InteractableNameText.text = string.IsNullOrEmpty(value) ? "Interactable mapping" : value;
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/interactableMapping";
    public virtual string USSCustomClassName => "interactable__mapping";
    public virtual string USSCustomClassMain => $"{USSCustomClassName}-main";
    public virtual string USSCustomClassInteractableName => $"{USSCustomClassName}-interactable__name";

    public VisualElement Main = new VisualElement { name = "main" };
    public CustomText InteractableNameText;
    public CustomScrollView ScrollView;

    protected bool m_hasBeenInitialized;
    protected ControllerEnum m_controller;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
        this.AddToClassList(USSCustomClassName);
        Main.AddToClassList(USSCustomClassMain);
        InteractableNameText.AddToClassList(USSCustomClassInteractableName);

        KeyboardInteraction.Mapped = AddMapping;
        KeyboardInteraction.Unmapped = RemoveMapping;

        Add(Main);
        Main.Add(InteractableNameText);
        Main.Add(ScrollView);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Set() => Set(ControllerEnum.MouseAndKeyboard, null);

    /// <summary>
    /// Set the UI element.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="interactableName"></param>
    public virtual void Set(ControllerEnum controller, string interactableName)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Controller = controller;
        InteractableName = interactableName;
    }

    #region Implementation

    /// <summary>
    /// Event raised when an interaction has been mapped.
    /// </summary>
    public event System.Action MappingAdded;
    /// <summary>
    /// Event raised when all interactions have been unmapped.
    /// </summary>
    public event System.Action MappingRemoved;
    /// <summary>
    /// Key: Keyboard interaction. Value: the customInteractableMappingRow corresponding to this interaction.
    /// </summary>
    public static Dictionary<KeyboardInteraction, CustomInteractableMappingRow> S_interactionMapping = new Dictionary<KeyboardInteraction, CustomInteractableMappingRow>();

    /// <summary>
    /// Add a mapping for this <paramref name="interaction"/> with this <paramref name="action"/>
    /// </summary>
    /// <param name="interaction"></param>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddMapping(KeyboardInteraction interaction, string name, InputAction action)
    {
        var row = CreateMappingRow();
        row.AddMapping(name, Controller, action);

        S_interactionMapping.Add(interaction, row);
        MappingAdded?.Invoke();
    }

    /// <summary>
    /// Remove the mapping corresponding to the <paramref name="interaction"/>.
    /// </summary>
    /// <param name="interaction"></param>
    public void RemoveMapping(KeyboardInteraction interaction)
    {
        if (!S_interactionMapping.ContainsKey(interaction)) return;
        RemoveMappingRow(S_interactionMapping[interaction]);
        S_interactionMapping.Remove(interaction);
        if (S_interactionMapping.Count == 0) MappingRemoved?.Invoke();
    }

    protected abstract CustomInteractableMappingRow CreateMappingRow();
    protected abstract void RemoveMappingRow(CustomInteractableMappingRow row);

    #endregion
}
