/*
Copyright 2019 - 2023 Inetum

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
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace inetum.unityUtils.editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class UMI3DInspector : Editor
    {
        private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();

        public override void OnInspectorGUI()
        {
            GetSerializedProperties(ref _serializedProperties);

            bool anyUMI3DAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<IUMI3DAttribute>(p) != null);
            if (anyUMI3DAttribute)
                DrawSerializedProperties();
            else
                DrawDefaultInspector();
        }

        private void OnDisable()
        {
            TableListDrawer.Instance.ClearCache();
        }

        protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
        {
            outSerializedProperties.Clear();
            using (var iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                        outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
                    while (iterator.NextVisible(false));
                }
            }
        }

        private void DrawSerializedProperties()
        {
            serializedObject.Update();

            foreach (var property in _serializedProperties)
            {
                if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
                {
                    using (new EditorGUI.DisabledScope(disabled: true))
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                }
                else
                    UMI3DEditorGUI.PropertyField_Layout(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif