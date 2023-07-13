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
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using umi3d.baseBrowser.preferences;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class LocalisationSettingsProvider : SettingsProvider
{
    private SerializedObject _settings;

    List<string> _sectionNames;
    SerializedProperty _languagesProperty;
    List<SerializedProperty> _properties;
    string _newLanguageName;
    string _pathLocalisationTable = "Assets/Dependencies/UMI3D Screen Browsers Base/Resources/Scriptables/LocalizationTables/";

    GUIStyle _headerStyle;
    GUIStyle _rowStyle;
    GUIStyle _row2Style;

    bool _show = true;
    float _cellWidth;

    class Styles
    {
        public static GUIContent Languages = new GUIContent("Languages");
        public static GUIContent BaseLanguage = new GUIContent("Base Language");
    }

    const string k_LocalisationSettingsPath = "Assets/LocalisationSettings.asset";
    public LocalisationSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords) { }

    public static bool IsSettingsAvailable()
    {
        if (!File.Exists(k_LocalisationSettingsPath))
            LocalisationSettings.GetSerializedSettings();
        return true;
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        InitLanguagesProperty();
    }

    private void InitLanguagesProperty()
    {
        _settings = LocalisationSettings.GetSerializedSettings();
        _languagesProperty = _settings.FindProperty("_languages");
        SetupHeaderVariable();

        _cellWidth = (EditorGUIUtility.currentViewWidth - 40 - 20) / 2f / _sectionNames.Count - 2;

        SetupStyle();
    }

    private void SetupHeaderVariable()
    {
        _sectionNames = new List<string>();
        Type parentType = _languagesProperty.serializedObject.targetObject.GetType();
        FieldInfo fi = parentType.GetField(_languagesProperty.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var arrayType = fi.FieldType;
        var propertyType = arrayType.GenericTypeArguments[0];

        foreach (var arg in propertyType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (!arg.IsPublic && !arg.CustomAttributes.Any(p => p.AttributeType.Equals(typeof(SerializeField)))) continue;
            _sectionNames.Add(arg.Name);
        }
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

    public override void OnGUI(string searchContext)
    {
        _settings.Update();
        GetElements();

        DrawLanguages();
        DrawDropdownCurrentLanguage();

        EditorGUILayout.Space(20);
        DrawCreateFromCSV();

        _settings.ApplyModifiedProperties();
    }

    private void GetElements()
    {
        _properties = new List<SerializedProperty>();
        for (int i = 0; i < _languagesProperty.arraySize; i++)
        {
            _properties.Add(_languagesProperty.GetArrayElementAtIndex(i));
        }
    }

    private void DrawLanguages()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        DrawLanguageHeader();
        DrawLanguageTable();
        EditorGUILayout.EndVertical();
    }

    private void DrawLanguageTable()
    {
        if (!_show) return;

        DrawLanguageTableHeader();
        DrawLanguageTableElements();
        DrawLanguageTableAddButton();
    }

    private void DrawLanguageHeader()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("v", GUILayout.Width(19))) _show = !_show;
        EditorGUILayout.LabelField(_languagesProperty.displayName + " [" + _languagesProperty.arraySize + " items]");
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLanguageTableHeader()
    {
        EditorGUILayout.BeginHorizontal(_headerStyle);
        foreach (var sectionName in _sectionNames)
        {
            GUILayout.Label(sectionName, GUILayout.Width(_cellWidth));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLanguageTableElements()
    {
        var indexToRemove = -1;
        bool mustBorder = true;
        for (int i = 0; i < _properties.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(mustBorder ? _rowStyle : _row2Style);
            mustBorder = !mustBorder;
            foreach (var propertyName in _sectionNames)
            {
                EditorGUILayout.PropertyField(_properties[i].FindPropertyRelative(propertyName), GUIContent.none, GUILayout.Width(_cellWidth));
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                indexToRemove = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        RemoveLanguageIfNeeded(indexToRemove);
    }

    private void RemoveLanguageIfNeeded(int indexToRemove)
    {
        if (indexToRemove == -1) return;

        var tables = Resources.LoadAll<LocalisationTable>("");
        foreach (var table in tables)
        {
            EditorUtility.SetDirty(table);
            foreach (var item in table.Items)
            {
                var settings = (LocalisationSettings)_settings.targetObject;
                item.RemoveLanguageIfExist(settings.Languages[indexToRemove]);
            }
        }
        if (indexToRemove == _languagesProperty.arraySize - 1)
        {
            _settings.FindProperty("_baseLanguageIndex").intValue = 0;
        }

        _languagesProperty.DeleteArrayElementAtIndex(indexToRemove);
        _settings.ApplyModifiedProperties();
    }

    private void DrawLanguageTableAddButton()
    {
        EditorGUILayout.BeginHorizontal();
        _newLanguageName = EditorGUILayout.TextField(_newLanguageName);
        if (GUILayout.Button("+") && _newLanguageName != "")
        {
            var index = _languagesProperty.arraySize;
            _languagesProperty.InsertArrayElementAtIndex(index);
            _languagesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue = _newLanguageName;
            _newLanguageName = "";
            _settings.ApplyModifiedProperties();

            var tables = Resources.LoadAll<LocalisationTable>("");
            foreach (var table in tables)
            {
                EditorUtility.SetDirty(table);
                foreach (var item in table.Items)
                {
                    var settings = (LocalisationSettings)_settings.targetObject;
                    item.AddLanguageIfNotExist(settings.Languages[index]);
                }
            }
            _newLanguageName = "";
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDropdownCurrentLanguage()
    {
        GetLanguageChoice(out var languages, out var languageNames);

        if (languages.Count > 0)
            _settings.FindProperty("_baseLanguageIndex").intValue = EditorGUILayout.Popup("Current Language", _settings.FindProperty("_baseLanguageIndex").intValue, languageNames.ToArray());
        else
            EditorGUILayout.LabelField("Start by adding languages");
    }

    private void GetLanguageChoice(out List<Language> languages, out List<string> languageNames)
    {
        languages = new List<Language>();
        languageNames = new List<string>();
        for (int i = 0; i < _settings.FindProperty("_languages").arraySize; i++)
        {
            var settings = (LocalisationSettings)_settings.targetObject;
            languages.Add(settings.Languages[i]);
            languageNames.Add(settings.Languages[i].Name);
        }
    }

    private void DrawCreateFromCSV()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path to Localisation table objects :");
        _pathLocalisationTable = EditorGUILayout.TextField(_pathLocalisationTable);
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Create table from csv", GUILayout.Height(40)))
        {
            var path = EditorUtility.OpenFilePanel("Import CSV", "", "*.*");

            if (!path.EndsWith(".csv"))
            {
                Debug.LogError(path + " must be a .csv file!");
                return;
            }

            var table = ScriptableObject.CreateInstance<LocalisationTable>();
            table.Title = path.Split("/").Last().Split(".").First();
            table.ImportCSV(path);
            AssetDatabase.CreateAsset(table, _pathLocalisationTable + table.Title.Replace(" ", "") + ".asset");
            AssetDatabase.SaveAssets();
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

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new LocalisationSettingsProvider("Project/Localisation Settings", SettingsScope.Project);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        return null;
    }
}