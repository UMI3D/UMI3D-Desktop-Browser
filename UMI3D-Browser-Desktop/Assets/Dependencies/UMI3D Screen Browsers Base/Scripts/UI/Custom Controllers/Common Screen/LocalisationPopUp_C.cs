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
using System.Threading.Tasks;
using umi3d.baseBrowser.preferences;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class LocalisationPopUp_C : VisualElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;
            base.Init(ve, bag, cc);
            var custom = ve as LocalisationPopUp_C;
        }
    }

    private VisualElement m_Background = new VisualElement() { name = "LocalisationPopUp" };
    private Text_C m_Message = new Text_C();
    private DropdownDisplayer_C m_LanguageDropdown = new DropdownDisplayer_C();
    private ButtonDisplayer_C m_Submit = new ButtonDisplayer_C();

    public GeneralData Data;

    public LocalisationPopUp_C()
    {
        InitElement();
    }

    public virtual void InitElement()
    {
        m_Message.LocalisedText = new("Select the language the app will use.", "LauncherScreen", "LocalisationChoice");

        m_LanguageDropdown.LocalisedLabel = new LocalisationAttribute("Language", "GeneralSettings", "Language");
        var localisedOptions = new List<LocalisationAttribute>();
        List<Language> languages = LocalisationSettings.Instance.Languages;
        foreach (var language in languages)
        {
            localisedOptions.Add(new LocalisationAttribute(language.Name, "GeneralSettings", language.Name));
        }
        m_LanguageDropdown.LocalisedOptions = localisedOptions;
        m_LanguageDropdown.index = 0;
        m_LanguageDropdown.ValueChanged += (index, newValue) => _ = LanguageValueChanged(index);

        m_Submit.LocaliseText = new LocalisationAttribute("Accept", "GenericStrings", "Accept");
        m_Submit.clicked += () =>
        {
            _ = LanguageValueChanged(m_LanguageDropdown.index, true);
            RemoveFromHierarchy();
        };

        m_Background.Add(m_Message);
        m_Background.Add(m_LanguageDropdown);
        m_Background.Add(m_Submit);

        Add(m_Background);

        style.position = Position.Absolute;
        style.minHeight = new Length(100, LengthUnit.Percent);
        style.minWidth = new Length(100, LengthUnit.Percent);
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
        style.backgroundColor = new Color(38f / 255f, 38f / 255f, 38f / 255f, 0.5f);
        m_Background.style.height = 300;
        m_Background.style.width = 700;
        m_Background.style.backgroundColor = new Color(38f/255f, 38f/255f, 38f/255f);
        m_Background.style.paddingRight = 40;
        m_Background.style.paddingLeft = 40;
        m_Background.style.paddingTop = 40;
        m_Background.style.paddingBottom = 40;
        m_Background.style.justifyContent = Justify.Center;
        m_Background.style.borderBottomLeftRadius = 16;
        m_Background.style.borderTopLeftRadius = 16;
        m_Background.style.borderBottomRightRadius = 16;
        m_Background.style.borderTopRightRadius = 16;
        style.alignItems = Align.Center;
        m_Message.style.marginBottom = 16;
    }

    private async Task LanguageValueChanged(int index, bool hasChoosen = false)
    {
        if (!UnityEngine.Application.isPlaying) return;

        while (!LocalisationManager.Exists) await UMI3DAsyncManager.Yield();

        var currentLanguage = index > -1 ? index : 0;
        LocalisationSettings.Instance.CurrentLanguageIndex = currentLanguage;

        if (hasChoosen)
        {
            Data.LanguageChoice = LocalisationSettings.Instance.Languages[index];
            Data.HasChosenLanguage = true;
            StoreGeneralData(Data);
        }

        Text_C.OnLanguageChanged();
        Textfield_C.OnLanguageChanged();
        Dropdown_C.OnLanguageChanged();
        Toggle_C.OnLanguageChanged();
        Slider_C.OnLanguageChanged();
        //CustomLoadingBar.OnLanguageChanged();

        m_LanguageDropdown.index = index;
    }
}
