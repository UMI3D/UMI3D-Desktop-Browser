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
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleBackground))]
    public class CustomStyleBackgroundPropertyDrawer : PropertyDrawer
    {
        private float spaceBetweenLine = 2f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;
            
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
                return (base.GetPropertyHeight(property, label) + spaceBetweenLine) * 4f - spaceBetweenLine;
            else
                return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float heightDelta = (position.height + spaceBetweenLine) / 4f;
            float height = (keywordValue == CustomStyleKeyword.VariableUndefined || keywordValue == CustomStyleKeyword.ConstUndefined) ? position.height : heightDelta - spaceBetweenLine;
            Rect keywordRect = new Rect(position.x, position.y, position.width, height);
            Rect valueRect;
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
                valueRect = new Rect(position.x, position.y + heightDelta, position.width, position.height - heightDelta);
            else
                valueRect = new Rect();

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(CustomBackground))]
    public class CustomBackgroundPropertyDrawer : PropertyDrawer
    {
        private float spaceBetweenLine = 2f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var color = property.FindPropertyRelative("m_color");
            var image = property.FindPropertyRelative("m_image");
            var imageTintColor = property.FindPropertyRelative("m_imageTintColor");

            EditorGUI.BeginProperty(position, label, property);

            float height = position.height / 3f - spaceBetweenLine;
            float heightDelta = height + spaceBetweenLine;
            float labelWidth = 80f;

            Rect colorRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, height);
            Rect colorLabelRect = new Rect(position.x, position.y, labelWidth, height);

            Rect imageRect = new Rect(position.x + labelWidth, position.y + heightDelta, position.width - labelWidth, height);
            Rect imageLabelRect = new Rect(position.x, position.y + heightDelta, labelWidth, height);

            Rect imageTintColorRect = new Rect(position.x + labelWidth, position.y + 2f * heightDelta, position.width - labelWidth, height);
            Rect imageTintColorLabelRect = new Rect(position.x, position.y + 2f * heightDelta, labelWidth, height);

            GUIContent colorLabel = new GUIContent();

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