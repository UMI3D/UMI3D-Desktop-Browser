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
using System.Linq;
using inetum.unityUtils;
using System.Threading.Tasks;

public struct LocalisationAttribute
{
    /// <summary>
    /// Text display if it can not be localise.
    /// </summary>
    public string DefaultText;
    /// <summary>
    /// Name of the table containing the translation.
    /// </summary>
    public string Table;
    /// <summary>
    /// Key from the table <see cref="Table"/> containing the translation.
    /// </summary>
    public string Key;
    /// <summary>
    /// Arguments of the translation.
    /// </summary>
    public string[] Arguments;

    /// <summary>
    /// Whether or not this element can be localised.
    /// </summary>
    public bool CanBeLocalised => !string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(Key);
    /// <summary>
    /// Whether or not this element is empty (<see cref="CanBeLocalised"/> false and <see cref="DefaultText"/> is null or empty).
    /// </summary>
    public bool IsEmpty => !CanBeLocalised && string.IsNullOrEmpty(DefaultText);
    /// <summary>
    /// The value of this element (default if it can not be localised or if <see cref="LocalisationManager"/> doesn't exist)
    /// </summary>
    public string Value => this.ToString();

    /// <summary>
    /// Instanciate a <see cref="LocalisationAttribute"/>.
    /// </summary>
    /// <param name="defaultText"></param>
    /// <param name="table"></param>
    /// <param name="key"></param>
    /// <param name="arguments"></param>
    public LocalisationAttribute(string defaultText, string table, string key, string[] arguments = null)
    {
        DefaultText = defaultText;
        Table = table; 
        Key = key;
        Arguments = arguments;
    }

    /// <summary>
    /// Set <see cref="DefaultText"/>. Set null for the other properties.
    /// </summary>
    /// <param name="defaultText"></param>
    public static implicit operator LocalisationAttribute(string defaultText)
        => new LocalisationAttribute(defaultText, null, null, null);

    /// <summary>
    /// Set <see cref="DefaultText"/> and <see cref="Key"/>. Set null for the other properties.
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator LocalisationAttribute((string, string) tuple)
        => new LocalisationAttribute(tuple.Item1, null, tuple.Item2, null);

    /// <summary>
    /// Set <see cref="DefaultText"/>, <see cref="Table"/> and <see cref="Key"/>. Set null for <see cref="Arguments"/>.
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator LocalisationAttribute((string, string, string) tuple)
        => new LocalisationAttribute(tuple.Item1, tuple.Item2, tuple.Item3, null);

    /// <summary>
    /// Set <see cref="DefaultText"/>, <see cref="Key"/> and <see cref="Arguments"/>. Set null for <see cref="Table"/>.
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator LocalisationAttribute((string, string, string[]) tuple)
        => new LocalisationAttribute(tuple.Item1, null, tuple.Item2, tuple.Item3);

    /// <summary>
    /// <see cref="LocalisationAttribute.LocalisationAttribute"/>
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator LocalisationAttribute((string, string, string, string[]) tuple)
        => new LocalisationAttribute(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);

    /// <summary>
    /// <see cref="ToString"/>.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(LocalisationAttribute value)
        => value.Value;

    /// <summary>
    /// Return the localisation of this element if <see cref="CanBeLocalised"/> and <see cref="LocalisationManager"/> Exists. Else return <see cref="DefaultText"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (CanBeLocalised && LocalisationManager.Exists) return LocalisationManager.Instance.GetTranslation(this);
        else return DefaultText;
    }
}

public struct LocalisationForOptionsAttribute
{
    /// <summary>
    /// Separator used to saparate options in a string.
    /// </summary>
    public char Separator;
    /// <summary>
    /// Options
    /// </summary>
    public List<LocalisationAttribute> Options;

    /// <summary>
    /// Getter and Setter for <see cref="Options"/>.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public LocalisationAttribute this[int index]
    {
        get => Options[index];
        set => Options[index] = value;
    }

    /// <summary>
    /// Instanciate a <see cref="LocalisationForOptionsAttribute"/>
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="keysAndArguments"></param>
    public LocalisationForOptionsAttribute(char separator, List<LocalisationAttribute> options)
    {
        Separator = separator;
        Options = options;
    }

