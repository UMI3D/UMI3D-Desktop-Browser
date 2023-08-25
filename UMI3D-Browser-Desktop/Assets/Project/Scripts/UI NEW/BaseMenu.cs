using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] protected UIDocument m_UiDocument;

    private VisualElement m_ErrorBox;
    private SettingScreen m_Settings;

    protected List<BaseScreen> m_Screens = new();
    protected BaseScreen m_MainScreen;

    protected virtual void Start()
    {
        Debug.Assert(m_UiDocument != null, "UI Document null");

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        m_UiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        SetupSettings();
        SetupErrorBox();

        InitLocalisation();
    }

    private void SetupSettings()
    {
        m_Settings = new SettingScreen(m_UiDocument.rootVisualElement.Q("Settings"));
        m_Screens.Add(m_Settings);

        m_Settings.Back.clicked += () => ShowScreen(m_MainScreen);

        m_UiDocument.rootVisualElement.Q<Button>("ButtonSettings").clicked += () => ShowScreen(m_Settings);

        m_Settings.OnLanguageChanged += InitLocalisation;
        m_Settings.OnLanguageChanged += () => Debug.Log("test");
    }

    protected void InitLocalisation()
    {
        var textFields = m_UiDocument.rootVisualElement.Query<TextField>().ToList();

        foreach (var textField in textFields)
        {
            if (textField.label == "") continue;
            if (textField.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(textField.name);
            if (trad != null)
                textField.label = trad;
        }

        var radioButtonGroups = m_UiDocument.rootVisualElement.Query<RadioButtonGroup>().ToList();

        foreach (var radioButtonGroup in radioButtonGroups)
        {
            if (radioButtonGroup.label == "") continue;
            if (radioButtonGroup.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(radioButtonGroup.name);
            if (trad != null)
                radioButtonGroup.label = trad;
        }

        var sliders = m_UiDocument.rootVisualElement.Query<Slider>().ToList();

        foreach (var slider in sliders)
        {
            if (slider.label == "") continue;
            if (slider.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(slider.name);
            if (trad != null)
                slider.label = trad;
        }

        var dropdowns = m_UiDocument.rootVisualElement.Query<DropdownField>().ToList();

        foreach (var dropdown in dropdowns)
        {
            if (dropdown.label == "") continue;
            if (dropdown.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(dropdown.name);
            if (trad != null)
                dropdown.label = trad;
        }

        var radioButtons = m_UiDocument.rootVisualElement.Query<RadioButton>().ToList();

        foreach (var radioButton in radioButtons)
        {
            if (radioButton.label == "") continue;
            if (radioButton.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(radioButton.name);
            if (trad != null)
                radioButton.label = trad;
        }

        var labels = m_UiDocument.rootVisualElement.Query<TextElement>().ToList();

        foreach (var label in labels)
        {
            if (label.text == "") continue;
            if (label.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(label.name);
            if (trad != null)
                label.text = trad;
        }
    }

    protected void ShowScreen(BaseScreen pScreen)
    {
        foreach (var screen in m_Screens)
        {
            screen.Hide();
        }
        pScreen.Show();
    }

    private void SetupErrorBox()
    {
        m_ErrorBox = m_UiDocument.rootVisualElement.Q("ErrorBox");
        m_ErrorBox.Q<Button>("ButtonOk").clicked += () => m_ErrorBox.AddToClassList("hidden");
        m_ErrorBox.AddToClassList("hidden");
    }

    protected void OpenErrorBox(string message)
    {
        m_ErrorBox.Q<TextElement>("Message").text = message;
        m_ErrorBox.RemoveFromClassList("hidden");
    }
}