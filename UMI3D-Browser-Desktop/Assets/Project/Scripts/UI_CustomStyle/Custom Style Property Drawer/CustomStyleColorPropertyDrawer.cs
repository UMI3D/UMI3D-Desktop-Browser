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
    [UnityEditor.CustomPropertyDrawer(typeof(CustomStyleColor))]
    public class CustomStyleColorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyword = property.FindPropertyRelative("m_keyword");
            var value = property.FindPropertyRelative("m_value");
            CustomStyleKeyword keywordValue = (CustomStyleKeyword)keyword.intValue;

            EditorGUI.BeginProperty(position, label, property);

            Rect keywordRect;
            Rect valueRect;
            if (keywordValue != CustomStyleKeyword.VariableUndefined && keywordValue != CustomStyleKeyword.ConstUndefined)
            {
                float width = (position.width) / 2f - 4f;
                keywordRect = new Rect(position.x, position.y, width, position.height);
                valueRect = new Rect(position.x + width + 2f, position.y, width, position.height);
            }
            else
            {
                keywordRect = new Rect(position.x, position.y, position.width, position.height);
                valueRect = new Rect();
            }

            EditorGUI.PropertyField(keywordRect, keyword, GUIContent.none);
            value.colorValue = EditorGUI.ColorField(valueRect, GUIContent.none, value.colorValue);

            EditorGUI.EndProperty();
        }
    }
}
