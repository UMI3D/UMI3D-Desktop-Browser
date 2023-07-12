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
using inetum.unityUtils.editor;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.preferences;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalisationTable))]
public class LocalisationTableEditor : UMI3DInspector
{
    private float k_minElementWidth = 200;

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

    private List<Language> _languages;
    private List<string> _sectionNames;
    List<Element> _elements;
    SerializedProperty _items;

    private Vector2 _scrollPosition;

    private float _cellWidth;

    private GUIStyle _headerStyle;
    private GUIStyle _rowStyle;
    private GUIStyle _row2Style;

    public override void OnInspectorGUI()
    {
        if (_sectionNames == null)
            Initialize();

        GetElements();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Title"));
        EditorGUILayout.Space(15);
        DrawTable();
        EditorGUILayout.Space(15);

        serializedObject.ApplyModifiedProperties();

        DrawButtons();
    }

    private void Initialize()
    {
        _languages = LocalisationSettings.Instance.Languages;
        _sectionNames = new List<string> { "Key" };
        _sectionNames.AddRange(_languages.Select(e => e.Name));

        _cellWidth = (EditorGUIUtility.currentViewWidth - 40 - 20) / (_languages.Count + 1) - 2;
        if (_cellWidth < k_minElementWidth) _cellWidth = k_minElementWidth;

        SetupStyle();
    }

    private void SetupStyle()
    {
        _headerStyle = new GUIStyle();
        _rowStyle = new GUIStyle();
        _row2Style = new GUIStyle();

        _headerStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.3f, .3f, .3f));
        _row2Style.normal.background = MakeTex((int)_cellWidth, 20, new Color(.2f, .2f, .2f));
        _rowStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.15f, .15f, .15f));
    }

    private void GetElements()
    {
        _elements = new List<Element>();
        _items = serializedObject.FindProperty("Items");
        for (int i = 0; i < _items.arraySize; i++)
        {
            var trads = _items.GetArrayElementAtIndex(i).FindPropertyRelative("_trads");
            var key = _items.GetArrayElementAtIndex(i).FindPropertyRelative("Key").stringValue;
            var element = new Element(key);
            for (int j = 0; j < trads.arraySize; j++)
            {
                var language = trads.GetArrayElementAtIndex(j).FindPropertyRelative("Key").FindPropertyRelative("Name").stringValue;
                var trad = trads.GetArrayElementAtIndex(j).FindPropertyRelative("Value");
                element.Trad.Add(language, trad);
            }
            _elements.Add(element);
        }
    }

    private void DrawTable()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        EditorGUILayout.BeginVertical();

        DrawTableHeader();
        DrawTableElements();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void DrawTableElements()
    {
        var mustBorder = false;
        for (int i = _elements.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal(mustBorder ? _rowStyle : _row2Style);
            mustBorder = !mustBorder;
            EditorGUILayout.TextField(_elements[i].Key, GUILayout.Width(_cellWidth));
            foreach (var language in _languages)
            {
                EditorGUILayout.PropertyField(_elements[i].Trad[language.Name], GUIContent.none, GUILayout.Width(_cellWidth));
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _items.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawTableHeader()
    {
        EditorGUILayout.BeginHorizontal(_headerStyle);
        foreach (var section in _sectionNames)
        {
            EditorGUILayout.LabelField(section, GUILayout.Width(_cellWidth));
        }
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            _items.InsertArrayElementAtIndex(0);
        }
        EditorGUILayout.EndHorizontal();
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