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
using UnityEditor;
using UnityEngine;

namespace Browser.UICustomStyle
{
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleFloat))]
    public class CustomStyleFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            --EditorGUI.indentLevel;

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


    [UnityEditor.CustomPropertyDrawer(typeof(CustomStylePXAndPercentFloat))]
    public class CustomStylePxAndPercentFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var valueMode = property.FindPropertyRelative("m_valueMode");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordV = (CustomStyleKeyword)keyword.intValue;
            CustomStyleValueMode valueModeV = (CustomStyleValueMode)valueMode.intValue;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            --EditorGUI.indentLevel;

            Rect keywordRect;
            Rect valueModeRect;
            Rect valueRect;
            float valueWidth = 0f;
            float keywordAndModeWidth = 0f;
            switch (keywordV)
            {
                case CustomStyleKeyword.VariableUndefined:
                case CustomStyleKeyword.ConstUndefined:
                    keywordRect = new Rect(position.x, position.y, position.width, position.height);
                    valueModeRect = new Rect();
                    valueRect = new Rect();
                    break;
                case CustomStyleKeyword.Variable:
                    valueWidth = position.width / 2f - 2f;
                    keywordRect = new Rect(position.x, position.y, valueWidth, position.height);
                    valueModeRect = new Rect();
                    valueRect = new Rect(position.x + valueWidth + 4, position.y, valueWidth, position.height);
                    valueModeV = CustomStyleValueMode.Px;
                    break;
                case CustomStyleKeyword.Const:
                    valueWidth = position.width / 2f - 2f;
                    keywordAndModeWidth = valueWidth / 2f - 2f;
                    keywordRect = new Rect(position.x, position.y, keywordAndModeWidth, position.height);
                    valueModeRect = new Rect(position.x + keywordAndModeWidth + 2, position.y, keywordAndModeWidth, position.height);
                    valueRect = new Rect(position.x + valueWidth + 4, position.y, valueWidth, position.height);
                    break;
                default:
                    keywordRect = new Rect();
                    valueModeRect = new Rect();
                    valueRect = new Rect();
                    break;
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

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStylePercentFloat))]
    public class CustomStylePercentFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            --EditorGUI.indentLevel;

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

