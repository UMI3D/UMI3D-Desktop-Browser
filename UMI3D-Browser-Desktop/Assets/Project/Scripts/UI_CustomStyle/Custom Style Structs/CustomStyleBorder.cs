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
    public struct Border
    {
        [SerializeField]
        private CustomStyleCrossPosition<CustomStyleColorKeyword, Color> m_color;
        [SerializeField]
        private CustomStyleCrossPosition<CustomStyleSizeKeyword, float> m_width;
        [SerializeField]
        private CustomStyleSquarePosition<CustomStyleSizeKeyword, float> m_radius;

        public CustomStyleCrossPosition<CustomStyleColorKeyword, Color> Color => m_color;
        public CustomStyleCrossPosition<CustomStyleSizeKeyword, float> Width => m_width;
        public CustomStyleSquarePosition<CustomStyleSizeKeyword, float> Radius => m_radius;
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Border))]
    public class BorderPropertyDrawer : CustomPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var color = property.FindPropertyRelative("m_color");
            var width = property.FindPropertyRelative("m_width");
            var radius = property.FindPropertyRelative("m_radius");

            CustomStyleKeyword colorKeyword = (CustomStyleKeyword)color.FindPropertyRelative("m_keyword").intValue;
            CustomStyleKeyword widthKeyword = (CustomStyleKeyword)width.FindPropertyRelative("m_keyword").intValue;
            CustomStyleKeyword radiusKeyword = (CustomStyleKeyword)radius.FindPropertyRelative("m_keyword").intValue;

            Rect colorLabelRect = CurrentLineRect(position);
            Rect widthLabelRect = !colorKeyword.IsCustom() ? NextLineRect(colorLabelRect) : LineRect(colorLabelRect, 5);
            Rect radiusLabelRect = !widthKeyword.IsCustom() ? NextLineRect(widthLabelRect) : LineRect(widthLabelRect, 5);

            Rect colorLineRect = NextLineRect(colorLabelRect);
            Rect widthLineRect = NextLineRect(widthLabelRect);
            Rect radiusLineRect = NextLineRect(radiusLabelRect);

            EditorGUI.indentLevel += 2;
            EditorGUI.LabelField(colorLabelRect, new GUIContent("Color :"));
            EditorGUI.LabelField(widthLabelRect, new GUIContent("Width :"));
            EditorGUI.LabelField(radiusLabelRect, new GUIContent("Radius :"));

            EditorGUI.PropertyField(colorLineRect, color, GUIContent.none);
            EditorGUI.PropertyField(widthLineRect, width, GUIContent.none);
            EditorGUI.PropertyField(radiusLineRect, radius, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [Serializable]
    public struct CustomStyleBorder : ICustomStyleValue<CustomStyleExtraSimpleKeyword, Border>
    {
        [SerializeField]
        private CustomStyleExtraSimpleKeyword m_keyword;
        [SerializeField]
        private Border m_value;

        public CustomStyleExtraSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public Border Value { get => m_value; set => m_value = value; }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleBorder))]
    public class CustomStyleBorderPropertyDrawer : CustomPropertyDrawer
    {
        public override int GetNumberOfLine(SerializedProperty property)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            CustomStyleKeyword colorKeyword = (CustomStyleKeyword)property.FindPropertyRelative("m_value")
                .FindPropertyRelative("m_color")
                .FindPropertyRelative("m_keyword")
                .intValue;
            CustomStyleKeyword widthKeyword = (CustomStyleKeyword)property.FindPropertyRelative("m_value")
                .FindPropertyRelative("m_width")
                .FindPropertyRelative("m_keyword")
                .intValue;
            CustomStyleKeyword radiusKeyword = (CustomStyleKeyword)property.FindPropertyRelative("m_value")
                .FindPropertyRelative("m_radius")
                .FindPropertyRelative("m_keyword")
                .intValue;
            int nbOfLine = 0;
            if (colorKeyword.IsCustom())
                ++nbOfLine;
            if (widthKeyword.IsCustom())
                ++nbOfLine;
            if (radiusKeyword.IsCustom())
                ++nbOfLine;

            return keywordValue.IsCustom() ? 3 + nbOfLine * 4 : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            Rect keywordRect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect valueRect = CurrentLineRect(position);

            --EditorGUI.indentLevel;
            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (keywordValue.IsCustom())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}