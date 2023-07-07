using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalisationSettings : ScriptableObject
{
    public const string k_LocalisationSettingsPath = "Assets/LocalisationSettings.asset";

    [SerializeField] private List<string> _languages;

    [SerializeField] private string _baseLanguage;

    private static LocalisationSettings _instance;
    public static LocalisationSettings Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = AssetDatabase.LoadAssetAtPath<LocalisationSettings>(k_LocalisationSettingsPath);
            if (_instance != null) return _instance;

            _instance = CreateInstance<LocalisationSettings>();
            _instance._languages = new List<string>();
            _instance._baseLanguage = "";
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

    private int _languageIndex;

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
        _settings = LocalisationSettings.GetSerializedSettings();
        _languageIndex = 0;
    }

    public override void OnGUI(string searchContext)
    {
        _settings.Update();
        EditorGUILayout.PropertyField(_settings.FindProperty("_languages"), Styles.Languages);
        var test = new List<string>();
        for (int i = 0; i < _settings.FindProperty("_languages").arraySize; i++)
        {
            test.Add(_settings.FindProperty("_languages").GetArrayElementAtIndex(i).stringValue);
        }
        if (test.Count > 0)
        {
            _languageIndex = EditorGUILayout.Popup("Base Language", _languageIndex, test.ToArray());
            _settings.FindProperty("_baseLanguage").stringValue = test[_languageIndex];
        }
        _settings.ApplyModifiedProperties();
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