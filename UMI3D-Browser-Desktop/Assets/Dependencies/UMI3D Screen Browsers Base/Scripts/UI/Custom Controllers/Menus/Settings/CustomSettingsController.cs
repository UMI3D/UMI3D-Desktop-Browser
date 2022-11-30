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

public class CustomSettingsController : CustomSettingScreen
{
    public new class UxmlTraits : CustomSettingScreen.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller"
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomSettingsController;

            custom.Set();
        }
    }

    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            m_controller = value;
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    break;
                case ControllerEnum.Touch:
                    ScrollView.Add(JoystickStaticToggle);
                    ScrollView.Add(LeftHandToggle);
                    break;
                case ControllerEnum.GameController:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    break;
                default:
                    break;
            }
        }
    }

    public override string USSCustomClassName => "setting-controller";

    public CustomSlider CamreraSensibility;

    public CustomToggle JoystickStaticToggle;
    public CustomToggle LeftHandToggle;

    protected ControllerEnum m_controller;

    public override void InitElement()
    {
        base.InitElement();

        fpsData = Resources.Load<BaseFPSData>("Scriptables/GamePanel/FPSData");

        CamreraSensibility.label = "Camera sensibility";
        CamreraSensibility.lowValue = 2f;
        CamreraSensibility.highValue = 10f;
        CamreraSensibility.showInputField = true;
        CamreraSensibility.RegisterValueChangedCallback(ce => OnCameraSensibilityValueChanged(ce.newValue));

        ScrollView.Add(CamreraSensibility);

        JoystickStaticToggle.label = "Static joystick";
        JoystickStaticToggle.RegisterValueChangedCallback(ce =>
        {
            CustomJoystickArea.IsJoystickStatic = !CustomJoystickArea.IsJoystickStatic;
            CustomJoystickArea.JoystickStaticModeUpdated?.Invoke();
            Data.JoystickStatic = ce.newValue;
            StoreControllerrData(Data);
        });

        LeftHandToggle.label = "Left hand interface";
        LeftHandToggle.RegisterValueChangedCallback(ce =>
        {
            CustomGame.LeftHandModeUpdated?.Invoke();
            Data.LeftHand = ce.newValue;
            StoreControllerrData(Data);
        });
        
        if (TryGetControllerData(out Data))
        {
            OnCameraSensibilityValueChanged(Data.CameraSensibility);
            JoystickStaticToggle.value = Data.JoystickStatic;
            LeftHandToggle.value = Data.LeftHand;
        }
        else
        {
            OnCameraSensibilityValueChanged(5f);
            JoystickStaticToggle.value = false;
            LeftHandToggle.value = false;
        }
    }

    public override void Set() => Set("Controls");

    #region Implementation

    public ControllerData Data;
    public BaseFPSData fpsData;

    public virtual void OnCameraSensibilityValueChanged(float value)
    {
        value = (int)value;
        CamreraSensibility.SetValueWithoutNotify(value);
        fpsData.AngularViewSpeed = new Vector2(value, value);
        Data.CameraSensibility = (int)value;
        StoreControllerrData(Data);
    }

    #endregion
}
