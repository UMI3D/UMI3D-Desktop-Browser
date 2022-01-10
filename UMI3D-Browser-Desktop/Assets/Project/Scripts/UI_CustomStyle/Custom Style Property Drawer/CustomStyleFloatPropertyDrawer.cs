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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Browser.UICustomStyle
{
    [CustomPropertyDrawer(typeof(CustomStyleFloat))]
    public class CustomStyleFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            Rect keywordRect;
            Rect valueRect;
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
            {
                float width = position.width / 2 - 2;
                keywordRect = new Rect(position.x, position.y, width, position.height);
                valueRect = new Rect(position.x + width + 4, position.y, width, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x, position.y, position.width, position.height);
                valueRect = new Rect();
            }

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }


    [CustomPropertyDrawer(typeof(CustomStylePXAndPercentFloat))]
    public class CustomStylePxAndPercentFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var keyword = property.FindPropertyRelative("m_keyword");
            var valueMode = property.FindPropertyRelative("m_valueMode");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordV = (CustomStyleKeyword)keyword.intValue;
            CustomStyleValueMode valueModeV = (CustomStyleValueMode)valueMode.intValue;
            Rect keywordRect;
            Rect valueModeRect;
            Rect valueRect;
            if (keywordV != CustomStyleKeyword.VariableUndefined && keywordV != CustomStyleKeyword.ConstUndefined)
            {
                float valueWidth = position.width / 2 - 2;
                float keywordAndModeWidth = valueWidth / 2 - 2;
                keywordRect = new Rect(position.x, position.y, keywordAndModeWidth, position.height);
                valueModeRect = new Rect(position.x + keywordAndModeWidth + 2, position.y, keywordAndModeWidth, position.height);
                valueRect = new Rect(position.x + valueWidth + 4, position.y, valueWidth, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x, position.y, position.width, position.height);
                valueModeRect = new Rect();
                valueRect = new Rect();
            }

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            EditorGUI.PropertyField(valueModeRect, valueMode, GUIContent.none);
            switch (valueModeV)
            {
                case CustomStyleValueMode.Percent:
                    EditorGUI.Slider(valueRect, value, 0f, 100f, GUIContent.none);
                    break;
                default:
                    EditorGUI.PropertyField(valueRect, value, GUIContent.none);
                    break;
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(CustomStylePercentFloat))]
    public class CustomStylePercentFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordV = (CustomStyleKeyword)keyword.intValue;
            Rect keywordRect;
            Rect valueRect;
            if (keywordV != CustomStyleKeyword.VariableUndefined && keywordV != CustomStyleKeyword.ConstUndefined)
            {
                float valueWidth = position.width / 2 - 2;
                keywordRect = new Rect(position.x, position.y, valueWidth, position.height);
                valueRect = new Rect(position.x + valueWidth + 4, position.y, valueWidth, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x, position.y, position.width, position.height);
                valueRect = new Rect();
            }

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            EditorGUI.Slider(valueRect, value, 0f, 100f, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}

