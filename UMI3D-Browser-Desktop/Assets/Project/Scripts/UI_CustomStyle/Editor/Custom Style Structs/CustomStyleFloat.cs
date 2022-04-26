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
using System;
using UnityEditor;
using UnityEngine;

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct CustomStyleSize : ICustomStyleValue<CustomStyleSizeKeyword, float>
    {
        [SerializeField]
        private CustomStyleSizeKeyword m_keyword;
        [SerializeField]
        private CustomStyleSizeMode m_valueMode;
        [SerializeField]
        private float m_value;

        public CustomStyleSizeKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public CustomStyleSizeMode ValueMode => m_valueMode;
        public float Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyleFloat[Keyword=[{m_keyword}],ValueMode=[{m_valueMode}] Value=[{m_value}]";
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleSize))]
    public class CustomStyleSizePropertyDrawer : CustomPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyword = property.FindPropertyRelative("m_keyword");
            var valueMode = property.FindPropertyRelative("m_valueMode");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordV = (CustomStyleKeyword)keyword.intValue;
            CustomStyleSizeMode valueModeV = (CustomStyleSizeMode)valueMode.intValue;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            --EditorGUI.indentLevel;
            
            Rect keywordRect;
            Rect valueModeRect;
            Rect valueRect;
            float valueWidth = (position.width - m_horizontalSpaceBetweenItem) / 2f;
            switch (keywordV)
            {
                case CustomStyleKeyword.Default:
                case CustomStyleKeyword.Undefined:
                    keywordRect = new Rect(position.x, position.y, position.width, position.height);
                    valueModeRect = new Rect();
                    valueRect = new Rect();
                    break;
                case CustomStyleKeyword.CustomResizable:
                    keywordRect = new Rect(position.x, position.y, valueWidth, position.height);
                    valueModeRect = new Rect();
                    valueRect = new Rect(position.x + valueWidth + m_horizontalSpaceBetweenItem, position.y, valueWidth, position.height);
                    valueModeV = CustomStyleSizeMode.Px;
                    break;
                case CustomStyleKeyword.CustomUnresizabe:
                    float keywordAndModeWidth = (valueWidth - m_horizontalSpaceBetweenItem) / 2f;
                    keywordRect = new Rect(position.x, position.y, keywordAndModeWidth, position.height);
                    valueModeRect = new Rect(position.x + keywordAndModeWidth + m_horizontalSpaceBetweenItem, position.y, keywordAndModeWidth, position.height);
                    valueRect = new Rect(position.x + valueWidth + m_horizontalSpaceBetweenItem, position.y, valueWidth, position.height);
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
                case CustomStyleSizeMode.Percent:
                    EditorGUI.Slider(valueRect, value, 0f, 100f, GUIContent.none);
                    break;
                default:
                    EditorGUI.PropertyField(valueRect, value, GUIContent.none);
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
#endif

    [Serializable]
    public struct CustomStylePercentFloat : ICustomStyleValue<CustomStyleSimpleKeyword, float>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private float m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public float Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyleFloat[Keyword=[{m_keyword}], Value=[{m_value}]";
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStylePercentFloat))]
    public class CustomStylePercentFloatPropertyDrawer : CustomPropertyDrawer
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
            if (keywordValue.IsCustom())
            {
                float valueWidth = (position.width - m_horizontalSpaceBetweenItem) / 2f;
                keywordRect = new Rect(position.x, position.y, valueWidth, position.height);
                valueRect = new Rect(position.x + valueWidth + m_horizontalSpaceBetweenItem, position.y, valueWidth, position.height);
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
#endif
}

