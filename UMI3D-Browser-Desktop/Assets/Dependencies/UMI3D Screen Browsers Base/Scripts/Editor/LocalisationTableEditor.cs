using inetum.unityUtils.editor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using umi3d.baseBrowser.preferences;
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

    List<string> _sectionNames;

    GUIStyle _headerStyle;
    GUIStyle _rowStyle;
    GUIStyle _row2Style;

    float _cellWidth;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Title"));

        var languages = LocalisationSettings.Instance.Languages;
        languages.Reverse();

        if (_sectionNames == null) Initialize(languages);

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
        var mustBorder = false;

        EditorGUILayout.BeginVertical();
        // Header
        EditorGUILayout.BeginHorizontal(_headerStyle);
        foreach (var section in _sectionNames) 
        {
            EditorGUILayout.LabelField(section, GUILayout.Width(_cellWidth));
        }
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            items.InsertArrayElementAtIndex(0);
        }
        EditorGUILayout.EndHorizontal();

        // Element
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal(mustBorder ? _rowStyle : _row2Style);
            mustBorder = !mustBorder;
            EditorGUILayout.TextField(elements[i].Key, GUILayout.Width(_cellWidth));
            foreach (var language in languages)
            {
                EditorGUILayout.PropertyField(elements[i].Trad[language.Name], GUIContent.none, GUILayout.Width(_cellWidth));
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                items.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        DrawButtons();
    }

    private void Initialize(List<Language> languages)
    {
        _sectionNames = new List<string> { "Key" };
        _sectionNames.AddRange(languages.Select(e => e.Name));

        _cellWidth = (EditorGUIUtility.currentViewWidth - 40 - 20) / (languages.Count + 1) - 2;

        _headerStyle = new GUIStyle();
        _rowStyle = new GUIStyle();
        _row2Style = new GUIStyle();

        _headerStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.3f, .3f, .3f));
        _row2Style.normal.background = MakeTex((int)_cellWidth, 20, new Color(.2f, .2f, .2f));
        _rowStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.15f, .15f, .15f));
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}