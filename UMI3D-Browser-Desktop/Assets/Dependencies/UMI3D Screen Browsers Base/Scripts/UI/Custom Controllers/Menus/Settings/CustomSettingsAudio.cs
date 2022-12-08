/*
Copyright 2019 - 2022 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsAudio : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-audio";

    public CustomSlider GeneralVolume_Visual;
    public CustomDropdown MicDropdown;
    public CustomToggle NoiseReductionToggle;
    public CustomSegmentedPicker<MicModeEnum> MicModeSegmentedPicker;
    public CustomThresholdSlider AmplitudeSlider;
    public CustomTextfield DelayBeaforeShutingMicTextfield;
    public CustomDropdown PushToTalkKeyDropdown;
    public CustomButton LoopBackButton;

    public override void InitElement()
    {
        base.InitElement();

        RegisterCallback<AttachToPanelEvent>(ce =>
        {
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.debugSampling = true;
            OnLoopBackValueChanged(false);
        });

        RegisterCallback<DetachFromPanelEvent>(ce =>
        {
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.debugSampling = false;
            OnLoopBackValueChanged(false);
        });

        this.schedule.Execute(() =>
        {
            MicDropdown.choices = umi3d.cdk.collaboration.MicrophoneListener.GetMicrophonesNames().ToList();
        }).Every(1000);

        this.schedule.Execute(() =>
        {
            if (MicModeSegmentedPicker.ValueEnum != MicModeEnum.Amplitude) return;
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                AmplitudeSlider.ContentValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.rms * 10f;
        }).Every(200);

        GeneralVolume_Visual.label = "General volume";
        GeneralVolume_Visual.DirectionDisplayer = ElemnetDirection.Leading;
        GeneralVolume_Visual.lowValue = 0f;
        GeneralVolume_Visual.highValue = 10f;
        GeneralVolume_Visual.showInputField = true;
        GeneralVolume_Visual.RegisterValueChangedCallback(ce => OnGeneralVolumeValueChanged(ce.newValue));
        GeneralVolume_Visual.RegisterCallback<AttachToPanelEvent>(callback => GeneralVolume_Visual.SetValueWithoutNotify(Data.GeneralVolume));
        ScrollView.Add(GeneralVolume_Visual);

        MicDropdown.label = "Microphone";
        MicDropdown.RegisterValueChangedCallback(ce => OnMicDropdownValueChanged(ce.newValue));
#if UNITY_STANDALONE
        ScrollView.Add(MicDropdown);
#endif

        NoiseReductionToggle.label = "Use noise reduction";
        NoiseReductionToggle.RegisterValueChangedCallback(ce => OnNoiseReductionValueChanged(ce.newValue));
#if UNITY_STANDALONE
        ScrollView.Add(NoiseReductionToggle);
#endif

        MicModeSegmentedPicker.Label = "Mode";
        MicModeSegmentedPicker.ValueEnumChanged += value => OnMicModeValueChanged(value);
#if UNITY_STANDALONE
        ScrollView.Add(MicModeSegmentedPicker);
#endif

        AmplitudeSlider.label = "Noise Threshold";
        AmplitudeSlider.RegisterValueChangedCallback(ce => OnAmplitudeValueChanged(ce.newValue));
        AmplitudeSlider.lowValue = 0f;
        AmplitudeSlider.highValue = 1f;
#if UNITY_STANDALONE
        ScrollView.Add(AmplitudeSlider);
#endif

        DelayBeaforeShutingMicTextfield.label = "Delay before mute mic when lower than threshold";
        DelayBeaforeShutingMicTextfield.RegisterValueChangedCallback(ce => OnDelayBeforeShutingMicValueChanged(ce.newValue));
#if UNITY_STANDALONE
        ScrollView.Add(DelayBeaforeShutingMicTextfield);
#endif

        PushToTalkKeyDropdown.label = "Push to talk key";
        PushToTalkKeyDropdown.RegisterValueChangedCallback(ce => OnPushToTalkValueChanged(ce.newValue));
        PushToTalkKeyDropdown.choices = Enum.GetNames(typeof(KeyCode)).ToList();
#if UNITY_STANDALONE
        ScrollView.Add(PushToTalkKeyDropdown);
#endif

        LoopBackButton.ClickedDown += () => OnLoopBackValueChanged(!m_loopBack);
        ScrollView.Add(LoopBackButton);
    }

    public override void Set() => Set("Audio");

    #region Implementation

    /// <summary>
    /// Value is between 0 and 1
    /// </summary>
    public event System.Action<float> GeneralVolumeValeChanged;
    public AudioData Data;

    protected bool m_loopBack;

#if UNITY_STANDALONE
    /// <summary>
    /// Set the audio for a desktop browser.
    /// </summary>
    public void SetAudio()
    {
        if (umi3d.cdk.collaboration.MicrophoneListener.Exists) umi3d.cdk.collaboration.MicrophoneListener.Instance.inputType = umi3d.cdk.collaboration.MicrophoneInputType.NAudio;

        var mics = umi3d.cdk.collaboration.MicrophoneListener.GetMicrophonesNames().ToList();
        MicDropdown.choices = mics;

        umi3d.cdk.collaboration.MicrophoneListener.OnSaturated.AddListener(value => AmplitudeSlider.IsSaturated = value);

        if (TryGetAudiorData(out Data))
        {
            OnGeneralVolumeValueChanged(Data.GeneralVolume);
            if (mics.Contains(Data.CurrentMic)) OnMicDropdownValueChanged(Data.CurrentMic);
            OnNoiseReductionValueChanged(Data.NoiseReduction);
            OnMicModeValueChanged(Data.Mode);
            OnAmplitudeValueChanged(Data.Amplitude);
            OnDelayBeforeShutingMicValueChanged(Data.DelayBeforeShutMic.ToString());
            OnPushToTalkValueChanged(Data.PushToTalkKey.ToString());
        }
        else
        {
            OnGeneralVolumeValueChanged(10f);
            string mic;
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists) mic = umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName();
            else mic = MicDropdown.choices.FirstOrDefault();
            OnMicDropdownValueChanged(mic);
            OnNoiseReductionValueChanged(true);
            OnMicModeValueChanged(MicModeEnum.AlwaysSend);
            OnAmplitudeValueChanged(0f);
            OnDelayBeforeShutingMicValueChanged("0");
            OnPushToTalkValueChanged(KeyCode.M.ToString());
        }
    }
#else
    /// <summary>
    /// Set the audio for a mobile browser.
    /// </summary>
    public void SetAudio()
    {
        if (TryGetAudiorData(out Data)) OnGeneralVolumeValueChanged(Data.GeneralVolume);
        else OnGeneralVolumeValueChanged(10f);
    }
#endif

    public void OnGeneralVolumeValueChanged(float value)
    {
        SetGeneralVolumeValueWithoutNotify(value);
        GeneralVolumeValeChanged?.Invoke(((int)value) / 10f);
    }
    public void SetGeneralVolumeValueWithoutNotify(float value)
    {
        value = (int)value;
        GeneralVolume_Visual.SetValueWithoutNotify(value);
        Data.GeneralVolume = value;
        StoreAudioData(Data);
    }

    public void OnMicDropdownValueChanged(string value)
    {
        MicDropdown.SetValueWithoutNotify(value);

        if (!MicDropdown.choices.Contains(value)
            || (umi3d.cdk.collaboration.MicrophoneListener.Exists && value == umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName()))
            return;

        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
            umi3d.cdk.collaboration.MicrophoneListener.Instance.SetCurrentMicrophoneName(value);

        Data.CurrentMic = value;
        StoreAudioData(Data);
    }


    public void OnNoiseReductionValueChanged(bool value)
    {
        NoiseReductionToggle.SetValueWithoutNotify(value);

#if UNITY_STANDALONE
        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
            umi3d.cdk.collaboration.MicrophoneListener.Instance.UseNoiseReduction = value;
#endif

        Data.NoiseReduction = value;
        StoreAudioData(Data);
    }

    public void OnMicModeValueChanged(MicModeEnum value)
    {
        MicModeSegmentedPicker.SetValueEnumWithoutNotify(value);
        switch (value)
        {
            case MicModeEnum.AlwaysSend:
                AmplitudeSlider.Hide();
                DelayBeaforeShutingMicTextfield.Hide();
                PushToTalkKeyDropdown.Hide();
                break;
            case MicModeEnum.Amplitude:
                AmplitudeSlider.Display();
                DelayBeaforeShutingMicTextfield.Display();
                PushToTalkKeyDropdown.Hide();
                break;
            case MicModeEnum.PushToTalk:
                AmplitudeSlider.Hide();
                DelayBeaforeShutingMicTextfield.Hide();
                PushToTalkKeyDropdown.Display();
                break;
            default:
                break;
        }
        if (!Enum.TryParse<umi3d.cdk.collaboration.MicrophoneMode>(value.ToString(), out var valueEnum))
            return;

        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
        {
            if (valueEnum == umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneMode())
                return;
            umi3d.cdk.collaboration.MicrophoneListener.Instance.SetCurrentMicrophoneMode(valueEnum);
        }

        Data.Mode = value;
        StoreAudioData(Data);
    }

    public void OnAmplitudeValueChanged(float value)
    {
        AmplitudeSlider.SetValueWithoutNotify(value);

        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
            umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend = value / 10f;

        Data.Amplitude = value;
        StoreAudioData(Data);
    }

    public void OnDelayBeforeShutingMicValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            DelayBeaforeShutingMicTextfield.SetValueWithoutNotify(value);

            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.voiceStopingDelaySeconds = 0f;

            Data.DelayBeforeShutMic = 0f;
            StoreAudioData(Data);
        }
        else if (float.TryParse(value, out var valueFloat))
        {
            DelayBeaforeShutingMicTextfield.SetValueWithoutNotify(valueFloat.ToString());

            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.voiceStopingDelaySeconds = valueFloat;

            Data.DelayBeforeShutMic = valueFloat;
            StoreAudioData(Data);
        }
        else DelayBeaforeShutingMicTextfield.SetValueWithoutNotify(Data.DelayBeforeShutMic.ToString());
    }


    public void OnPushToTalkValueChanged(string value)
    {
        PushToTalkKeyDropdown.SetValueWithoutNotify(value);
        if (!Enum.TryParse<KeyCode>(value.ToString(), out var valueEnum))
            return;

        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
            umi3d.cdk.collaboration.MicrophoneListener.Instance.pushToTalkKeycode = valueEnum;

        Data.PushToTalkKey = valueEnum;
        StoreAudioData(Data);
    }

    protected void OnLoopBackValueChanged(bool value)
    {
        m_loopBack = value;
        LoopBackButton.text = m_loopBack ? "Loop back on" : "Loop back off";
        if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
            umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback = m_loopBack;
    }

    #endregion
}
