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
        private class TextFont
        {
            /// <summary>
            /// The text style to be applied to the label (Font, USS or Both).
            /// </summary>
            private enum TextStyle
            {
                FONT,
                USS,
                FONT_AND_USS
            }

            [Tooltip("Name of the text Font.")]
            [SerializeField]
            private string textFontName;
            public string TextFontName => textFontName;

            [Header("Font")]
            [Tooltip("Font of the label (can be empty or null).")]
            [SerializeField]
            private Font labelFont;
            [Tooltip("Font style of the label (Normal, Bold, Italic, Bold and Italic).")]
            [SerializeField]
            private FontStyle labelFontStyle;
            [Tooltip("Font size (From 6 to 24)")]
            [Range(6, 24)]
            [SerializeField]
            private int labelFontSize;
            [Tooltip("Color of the label.")]
            [SerializeField]
            private Color labelColor;

            [Header("USS")]
            [Tooltip("USS classes of the label (can be empty or null).")]
            [SerializeField]
            private string[] labelUSSClasses;

            [Header("Style")]
            [Tooltip("The text style to be applied to the label (Font, USS or Both).")]
            [SerializeField]
            private TextStyle textStyle;

            /// <summary>
            /// Set the label's font and USS classes.
            /// </summary>
            /// <param name="label">The label to be set.</param>
            public void SetLabel(Label label, float globalFontSize, Font globalFont)
            {
                switch (textStyle)
                {
                    case TextStyle.FONT:
                        Debug.Assert(globalFont != null, "Global font is null");
                        label.ClearClassList();
                        label.style.unityFont = (labelFont != null) ? labelFont : globalFont;
                        label.style.unityFontStyleAndWeight = labelFontStyle;
                        label.style.fontSize = labelFontSize;
                        label.style.color = labelColor;
                        //Debug.Log("fontStyle = " + label.resolvedStyle.unityFontStyleAndWeight + ", size = " + label.resolvedStyle.fontSize);
                        break;
                    case TextStyle.USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textFontName + " text.");
                        label.ClearClassList();
                        foreach (string labelClass in labelUSSClasses)
                        {
                            label.AddToClassList(labelClass);
                        }
                        break;
                    case TextStyle.FONT_AND_USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textFontName + " text.");
                        label.ClearClassList();
                        foreach (string labelClass in labelUSSClasses)
                        {
                            label.AddToClassList(labelClass);
                        }

                        label.style.unityFont = labelFont != null ? labelFont : label.resolvedStyle.unityFont;
                        label.style.unityFontStyleAndWeight = labelFontStyle;
                        label.style.fontSize = labelFontSize;
                        break;
                }
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
        private TextFont[] textFonts;

        public FontPreferences_SO(FontPreferences_SO font)
        {
            //TODO Copy properties.
        }

        public IEnumerator ApplyFont(Label label, string textFontName)
        {
            yield return null;

            foreach (TextFont textFont in textFonts)
            {
                if (textFont.TextFontName == textFontName)
                {
                    textFont.SetLabel(label, globalFontSize, globalFont);
                    Debug.Log("Text font name = " + textFontName);
                    yield break;
                }
            }
            Debug.LogError("TextFontName = " + textFontName + " not recognized.");
        }

        [ContextMenu("Apply User Pref")]
        private void ApplyUserPref()
        {
            UserPreferences.Instance.OnApplyUserPreferences.Invoke();
        }

    }
}