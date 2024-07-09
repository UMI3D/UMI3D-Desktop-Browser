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

using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using static umi3d.baseBrowser.preferences.SettingsPreferences;
using umi3d.commonScreen.Displayer;
using System.Collections.Generic;

namespace umi3d.commonScreen.menu
{
    public class SettingsAudio_C : BaseSettingScreen_C
    {
        public override string UssCustomClass_Emc => "setting-audio";

        public Slider_C GeneralVolume_Visual = new Slider_C { name = "general-volume" };
        public Dropdown_C MicDropdown = new Dropdown_C { name = "mic" };
        public Toggle_C NoiseReductionToggle = new Toggle_C();
        public SegmentedPicker_C<MicModeEnum> MicModeSegmentedPicker = new SegmentedPicker_C<MicModeEnum>();
        public ThresholdSlider_C AmplitudeSlider = new ThresholdSlider_C();
        public Textfield_C DelayBeaforeShutingMicTextfield = new Textfield_C();
        public Dropdown_C PushToTalkKeyDropdown = new Dropdown_C { name = "push-to-talk" };
        public Button_C LoopBackButton = new Button_C { name = "loop-back" };
        public Button_C ResetAudioConfButton = new Button_C { name = "reset-audio" };

        public SettingsAudio_C() { }

        protected override void InitElement()
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
                MicDropdown.LocalisedOptions = umi3d.cdk.collaboration.MicrophoneListener.GetMicrophonesNames().ToList();
            }).Every(1000);

            this.schedule.Execute(() =>
            {
                if (MicModeSegmentedPicker.ValueEnum != MicModeEnum.Amplitude) return;
                if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                    AmplitudeSlider.ContentValue = umi3d.cdk.collaboration.MicrophoneListener.Instance.rms * 10f;
            }).Every(200);

            GeneralVolume_Visual.LocaliseLabel = new LocalisationAttribute("General volume", "AudioSettings", "GeneralVolume");
            GeneralVolume_Visual.DirectionDisplayer = ElementDirection.Leading;
            GeneralVolume_Visual.lowValue = 0f;
            GeneralVolume_Visual.highValue = 10f;
            GeneralVolume_Visual.showInputField = true;
            GeneralVolume_Visual.RegisterValueChangedCallback(ce => OnGeneralVolumeValueChanged(ce.newValue, ce.newValue));
            GeneralVolume_Visual.RegisterCallback<AttachToPanelEvent>(callback => GeneralVolume_Visual.SetValueWithoutNotify(Data.GeneralVolume));
            ScrollView.Add(GeneralVolume_Visual);

            MicDropdown.LocalisedLabel = new LocalisationAttribute("Microphone", "AudioSettings", "Mic_Label");
            MicDropdown.RegisterValueChangedCallback(ce => OnMicDropdownValueChanged(ce.newValue));
#if UNITY_STANDALONE
            ScrollView.Add(MicDropdown);
#endif

            NoiseReductionToggle.LocaliseLabel = new LocalisationAttribute("Use noise reduction", "AudioSettings", "NoiseReduc_Label");
            NoiseReductionToggle.RegisterValueChangedCallback(ce => OnNoiseReductionValueChanged(ce.newValue));
#if UNITY_STANDALONE
            ScrollView.Add(NoiseReductionToggle);
#endif

            MicModeSegmentedPicker.LocalisedLabel = new LocalisationAttribute("Mode", "AudioSettings", "MicMode_Label");
            MicModeSegmentedPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Always send", "AudioSettings", "MicMode_AlwaysSend"),
                new LocalisationAttribute("Amplitude", "AudioSettings", "MicMode_Amplitude"),
                new LocalisationAttribute("Push to talk", "AudioSettings", "MicMode_PushToTalk")
            };
            MicModeSegmentedPicker.ValueEnumChanged += value => OnMicModeValueChanged(value);
#if UNITY_STANDALONE
            ScrollView.Add(MicModeSegmentedPicker);
#endif

            AmplitudeSlider.LocaliseLabel = new LocalisationAttribute("Noise Threshold", "AudioSettings", "AmplitudeSlider");
            AmplitudeSlider.RegisterValueChangedCallback(ce => OnAmplitudeValueChanged(ce.newValue));
            AmplitudeSlider.lowValue = 0f;
            AmplitudeSlider.highValue = 1f;
#if UNITY_STANDALONE
            ScrollView.Add(AmplitudeSlider);
#endif

            DelayBeaforeShutingMicTextfield.LocaliseLabel = new LocalisationAttribute
            (
                "Delay before mute mic when lower than threshold",
                "AudioSettings", "DelayShutMic"
            );
            DelayBeaforeShutingMicTextfield.RegisterValueChangedCallback(ce => OnDelayBeforeShutingMicValueChanged(ce.newValue));
#if UNITY_STANDALONE
            ScrollView.Add(DelayBeaforeShutingMicTextfield);
