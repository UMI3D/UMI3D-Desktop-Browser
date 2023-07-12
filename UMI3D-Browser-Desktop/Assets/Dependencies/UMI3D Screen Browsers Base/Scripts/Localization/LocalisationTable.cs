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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTableLocalization", menuName = "ScriptableObjects/LocalizationTable")]
public class LocalisationTable : ScriptableObject
{
    public string Title;
    public List<LocalisationTableItem> Items;

    /// <summary>
    /// Get translation of a text located in this table with the key <paramref name="key"/> and arguments <paramref name="args"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetTranslation(string key, string[] args = null)
    {
        if (Items.Any(x => x.Key == key))
            return Items.Find(x => x.Key == key).GetTranslation(args);
        return null;
    }
    
    #if UNITY_EDITOR
    const string k_path = "./Localization/";

    [Button("Import from csv (refresh inspector to see changes)")]
    public void Import()
    {
        var languages = LocalisationSettings.Instance.Languages;
        var path = EditorUtility.OpenFilePanel("Import CSV", k_path, "*.*");

        if (!path.EndsWith(".csv"))
        {
            Debug.LogError(path + " must be a .csv file!");
            return;
        }

        Items = new List<LocalisationTableItem>();
        var lines = File.ReadLines(path).ToList();

        var elements = lines[0].Split(",");
        var indexes = new List<string>();
        foreach (var e in elements) 
        {
            if (e == "Key") continue;
            indexes.Add(e);
        }

        for (int i = 1; i < lines.Count; i++)
        {
            var element = lines[i].Split(",");
            var item = new LocalisationTableItem()
            {
                Key = element[0],
            };
            foreach (var language in languages) 
            {
                if (!indexes.Contains(language.Name))
                {
                    Debug.LogWarning(element[0] + " does not have a traduction in : " +  language.Name);
                    item.AddLanguageIfNotExist(language, "");
                    continue;
                }
                item.AddLanguageIfNotExist(language, element[indexes.IndexOf(language.Name)+1]);
            }
            Items.Add(item);
        }
        Debug.Log($"Loaded {Title} from csv " + path);
    }

    [Button("Export to csv")]
    public void Export()
    {
        var languages = LocalisationSettings.Instance.Languages;
        string t = "Key";
        foreach (var language in languages)
        {
            t += "," + language.Name;
        }
        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var trads = item.GetTradDictionary();
            t += $"\n{item.Key}";
            foreach (var language in languages)
            {
                t += "," + trads[language.Name];
            }
        }

        using (StreamWriter sw = File.CreateText(k_path + Title + ".csv"))
        {
            sw.Write(t);
        }
        Debug.Log($"Saved {Title} as CSV");
    }
    #endif
}
