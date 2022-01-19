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

namespace umi3DBrowser.UICustomStyle
{
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

            Rect line0Rect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect line1Rect = NextLineRect(line0Rect);
            Rect line2Rect = NextLineRect(line1Rect);

            float width = line1Rect.width / 2f - m_spaceBetweenLine;

            Rect line0LabelRect = new Rect(line0Rect.x - m_deltaLabelWidth, line0Rect.y, m_labelWidth, line0Rect.height);

            Rect line1LabelRect = new Rect(line1Rect.x - m_deltaLabelWidth, line1Rect.y, m_labelWidth, line1Rect.height);
            Rect topLeftRect = new Rect(line1Rect.x, line1Rect.y, width, line1Rect.height);
            Rect topRightRect = new Rect(line1Rect.x + width + m_spaceBetweenLine, line1Rect.y, width, line1Rect.height);

            Rect line2LabelRect = new Rect(line2Rect.x - m_deltaLabelWidth, line2Rect.y, m_labelWidth, line2Rect.height);
            Rect bottomLeftRect = new Rect(line2Rect.x, line2Rect.y, width, line2Rect.height);
            Rect bottomRightRect = new Rect(line2Rect.x + width + m_spaceBetweenLine, line2Rect.y, width, line2Rect.height);

            //EditorGUI.PropertyField(line0Rect, global, GUIContent.none);
            EditorGUI.LabelField(line0LabelRect, new GUIContent("Global :"));
            float newGlobal = EditorGUI.FloatField(line0Rect, global.floatValue);

            if (newGlobal != global.floatValue)
            {
                if (newGlobal < 0f)
                    newGlobal = 0f;
                global.floatValue = newGlobal;
                topRight.floatValue = newGlobal;
                bottomRight.floatValue = newGlobal;
                topLeft.floatValue = newGlobal;
                bottomLeft.floatValue = newGlobal;
            }

            EditorGUI.LabelField(line1LabelRect, new GUIContent("Top Left, Top Right :"));
            topLeft.floatValue = EditorGUI.FloatField(topLeftRect, topLeft.floatValue);
            topRight.floatValue = EditorGUI.FloatField(topRightRect, topRight.floatValue);

            EditorGUI.LabelField(line2LabelRect, new GUIContent("Bottom Left, Bottom Right :"));
            bottomLeft.floatValue = EditorGUI.FloatField(bottomLeftRect, bottomLeft.floatValue);
            bottomRight.floatValue = EditorGUI.FloatField(bottomRightRect, bottomRight.floatValue);
            
            EditorGUI.EndProperty();
        }
    }
}