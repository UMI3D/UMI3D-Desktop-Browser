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
    public struct Background : IBackground
    {
        [SerializeField]
        private CustomStyleValue<CustomStyleColorKeyword, Color> m_color;
        [SerializeField]
        private CustomStyleValue<CustomStyleSimpleKeyword, Sprite> m_image;
        [SerializeField]
        private CustomStyleValue<CustomStyleColorKeyword, Color> m_imageTintColor;

        public CustomStyleValue<CustomStyleColorKeyword, Color> BackgroundColor => m_color;
        public CustomStyleValue<CustomStyleSimpleKeyword, Sprite> BackgroundImage => m_image;
        public CustomStyleValue<CustomStyleColorKeyword, Color> BackgroundImageTintColor => m_imageTintColor;
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Background))]
    public class BackgroundPropertyDrawer : CustomPropertyDrawer
    {
        //protected override int m_numberOfLine => 3;
        //protected override float m_labelWidth => 80f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var color = property.FindPropertyRelative("m_color");
            var image = property.FindPropertyRelative("m_image");
            var imageTintColor = property.FindPropertyRelative("m_imageTintColor");

            Rect colorLineRect = CurrentLineRect(position, m_deltaLabelWidth);
            Rect imageLineRect = NextLineRect(colorLineRect);
            Rect tintLineRect = NextLineRect(imageLineRect);

            Rect colorLabelRect = new Rect(colorLineRect.x - m_deltaLabelWidth, colorLineRect.y, m_labelWidth, colorLineRect.height);
            Rect imageLabelRect = NextLineRect(colorLabelRect);
            Rect imageTintColorLabelRect = NextLineRect(imageLabelRect);

            EditorGUI.indentLevel += 2;
            EditorGUI.LabelField(colorLabelRect, new GUIContent("Color :"));
            EditorGUI.LabelField(imageLabelRect, new GUIContent("Image :"));
            EditorGUI.LabelField(imageTintColorLabelRect, new GUIContent("Image tint :"));
            EditorGUI.indentLevel -= 1;

            EditorGUI.PropertyField(colorLineRect, color, GUIContent.none);
            EditorGUI.PropertyField(imageLineRect, image, GUIContent.none);
            EditorGUI.PropertyField(tintLineRect, imageTintColor, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [Serializable]
    public struct CustomStyleBackground : ICustomStyleValue<CustomStyleSimpleKeyword, Background>
    {
        [SerializeField]
        private CustomStyleSimpleKeyword m_keyword;
        [SerializeField]
        private Background m_value;

        public CustomStyleSimpleKeyword Keyword { get => m_keyword; set => m_keyword = value; }
        public Background Value { get => m_value; set => m_value = value; }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleBackground))]
    public class CustomStyleBackgroundPropertyDrawer : CustomPropertyDrawer
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

            //label.text = label.text.Substring(11);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            //--EditorGUI.indentLevel;

            Rect keywordRect = PreviousLineRect(position, m_deltaLabelWidth);
            Rect valueRect = CurrentLineRect(position);

            --EditorGUI.indentLevel;
            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            if (!keywordValue.IsDefaultOrUndefined())
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}