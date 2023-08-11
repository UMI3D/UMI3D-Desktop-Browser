/*
Copyright 2019 - 2022 Inetum

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
using System.Threading.Tasks;
using umi3d.baseBrowser.preferences;
using umi3d.commonScreen.Displayer;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

namespace umi3d.commonScreen.menu
{
    public class SettingsGeneral_C : BaseSettingScreen_C
    {
        public override string UssCustomClass_Emc => "setting-general";

        public Dropdown_C LanguageDropdown = new Dropdown_C { name = "language" };
        public Dropdown_C ThemeDropdown = new Dropdown_C { name = "theme" };

        public SettingsGeneral_C() { }

        protected override void InitElement()
        {
            base.InitElement();

            LanguageDropdown.LocalisedLabel = new LocalisationAttribute("Language", "GeneralSettings", "Language");
            var localisedOptions = new List<LocalisationAttribute>();
            List<Language> languages = LocalisationSettings.Instance.Languages;
            foreach (var language in languages)
            {
                localisedOptions.Add(new LocalisationAttribute(language.Name, "GeneralSettings", language.Name));
            }
            LanguageDropdown.LocalisedOptions = localisedOptions;

            LanguageDropdown.ValueChanged += (index, newValue) => _ = LanguageValueChanged(index);

            ThemeDropdown.LocalisedLabel = new LocalisationAttribute("Theme", "GeneralSettings", "Theme_Titre");
            ThemeDropdown.LocalisedOptions = new List<string>() { "Default" };
            ThemeDropdown.LocalisedValue = "Default";

            ScrollView.Add(LanguageDropdown);
            ScrollView.Add(ThemeDropdown);

            if (TryGetGeneralData(out Data)) _ = LanguageValueChanged(languages.IndexOf(Data.LanguageChoice));
            else _ = LanguageValueChanged(1);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("General", "GenericStrings", "General");
        }

        #region Implementation

        public GeneralData Data;

        public async Task LanguageValueChanged(int index)
        {
            if (!UnityEngine.Application.isPlaying) return;

            while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

            var currentLanguage = index > -1 ? index : 0;
            LocalisationSettings.Instance.CurrentLanguageIndex = currentLanguage;
            Data.LanguageChoice = LocalisationSettings.Instance.Languages[index];
            StoreGeneralData(Data);

            Text_C.OnLanguageChanged();
            Textfield_C.OnLanguageChanged();
            Dropdown_C.OnLanguageChanged();
            Toggle_C.OnLanguageChanged();
            Slider_C.OnLanguageChanged();
            //CustomLoadingBar.OnLanguageChanged();

            LanguageDropdown.index = index;
        }

        #endregion
    }
}
