using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralSettings : BaseSettings
{
    private Dropdown_C m_Language;

    public GeneralSettings(VisualElement pRoot) : base(pRoot)
    {
        SetupLanguage(pRoot);
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
}