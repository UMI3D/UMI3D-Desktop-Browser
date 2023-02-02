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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static umi3d.baseBrowser.preferences.SettingsPreferences;
using System.Linq;
using inetum.unityUtils;

public class LocalisationManager : PersistentSingleBehaviour<LocalisationManager>
{
    public List<LocalisationTable> Tables;
    public Language curr_language = Language.English;

    /// <summary>
    /// Get the translation of a text located in the table <paramref name="title"/> with the key <paramref name="key"/> and arguments <paramref name="args"/>.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetTranslation(string title, string key, string[] args = null)
    {
        if (Tables.Any(x => x.Title == title))
        return Tables.Find(x => x.Title == title).GetTranslation(key, args);
        Debug.Log("table not found: "+title+", key: "+key);
        return null;
    }
}