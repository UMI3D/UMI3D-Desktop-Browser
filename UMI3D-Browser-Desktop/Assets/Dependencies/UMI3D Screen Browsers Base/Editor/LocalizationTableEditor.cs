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
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

//[CustomEditor(typeof(LocalisationTable))]
public class LocalizationTableEditor : Editor
{
    const string k_path = "./Localization/";

    SerializedProperty _title;
    SerializedProperty _items;

    bool _isLoadToggle;
    string _pathLoad;

    void OnEnable()
    {
        _isLoadToggle = false;
        _title = serializedObject.FindProperty("Title");
        _items = serializedObject.FindProperty("Items");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_title);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save to CSV"))
        {
            Export();
        }
        if (GUILayout.Button("Load from CSV"))
        {
            _isLoadToggle = !_isLoadToggle;
        }
        EditorGUILayout.EndHorizontal();
        if (_isLoadToggle)
        {
            GUILayout.BeginVertical("Import", "window");
            _pathLoad = EditorGUILayout.TextField("Path", _pathLoad);
            if (GUILayout.Button("Import"))
            {
                Import();
            }
            GUILayout.EndVertical();
        }

        EditorGUILayout.PropertyField(_items);
        serializedObject.ApplyModifiedProperties();
    }

    private void Export()
    {
        string t = "Key,English,French,Spanish";
        for (int i = 0; i < _items.arraySize; i++)
        {
            var item = _items.GetArrayElementAtIndex(i);
            t += $"\n{item.FindPropertyRelative("Key").stringValue}," +
                $"{item.FindPropertyRelative("English").stringValue}," +
                $"{item.FindPropertyRelative("French").stringValue}," +
                $"{item.FindPropertyRelative("Spanish").stringValue}";
        }

        using (StreamWriter sw = File.CreateText(k_path + _title.stringValue + ".csv"))
        {
            sw.Write(t);
        }
        Debug.Log($"Saved {_title.stringValue} as CSV");
    }

    private void Import()
    {
        LocalisationTable to = serializedObject.targetObject as LocalisationTable;
        if (!File.Exists(_pathLoad)) return;

        to.Items = new List<LocalisationTableItem>();
        var lines = File.ReadLines(_pathLoad).ToList();
        for (int i = 1; i < lines.Count; i++)
        {
            var element = lines[i].Split(",");
            to.Items.Add(new LocalisationTableItem()
            {
                Key = element[0],
                English = element[1],
                French = element[2],
                Spanish = element[3]
            });
        }
    }
}
