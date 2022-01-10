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

namespace Browser.UICustomStyle
{
    [CustomPropertyDrawer(typeof(CustomStyleBackground))]
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

    [CustomPropertyDrawer(typeof(CustomBackground))]
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
            Rect colorRect = new Rect(position.x, position.y, position.width, height);
            Rect imageRect = new Rect(position.x, position.y + heightDelta, position.width, height);
            Rect imageTintColorRect = new Rect(position.x, position.y + 2f * heightDelta, position.width, height);

            GUIContent colorLabel = new GUIContent();

            EditorGUI.PropertyField(colorRect, color, new GUIContent("Color :"));
            //EditorGUI.PropertyField()
            EditorGUI.PropertyField(imageRect, image, new GUIContent("Image :"));
            EditorGUI.PropertyField(imageTintColorRect, imageTintColor, new GUIContent("Image tint :"));

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(CustomStyleImage))]
    public class CustomStyleImagePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float labelWidth = 80f;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect keywordRect;
            Rect valueRect;
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
            {
                float width = (position.width - labelWidth) / 2f - 4f;
                keywordRect = new Rect(position.x + labelWidth + 2f, position.y, width, position.height);
                valueRect = new Rect(position.x + labelWidth + width + 4f, position.y, width, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x + labelWidth + 2f, position.y, position.width - labelWidth - 2f, position.height);
                valueRect = new Rect();
            }

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none);


            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(CustomStyleColor))]
    public class CustomStyleColorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            float labelWidth = 80f;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect keywordRect;
            Rect valueRect;
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
            {
                float width = (position.width - labelWidth) / 2f - 4f;
                keywordRect = new Rect(position.x + labelWidth + 2f, position.y, width, position.height);
                valueRect = new Rect(position.x + labelWidth + width + 4f, position.y, width, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x + labelWidth + 2f, position.y, position.width - labelWidth - 2f, position.height);
                valueRect = new Rect();
            }

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            value.colorValue = EditorGUI.ColorField(valueRect, GUIContent.none, value.colorValue);

            EditorGUI.EndProperty();
        }
    }
}