    /// <summary>
    /// Get Default options (do not localise).
    /// </summary>
    /// <returns></returns>
    public List<string> GetDefaultOptions()
    {
        if (Options == null) return null;

        List<string> result = new List<string>();
        foreach (var option in Options) result.Add(option.DefaultText);
        return result;
    }

    /// <summary>
    /// Try to localise. Get default if it cannot be localised.
    /// </summary>
    /// <returns></returns>
    public List<string> GetTranslatedOrDefaultOptions()
    {
        if (Options == null) return null;

        List<string> result = new List<string>();
        foreach (var option in Options) result.Add(option.Value);
        return result;
    }

    /// <summary>
    /// Localise this element. Return false if there has been no localisation.
    /// </summary>
    /// <returns></returns>
    public async Task<(bool, List<string>)> GetTranslatedOptions()
    {
        List<string> result = new List<string>();
        bool canBeLocalised = false;
        foreach (var option in Options)
        {
            if (option.CanBeLocalised)
            {
                canBeLocalised = true;
                while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();
            }
            result.Add(option.Value);
        }
        return (canBeLocalised, result);
    }

    /// <summary>
    /// Instanciate <see cref="LocalisationForOptionsAttribute"/> with options in a string. Fetch <see cref="Separator"/>.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator LocalisationForOptionsAttribute(string value)
    {
        if (string.IsNullOrEmpty(value)) return new LocalisationForOptionsAttribute();

        // Split value into options considering they are separated by space.
        List<string> options = value.Split(' ').ToList();
        // Searching for concrete separator. If it exist it should be the last char of the first option.
        char separator = ' ';
        if (options.Count > 1) separator = options[0][options[0].Length - 1];

        // Created the localised options.
        List<LocalisationAttribute> localiseOptions = new List<LocalisationAttribute>();
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].EndsWith(separator)) localiseOptions.Add(options[i].Substring(0, options[i].Length - 1));
            else localiseOptions.Add(options[i]);
        }

        return new LocalisationForOptionsAttribute(separator, localiseOptions);
    }

    /// <summary>
    /// Instanciate <see cref="LocalisationForOptionsAttribute"/> with options in a list of string. Set <see cref="Separator"/> to ','.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator LocalisationForOptionsAttribute(List<string> value)
    {
        if (value == null) return new LocalisationForOptionsAttribute();

        List<LocalisationAttribute> options = new List<LocalisationAttribute>();
        foreach (string option in value) options.Add(option);
       return new LocalisationForOptionsAttribute(',', options);
    }

    /// <summary>
    /// Instanciate <see cref="LocalisationForOptionsAttribute"/> with a list of <see cref="LocalisationAttribute"/>. Set <see cref="Separator"/> to ','.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator LocalisationForOptionsAttribute(List<LocalisationAttribute> value)
        => new LocalisationForOptionsAttribute(',', value);

    /// <summary>
    /// Get the result of <see cref="GetTranslatedOrDefaultOptions"/> and <see cref="string.Join(char, string[])"/> it with <see cref="Separator"/>. 
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(LocalisationForOptionsAttribute value)
        => value.ToString();

    /// <summary>
    /// <see cref="GetTranslatedOrDefaultOptions"/> and  <see cref="string.Join(char, string[])"/> it with <see cref="Separator"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        List<string> result;
        result = GetTranslatedOrDefaultOptions();
        return result == null ? "" : string.Join($"{Separator} ", result);
    }
}

public class LocalisationManager : PersistentSingleBehaviour<LocalisationManager>
{
    public List<LocalisationTable> Tables;

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
    
    /// <summary>
    /// Get the translation of a text, searching in all tables.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public string GetTranslation(string key, string[] args = null)
    {
        foreach (var table in Tables)
        {
            if (table.GetTranslation(key, args) is var trad && trad != null)
                return trad;
        }
        Debug.Log("Traduction not found: (all tables), key: " + key);
        return null;
    }

    /// <summary>
    /// Get the translation of a text located in <paramref name="localisation"/>.
    /// </summary>
    /// <param name="localisation"></param>
    /// <returns></returns>
    public string GetTranslation(LocalisationAttribute localisation) => GetTranslation(localisation.Table, localisation.Key, localisation.Arguments);
}