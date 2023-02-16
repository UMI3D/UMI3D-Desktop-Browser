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
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;

namespace umi3d.commonScreen.Displayer
{
    public class Mapping_C : BaseVisual_C
    {
        public new class UxmlFactory : UxmlFactory<Mapping_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlLocaliseAttributeDescription m_mappingName = new UxmlLocaliseAttributeDescription
            {
                name = "mapping-name"
            };

            protected UxmlEnumAttributeDescription<MappingType> m_type = new UxmlEnumAttributeDescription<MappingType>
            {
                name = "type",
                defaultValue = MappingType.Keyboard
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as Mapping_C;

                custom.MappingName = m_mappingName.GetValueFromBag(bag, cc);
                custom.Type = m_type.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Name of the mapping (it is basicly the name of the key).
        /// </summary>
        public virtual LocalisationAttribute MappingName
        {
            get => MappingNameText.LocaliseText;
            set
            {
                IsSet = false;
                if (value.IsEmpty) MappingNameText.RemoveFromHierarchy();
                else Insert(0, MappingNameText);
                MappingNameText.LocaliseText = value;
                IsSet = true;
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

        public override string StyleSheetPath_MainTheme => $"USS/displayer";
        public virtual string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetDisplayersFolderPath}/mapping";

        public override string UssCustomClass_Emc => "mapping";
        public virtual string USSCustomClassType(MappingType type) => $"{UssCustomClass_Emc}-{type}".ToLower();

        public Text_C MappingNameText = new Text_C { name = "mapping-name" };

        protected MappingType m_type;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            MappingName = null;
            Type = MappingType.Keyboard;
        }
    }
}
