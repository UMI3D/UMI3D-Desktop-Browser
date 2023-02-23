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
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class Manipulation_C : Button_C
    {
        public new class UxmlFactory : UxmlFactory<Manipulation_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            
            protected UxmlLocaliseAttributeDescription m_localisedLabel = new UxmlLocaliseAttributeDescription
            {
                name = "localised-label"
            };

            protected UxmlBoolAttributeDescription m_isToggle = new UxmlBoolAttributeDescription
            {
                name = "is-toggle",
                defaultValue = false
            };
            protected UxmlBoolAttributeDescription m_toogleValue = new UxmlBoolAttributeDescription
            {
                name = "toggle-value",
                defaultValue = false
            };

            protected UxmlLocaliseAttributeDescription m_localiseText = new UxmlLocaliseAttributeDescription
            {
                name = "localise-text"
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var custom = ve as Manipulation_C;

                custom.LocalisedLabel = m_localisedLabel.GetValueFromBag(bag, cc);
                custom.LocaliseText = m_localiseText.GetValueFromBag(bag, cc);
            }
        }

        public DofGroupDto dofGroup;

        protected override void SetProperties()
        {
            base.SetProperties();
            IsToggle = true;
            Height = ElementSize.Large;
            Width = ElementSize.Large;
        }

        #region Implementation



        #endregion
    }
}
