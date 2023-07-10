using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class LocalisationSettings : ScriptableObject
{
    public const string k_LocalisationSettingsPath = "Assets/LocalisationSettings.asset";

    [Serializable]
    public struct Language
    {
        public string Name;
        public bool IsActive;
    }

    [SerializeField] private List<Language> _languages;

    [SerializeField] private int _baseLanguageIndex;

    public List<Language> Languages => _languages.Where(e => e.IsActive).ToList();
    public Language BaseLanguage => _languages[_baseLanguageIndex];

    private static LocalisationSettings _instance;
    public static LocalisationSettings Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = AssetDatabase.LoadAssetAtPath<LocalisationSettings>(k_LocalisationSettingsPath);
            if (_instance != null) return _instance;

            _instance = CreateInstance<LocalisationSettings>();
            _instance._languages = new List<Language>();
            _instance._baseLanguageIndex = 0;
            AssetDatabase.CreateAsset(_instance, k_LocalisationSettingsPath);
            AssetDatabase.SaveAssets();

            return _instance;
        }
    }

    internal static SerializedObject GetSerializedSettings() => new SerializedObject(Instance);


}

class LocalisationSettingsProvider : SettingsProvider
{
    private SerializedObject _settings;

    private int _languageIndex = 0;

    List<string> _sectionNames;
    SerializedProperty _languagesProperty;
    List<SerializedProperty> _properties;
    string _newLanguageName;

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
        _sectionNames = new List<string>();

        _headerStyle = new GUIStyle();
        _rowStyle = new GUIStyle();
        _row2Style = new GUIStyle();
        Type parentType = _languagesProperty.serializedObject.targetObject.GetType();
        FieldInfo fi = parentType.GetField(_languagesProperty.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var arrayType = fi.FieldType;
        var propertyType = arrayType.GenericTypeArguments[0];

        foreach (var arg in propertyType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (!arg.IsPublic && !arg.CustomAttributes.Any(p => p.AttributeType.Equals(typeof(SerializeField)))) continue;
            _sectionNames.Add(arg.Name);
        }

        _cellWidth = (EditorGUIUtility.currentViewWidth - 40 - 20)/2f / _sectionNames.Count - 2;

        _headerStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.3f, .3f, .3f));
        _row2Style.normal.background = MakeTex((int)_cellWidth, 20, new Color(.2f, .2f, .2f));
        _rowStyle.normal.background = MakeTex((int)_cellWidth, 20, new Color(.15f, .15f, .15f));
    }

    private string t = "";

    public override void OnGUI(string searchContext)
    {
        _settings.Update();

        DrawLanguages();

        var test = new List<LocalisationSettings.Language>();
        var test2 = new List<string>();
        for (int i = 0; i < _settings.FindProperty("_languages").arraySize; i++)
        {
            var settings = (LocalisationSettings)_settings.targetObject;
            test.Add(settings.Languages[i]);
            test2.Add(settings.Languages[i].Name);
        }
        if (test.Count > 0)
        {
            _settings.FindProperty("_baseLanguageIndex").intValue = EditorGUILayout.Popup("Base Language", _settings.FindProperty("_baseLanguageIndex").intValue, test2.ToArray());
        }

        _settings.ApplyModifiedProperties();
    }

    private void DrawLanguages()
    {
        _properties = new List<SerializedProperty>();
        for (int i = 0; i < _languagesProperty.arraySize; i++)
        {
            _properties.Add(_languagesProperty.GetArrayElementAtIndex(i));
        }
        bool mustBorder = true;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        //Header
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("v", GUILayout.Width(19)))
        {
            _show = !_show;
        }
        EditorGUILayout.LabelField(_languagesProperty.displayName + " [" + _languagesProperty.arraySize + " items]");
        EditorGUILayout.EndHorizontal();

        if (_show)
        {
            EditorGUILayout.BeginHorizontal(_headerStyle);
            foreach (var sectionName in _sectionNames)
            {
                GUILayout.Label(sectionName, GUILayout.Width(_cellWidth));
            }
            EditorGUILayout.EndHorizontal();

            // Elements
            for (int i = _properties.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal(mustBorder ? _rowStyle : _row2Style);
                mustBorder = !mustBorder;
                foreach (var propertyName in _sectionNames)
                {
                    EditorGUILayout.PropertyField(_properties[i].FindPropertyRelative(propertyName), GUIContent.none, GUILayout.Width(_cellWidth));
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    var tables = Resources.LoadAll<LocalisationTable>("");
                    foreach (var table in tables)
                    {
                        EditorUtility.SetDirty(table);
                        foreach (var item in table.Items) 
                        {
                            var settings = (LocalisationSettings)_settings.targetObject;
                            item.RemoveLanguageIfExist(settings.Languages[i]);
                        }
                    }

                    _languagesProperty.DeleteArrayElementAtIndex(i);
                    _settings.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal(mustBorder ? _rowStyle : _row2Style);
            _newLanguageName = EditorGUILayout.TextField(_newLanguageName);
            if (GUILayout.Button("+") && _newLanguageName != "")
            {
                _languagesProperty.InsertArrayElementAtIndex(0);
                _languagesProperty.GetArrayElementAtIndex(0).FindPropertyRelative("Name").stringValue = _newLanguageName;
                _languagesProperty.GetArrayElementAtIndex(0).FindPropertyRelative("IsActive").boolValue = true;
                _newLanguageName = "";
                _settings.ApplyModifiedProperties();

                var tables = Resources.LoadAll<LocalisationTable>("");
                foreach (var table in tables)
                {
                    EditorUtility.SetDirty(table);
                    foreach (var item in table.Items)
                    {
                        var settings = (LocalisationSettings)_settings.targetObject;
                        item.AddLanguageIfNotExist(settings.Languages[0]);
                    }
                }
                _newLanguageName = "";
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
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