using System.Collections.Generic;
using UnityEngine.UIElements;

public class SettingScreen : BaseScreen
{
    private GeneralSettings m_General;
    private AudioSettings m_Audio;
    private GraphicsSettings m_Graphics;
    private ControlsSettings m_Controls;
    private NotificationSettings m_Notifications;

    public SettingScreen(VisualElement pElement) : base(pElement)
    {
        m_General = new GeneralSettings(pElement.Q("General"));
        m_Audio = new AudioSettings(pElement.Q("Audio"));
        m_Graphics = new GraphicsSettings(pElement.Q("Graphics"));
        m_Controls = new ControlsSettings(pElement.Q("Controls"));
        m_Notifications = new NotificationSettings(pElement.Q("Notification"));

        pElement.Q<RadioButton>("ButtonGeneral").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_General));
        pElement.Q<RadioButton>("ButtonAudio").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Audio));
        pElement.Q<RadioButton>("ButtonGraphics").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Graphics));
        pElement.Q<RadioButton>("ButtonControls").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Controls));
        pElement.Q<RadioButton>("ButtonNotification").RegisterValueChangedCallback(callback => MenuValueChanged(callback, m_Notifications));
    }

    private void MenuValueChanged(ChangeEvent<bool> callback, BaseSettings settings)
    {
        if (callback.newValue)
            settings.Show();
        else
            settings.Hide();
    }
}