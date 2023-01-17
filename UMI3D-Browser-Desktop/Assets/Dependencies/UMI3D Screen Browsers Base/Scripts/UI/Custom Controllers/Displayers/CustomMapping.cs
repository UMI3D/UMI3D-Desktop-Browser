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
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

public class CustomMapping : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_mappingName = new UxmlStringAttributeDescription
        {
            name = "mapping-name",
            defaultValue = null
        };

        protected UxmlEnumAttributeDescription<MappingType> m_type = new UxmlEnumAttributeDescription<MappingType>
        {
            name = "type",
            defaultValue = MappingType.Keyboard
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomMapping;

            custom.Set
                (
                    m_mappingName.GetValueFromBag(bag, cc),
                    m_type.GetValueFromBag(bag, cc)
                );
        }
    }

    /// <summary>
    /// Name of the mapping (it is basicly the name of the key).
    /// </summary>
    public virtual string MappingName
    {
        get => MappingNameText.text;
        set
        {
            m_isSet = false;
            if (string.IsNullOrEmpty(value)) MappingNameText.RemoveFromHierarchy();
            else Insert(0, MappingNameText);
            MappingNameText.text = value;
            m_isSet = true;
        }
    }
    /// <summary>
    /// Type of the mapping.
    /// </summary>
    public virtual MappingType Type
    {
        get => m_type;
        set
        {
            RemoveFromClassList(USSCustomClassType(m_type));
            AddToClassList(USSCustomClassType(value));
            m_type = value;
        }
    }

    public virtual string StyleSheetDisplayerPath => $"USS/displayer";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/mapping";
    public virtual string USSCustomClassName => "mapping";
    public virtual string USSCustomClassType(MappingType type) => $"{USSCustomClassName}-{type}".ToLower();

    public CustomText MappingNameText;

    protected bool m_isSet = false;
    protected bool m_hasBeenInitialized;
    protected MappingType m_type;

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
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Set() => Set(null, MappingType.Keyboard);

    /// <summary>
    /// Set this UI element.
    /// </summary>
    /// <param name="mappingName"></param>
    /// <param name="type"></param>
    public virtual void Set(string mappingName, MappingType type)
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        MappingName = mappingName;
        Type = type;

        m_isSet = true;
    }
}
