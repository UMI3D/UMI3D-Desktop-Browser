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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsAudio : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-audio";

    public CustomSlider GeneralVolume_Visual;
    public CustomDropdown MicDropdown;

    public override void InitElement()
    {
        base.InitElement();

        GeneralVolume_Visual.label = "General volume";
        GeneralVolume_Visual.DirectionDisplayer = ElemnetDirection.Leading;
        GeneralVolume_Visual.lowValue = 0f;
        GeneralVolume_Visual.highValue = 10f;
        GeneralVolume_Visual.showInputField = true;
        GeneralVolume_Visual.RegisterValueChangedCallback(ce => OnGeneralVolumeValueChanged(ce.newValue));
        GeneralVolume_Visual.RegisterCallback<AttachToPanelEvent>(callback => GeneralVolume_Visual.SetValueWithoutNotify(Data.GeneralVolume));
        ScrollView.Add(GeneralVolume_Visual);

        
        //MicDropdown.label = "Microphone";
        //MicDropdown

        if (TryGetAudiorData(out Data))
        {
            OnGeneralVolumeValueChanged(Data.GeneralVolume);
        }
        else
        {
            OnGeneralVolumeValueChanged(10f);
        }
    }

    public override void Set() => Set("Audio");

    #region Implementation
    /// <summary>
    /// Value is between 0 and 1
    /// </summary>
    public event System.Action<float> GeneralVolumeValeChanged;
    public AudioData Data;

    public void SetMicDropdown()
    {
        var mics = umi3d.cdk.collaboration.MicrophoneListener.GetMicrophonesNames().ToList();
        MicDropdown.choices = mics;

        var mic = umi3d.cdk.collaboration.MicrophoneListener.Instance.GetCurrentMicrophoneName();
        if (mics.Contains(mic)) MicDropdown.value = mic;
        
    }

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

    #endregion
}
