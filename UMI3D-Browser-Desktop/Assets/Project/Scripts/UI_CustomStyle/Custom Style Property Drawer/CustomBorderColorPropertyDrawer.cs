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
    public class CustomBorderPropertyDrawer : PropertyDrawer
    {
        private float spaceBetweenLine = EditorGUIUtility.standardVerticalSpacing;
        private int numberOfLine = 4;

        Rect line1Rect;
        Rect line2Rect;
        Rect line3Rect;
        Rect line4Rect;
    }

    [CustomPropertyDrawer(typeof(BorderColor))]
    public class CustomBorderColorPropertyDrawer : PropertyDrawer
    {
        private float spaceBetweenLine = EditorGUIUtility.standardVerticalSpacing;
        private int numberOfLine = 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + spaceBetweenLine) * (float)numberOfLine - spaceBetweenLine;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var global = property.FindPropertyRelative("m_globalColor");
            var bottom = property.FindPropertyRelative("m_borderBottomColor");
            var left = property.FindPropertyRelative("m_borderLeftColor");
            var right = property.FindPropertyRelative("m_borderRightColor");
            var top = property.FindPropertyRelative("m_borderTopColor");

            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float lineDeltaHeight = lineHeight + spaceBetweenLine;
            float labelWidth = EditorGUIUtility.labelWidth;

            Rect line1Rect = new Rect(position.x, position.y, position.width, lineHeight);
            Rect line2Rect = new Rect(position.x, position.y + lineDeltaHeight, position.width, lineHeight);
            Rect line3Rect = new Rect(position.x, position.y + lineDeltaHeight * 2f, position.width, lineHeight);
            Rect line4Rect = new Rect(position.x, position.y + lineDeltaHeight * 3f, position.width, lineHeight);

            Rect topLabelRect = new Rect(line1Rect.x, line1Rect.y, labelWidth, line1Rect.height);
            Rect topRect = new Rect(line1Rect.x + labelWidth + 2f, line1Rect.y, line1Rect.width - labelWidth - 2f, line1Rect.height);

            Rect leftLabelRect = new Rect(line2Rect.x, line2Rect.y, labelWidth, line2Rect.height);
            Rect leftRect = new Rect(line2Rect.x + labelWidth + 2f, line2Rect.y, line2Rect.width - labelWidth - 2f, line2Rect.height);

            Rect rightLabelRect = new Rect(line3Rect.x, line3Rect.y, labelWidth, line3Rect.height);
            Rect rightRect = new Rect(line3Rect.x + labelWidth + 2f, line3Rect.y, line3Rect.width - labelWidth - 2f, line3Rect.height);

            Rect bottomLabelRect = new Rect(line4Rect.x, line4Rect.y, labelWidth, line4Rect.height);
            Rect bottomRect = new Rect(line4Rect.x + labelWidth + 2f, line4Rect.y, line4Rect.width - labelWidth - 2f, line4Rect.height);

            Rect globalRect = new Rect(topRect.x, topRect.y - lineDeltaHeight, topRect.width, topRect.height);

            Color newGlobalColor = EditorGUI.ColorField(globalRect, GUIContent.none, global.colorValue);
            if (newGlobalColor != global.colorValue)
            {
                global.colorValue = newGlobalColor;
                top.colorValue = newGlobalColor;
                left.colorValue = newGlobalColor;
                right.colorValue = newGlobalColor;
                bottom.colorValue = newGlobalColor;
            }

            EditorGUI.LabelField(topLabelRect, new GUIContent("Top color :"));
            top.colorValue = EditorGUI.ColorField(topRect, GUIContent.none, top.colorValue);

            EditorGUI.LabelField(leftLabelRect, new GUIContent("Left color :"));
            left.colorValue = EditorGUI.ColorField(leftRect, GUIContent.none, left.colorValue);

            EditorGUI.LabelField(rightLabelRect, new GUIContent("Right color :"));
            right.colorValue = EditorGUI.ColorField(rightRect, GUIContent.none, right.colorValue);

            EditorGUI.LabelField(bottomLabelRect, new GUIContent("Bottom color :"));
            bottom.colorValue = EditorGUI.ColorField(bottomRect, GUIContent.none, bottom.colorValue);

            

            //if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
            //{
            //    float width = (position.width) / 2f - 4f;
            //    keywordRect = new Rect(position.x, position.y, width, position.height);
            //    valueRect = new Rect(position.x + width + 2f, position.y, width, position.height);
            //}
            //else
            //{
            //    keywordRect = new Rect(position.x, position.y, position.width, position.height);
            //    valueRect = new Rect();
            //}

            //EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            //EditorGUI.PropertyField(valueRect, value, GUIContent.none);


            EditorGUI.EndProperty();
        }
    }
}