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
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.preferences;
using UnityEngine;

[System.Serializable]
public class LocalisationTableItem
{
    [Serializable]
    private class Trad
    {
        public Language Key;
        public string Value;

        public Trad(Language key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public string Key;
    [SerializeField] private List<Trad> _trads;

    /// <summary>
    /// Get the translation of a text with arguments <paramref name="args"/>.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetTranslation(string[] args = null)
    {
        var language = LocalisationSettings.Instance.CurrentLanguage;
        var trad = _trads.Where(e => e.Key.Name == language.Name).ToList()[0];
        if (trad != null)
        {
            string tmpFr = String.Copy(trad.Value);
            if (args != null)
                for (int i = 0; i < args.Length; i++) tmpFr = tmpFr.Replace($"{{{i}}}", args[i]);
            return tmpFr;
        }

        Debug.LogError("Missing Language on " + Key);
        return Key;

        /*switch (LocalisationManager.Instance.curr_language)
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
        }*/
    }

    public void AddLanguageIfNotExist(Language language, string value = "")
    {
        if (_trads == null) _trads = new List<Trad>();

        if (_trads.Any(e => e.Key.Equals(language))) return;
        _trads.Add(new Trad(language, value));
    }
    public void RemoveLanguageIfExist(Language language)
    {
        if (_trads == null) _trads = new List<Trad>();

        if (!_trads.Any(e => e.Key.Equals(language))) return;

        _trads.Remove(_trads.Find(e => e.Key.Equals(language)));
    }

    public Dictionary<string, string> GetTradDictionary()
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var trad in _trads) 
        {
            dictionary.Add(trad.Key.Name, trad.Value);
        }
        return dictionary;
    }
}
