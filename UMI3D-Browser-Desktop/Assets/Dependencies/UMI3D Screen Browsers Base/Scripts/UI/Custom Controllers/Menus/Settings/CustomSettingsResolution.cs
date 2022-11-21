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
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsResolution : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-resolution";

    public int MaxFPS;
    public int TargetFPS;
    public CustomSegmentedPicker<ResolutionEnum> SegmentedResolution;
    public CustomDropdown ResolutionsDropdown;
    public CustomToggle SupportHDR;
    public CustomSlider RenderScale;
    public CustomToggle ReduceAnimation;

    public CustomSlider DPI; 

    public override void InitElement()
    {
        base.InitElement();

        UIPanelSettings = Resources.Load<PanelSettings>("PanelSettings");
        RenderPipeline_Low = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_low");
        RenderPipeline_Medium = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_medium");
        RenderPipeline_High = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_high");

        SegmentedResolution.LabelDirection = ElemnetDirection.Top;
        SegmentedResolution.ValueEnumChanged += value => SegmentedResolutionValueChanged(value);

        MaxFPS = Screen.currentResolution.refreshRate;

        ResolutionsDropdown.label = "Resolution";
        List<string> res = new List<string>();
        foreach (var resolution in Screen.resolutions) res.Insert(0, $"{resolution.width}x{resolution.height}");
        ResolutionsDropdown.choices = res;
        ResolutionsDropdown.RegisterValueChangedCallback((ce_resolution) => ResolutionValueChanged(ce_resolution.newValue));

        SupportHDR.label = "Enable HDR";
        SupportHDR.RegisterValueChangedCallback(value => SupportHDRValueChanged(value.newValue));

        RenderScale.label = "Render Scale";
        RenderScale.DirectionDisplayer = ElemnetDirection.Leading;
        RenderScale.lowValue = 0f;
        RenderScale.highValue = 2f;
        RenderScale.showInputField = true;
        RenderScale.RegisterValueChangedCallback(value => RenderScaleValueChanged(value.newValue));

        ReduceAnimation.label = "Reduce animation";
        ReduceAnimation.value = false;
        ReduceAnimation.RegisterValueChangedCallback((ce_value) => ReduceAnimationValueChanged(ce_value.newValue));

        DPI.label = "DPI";
        DPI.DirectionDisplayer = ElemnetDirection.Leading;
        DPI.lowValue = 90f;
        DPI.highValue = 200f;
        DPI.value = UIPanelSettings.referenceDpi;
        DPI.showInputField = true;
        DPI.RegisterValueChangedCallback(value => DPIValueChanged(value.newValue));

        ScrollView.Add(SegmentedResolution);
#if UNITY_STANDALONE
        ScrollView.Add(ResolutionsDropdown);
        ScrollView.Add(DPI);
#endif
        ScrollView.Add(SupportHDR);
        ScrollView.Add(RenderScale);
        ScrollView.Add(ReduceAnimation);

        if (TryGetResolutionData(out Data))
        {
            SegmentedResolution.Value = Data.SegmentedResolution.ToString();
            ResolutionValueChanged(Data.Resolution);
            SupportHDR.value = Data.SupportHDR;
            RenderScale.value = Data.RenderScale;
            ReduceAnimation.value = Data.ReduceAnimation;
        }
        else
        {
            SegmentedResolution.Value = ResolutionEnum.Medium.ToString();
            ResolutionValueChanged(res[0]);
        }
    }

    public override void Set() => Set("Resolution");


    #region Implementation

    public PanelSettings UIPanelSettings;
    public UniversalRenderPipelineAsset RenderPipeline;
    protected UniversalRenderPipelineAsset RenderPipeline_Low;
    protected UniversalRenderPipelineAsset RenderPipeline_Medium;
    protected UniversalRenderPipelineAsset RenderPipeline_High;
    public ResolutionData Data;

    public void SegmentedResolutionValueChanged(ResolutionEnum value)
    {
        SegmentedResolution.SetValueEnumWithoutNotify(value);
        switch (value)
        {
            case ResolutionEnum.Low:
                RenderPipeline = RenderPipeline_Low;
                QualitySettings.renderPipeline = RenderPipeline;

                TargetFPS = MaxFPS;
                Application.targetFrameRate = TargetFPS;
                SegmentedResolution.Label = $"Low resolution is targetting {TargetFPS}fps with lower rendering";
                SupportHDR.value = false;
                RenderScale.value = 0.7f;
                QualitySettings.SetQualityLevel(0, false);
                RenderPipeline.shadowDistance = 20f;
                SupportHDR.Hide();
                RenderScale.Hide();
                break;
            case ResolutionEnum.Medium:
                RenderPipeline = RenderPipeline_Medium;
                QualitySettings.renderPipeline = RenderPipeline;

                TargetFPS = MaxFPS / 2;
                Application.targetFrameRate = TargetFPS;
                SegmentedResolution.Label = $"Medium resolution is targetting {TargetFPS}fps with midium rendering";
                SupportHDR.value = false;
                RenderScale.value = 1f;
                QualitySettings.SetQualityLevel(1, false);
                RenderPipeline.shadowDistance = 35f;
                SupportHDR.Hide();
                RenderScale.Hide();
                break;
            case ResolutionEnum.High:
                RenderPipeline = RenderPipeline_High;
                QualitySettings.renderPipeline = RenderPipeline;

                TargetFPS = MaxFPS / 3;
                Application.targetFrameRate = TargetFPS;
                SegmentedResolution.Label = $"High resolution is targetting {TargetFPS}fps with higher rendering";
                SupportHDR.value = true;
                RenderScale.value = 1.3f;
                QualitySettings.SetQualityLevel(2, false);
                RenderPipeline.shadowDistance = 50f;
                SupportHDR.Hide();
                RenderScale.Hide();
                break;
            case ResolutionEnum.Custom:
                SegmentedResolution.Label = "Custom resolution let you choose your settings";
                SupportHDR.Display();
                RenderScale.Display();
                break;
            default:
                break;
        }

        Data.SegmentedResolution = SegmentedResolution.ValueEnum.Value;
        StoreResolutionData(Data);
    }

    public void ResolutionValueChanged(string value)
    {
        ResolutionsDropdown.SetValueWithoutNotify(value);
        var resolution = value.Split('x');
        int.TryParse(resolution[0], out var width);
        int.TryParse(resolution[1], out var height);
        Screen.SetResolution(width, height, true);
        Data.Resolution = value;
        StoreResolutionData(Data);
    }

    public void SupportHDRValueChanged(bool value)
    {
        SupportHDR.SetValueWithoutNotify(value);
        RenderPipeline.supportsHDR = value;
        Data.SupportHDR = value;
        StoreResolutionData(Data);
    }

    public void RenderScaleValueChanged(float value)
    {
        RenderScale.SetValueWithoutNotify(value);
        RenderPipeline.renderScale = value;
        Data.RenderScale = value;
        StoreResolutionData(Data);
    }

    public void ReduceAnimationValueChanged(bool value)
    {
        ReduceAnimation.SetValueWithoutNotify(value);
        AnimatorManager.ReduceAnimation = value;
        Data.ReduceAnimation = value;
        StoreResolutionData(Data);
    }

    public void DPIValueChanged(float value)
    {
        DPI.SetValueWithoutNotify(value);
        UIPanelSettings.referenceDpi = value;
        //Data.RenderScale = value;
        //StoreResolutionData(Data);
    }

    #endregion
}
