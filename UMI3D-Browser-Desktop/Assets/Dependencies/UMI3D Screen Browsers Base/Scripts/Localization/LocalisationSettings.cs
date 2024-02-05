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
using umi3d.baseBrowser.preferences;
using UnityEngine;

public class LocalisationSettings : ScriptableObject
{
    public const string k_LocalisationSettingsPath = "Assets/Dependencies/UMI3D Screen Browsers Base/Resources/Scriptables/LocalisationSettings.asset";


    [SerializeField] private List<Language> _languages;

    [SerializeField] private int _baseLanguageIndex;

    public List<Language> Languages => _languages;
    public Language CurrentLanguage => _languages.Count > _baseLanguageIndex ?  _languages[_baseLanguageIndex] : default;
    public int CurrentLanguageIndex
    {
        get { return _baseLanguageIndex; }
        set { _baseLanguageIndex = value; }
    }

    private static LocalisationSettings _instance;
    public static LocalisationSettings Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Resources.Load<LocalisationSettings>("Scriptables/LocalisationSettings");
            if (_instance != null) return _instance;
            _instance = Create();

            return _instance;
        }
    }

    private static LocalisationSettings Create()
    {
        var settings = CreateInstance<LocalisationSettings>();
        settings._languages = new List<Language>();
        settings._baseLanguageIndex = 0;
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(settings, k_LocalisationSettingsPath);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        return settings;
    }

#if UNITY_EDITOR
    public static UnityEditor.SerializedObject GetSerializedSettings() => new UnityEditor.SerializedObject(Instance);
#endif
}