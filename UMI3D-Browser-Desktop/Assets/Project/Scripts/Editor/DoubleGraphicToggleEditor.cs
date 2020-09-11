/*
Copyright 2019 Gfi Informatique

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

namespace BrowserDesktop.Menu
{
    [CustomEditor(typeof(DoubleGraphicsToggle))]
    public class DoubleGraphicToggleEditor : ToggleEditor
    {

        SerializedProperty OnGraphic;
        SerializedProperty OffGraphic;

        protected override void OnEnable()
        {
            OnGraphic = serializedObject.FindProperty("OnGraphic");
            OffGraphic = serializedObject.FindProperty("OffGraphic");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(OnGraphic);
            EditorGUILayout.PropertyField(OffGraphic);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}