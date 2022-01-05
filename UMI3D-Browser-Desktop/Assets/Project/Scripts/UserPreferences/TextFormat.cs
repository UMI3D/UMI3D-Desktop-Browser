using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.UserPreferences
{
    [System.Serializable]
    public partial class TextFormat
    {
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
        private bool resizeWidth = false;
    }

    public partial class TextFormat
    {
        private enum CaseStyle
        {
            NONE,
            LOWER_CASE,
            UPPER_CASE
        }

        private const int noMaxNumberOfCharacters = 101;

        

        //private float fontSizeAfterZoom;
        public float FontSizeAfterZoom { get; set; }

        public void SetFormat(TextElement label, string labelText)
        {
            if (applyFormat)
            {
                if (maxNumberOfCharacters != noMaxNumberOfCharacters && labelText.Length > maxNumberOfCharacters)
                {
                    if (maxNumberOfCharacters >= 6)
                    {
                        labelText = $"{labelText.Substring(0, maxNumberOfCharacters - 3)}...";
                    }
                    else
                    {
                        labelText = labelText.Substring(0, maxNumberOfCharacters);
                    }
                }

                switch (caseStyle)
                {
                    case CaseStyle.LOWER_CASE:
                        labelText = labelText.ToLowerInvariant();
                        break;
                    case CaseStyle.UPPER_CASE:
                        labelText = labelText.ToUpperInvariant();
                        break;
                    case CaseStyle.NONE:
                        break;
                }

                if (resizeWidth)
                {
                    if (maxNumberOfCharacters == noMaxNumberOfCharacters) label.style.width = StyleKeyword.Auto;
                    else label.style.width = (maxNumberOfCharacters / 2) * FontSizeAfterZoom;
                }
            }
            label.text = labelText;
        }
    }
}
