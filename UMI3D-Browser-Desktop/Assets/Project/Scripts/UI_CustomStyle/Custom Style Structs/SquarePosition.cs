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
    public struct SquarePosition<T> : ISquarePosition<T>
    {
        [SerializeField]
        private T m_global;
        [SerializeField]
        private T m_bottomLeft;
        [SerializeField]
        private T m_bottomRight;
        [SerializeField]
        private T m_topLeft;
        [SerializeField]
        private T m_TopRight;

        public T BottomLeft => m_bottomLeft;
        public T BottomRight => m_bottomRight;
        public T TopLeft => m_topLeft;
        public T TopRight => m_TopRight;
    }

    public abstract class AbstractSquarePositionPropertyDrawer : CustomPropertyDrawer
    {
        //protected override int m_numberOfLine => 3;

        protected SerializedProperty m_global;
        protected SerializedProperty m_bottomLeft;
        protected SerializedProperty m_bottomRight;
        protected SerializedProperty m_topLeft;
        protected SerializedProperty m_topRight;

        protected Rect m_line0Rect;
        protected Rect m_topLeftRect;
        protected Rect m_topRightRect;
        protected Rect m_bottomLeftRect;
        protected Rect m_bottomRightRect;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_global = property.FindPropertyRelative("m_global");
            m_bottomLeft = property.FindPropertyRelative("m_bottomLeft");
            m_bottomRight = property.FindPropertyRelative("m_bottomRight");
            m_topLeft = property.FindPropertyRelative("m_topLeft");
            m_topRight = property.FindPropertyRelative("m_TopRight");

            m_line0Rect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(m_line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);

            Rect line0LabelRect = new Rect(m_line0Rect.x - m_deltaLabelWidth, m_line0Rect.y, m_labelWidth, m_line0Rect.height);
            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);

            float width = line1Rect.width / 2f - m_spaceBetweenLine;

            m_topLeftRect = new Rect(line1Rect.x, line1Rect.y, width, line1Rect.height);
            m_topRightRect = new Rect(line1Rect.x + width + m_spaceBetweenLine, line1Rect.y, width, line1Rect.height);
            m_bottomLeftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            m_bottomRightRect = new Rect(line2Rect.x + width + m_spaceBetweenLine, line2Rect.y, width, line2Rect.height);

            EditorGUI.indentLevel += 2;
            EditorGUI.LabelField(line0LabelRect, new GUIContent("Global :"));
            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top Left, Top Right :"));
            EditorGUI.LabelField(line2LabelRect, new GUIContent("Bottom Left, Bottom Right :"));
            EditorGUI.indentLevel -= 2;
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(SquarePosition<float>))]
    public class FloatSquarePositionPropertyDrawer : AbstractSquarePositionPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            base.OnGUI(position, property, label);

            float newGlobal = EditorGUI.FloatField(m_line0Rect, m_global.floatValue);

            if (newGlobal != m_global.floatValue)
            {
                if (newGlobal < 0f)
                    newGlobal = 0f;
                m_global.floatValue = newGlobal;
                m_topRight.floatValue = newGlobal;
                m_bottomRight.floatValue = newGlobal;
                m_topLeft.floatValue = newGlobal;
                m_bottomLeft.floatValue = newGlobal;
            }

            m_topLeft.floatValue = EditorGUI.FloatField(m_topLeftRect, m_topLeft.floatValue);
            m_topRight.floatValue = EditorGUI.FloatField(m_topRightRect, m_topRight.floatValue);
            m_bottomLeft.floatValue = EditorGUI.FloatField(m_bottomLeftRect, m_bottomLeft.floatValue);
            m_bottomRight.floatValue = EditorGUI.FloatField(m_bottomRightRect, m_bottomRight.floatValue);

            EditorGUI.EndProperty();
        }
    }

    [Serializable]
    public struct CustomStyleSquarePosition<K,T> : ICustomStyleValue<K, SquarePosition<T>>
    {
        [SerializeField]
        private K m_keyword;
        [SerializeField]
        private SquarePosition<T> m_value;

        public K Keyword { get => m_keyword; set => m_keyword = value; }
        public SquarePosition<T> Value { get => m_value; set => m_value = value; }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleSquarePosition<CustomStyleSimpleKeyword, float>))]
    public class CustomStyleValue3LinesPropertyDrawer : CustomPropertyDrawer
    {
        public override int GetNumberOfLine(SerializedProperty property)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            return !keywordValue.IsDefaultOrUndefined() ? 3 : 0;
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
            if (!keywordValue.IsDefaultOrUndefined())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);
            ++EditorGUI.indentLevel;

            EditorGUI.EndProperty();
        }
    }
}