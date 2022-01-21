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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3DBrowser.UICustomStyle
{
    [Serializable]
    public struct CustomStyleValue<K,V> : ICustomStyleValue<K, V>
    {
        [SerializeField]
        private K m_keyword;
        [SerializeField]
        private V m_value;

        public K Keyword { get => m_keyword; set => m_keyword = value; }
        public V Value { get => m_value; set => m_value = value; }

        public override string ToString()
        {
            return $"CustomStyle[Keyword=[{m_keyword}], Value=[{m_value}]";
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleValue<CustomStyleSimpleKeyword, float>))]
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleValue<CustomStyleColorKeyword, Color>))]
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleValue<CustomStyleSimpleKeyword, Sprite>))]
    public class CustomStyleImagePropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            if (label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
                --EditorGUI.indentLevel;
            }
            
            Rect keywordRect;
            Rect valueRect;
            if (!keywordValue.IsDefaultOrUndefined())
            {
                float width = (position.width) / 2f - m_spaceBetweenLine;
                keywordRect = new Rect(position.x, position.y, width, position.height);
                valueRect = new Rect(position.x + width + 2f, position.y, width, position.height);
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
}
