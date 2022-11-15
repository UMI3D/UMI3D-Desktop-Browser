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
using umi3d.baseBrowser.Navigation;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsPlayer : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-player";

    
    public CustomSlider CamreraSensibility;

    public override void InitElement()
    {
        base.InitElement();

        fpsData = Resources.Load<BaseFPSData>("Scriptables/GamePanel/FPSData");

        CamreraSensibility.label = "Camera sensibility";
        CamreraSensibility.lowValue = 2f;
        CamreraSensibility.highValue = 10f;
        CamreraSensibility.showInputField = true;
        CamreraSensibility.RegisterValueChangedCallback(ce =>
        {
            var value = (int)ce.newValue;
            CamreraSensibility.SetValueWithoutNotify(value);
            fpsData.AngularViewSpeed = new Vector2(value, value);
            Data.CameraSensibility = value;
            StorePlayerData(Data);
        });

        ScrollView.Add(CamreraSensibility);

        if (TryGetPlayerData(out Data))
        {
            CamreraSensibility.value = Data.CameraSensibility;
        }
        else
        {
            CamreraSensibility.value = 5;
        }
    }

    public override void Set() => Set("Player");

    protected float CameraSpeedToSensibility(float speed) { return speed; }

    #region Implementation

    public BaseFPSData fpsData;
    public PlayerData Data;

    #endregion
}
