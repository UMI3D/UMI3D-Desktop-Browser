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
    public class CustomPropertyDrawer : PropertyDrawer
    {
        protected float m_lineHeight { get; } = EditorGUIUtility.singleLineHeight;
        protected float m_spaceBetweenLine { get; } = EditorGUIUtility.standardVerticalSpacing;
        protected float m_deltaLineHeight => m_lineHeight + m_spaceBetweenLine;
        protected virtual float m_labelWidth => EditorGUIUtility.labelWidth;
        protected float m_deltaLabelWidth => m_labelWidth + m_spaceBetweenLine;
        protected virtual int m_numberOfLine => 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + m_spaceBetweenLine) * (float)GetNumberOfLine(property) - m_spaceBetweenLine;
        }

        public virtual int GetNumberOfLine(SerializedProperty property)
        {
            return m_numberOfLine;
        }

        protected virtual Rect CurrentLineRect(Rect currentPosition, float indent = 0f)
        {
            return new Rect(currentPosition.x + indent, currentPosition.y, currentPosition.width - indent, m_lineHeight);
        }
        protected virtual Rect NextLineRect(Rect currentPosition, float indent = 0f)
        {
            return new Rect(currentPosition.x + indent, currentPosition.y + m_deltaLineHeight, currentPosition.width - indent, m_lineHeight);
        }
        protected virtual Rect PreviousLineRect(Rect currentPosition, float indent = 0f)
        {
            return new Rect(currentPosition.x + indent, currentPosition.y - m_deltaLineHeight, currentPosition.width - indent, m_lineHeight);
        }
    }
}