/*
Copyright 2019 - 2022 Inetum

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
using UnityEditor;
using UnityEngine;

namespace umi3DBrowser.UICustomStyle
{
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleColorCrossPosition))]
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleFloatCrossPosition))]
    public class CustomStyleValue4LinesPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            if (!keywordValue.IsDefaultOrUndefined())
                return base.GetPropertyHeight(property, label);
            else
                return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            Rect keywordRect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect valueRect = CurrentLineRect(position);

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (!keywordValue.IsDefaultOrUndefined())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleBorderRadius))]
    public class CustomStyleValue3LinesPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 3;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            if (!keywordValue.IsDefaultOrUndefined())
                return base.GetPropertyHeight(property, label);
            else
                return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            Rect keywordRect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect valueRect = CurrentLineRect(position);

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (!keywordValue.IsDefaultOrUndefined())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}