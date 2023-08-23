using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettings : BaseSettings
{
    private Dropdown_C m_Language;
    private Dropdown_C m_Theme;

    public GeneralSettings(VisualElement pRoot) : base(pRoot)
    {
        SetupLanguage(pRoot);
        SetupTheme(pRoot);
    }

    private void SetupLanguage(VisualElement pRoot)
    {
        m_Language = pRoot.Q<Dropdown_C>("Language");
        m_Language.choices = LocalisationSettings.Instance.Languages.Select(language => language.Name).ToList();
        m_Language.value = LocalisationSettings.Instance.CurrentLanguage.Name.ToString();

        m_Language.RegisterValueChangedCallback(language =>
        {
            LocalisationSettings.Instance.CurrentLanguageIndex =
                LocalisationSettings.Instance.Languages.FindIndex(l => l.Name == language.newValue);
            Debug.LogWarning("TODO : Correct change of traduciton!");
        });
    }

    private void SetupTheme(VisualElement pRoot)
    {
        m_Language = pRoot.Q<Dropdown_C>("Theme");
        m_Language.choices = new List<string>() { "Dark", "Light" };
        m_Language.value = "Dark";

        m_Language.RegisterValueChangedCallback(language =>
        {
            Debug.LogWarning("TODO : THEME SELECTION!");
        });
    }
}