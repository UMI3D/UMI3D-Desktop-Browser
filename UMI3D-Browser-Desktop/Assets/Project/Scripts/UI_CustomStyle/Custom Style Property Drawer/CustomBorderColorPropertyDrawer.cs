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
    public class CustomPropertyDrawer : PropertyDrawer
    {
        protected float m_lineHeight { get; } = EditorGUIUtility.singleLineHeight;
        protected float m_spaceBetweenLine { get; } = EditorGUIUtility.standardVerticalSpacing;
        protected float m_deltaLineHeight => m_lineHeight + m_spaceBetweenLine;
        protected float m_labelWidth => EditorGUIUtility.labelWidth;
        protected float m_deltaLabelWidth => m_labelWidth + m_spaceBetweenLine;
        protected virtual int m_numberOfLine { get; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + m_spaceBetweenLine) * (float)m_numberOfLine - m_spaceBetweenLine;
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

    [UnityEditor.CustomPropertyDrawer(typeof(BorderWidth))]
    public class CustomBorderWidthPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var global = property.FindPropertyRelative("m_global");
            var bottom = property.FindPropertyRelative("m_bottom");
            var left = property.FindPropertyRelative("m_left");
            var right = property.FindPropertyRelative("m_right");
            var top = property.FindPropertyRelative("m_top");

            EditorGUI.BeginProperty(position, label, property);

            Rect line0Rect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);
            Rect line3Rect = NextLineRect(line2Rect);

            float width = line1Rect.width / 2f - m_spaceBetweenLine;

            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            Rect topRect = new Rect(line1Rect.x + (line1Rect.width - width) / 2f, line1Rect.y, width, line1Rect.height);

            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);
            Rect leftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            Rect rightRect = new Rect(line2Rect.x + width + m_spaceBetweenLine, line2Rect.y, width, line2Rect.height);

            Rect line3LabelRect = new Rect(line3Rect.x - m_deltaLabelWidth, line3Rect.y, m_labelWidth, line3Rect.height);
            Rect bottomRect = new Rect(line3Rect.x + (line3Rect.width - width) / 2f, line3Rect.y, width, line3Rect.height);

            float newGlobal = EditorGUI.FloatField(line0Rect, global.floatValue);

            if (newGlobal != global.floatValue)
            {
                if (newGlobal < 0f)
                    newGlobal = 0f;
                global.floatValue = newGlobal;
                top.floatValue = newGlobal;
                left.floatValue = newGlobal;
                right.floatValue = newGlobal;
                bottom.floatValue = newGlobal;
            }

            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top :"));
            top.floatValue = EditorGUI.FloatField(topRect, top.floatValue);

            EditorGUI.LabelField(line2LabelRect, new GUIContent("Left, Right :"));
            left.floatValue = EditorGUI.FloatField(leftRect, left.floatValue);
            right.floatValue = EditorGUI.FloatField(rightRect, right.floatValue);

            EditorGUI.LabelField(line3LabelRect, new GUIContent("Bottom :"));
            bottom.floatValue = EditorGUI.FloatField(bottomRect, bottom.floatValue);

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(BorderRadius))]
    public class CustomBorderRadiusPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var global = property.FindPropertyRelative("m_global");
            var bottomLeft = property.FindPropertyRelative("m_bottomLeft");
            var bottomRight = property.FindPropertyRelative("m_bottomRight");
            var topLeft = property.FindPropertyRelative("m_topLeft");
            var topRight = property.FindPropertyRelative("m_TopRight");

            EditorGUI.BeginProperty(position, label, property);

            Rect line0Rect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);

            float width = line1Rect.width / 2f - m_spaceBetweenLine;

            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            Rect topLeftRect = new Rect(line1Rect.x, line1Rect.y, width, line1Rect.height);
            Rect topRightRect = new Rect(line1Rect.x + width + m_spaceBetweenLine, line1Rect.y, width, line1Rect.height);

            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);
            Rect bottomLeftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            Rect bottomRightRect = new Rect(line2Rect.x + width + m_spaceBetweenLine, line2Rect.y, width, line2Rect.height);

            EditorGUI.PropertyField(line0Rect, global, GUIContent.none);
            //float newGlobal = EditorGUI.FloatField(line0Rect, global.floatValue);

            //if (newGlobal != global.floatValue)
            //{
            //    if (newGlobal < 0f)
            //        newGlobal = 0f;
            //    global.floatValue = newGlobal;
            //    topRight.floatValue = newGlobal;
            //    bottomRight.floatValue = newGlobal;
            //    topLeft.floatValue = newGlobal;
            //    bottomLeft.floatValue = newGlobal;
            //}

            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top Left, Top Right :"));
            topLeft.floatValue = EditorGUI.FloatField(topLeftRect, topLeft.floatValue);
            topRight.floatValue = EditorGUI.FloatField(topRightRect, topRight.floatValue);

            EditorGUI.LabelField(line2LabelRect, new GUIContent("Bottom Left, Bottom Right :"));
            bottomLeft.floatValue = EditorGUI.FloatField(bottomLeftRect, bottomLeft.floatValue);
            bottomRight.floatValue = EditorGUI.FloatField(bottomRightRect, bottomRight.floatValue);
            
            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(BorderColor))]
    public class CustomBorderColorPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var global = property.FindPropertyRelative("m_global");
            var bottom = property.FindPropertyRelative("m_bottom");
            var left = property.FindPropertyRelative("m_left");
            var right = property.FindPropertyRelative("m_right");
            var top = property.FindPropertyRelative("m_top");

            EditorGUI.BeginProperty(position, label, property);

            Rect line0Rect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);
            Rect line3Rect = NextLineRect(line2Rect);

            float width = line1Rect.width / 2f - m_spaceBetweenLine;

            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            Rect topRect = new Rect(line1Rect.x + (line1Rect.width - width) / 2f, line1Rect.y, width, line1Rect.height);

            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);
            Rect leftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            Rect rightRect = new Rect(line2Rect.x + width + m_spaceBetweenLine, line2Rect.y, width, line2Rect.height);

            Rect line3LabelRect = new Rect(line3Rect.x - m_deltaLabelWidth, line3Rect.y, m_labelWidth, line3Rect.height);
            Rect bottomRect = new Rect(line3Rect.x + (line3Rect.width - width) / 2f, line3Rect.y, width, line3Rect.height);

            Color newGlobal = EditorGUI.ColorField(line0Rect, GUIContent.none, global.colorValue);

            if (newGlobal != global.colorValue)
            {
                global.colorValue = newGlobal;
                top.colorValue = newGlobal;
                left.colorValue = newGlobal;
                right.colorValue = newGlobal;
                bottom.colorValue = newGlobal;
            }

            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top :"));
            top.colorValue = EditorGUI.ColorField(topRect, GUIContent.none, top.colorValue);

            EditorGUI.LabelField(line2LabelRect, new GUIContent("Left, Right :"));
            left.colorValue = EditorGUI.ColorField(leftRect, GUIContent.none, left.colorValue);
            right.colorValue = EditorGUI.ColorField(rightRect, GUIContent.none, right.colorValue);

            EditorGUI.LabelField(line3LabelRect, new GUIContent("Bottom :"));
            bottom.colorValue = EditorGUI.ColorField(bottomRect, GUIContent.none, bottom.colorValue);

            EditorGUI.EndProperty();
        }
    }
}