#endif

            PushToTalkKeyDropdown.LocalisedLabel = new LocalisationAttribute("Push to talk key", "AudioSettings", "PushToTalk_Label");
            PushToTalkKeyDropdown.RegisterValueChangedCallback(ce => OnPushToTalkValueChanged(ce.newValue));
            PushToTalkKeyDropdown.LocalisedOptions = Enum.GetNames(typeof(KeyCode)).ToList();
#if UNITY_STANDALONE
            ScrollView.Add(PushToTalkKeyDropdown);
#endif

            LoopBackButton.ClickedDown += () => OnLoopBackValueChanged(!m_loopBack);
            ScrollView.Add(LoopBackButton);


            ResetAudioConfButton.LocalisedLabel = new LocalisationAttribute("Reset Audio", "AudioSettings", "ResetAudioConfButton_Label");
            ResetAudioConfButton.ClickedDown += () => OnResetAudio();
            ScrollView.Add(ResetAudioConfButton);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Audio", "GenericStrings", "Audio");
        }

        #region Implementation

        /// <summary>
        /// Value is between 0 and 1
        /// </summary>
        public event System.Action<float> GeneralVolumeValeChanged;
        public event System.Action<float> LastGeneralVolumeNotZeroLoaded;
        public AudioData Data;

        protected bool m_loopBack;

#if UNITY_STANDALONE
        /// <summary>
        /// Set the audio for a desktop browser.
        /// </summary>
        public void SetAudio()
        {
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.inputType = umi3d.cdk.collaboration.MicrophoneInputType.NAudio;

            var mics = umi3d.cdk.collaboration.MicrophoneListener.GetMicrophonesNames().ToList();
            MicDropdown.LocalisedOptions = mics;

            umi3d.cdk.collaboration.MicrophoneListener.OnSaturated.AddListener(value => AmplitudeSlider.IsSaturated = value);

            if (TryGetAudiorData(out Data))
            {
                OnGeneralVolumeValueChanged(Data.GeneralVolume, Data.LastGeneralVolumeNotZero);
                OnMicDropdownValueChanged(mics.Contains(Data.CurrentMic) ? Data.CurrentMic : cdk.collaboration.MicrophoneListener.GetMicrophonesNames().FirstOrDefault());
                OnNoiseReductionValueChanged(Data.NoiseReduction);
                OnMicModeValueChanged(Data.Mode);
                OnAmplitudeValueChanged(Data.Amplitude);
                OnDelayBeforeShutingMicValueChanged(Data.DelayBeforeShutMic.ToString());
                OnPushToTalkValueChanged(Data.PushToTalkKey.ToString());
            }
            else
            {
                OnGeneralVolumeValueChanged(10f, 10f);
                OnMicDropdownValueChanged(cdk.collaboration.MicrophoneListener.GetMicrophonesNames().FirstOrDefault());
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
        if (TryGetAudiorData(out Data)) OnGeneralVolumeValueChanged(Data.GeneralVolume, Data.LastGeneralVolumeNotZero);
        else OnGeneralVolumeValueChanged(10f, 10f);
    }
#endif

        /// <summary>
        /// Update the value of the general volume and notify.
        /// </summary>
        /// <param name="value"></param>
        public void OnGeneralVolumeValueChanged(float value, float valueNotZero)
        {
            SetGeneralVolumeValueWithoutNotify(value);
            GeneralVolumeValeChanged?.Invoke(((int)value) / 10f);
            if (valueNotZero != 0) LastGeneralVolumeNotZeroLoaded?.Invoke(((int)valueNotZero) / 10f);
        }
        /// <summary>
        /// Update the value of the general volume and without notifying.
        /// </summary>
        /// <param name="value"></param>
        public void SetGeneralVolumeValueWithoutNotify(float value)
        {
            value = (int)value;
            GeneralVolume_Visual.SetValueWithoutNotify(value);
            Data.GeneralVolume = value;
            if (value != 0f) Data.LastGeneralVolumeNotZero = value;
            StoreAudioData(Data);
        }

        /// <summary>
        /// Update the value of mic dropdown and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the noise reduction and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the mic mode and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the amplitude and notify.
        /// </summary>
        /// <param name="value"></param>
        public void OnAmplitudeValueChanged(float value)
        {
            AmplitudeSlider.SetValueWithoutNotify(value);

            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.minAmplitudeToSend = value / 10f;

            Data.Amplitude = value;
            StoreAudioData(Data);
        }

        /// <summary>
        /// Update the value of the delay before shutting down mic and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the push to talk and notify.
        /// </summary>
        /// <param name="value"></param>
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
            LoopBackButton.LocaliseText = new LocalisationAttribute
            (
                m_loopBack ? "Loop back on" : "Loop back off",
                "AudioSettings",
                m_loopBack ? "LoopBack_on" : "LoopBack_off"
            );
            if (umi3d.cdk.collaboration.MicrophoneListener.Exists)
                umi3d.cdk.collaboration.MicrophoneListener.Instance.useLocalLoopback = m_loopBack;
        }

        private void OnResetAudio()
        {
            if (umi3d.cdk.collaboration.AudioManager.Exists)
                umi3d.cdk.collaboration.AudioManager.Instance.ResetAudioConference();
        }

        #endregion
    }
}
