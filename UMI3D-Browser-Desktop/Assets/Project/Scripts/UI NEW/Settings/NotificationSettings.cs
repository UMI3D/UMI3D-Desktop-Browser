using System;
using umi3d.baseBrowser.preferences;
using umi3d.commonScreen.game;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class NotificationSettings : BaseSettings
{
    private RadioButton m_HideNotificationOn;
    private RadioButton m_HideNotificationOff;

    private NotificationData m_NotificationData;

    public NotificationSettings(VisualElement pRoot) : base(pRoot)
    {
        SetupHideNotificaiton();

        SetupValues();
    }

    private void SetupValues()
    {
        if (TryGetNotificationData(out m_NotificationData))
        {
            if (m_NotificationData.HideNotification)
                m_HideNotificationOn.value = true;
            else
                m_HideNotificationOff.value = true;

            OnHideNotificationChanged(m_NotificationData.HideNotification);
        } else
        {
            m_HideNotificationOff.value = true;
            OnHideNotificationChanged(false);
        }
    }

    private void SetupHideNotificaiton()
    {
        m_HideNotificationOn = m_Root.Q("HideNotification").Q<RadioButton>("On");
        m_HideNotificationOff = m_Root.Q("HideNotification").Q<RadioButton>("Off");

        m_HideNotificationOn.RegisterValueChangedCallback(e 
            => OnHideNotificationChanged(e.newValue));
    }

    private void OnHideNotificationChanged(bool pValue)
    {
        InformationArea_C.HideNotification = pValue;
        m_NotificationData.HideNotification = pValue;
        StoreNotificationData(m_NotificationData);
    }
    public static bool TryGetNotificationData(out NotificationData data) => PreferencesManager.TryGet(out data, c_notificationPath, c_dataFolderPath);
    public static void StoreNotificationData(NotificationData data) => PreferencesManager.StoreData(data, c_notificationPath, c_dataFolderPath);

}