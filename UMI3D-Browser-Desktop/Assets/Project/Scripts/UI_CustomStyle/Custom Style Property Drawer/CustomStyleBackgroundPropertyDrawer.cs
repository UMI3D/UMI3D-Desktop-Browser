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
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleBackground))]
    public class CustomStyleBackgroundPropertyDrawer : CustomPropertyDrawer
    {
        public override int GetNumberOfLine(SerializedProperty property)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            return !keywordValue.IsDefaultOrUndefined() ? 4 : 1;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            label.text = label.text.Substring(11);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            --EditorGUI.indentLevel;

            Rect keywordRect = CurrentLineRect(position);
            Rect valueRect = NextLineRect(keywordRect);

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (!keywordValue.IsDefaultOrUndefined())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Background))]
    public class CustomBackgroundPropertyDrawer : CustomPropertyDrawer
    {
        protected override int m_numberOfLine => 3;
        protected override float m_labelWidth => 80f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var color = property.FindPropertyRelative("m_color");
            var image = property.FindPropertyRelative("m_image");
            var imageTintColor = property.FindPropertyRelative("m_imageTintColor");

            EditorGUI.BeginProperty(position, label, property);

            Rect colorRect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect colorLabelRect = new Rect(colorRect.x - m_deltaLabelWidth, colorRect.y, m_labelWidth, colorRect.height);

            Rect imageRect = NextLineRect(colorRect);
            Rect imageLabelRect = NextLineRect(colorLabelRect);

            Rect imageTintColorRect = NextLineRect(imageRect);
            Rect imageTintColorLabelRect = NextLineRect(imageLabelRect);

            EditorGUI.LabelField(colorLabelRect, new GUIContent("Color :"));
            EditorGUI.PropertyField(colorRect, color, GUIContent.none);

            EditorGUI.LabelField(imageLabelRect, new GUIContent("Image :"));
            EditorGUI.PropertyField(imageRect, image, GUIContent.none);

            EditorGUI.LabelField(imageTintColorLabelRect, new GUIContent("Image tint :"));
            EditorGUI.PropertyField(imageTintColorRect, imageTintColor, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}