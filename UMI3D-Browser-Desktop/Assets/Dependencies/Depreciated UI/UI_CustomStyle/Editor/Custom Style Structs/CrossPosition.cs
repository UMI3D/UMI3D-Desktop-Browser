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
    public struct CrossPosition<T> : ICrossPosition<T>
    {
        [SerializeField]
        private T m_global;
        [SerializeField]
        private T m_bottom;
        [SerializeField]
        private T m_left;
        [SerializeField]
        private T m_right;
        [SerializeField]
        private T m_top;

        public T Bottom => m_bottom;
        public T Left => m_left;
        public T Right => m_right;
        public T Top => m_top;
    }

#if UNITY_EDITOR
    public abstract class AbstractCrossPositionPropertyDrawer : CustomPropertyDrawer
    {
        protected SerializedProperty m_global;
        protected SerializedProperty m_bottom;
        protected SerializedProperty m_top;
        protected SerializedProperty m_left;
        protected SerializedProperty m_right;

        protected Rect m_line0Rect;
        protected Rect m_topRect;
        protected Rect m_leftRect;
        protected Rect m_rightRect;
        protected Rect m_bottomRect;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_global = property.FindPropertyRelative("m_global");
            m_bottom = property.FindPropertyRelative("m_bottom");
            m_left = property.FindPropertyRelative("m_left");
            m_right = property.FindPropertyRelative("m_right");
            m_top = property.FindPropertyRelative("m_top");

            m_line0Rect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(m_line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);
            Rect line3Rect = NextLineRect(line2Rect);

            float width = (line1Rect.width - m_horizontalSpaceBetweenItem) / 2f;

            Rect line0LabelRect = new Rect(m_line0Rect.x - m_deltaLabelWidth, m_line0Rect.y, m_labelWidth, m_line0Rect.height);

            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            m_topRect = new Rect(line1Rect.x + (line1Rect.width - width) / 2f, line1Rect.y, width, line1Rect.height);

            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);
            m_leftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            m_rightRect = new Rect(line2Rect.x + width + m_horizontalSpaceBetweenItem, line2Rect.y, width, line2Rect.height);

            Rect line3LabelRect = new Rect(line3Rect.x - m_deltaLabelWidth, line3Rect.y, m_labelWidth, line3Rect.height);
            m_bottomRect = new Rect(line3Rect.x + (line3Rect.width - width) / 2f, line3Rect.y, width, line3Rect.height);

            EditorGUI.indentLevel += 2;
            //EditorGUI.LabelField(line0LabelRect, new GUIContent("Global :"));
            EditorGUI.PrefixLabel(line0LabelRect, new GUIContent("Global :"));
            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top :"));
            EditorGUI.LabelField(line2LabelRect, new GUIContent("Left, Right :"));
            EditorGUI.LabelField(line3LabelRect, new GUIContent("Bottom :"));
            EditorGUI.indentLevel -= 2;
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CrossPosition<Color>))]
    public class ColorCrossPositionPropertyDrawer : AbstractCrossPositionPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            base.OnGUI(position, property, label);

            Color newGlobal = EditorGUI.ColorField(m_line0Rect, GUIContent.none, m_global.colorValue);

            if (newGlobal != m_global.colorValue)
            {
                m_global.colorValue = newGlobal;
                m_top.colorValue = newGlobal;
                m_left.colorValue = newGlobal;
                m_right.colorValue = newGlobal;
                m_bottom.colorValue = newGlobal;
            }

            m_top.colorValue = EditorGUI.ColorField(m_topRect, GUIContent.none, m_top.colorValue);
            m_left.colorValue = EditorGUI.ColorField(m_leftRect, GUIContent.none, m_left.colorValue);
            m_right.colorValue = EditorGUI.ColorField(m_rightRect, GUIContent.none, m_right.colorValue);
            m_bottom.colorValue = EditorGUI.ColorField(m_bottomRect, GUIContent.none, m_bottom.colorValue);

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CrossPosition<float>))]
    public class FloatCrossPositionPropertyDrawer : AbstractCrossPositionPropertyDrawer
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
                m_top.floatValue = newGlobal;
                m_left.floatValue = newGlobal;
                m_right.floatValue = newGlobal;
                m_bottom.floatValue = newGlobal;
            }

            m_top.floatValue = EditorGUI.FloatField(m_topRect, m_top.floatValue);
            m_left.floatValue = EditorGUI.FloatField(m_leftRect, m_left.floatValue);
            m_right.floatValue = EditorGUI.FloatField(m_rightRect, m_right.floatValue);
            m_bottom.floatValue = EditorGUI.FloatField(m_bottomRect, m_bottom.floatValue);

            EditorGUI.EndProperty();
        }
    }
#endif

    [Serializable]
    public struct CustomStyleCrossPosition<K,T> : ICustomStyleValue<K, CrossPosition<T>>
    {
        [SerializeField]
        private K m_keyword;
        [SerializeField]
        private CrossPosition<T> m_value;

        public K Keyword { get => m_keyword; set => m_keyword = value; }
        public CrossPosition<T> Value { get => m_value; set => m_value = value; }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleCrossPosition<CustomStyleColorKeyword, Color>))]
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleCrossPosition<CustomStyleSimpleKeyword, float>))]
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleCrossPosition<CustomStyleSizeKeyword, float>))]
    public class CustomStyleCrossPositionPropertyDrawer : CustomPropertyDrawer
    {
        public override int GetNumberOfLine(SerializedProperty property)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            return keywordValue.IsCustom() ? 4 : 0;
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
            ++EditorGUI.indentLevel;

            EditorGUI.EndProperty();
        }
    }
#endif
}
