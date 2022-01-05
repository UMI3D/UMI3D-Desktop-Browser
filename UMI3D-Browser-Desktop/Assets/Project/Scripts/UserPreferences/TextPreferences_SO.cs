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
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UserPreferences
{
    [CreateAssetMenu(fileName = "TextPreferences", menuName = "ScriptableObjects/UserPreferences/TextPreferences")]
    public class TextPreferences_SO : ScriptableObject
    {
        [System.Serializable]
        public class TextPref
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
            private string textPrefName;
            /// <summary>
            /// Name of this Text preference.
            /// </summary>
            public string TextPrefName => textPrefName;

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
            private int labelFontSize = 11;
            [Tooltip("Color of the label.")]
            [SerializeField]
            private Color labelColor = new Color(1f, 1f, 1f, 1f);

            [Space]
            [Tooltip("USS classes of the label (can be empty or null).")]
            [SerializeField]
            private string[] labelUSSClasses;

            #endregion

            [Space]
            [SerializeField]
            [Tooltip("")]
            private TextFormat textFormat;
            public TextFormat Format => textFormat;

            /// <summary>
            /// Set the label's font and USS classes.
            /// </summary>
            /// <param name="label">The label to be set.</param>
            public void SetLabel(TextElement label, string labelText, Font globalFont)
            {
                switch (textStyle)
                {
                    case TextStyle.FONT:
                        Debug.Assert(globalFont != null, "Global font is null");
                        label.ClearClassList();
                        SetFont(label, globalFont);
                        break;
                    case TextStyle.USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textPrefName + " text.");
                        SetUSS(label);
                        break;
                    case TextStyle.FONT_AND_USS:
                        Debug.Assert(labelUSSClasses.Length != 0, "USS classes empty for " + textPrefName + " text.");
                        SetUSS(label);
                        SetFont(label, globalFont);
                        break;
                }

                textFormat.FontSizeAfterZoom = FontSizeAfterZoom();
                textFormat.SetFormat(label, labelText);
            }

            private void SetFont(TextElement label, Font globalFont)
            {
                label.style.unityFont = (labelFont != null) ? labelFont: globalFont;
                label.style.unityFontStyleAndWeight = labelFontStyle;
                label.style.fontSize = FontSizeAfterZoom();
                label.style.color = labelColor;
            }

            private float FontSizeAfterZoom()
            {
                return labelFontSize * UserPreferences.GlobalPref.ZoomCoef;
            }

            private void SetUSS(TextElement label)
            {
                label.ClearClassList();
                foreach (string labelClass in labelUSSClasses)
                {
                    label.AddToClassList(labelClass);
                }
            }
        }

        [Header("Global Font")]
        [Tooltip("")]
        [SerializeField]
        private Font globalFont;

        //title, subtitle, label, sublabe, body
        [Space]
        [SerializeField]
        private TextPref[] textprefs;

        public TextPreferences_SO(TextPreferences_SO textPreferences)
        {
            //TODO Copy properties.
        }

        public IEnumerator ApplyPref(TextElement label, string textPrefName, string labelText = null)
        {
            foreach (TextPref textpref in textprefs)
            {
                if (textpref.TextPrefName == textPrefName)
                {
                    if (label == null) Debug.Log($"Label null = {labelText}, textprefname = {textPrefName}");
                    textpref.SetLabel(label, (labelText != null) ? labelText : label.text, globalFont);
                    yield break;
                }
            }
            Debug.LogError("TextPrefName = " + textPrefName + " not recognized.");
        }

        public TextFormat GetTextFormat(string textPrefName)
        {
            foreach (TextPref textpref in textprefs)
            {
                if (textpref.TextPrefName == textPrefName)
                {
                    return textpref.Format;
                }
            }
            throw new System.Exception($"TextPrefName = {textPrefName} not recognized.");
        }

        [ContextMenu("Apply User Pref")]
        private void ApplyUserPref()
        {
            UserPreferences.Instance.OnApplyUserPreferences.Invoke();
        }

    }
}