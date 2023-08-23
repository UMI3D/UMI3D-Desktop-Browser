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
    private RadioButton m_MicroModeAlwaysSend;
    private RadioButton m_MicroModeAmplitude;
    private RadioButton m_MicroModePushToTalk;

    private SliderFloat_C m_AmplitudeValue;
    private Numeral_C m_DelayMuteMic;

    private AudioData m_AudioData;

    public AudioSettings(VisualElement pRoot) : base(pRoot)
    {
        SetupMicrophone();
        SetupMicroMode();

        SetupValues();
    }

    private void SetupValues()
    {
        if (TryGetAudiorData(out m_AudioData))
        {
            // Micro
            m_Microphone.value = m_AudioData.CurrentMic;
            // Mode
            switch (m_AudioData.Mode)
            {
                case MicModeEnum.AlwaysSend:
                    m_MicroModeAlwaysSend.value = true;
                    break;
                case MicModeEnum.Amplitude:
                    m_MicroModeAmplitude.value = true;
                    break;
                case MicModeEnum.PushToTalk:
                    m_MicroModePushToTalk.value = true;
                    break;
                default:
                    break;
            }
            OnMicroModeChanged(true, m_AudioData.Mode);
            m_AmplitudeValue.value = m_AudioData.Amplitude;
            m_DelayMuteMic.value = m_AudioData.DelayBeforeShutMic.ToString();
        }
        else
        {
            // Micro
            m_Microphone.value = m_Microphone.choices.FirstOrDefault();
            // Mode
            m_MicroModeAlwaysSend.value = true;
            OnMicroModeChanged(true, MicModeEnum.AlwaysSend);
            m_AmplitudeValue.value = 0f;
            m_DelayMuteMic.value = "0";
        }

        OnMicrophoneChanged(m_Microphone.value);

        OnAmplitudeChanged(m_AmplitudeValue.value);
        OnDelayMuteMicChanged(m_DelayMuteMic.value);
    }

    #region Microphone
    private void SetupMicrophone()
    {
        m_Microphone = m_Root.Q<Dropdown_C>("Microphone");
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
    private void SetupMicroMode()
    {
        m_AmplitudeValue = m_Root.Q<SliderFloat_C>("AmplitudeValue");
        m_DelayMuteMic = m_Root.Q<Numeral_C>("DelayBeforeMuteMic");

        m_AmplitudeValue.RegisterValueChangedCallback(e => OnAmplitudeChanged(e.newValue));
        m_DelayMuteMic.RegisterValueChangedCallback(e => OnDelayMuteMicChanged(e.newValue));

        m_MicroModeAlwaysSend = m_Root.Q<RadioButton>("AlwaysSend");
        m_MicroModeAmplitude = m_Root.Q<RadioButton>("Amplitude");
        m_MicroModePushToTalk = m_Root.Q<RadioButton>("PushToTalk");

        m_MicroModeAlwaysSend.RegisterValueChangedCallback(e 
            => OnMicroModeChanged(e.newValue, MicModeEnum.AlwaysSend));
        m_MicroModeAmplitude.RegisterValueChangedCallback(e
            => OnMicroModeChanged(e.newValue, MicModeEnum.Amplitude));
        m_MicroModePushToTalk.RegisterValueChangedCallback(e
            => OnMicroModeChanged(e.newValue, MicModeEnum.PushToTalk));
    }

    private void OnAmplitudeChanged(float value)
    {
        if (MicrophoneListener.Exists)
            MicrophoneListener.Instance.minAmplitudeToSend = value / 10f;

        m_AudioData.Amplitude = value;
        StoreAudioData(m_AudioData);
    }

    private void OnDelayMuteMicChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) value = "0";
        if (float.TryParse(value, out var valueFloat))
        {
            if (MicrophoneListener.Exists)
                MicrophoneListener.Instance.voiceStopingDelaySeconds = valueFloat;

            m_AudioData.DelayBeforeShutMic = valueFloat;
            StoreAudioData(m_AudioData);
        }
        else m_DelayMuteMic.SetValueWithoutNotify(m_AudioData.DelayBeforeShutMic.ToString());

    }

    private void OnMicroModeChanged(bool pActive, MicModeEnum pMicroMode)
    {
        if (!pActive) return;
        switch (pMicroMode)
        {
            case MicModeEnum.AlwaysSend:
                m_AmplitudeValue.AddToClassList("hidden");
                m_DelayMuteMic.AddToClassList("hidden");
                break;
            case MicModeEnum.Amplitude:
                m_AmplitudeValue.RemoveFromClassList("hidden");
                m_DelayMuteMic.RemoveFromClassList("hidden");
                break;
            case MicModeEnum.PushToTalk:
                m_AmplitudeValue.AddToClassList("hidden");
                m_DelayMuteMic.AddToClassList("hidden");
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