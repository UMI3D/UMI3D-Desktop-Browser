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
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

public abstract class CustomInteractableMapping : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
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
                    m_interactableName.GetValueFromBag(bag, cc)
                );
        }
    }

    public string InteractableName
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

    public virtual void Set() => Set(null);

    public virtual void Set(string interactableName)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        InteractableName = interactableName;
    }

    #region Implementation

    public static Dictionary<KeyboardInteraction, CustomInteractableMappingRow> S_interactionMapping = new Dictionary<KeyboardInteraction, CustomInteractableMappingRow>();

    public void AddMapping(KeyboardInteraction interaction, string name, List<(string, MappingType)> mapping)
    {
        var row = CreateMappingRow();
        row.ActionName = name;

        S_interactionMapping.Add(interaction, row);
        ScrollView.Add(row);
    }

    public void RemoveMapping(KeyboardInteraction interaction)
    {
        if (!S_interactionMapping.ContainsKey(interaction)) return;
        S_interactionMapping[interaction].RemoveFromHierarchy();
        RemoveMappingRow(S_interactionMapping[interaction]);
        S_interactionMapping.Remove(interaction);
    }

    protected abstract CustomInteractableMappingRow CreateMappingRow();
    protected abstract void RemoveMappingRow(CustomInteractableMappingRow row);

    #endregion
}
