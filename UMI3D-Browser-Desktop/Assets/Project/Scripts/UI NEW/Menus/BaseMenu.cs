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

    private ScreenTooltip m_ScreenTooltip;

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

        m_ScreenTooltip = new ScreenTooltip(m_UiDocument.rootVisualElement.Q("PageTooltip"));
        m_UiDocument.rootVisualElement.Q<Button>("ButtonHelp").clicked += m_ScreenTooltip.Show;

        LocalisationUiToolkit.InitLocalisation(m_UiDocument);
        ShowScreen(m_MainScreen);
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
        Debug.Log(m_ScreenTooltip);
        Debug.Log(m_ScreenTooltip.LstTooltip);
        Debug.Log(pScreen.LstTooltip);
        m_ScreenTooltip.LstTooltip = pScreen.LstTooltip;
        pScreen.Show();
    }
}