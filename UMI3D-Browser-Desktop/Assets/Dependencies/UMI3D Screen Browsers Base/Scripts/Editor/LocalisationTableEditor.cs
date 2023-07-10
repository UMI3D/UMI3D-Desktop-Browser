using inetum.unityUtils.editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalisationTable))]
public class LocalisationTableEditor : UMI3DInspector
{
    public struct Element
    {
        public string Key;
        public Dictionary<string, SerializedProperty> Trad;
        public Element(string key)
        {
            Key = key;
            Trad = new Dictionary<string, SerializedProperty>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var languages = LocalisationSettings.Instance.Languages;
        languages.Reverse();

        var elements = new List<Element>();
        var items = serializedObject.FindProperty("Items");
        for (int i = 0; i < items.arraySize; i++)
        {
            var trads = items.GetArrayElementAtIndex(i).FindPropertyRelative("_trads");
            var key = items.GetArrayElementAtIndex(i).FindPropertyRelative("Key").stringValue;
            var element = new Element(key);
            for (int j = 0; j < trads.arraySize; j++)
            {
                var language = trads.GetArrayElementAtIndex(j).FindPropertyRelative("Key").FindPropertyRelative("Name").stringValue;
                var trad = trads.GetArrayElementAtIndex(j).FindPropertyRelative("Value");
                element.Trad.Add(language, trad);
            }
            elements.Add(element);
        }

        var cellWidth = (EditorGUIUtility.currentViewWidth - 40 - 20) / (languages.Count + 1) - 2;

        EditorGUILayout.BeginVertical();
        // Header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Keys", GUILayout.Width(cellWidth));
        foreach (var language in languages) 
        {
            EditorGUILayout.LabelField(language.Name, GUILayout.Width(cellWidth));
        }
        EditorGUILayout.EndHorizontal();

        // Element
        foreach (var element in elements)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(element.Key, GUILayout.Width(cellWidth));
            foreach (var language in languages)
            {
                EditorGUILayout.PropertyField(element.Trad[language.Name], GUIContent.none, GUILayout.Width(cellWidth));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
