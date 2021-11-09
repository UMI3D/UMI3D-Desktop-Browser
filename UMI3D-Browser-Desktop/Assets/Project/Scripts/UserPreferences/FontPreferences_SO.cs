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

            private enum CaseStyle
            {
                NONE,
                LOWER_CASE,
                UPPER_CASE
            }


            [Tooltip("Name of the text Font.")]
            [SerializeField]
            private string textFontName;
            public string TextFontName => textFontName;

            #region Font And USS

            [Header("Font And USS")]

            [Space]
            [Tooltip("The text style to be applied to the label (Font, USS or Both).")]
            [SerializeField]
            private TextStyle textStyle;

            [Space]
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

            [Space]
            [Tooltip("USS classes of the label (can be empty or null).")]
            [SerializeField]
            private string[] labelUSSClasses;

            #endregion

            private const int noMaxNumberOfCharacters = 101;

            [Header("Format")]

            [Space]
            [Tooltip("")]
            [SerializeField]
            private bool applyFormat = false;
            [Tooltip("")]
            [Range(1, noMaxNumberOfCharacters)]
            [SerializeField]
            private int maxNumberOfCharacters = 10;
            [Tooltip("")]
            [SerializeField]
            private CaseStyle caseStyle = CaseStyle.NONE;
            [Tooltip("")]
            [SerializeField]
            private bool resizeWidth;

            /// <summary>
            /// Set the label's font and USS classes.
            /// </summary>
            /// <param name="label">The label to be set.</param>
            public void SetLabel(Label label, int zoom, Font globalFont)
            {
                switch (textStyle)
                {
                    case TextStyle.FONT:
                        Debug.Assert(globalFont != null, "Global font is null");
                        label.ClearClassList();
                        SetFont(label, zoom, globalFont);
                        break;
                    case TextStyle.USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textFontName + " text.");
                        SetUSS(label);
                        break;
                    case TextStyle.FONT_AND_USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textFontName + " text.");
                        SetUSS(label);
                        SetFont(label, zoom, globalFont);
                        break;
                }

                if (applyFormat)
                {
                    if (maxNumberOfCharacters != noMaxNumberOfCharacters && label.text.Length > maxNumberOfCharacters)
                    {
                        string subString;
                        if (maxNumberOfCharacters >= 6)
                        {
                            subString = label.text.Substring(0, maxNumberOfCharacters - 3);
                            label.text = $"{subString}...";
                        }
                        else
                        {
                            subString = label.text.Substring(0, maxNumberOfCharacters);
                            label.text = subString;
                        }
                    }

                    switch (caseStyle)
                    {
                        case CaseStyle.LOWER_CASE:
                            label.text = label.text.ToLowerInvariant();
                            break;
                        case CaseStyle.UPPER_CASE:
                            label.text = label.text.ToUpperInvariant();
                            break;
                        case CaseStyle.NONE:
                            break;
                    }
                    
                    if (resizeWidth)
                    {
                        if (maxNumberOfCharacters == noMaxNumberOfCharacters) label.style.width = StyleKeyword.Auto;
                        else label.style.width = (maxNumberOfCharacters / 2) * FontSizeAfterZoom(zoom);
                    }
                }
            }

            private void SetFont(Label label, int zoom, Font globalFont)
            {
                label.style.unityFont = (labelFont != null) ? labelFont: globalFont;
                label.style.unityFontStyleAndWeight = labelFontStyle;
                label.style.fontSize = FontSizeAfterZoom(zoom);
                label.style.color = labelColor;
            }

            private float FontSizeAfterZoom(int zoom)
            {
                return labelFontSize * ((float)zoom / 100f);
            }

            private void SetUSS(Label label)
            {
                label.ClearClassList();
                foreach (string labelClass in labelUSSClasses)
                {
                    label.AddToClassList(labelClass);
                }
            }
        }

        [Header("Global Font")]
        [Range(0, 200)]
        [Tooltip("Zoom percentage")]
        [SerializeField]
        private int zoomPercentage = 100;
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
                    textFont.SetLabel(label, zoomPercentage, globalFont);
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