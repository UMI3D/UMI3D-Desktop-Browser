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
using System.Linq;
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
        Debug.Log("table: "+Title+", key not found: "+key);
        return null;
    }
}
