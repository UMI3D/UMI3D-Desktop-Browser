using System;
using System.Linq;
using umi3d.baseBrowser.preferences;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;
using static UnityEngine.Rendering.DebugUI;

public class AudioSettings : BaseSettings
{
    private Dropdown_C m_Microphone;

    private AudioData m_AudioData;

    public AudioSettings(VisualElement pRoot) : base(pRoot)
    {
        SetupMicrophone(pRoot);
        SetupMicroMode(pRoot);

        SetupValues();
    }

    private void SetupValues()
    {
        if (TryGetAudiorData(out m_AudioData))
        {
            // Micro
            m_Microphone.value = m_AudioData.CurrentMic;
            OnMicrophoneChanged(m_Microphone.value);
        }
        else
        {
            // Micro
            m_Microphone.value = m_Microphone.choices.FirstOrDefault();
            OnMicrophoneChanged(m_Microphone.value);
        }
    }

    #region Microphone
    private void SetupMicrophone(VisualElement pRoot)
    {
        m_Microphone = pRoot.Q<Dropdown_C>("Microphone");
        m_Microphone.choices = MicrophoneListener.GetMicrophonesNames().ToList();

        m_Microphone.RegisterValueChangedCallback(microphone => OnMicrophoneChanged(microphone.newValue));
    }

    private void OnMicrophoneChanged(string pMicrophone)
    {
        if (MicrophoneListener.Exists && pMicrophone == MicrophoneListener.Instance.GetCurrentMicrophoneName())
            return;

        if (MicrophoneListener.Exists)
            _ = MicrophoneListener.Instance.SetCurrentMicrophoneName(pMicrophone);

        m_AudioData.CurrentMic = pMicrophone;
        StoreAudioData(m_AudioData);
    }
    #endregion

    #region MicroMode
    private void SetupMicroMode(VisualElement pRoot)
    {

    }

    private void OnMicroModeChanged(MicModeEnum pMicroMode)
    {
        switch (pMicroMode)
        {
            case MicModeEnum.AlwaysSend:
                // HIDE ALL
                break;
            case MicModeEnum.Amplitude:
                // SHOW AMPLUTUDE SLIDER
                // SHOW DELAY
                break;
            case MicModeEnum.PushToTalk:
                // HIDE ALL
                break;
            default:
                break;
        }

        if (!Enum.TryParse<MicrophoneMode>(pMicroMode.ToString(), out var valueEnum))
            return;

        if (MicrophoneListener.Exists)
        {
            if (valueEnum == MicrophoneListener.Instance.GetCurrentMicrophoneMode())
                return;
            MicrophoneListener.Instance.SetCurrentMicrophoneMode(valueEnum);
        }

        m_AudioData.Mode = pMicroMode;
        StoreAudioData(m_AudioData);
    }
    #endregion

    public static void StoreAudioData(AudioData pData) => PreferencesManager.StoreData(pData, c_audioPath, c_dataFolderPath);
}