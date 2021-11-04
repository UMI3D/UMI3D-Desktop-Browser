/*
Copyright 2019 Gfi Informatique

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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BrowserDesktop.UserPreferences
{
    [System.Serializable]
    public class FontPreferences
    {
        [Tooltip("")]
        [SerializeField]
        private FontPreferences_SO defaultFontPref_SO;
        [Tooltip("")]
        [SerializeField]
        private FontPreferences_SO fontPref_SO;

        /// <summary>
        /// user font file path.
        /// </summary>
        private const string userFont = UserPreferences.UserPrefRootPath + "userFont";

        #region Load and Save Font.

        public void LoadFontPref()
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, userFont);

            if (File.Exists(path))
            {
                FileStream file;
                file = File.OpenRead(path);

                BinaryFormatter bf = new BinaryFormatter();

                try
                {
                    fontPref_SO = (FontPreferences_SO)bf.Deserialize(file);
                }
                catch
                {
                    LoadDefaultFont();
                }

                file.Close();
            }
            else
            {
                LoadDefaultFont();
            }
        }

        private void LoadDefaultFont()
        {
            //TODO chose default font according to theme.
            fontPref_SO = defaultFontPref_SO;
        }

        public void SaveFontPref()
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, userFont);

            FileStream file;
            if (File.Exists(path)) file = File.OpenWrite(path);
            else file = File.Create(path);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, fontPref_SO);

            file.Close();
        }

        #endregion

        private FontPreferences_SO CopyFont(FontPreferences_SO font)
        {
            return new FontPreferences_SO(font);
        }
    }
}