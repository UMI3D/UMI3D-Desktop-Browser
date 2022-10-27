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
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class CustomSettingsResolution : CustomSettingScreen
{
    public enum ResolutionEnum
    {
        Low,
        Medium,
        High,
        Custom
    }

    public override string USSCustomClassName => "setting-resolution";

    public CustomSegmentedPicker<ResolutionEnum> SegmentedResolution;
    public CustomDropdown ResolutionsDropdown;
    public CustomToggle SupportHDR;
    public CustomSlider RenderScale;
    public CustomToggle ReduceAnimation;

    public override void InitElement()
    {
        base.InitElement();

        SegmentedResolution.ValueEnumChanged += value =>
        {
            switch (value)
            {
                case ResolutionEnum.Low:
                    SupportHDR.value = false;
                    RenderPipeline.renderScale = 0.7f;
                    break;
                case ResolutionEnum.Medium:
                    SupportHDR.value = false;
                    RenderPipeline.renderScale = 1f;
                    break;
                case ResolutionEnum.High:
                    SupportHDR.value = true;
                    RenderPipeline.renderScale = 1.3f;
                    break;
                case ResolutionEnum.Custom:
                    break;
                default:
                    break;
            }
        };

        ResolutionsDropdown.label = "Resolustion";
        var screenResolutions = Screen.resolutions;
        List<string> res = new List<string>();
        foreach (var resolution in screenResolutions) res.Add($"{resolution.width}x{resolution.height}");
        ResolutionsDropdown.choices = res;
        ResolutionsDropdown.RegisterValueChangedCallback((ce_resolution) =>
        {
            var index = res.IndexOf(ce_resolution.newValue);
            UnityEngine.Debug.Log($"index = {index}");
            var resolution = screenResolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, true);
            UnityEngine.Debug.Log($"current resolution = {Screen.currentResolution.width}x{Screen.currentResolution.height}");
            UnityEngine.Debug.Log($"reso {resolution.width}x{resolution.height}");
        });
        ResolutionsDropdown.value = res[0];

        SupportHDR.label = "Enable HDR";
        SupportHDR.RegisterValueChangedCallback(value => RenderPipeline.supportsHDR = value.newValue);

        RenderScale.label = "Render Scale";
        RenderScale.lowValue = 0f;
        RenderScale.highValue = 2f;
        RenderScale.showInputField = true;
        RenderScale.RegisterValueChangedCallback(value => RenderPipeline.renderScale = value.newValue);

        ReduceAnimation.label = "Reduce animation";
        ReduceAnimation.value = false;
        ReduceAnimation.RegisterValueChangedCallback((ce_value) => AnimatorManager.ReduceAnimation = ce_value.newValue);

        ScrollView.Add(SegmentedResolution);
        ScrollView.Add(ReduceAnimation);
    }

    public override void Set() => Set("Resolution");


    #region Implementation

    public UniversalRenderPipelineAsset RenderPipeline;

    #endregion
}
