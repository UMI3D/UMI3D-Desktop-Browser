/*
Copyright 2019 - 2021 Inetum

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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Browser.UICustomStyle
{
    [CreateAssetMenu(fileName = "NewCustomUIStyleDictionary", menuName = "Browser_SO/CustomUIStyleDictionary")]
    public partial class CustomStyleDictionary_SO : ScriptableObject
    {
        [SerializeField]
        private CustomStyleTheme m_theme;
        [SerializeField]
        private CustomStyle_SO[] m_customStyles;
    }

    public partial class CustomStyleDictionary_SO
    {
        public CustomStyleTheme Theme { get => m_theme; }

        /// <summary>
        /// Return the customStyle corresponding to the [key], if no key correspond throw Exception.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CustomStyle_SO GetCustomStyle(string key)
        {
            foreach (CustomStyle_SO customStyle in m_customStyles)
            {
                if (customStyle.Key == key)
                    return customStyle;
            }
            throw new Exception($"Key not found in CustomStyleDictionary");
        }
    }
}

