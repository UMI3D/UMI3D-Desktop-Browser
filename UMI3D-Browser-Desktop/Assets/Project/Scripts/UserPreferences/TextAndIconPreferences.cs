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
using UnityEngine.UIElements;

namespace BrowserDesktop.UserPreferences
{
    [System.Serializable]
    public class TextAndIconPreferences
    {
        /*public enum TextAndIconSource
        {
            DEFAULT,
            USER
        }*/

        [System.Serializable]
        public class TextAndIconSO
        {
            [Tooltip("")]
            [SerializeField]
            private GlobalPreferences_SO.Theme name;
            public GlobalPreferences_SO.Theme Name => name;
            [Tooltip("")]
            [SerializeField]
            private IconPreferences_SO iconPref_SO;
            public IconPreferences_SO IconPref_SO => iconPref_SO;
            [Tooltip("")]
            [SerializeField]
            private TextPreferences_SO textPref_SO;
            public TextPreferences_SO TextPref_SO => textPref_SO;
        }

        [Tooltip("")]
        [SerializeField]
        private TextAndIconSO[] defaultsTextAndIcon_SO;
        [Tooltip("")]
        [SerializeField]
        private TextAndIconSO currentTextAndIcon_SO;

        /// <summary>
        /// User Text and Icon file path.
        /// </summary>
        private const string userTextAndIconPref = UserPreferences.UserPrefRootPath + "userTextAndIconPref";

        #region Load and Save Text and Icon Preferences.

        public void Load()
        {
            //TODO add theme path
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, userTextAndIconPref);

            if (File.Exists(path))
            {
                FileStream file;
                file = File.OpenRead(path);

                BinaryFormatter bf = new BinaryFormatter();

                try
                {
                    currentTextAndIcon_SO = (TextAndIconSO)bf.Deserialize(file);
                }
                catch
                {
                    LoadDefault();
                }

                file.Close();
            }
            else
            {
                LoadDefault();
            }
        }

        private void LoadDefault()
        {
            foreach (TextAndIconSO pref in defaultsTextAndIcon_SO)
            {
                if (pref.Name == UserPreferences.GlobalPref.CurrentTheme)
                {
                    currentTextAndIcon_SO = pref;
                    return;
                }
            }
        }

        public void Save()
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, userTextAndIconPref);

            FileStream file;
            if (File.Exists(path)) file = File.OpenWrite(path);
            else file = File.Create(path);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, currentTextAndIcon_SO);

            file.Close();
        }

        #endregion

        private TextPreferences_SO CopyFont(TextPreferences_SO font)
        {
            return new TextPreferences_SO(font);
        }

        public void ApplyTextPref(Label label, string textFontName, string labelText = null)
        {
            UserPreferences.Instance.StartCoroutine(currentTextAndIcon_SO.TextPref_SO.ApplyFont(label, textFontName, labelText));
        }

        public void ApplyIconPref(VisualElement icon)
        {
            //UserPreferences.Instance.StartCoroutine(currentTextAndIcon_SO.IconPref_SO.ApplyFont(icon));
        }

    }
}