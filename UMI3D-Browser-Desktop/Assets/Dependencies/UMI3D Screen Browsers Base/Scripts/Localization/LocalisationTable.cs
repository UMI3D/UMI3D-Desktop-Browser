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
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTableLocalization", menuName = "ScriptableObjects/LocalizationTable")]
public class LocalisationTable : ScriptableObject
{
    public string Title;
    //[TableList]
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
        Debug.Log("table: "+Title+", key not found: "+key);
        return null;
    }
    /*
    #if UNITY_EDITOR
        const string k_path = "./Localization/";

        [Button("Import from csv")]
        public void Import()
        {
            var path = EditorUtility.OpenFilePanel("test", k_path, "*.*");

            if (!path.EndsWith(".csv"))
            {
                Debug.LogError(path + " must be a .csv file!");
                return;
            }

            Items = new List<LocalisationTableItem>();
            var lines = File.ReadLines(path).ToList();
            for (int i = 1; i < lines.Count; i++)
            {
                var element = lines[i].Split(",");
                Items.Add(new LocalisationTableItem()
                {
                    Key = element[0],
                    English = element[1],
                    French = element[2],
                    Spanish = element[3]
                });
            }
        }

        [Button("Export to csv")]
        public void Export()
        {
            string t = "Key,English,French,Spanish";
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                t += $"\n{item.Key}," +
                    $"{item.English}," +
                    $"{item.French}," +
                    $"{item.Spanish}";
            }

            using (StreamWriter sw = File.CreateText(k_path + Title + ".csv"))
            {
                sw.Write(t);
            }
            Debug.Log($"Saved {Title} as CSV");
        }
    #endif*/
}
