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

namespace umi3d.commonScreen
{
    public class UxmlLocaliseForOptionsAttributeDescription : TypedUxmlAttributeDescription<LocalisationForOptionsAttribute>
    {
        //
        // Résumé :
        //     The default value for the attribute, as a string.
        public override string defaultValueAsString => defaultValue.ToString();

        //
        // Résumé :
        //     Constructor.
        public UxmlLocaliseForOptionsAttributeDescription()
        {
            base.type = "LocalisationForOptionsAttribute";
            base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
            base.defaultValue = new LocalisationForOptionsAttribute();
        }

        //
        // Résumé :
        //     Retrieves the value of this attribute from the attribute bag. Returns it if it
        //     is found, otherwise return defaultValue.
        //
        // Paramètres :
        //   bag:
        //     The bag of attributes.
        //
        //   cc:
        //     The context in which the values are retrieved.
        //
        // Retourne :
        //     The value of the attribute.
        public override LocalisationForOptionsAttribute GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
        {
            return GetValueFromBag(bag, cc, (string s, LocalisationForOptionsAttribute t) => s, base.defaultValue);
        }

        public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref LocalisationForOptionsAttribute value)
        {
            return TryGetValueFromBag(bag, cc, (string s, LocalisationForOptionsAttribute t) => s, base.defaultValue, ref value);
        }
    }
}
