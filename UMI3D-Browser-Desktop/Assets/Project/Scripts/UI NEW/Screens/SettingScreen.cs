using System;
using UnityEngine.UIElements;

public class SettingScreen : BaseScreen
{
    private GeneralSettings m_General;
    private AudioSettings m_Audio;
    private GraphicsSettings m_Graphics;
    private ControlsSettings m_Controls;
    private NotificationSettings m_Notifications;

    private Button m_Back;

    public Button Back => m_Back;

    public event Action OnLanguageChanged;

    public SettingScreen(VisualElement pRoot) : base(pRoot)
    {
        m_General = new GeneralSettings(m_Root.Q("General"));
        m_Audio = new AudioSettings(m_Root.Q("Audio"));
        m_Graphics = new GraphicsSettings(m_Root.Q("Graphics"));
        m_Controls = new ControlsSettings(m_Root.Q("Controls"));
        m_Notifications = new NotificationSettings(m_Root.Q("Notification"));

        m_Root.Q<RadioButton>("ButtonGeneral").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_General));
        m_Root.Q<RadioButton>("ButtonAudio").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Audio));
        m_Root.Q<RadioButton>("ButtonGraphics").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Graphics));
        m_Root.Q<RadioButton>("ButtonControls").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Controls));
        m_Root.Q<RadioButton>("ButtonNotification").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Notifications));

        m_Back = m_Root.Q<Button>("ButtonBack");

        m_General.OnLanguageChanged += () => OnLanguageChanged?.Invoke();
    }

    private void MenuValueChanged(ChangeEvent<bool> callback, BaseSettings settings)
    {
        if (callback.newValue)
            settings.Show();
        else
            settings.Hide();
    }
}