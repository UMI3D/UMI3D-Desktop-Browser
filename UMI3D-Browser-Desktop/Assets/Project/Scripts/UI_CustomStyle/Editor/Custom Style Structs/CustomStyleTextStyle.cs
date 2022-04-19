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
using UnityEditor;
using UnityEngine;

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct TextStyle
    {
        [SerializeField]
        private Font m_font;
        [SerializeField]
        private FontStyle m_fontStyleAndWeight;
        [SerializeField]
        private CustomStyleValue<CustomStyleColorKeyword, Color> m_color;
        [SerializeField]
        private CustomStyleValue<CustomStyleColorKeyword, Color> m_outlineColor;
        [SerializeField]
        private CustomStyleValue<CustomStyleSimpleKeyword, float> m_outlineWidth;

        public Font Font => m_font;
        public FontStyle FontStyle => m_fontStyleAndWeight;
        public CustomStyleValue<CustomStyleColorKeyword, Color> Color => m_color;
        public CustomStyleValue<CustomStyleColorKeyword, Color> OutlineColor => m_outlineColor;
        public CustomStyleValue<CustomStyleSimpleKeyword, float> OutlineWidth => m_outlineWidth;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(TextStyle))]
    public class ThemeTextPropertyDrawer : CustomPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var font = property.FindPropertyRelative("m_font");
            var fontStyle = property.FindPropertyRelative("m_fontStyleAndWeight");
            var color = property.FindPropertyRelative("m_color");
            var outlineColor = property.FindPropertyRelative("m_outlineColor");
            var outlineWidth = property.FindPropertyRelative("m_outlineWidth");

            Rect fontLineRect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect fontStyleLineRect = NextLineRect(fontLineRect);
            Rect colorLineRect = NextLineRect(fontStyleLineRect);
            Rect outlineColorLineRect = NextLineRect(colorLineRect);
            Rect outlineWidthLineRect = NextLineRect(outlineColorLineRect);

            Rect fontLabelRect = new Rect(fontLineRect.x - m_deltaLabelWidth, fontLineRect.y, m_labelWidth, fontLineRect.height);
            Rect fontStyleLabelRect = NextLineRect(fontLabelRect);
            Rect colorLabelRect = NextLineRect(fontStyleLabelRect);
            Rect outlineColorLabelRect = NextLineRect(colorLabelRect);
            Rect outlineWidthLabelRect = NextLineRect(outlineColorLabelRect);

            EditorGUI.indentLevel += 2;
            EditorGUI.LabelField(fontLabelRect, new GUIContent("Font :"));
            EditorGUI.LabelField(fontStyleLabelRect, new GUIContent("Font Style :"));
            EditorGUI.LabelField(colorLabelRect, new GUIContent("Color :"));
            EditorGUI.LabelField(outlineColorLabelRect, new GUIContent("Outline Color :"));
            EditorGUI.LabelField(outlineWidthLabelRect, new GUIContent("Outline Width :"));
            EditorGUI.indentLevel -= 2;

            EditorGUI.PropertyField(fontLineRect, font, GUIContent.none);
            EditorGUI.PropertyField(fontStyleLineRect, fontStyle, GUIContent.none);
            EditorGUI.PropertyField(colorLineRect, color, GUIContent.none);
            EditorGUI.PropertyField(outlineColorLineRect, outlineColor, GUIContent.none);
            EditorGUI.PropertyField(outlineWidthLineRect, outlineWidth, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
#endif

    [Serializable]
    public struct CustomStyleTextStyle : ICustomStyleValue<CustomStyleExtraSimpleKeyword, TextStyle>
    {
        [SerializeField]
        private CustomStyleExtraSimpleKeyword m_keyword;
        [SerializeField]
        private TextStyle m_value;

        public CustomStyleExtraSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public TextStyle Value { get => m_value; set => m_value = value; }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleTextStyle))]
    public class CustomStyleThemeTextPropertyDrawer : CustomPropertyDrawer
    {
        public override int GetNumberOfLine(SerializedProperty property)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            return keywordValue.IsCustom() ? 5 : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            //label.text = label.text.Substring(11);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            //--EditorGUI.indentLevel;

            Rect keywordRect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect valueRect = CurrentLineRect(position);

            --EditorGUI.indentLevel;
            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (keywordValue.IsCustom())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
#endif
}