using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] protected UIDocument m_UiDocument;
    [SerializeField] protected WindowsManager m_WindowManager;

    private VisualElement m_WindowBar;
    private Button m_WindowBarButton;
    private VisualElement m_ErrorBox;
    private SettingScreen m_Settings;

    protected List<BaseScreen> m_Screens = new();
    protected BaseScreen m_MainScreen;

    protected virtual void Start()
    {
        Debug.Assert(m_UiDocument != null, "UI Document null");

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        m_UiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        SetupWindowBar();
        SetupSettings();
        SetupErrorBox();
        SetupReportBug();

        InitLocalisation();

        m_WindowManager.FullScreenEnabled = value =>
        {
            if (value)
                m_UiDocument.rootVisualElement.Q("Header").RemoveFromClassList("hidden");
            else
                m_UiDocument.rootVisualElement.Q("Header").AddToClassList("hidden");
        };
        m_UiDocument.rootVisualElement.Q<Button>("Quit").clicked += () =>
        {
            QuittingManager.ApplicationIsQuitting = true;
            Application.Quit();
        };
        m_UiDocument.rootVisualElement.Q<Button>("Minimize").clicked += m_WindowManager.Minimize;
        m_UiDocument.rootVisualElement.Q<Button>("MinimizeWindow").clicked += m_WindowManager.Maximize;
    }

    private void SetupReportBug()
    {
        var reportBug = m_UiDocument.rootVisualElement.Q("ReportBug");

        m_UiDocument.rootVisualElement.Q<Button>("ButtonBug").clicked += () =>
        {
            reportBug.RemoveFromClassList("hidden");
        };

        reportBug.Q<Button>("ButtonClose").clicked += () =>
        {
            reportBug.AddToClassList("hidden");
        };
    }

    private void SetupWindowBar()
    {
        m_WindowBar = m_UiDocument.rootVisualElement.Q("WindowBar");

        m_WindowBarButton = m_UiDocument.rootVisualElement.Q<Button>("DisplayWindowBar");
        m_WindowBarButton.clicked += () =>
        {
            if (m_WindowBar.GetClasses().Contains("hidden"))
            {
                m_WindowBarButton.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                m_WindowBar.RemoveFromClassList("hidden");
            }
            else
            {
                m_WindowBarButton.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                m_WindowBar.AddToClassList("hidden");
            }

        };
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