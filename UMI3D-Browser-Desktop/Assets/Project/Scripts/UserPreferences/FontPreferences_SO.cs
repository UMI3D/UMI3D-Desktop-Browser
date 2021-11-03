/*
Copyright 2019 Gfi Informatique

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

namespace BrowserDesktop.UserPreferences
{
    [CreateAssetMenu(fileName = "FontPreferences", menuName = "ScriptableObjects/UserPreferences/FontPreferences")]
    public class FontPreferences_SO : ScriptableObject
    {
        [System.Serializable]
        public class TextFont
        {
            //[Header("Label")]
            [Tooltip("Name of the label.")]
            [SerializeField]
            private string labeName;

            [Space]
            [Tooltip("Font of the label (can be empty or null).")]
            [SerializeField]
            private Font labelFont;
            [Tooltip("Style of the label (Normal, Bold, Italic, Bold and Italic).")]
            [SerializeField]
            private FontStyle labelFontStyle;
            [Tooltip("Font size (From 6 to 24)")]
            [Range(6, 24)]
            [SerializeField]
            private int labelFontSize;
            [Tooltip("Color of the label.")]
            [SerializeField]
            private Color labelColor;
            [Tooltip("USS classes of the label (can be empty or null).")]
            [SerializeField]
            private string[] labelUSSClasses;


            public void SetLabel(Label label)
            {
                if (labelUSSClasses.Length != 0)
                {
                    label.ClearClassList();
                    foreach (string labelClass in labelUSSClasses)
                    {
                        label.AddToClassList(labelClass);
                    }
                }
                else
                {
                    label.style.unityFont = labelFont != null ? labelFont : label.resolvedStyle.unityFont;
                    label.style.unityFontStyleAndWeight = labelFontStyle;
                    label.style.fontSize = labelFontSize;
                }
                //else if both ?
            }
        }

        [Header("Global Font")]
        [Range(0f, 2f)]
        [Tooltip("Font size coefficient")]
        [SerializeField]
        private float globalFontSize;
        [Tooltip("")]
        [SerializeField]
        private Font globalFont;

        //title, subtitle, label, sublabe, body
        [Space]
        [SerializeField]
        private TextFont[] labels;

    }
}