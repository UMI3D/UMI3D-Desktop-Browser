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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

[System.Serializable]
public struct LocalisationTableItem
{
    public string Key;
    public string English;
    public string French;
    public string Spanish;

    /// <summary>
    /// Get the translation of a text with arguments <paramref name="args"/>.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetTranslation(string[] args = null)
    {
        switch (LocalisationManager.Instance.curr_language)
        {
            case Language.French:
                if (args == null)
                    return French;
                string tmpFr = String.Copy(French);
                for (int i = 0; i < args.Length; i++) tmpFr = tmpFr.Replace($"{{{i}}}", args[i]);
                return tmpFr;
            case Language.Spanish:
                if (args == null)
                    return Spanish;
                string tmpSp = String.Copy(Spanish);
                for (int i = 0; i < args.Length; i++) tmpSp = tmpSp.Replace($"{{{i}}}", args[i]);
                return tmpSp;
            default:
                if (args == null)
                    return English;
                string tmpEn = String.Copy(English);
                for (int i = 0; i < args.Length; i++) tmpEn = tmpEn.Replace($"{{{i}}}", args[i]);
                return tmpEn;
        }
    }
}
