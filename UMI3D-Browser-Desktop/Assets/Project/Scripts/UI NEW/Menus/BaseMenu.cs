using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] 
    protected UIDocument m_UiDocument;

    protected List<BaseScreen> m_LstScreen = new();
    protected BaseScreen m_MainScreen;

    private SettingScreen m_Settings;

    protected InfoBox m_InfoBox;
    protected InfoBox m_ErrorBox;

    protected virtual void Start()
    {
        Debug.Assert(m_UiDocument != null, "UI Document null");

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        m_UiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        SetupSettings();

        m_InfoBox = new InfoBox(m_UiDocument.rootVisualElement.Q("InfoBox"));
        m_ErrorBox = new InfoBox(m_UiDocument.rootVisualElement.Q("ErrorBox"));
        
        m_InfoBox.OnOpened += m_ErrorBox.Hide;
        m_ErrorBox.OnOpened += m_InfoBox.Hide;

        LocalisationUiToolkit.InitLocalisation(m_UiDocument);
    }

    private void SetupSettings()
    {
        m_Settings = new SettingScreen(m_UiDocument.rootVisualElement.Q("Settings"));
        m_LstScreen.Add(m_Settings);

        m_Settings.Back.clicked += () => ShowScreen(m_MainScreen);

        m_UiDocument.rootVisualElement.Q<Button>("ButtonSettings").clicked += () => ShowScreen(m_Settings);

        m_Settings.OnLanguageChanged += () => LocalisationUiToolkit.InitLocalisation(m_UiDocument);
    }

    protected void ShowScreen(BaseScreen pScreen)
    {
        foreach (var screen in m_LstScreen)
        {
            screen.Hide();
        }
        pScreen.Show();
    }
}