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
    public struct ResolutionDisplayer
    {
        public VisualElement Displayer;
        public CustomText Description;
        public VisualElement Box;

        public ResolutionDisplayer(VisualElement displayer, string text)
        {
            Displayer = displayer;

            Description = CreateText();
            Description.text = text;
            Description.Color = TextColor.Menu;

            Box = new VisualElement { name = "box-displayer" };
            Box.AddToClassList(USSCustomClassBox());
            Box.Add(Displayer);
            Box.Add(Description);
        }

        public static System.Func<CustomText> CreateText;
        public static System.Func<string> USSCustomClassBox;
    }

    public override string USSCustomClassName => "setting-resolution";
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box";

    #region Game Resolution

    public int MaxFPS;
    public int TargetFPS;
    public CustomSegmentedPicker<ResolutionEnum> GameResolutionSegmentedPicker;
    public ResolutionDisplayer GameResolution;

    public List<string> FullScreenResolutionList = new List<string>();
    public CustomDropdown FullScreenResolutionsDropdown;
    public ResolutionDisplayer FullScreenResolution;

    public CustomSegmentedPicker<QualityEnum> QualitySettingsSegmentedPicker;
    public ResolutionDisplayer GameQuality;

    public CustomToggle HDRToggle;
    public ResolutionDisplayer HDR;

    public CustomSlider RenderScaleSlider;
    public ResolutionDisplayer RenderScale;

    #endregion

    #region UI Resolution

    public CustomSegmentedPicker<UIZoom> UISizeSegmentedPicker;
    public ResolutionDisplayer UISize;

    public CustomSlider DPISlider;
    public ResolutionDisplayer DPI;

    #endregion

    public CustomToggle ReduceAnimation;

    public override void InitElement()
    {
        base.InitElement();

        UIPanelSettings = Resources.Load<PanelSettings>("PanelSettings");
        Debug.Assert(UIPanelSettings != null, "Make sure to have a PanelSettings file at the root of Resources folder in your project");
        RenderPipeline_Low = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_low");
        RenderPipeline_Medium = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_medium");
        RenderPipeline_High = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_high");

        MaxFPS = Screen.currentResolution.refreshRate;

        #region Game Resolution

        GameResolutionSegmentedPicker.Label = "Game resolution";
        GameResolutionSegmentedPicker.ValueEnumChanged += value => GameResolutionValueChanged(value);
        GameResolution = new ResolutionDisplayer(GameResolutionSegmentedPicker, "");
        ScrollView.Add(GameResolution.Box);

        FullScreenResolutionsDropdown.label = "Full screen resolution";
        foreach (var resolution in Screen.resolutions) FullScreenResolutionList.Insert(0, $"{resolution.width}x{resolution.height}");
        FullScreenResolutionsDropdown.choices = FullScreenResolutionList;
        FullScreenResolutionsDropdown.RegisterValueChangedCallback((ce_resolution) => FullScreenResolutionValueChanged(ce_resolution.newValue));
        FullScreenResolution = new ResolutionDisplayer(FullScreenResolutionsDropdown, "Will impact Game and UI Resolutions");
#if UNITY_STANDALONE
        ScrollView.Add(FullScreenResolution.Box);
#endif

        QualitySettingsSegmentedPicker.Label = "Quality Settings";
        QualitySettingsSegmentedPicker.ValueEnumChanged += value => QualitySettingsValueChanged(value);
        GameQuality = new ResolutionDisplayer(QualitySettingsSegmentedPicker, "");
        ScrollView.Add(GameQuality.Box);

        HDRToggle.label = "Enable HDR";
        HDRToggle.RegisterValueChangedCallback(value => HDRValueChanged(value.newValue));
        HDR = new ResolutionDisplayer(HDRToggle, "High Dynamic Range");
        ScrollView.Add(HDR.Box);

        RenderScaleSlider.label = "Render Scale";
        RenderScaleSlider.DirectionDisplayer = ElemnetDirection.Leading;
        RenderScaleSlider.lowValue = 0f;
        RenderScaleSlider.highValue = 2f;
        RenderScaleSlider.showInputField = true;
        RenderScaleSlider.RegisterValueChangedCallback(value => RenderScaleValueChanged(value.newValue));
        RenderScale = new ResolutionDisplayer(RenderScaleSlider, "Lower means less resolution and more FPS.");
        ScrollView.Add(RenderScale.Box);

        #endregion

        #region UI Resolution

        UISizeSegmentedPicker.Label = "UI Size";
        UISizeSegmentedPicker.ValueEnumChanged += value => UISizeValueChanged(value);
        UISize = new ResolutionDisplayer(UISizeSegmentedPicker, "Size of the user interface");

        DPISlider.label = "DPI";
        DPISlider.DirectionDisplayer = ElemnetDirection.Leading;
        DPISlider.lowValue = 90f;
        DPISlider.highValue = 200f;
        DPISlider.value = UIPanelSettings.referenceDpi;
        DPISlider.showInputField = true;
        DPISlider.RegisterValueChangedCallback(value => DPIValueChanged(value.newValue));
        DPI = new ResolutionDisplayer(DPISlider, "Dots per inch: Lower means the UI will be larger.");
#if UNITY_STANDALONE
        ScrollView.Add(UISize.Box);
        ScrollView.Add(DPI.Box);
#endif
        #endregion

        ReduceAnimation.label = "Reduce animation";
        ReduceAnimation.value = false;
        ReduceAnimation.RegisterValueChangedCallback((ce_value) => ReduceAnimationValueChanged(ce_value.newValue));
        ScrollView.Add(ReduceAnimation);

        if (TryGetResolutionData(out Data))
        {
            GameResolutionValueChanged(Data.GameResolution);
            if (Data.GameResolution == ResolutionEnum.Custom)
            {
#if UNITY_STANDALONE
                FullScreenResolutionValueChanged(Data.FullScreenResolution);
#endif
                QualitySettingsValueChanged(Data.Quality);
                HDRValueChanged(Data.HDR);
                RenderScaleValueChanged(Data.RenderScale);
            }
            ReduceAnimationValueChanged(Data.ReduceAnimation);
#if UNITY_STANDALONE
            UISizeValueChanged(Data.UISize);
            if (Data.UISize == UIZoom.Custom) DPIValueChanged(Data.DPI);
#endif
        }
        else
        {
            GameResolutionValueChanged(ResolutionEnum.Medium);
            ReduceAnimationValueChanged(false);
#if UNITY_STANDALONE
            UISizeValueChanged(UIZoom.Medium);
#endif
        }
    }

    public override void Set() => Set("Graphics");


#region Implementation

    public PanelSettings UIPanelSettings;
    public UniversalRenderPipelineAsset RenderPipeline;
    protected UniversalRenderPipelineAsset RenderPipeline_Low;
    protected UniversalRenderPipelineAsset RenderPipeline_Medium;
    protected UniversalRenderPipelineAsset RenderPipeline_High;
    public ResolutionData Data;

    public void GameResolutionValueChanged(ResolutionEnum value)
    {
        GameResolutionSegmentedPicker.SetValueEnumWithoutNotify(value);
        switch (value)
        {
            case ResolutionEnum.Low:
                TargetFPS = MaxFPS;
                Application.targetFrameRate = TargetFPS;
                GameResolution.Description.text = $"Low resolution is targetting {TargetFPS}fps with lower rendering";

#if UNITY_STANDALONE
                FullScreenResolutionValueChanged(FullScreenResolutionList[0]);
#endif
                QualitySettingsValueChanged(QualityEnum.Low);
                HDRValueChanged(false);
                RenderScaleValueChanged(0.7f);
                RenderPipeline.shadowDistance = 20f;

                FullScreenResolution.Box.Hide();
                GameQuality.Box.Hide();
                HDR.Box.Hide();
                RenderScale.Box.Hide();
                break;
            case ResolutionEnum.Medium:
                TargetFPS = MaxFPS / 2;
                Application.targetFrameRate = TargetFPS;
                GameResolution.Description.text = $"Medium resolution is targetting {TargetFPS}fps with midium rendering";

#if UNITY_STANDALONE
                FullScreenResolutionValueChanged(FullScreenResolutionList[0]);
#endif
                QualitySettingsValueChanged(QualityEnum.Medium);
                HDRValueChanged(false);
                RenderScaleValueChanged(1f);
                RenderPipeline.shadowDistance = 35f;

                FullScreenResolution.Box.Hide();
                GameQuality.Box.Hide();
                HDR.Box.Hide();
                RenderScale.Box.Hide();
                break;
            case ResolutionEnum.High:
                TargetFPS = MaxFPS / 3;
                Application.targetFrameRate = TargetFPS;
                GameResolution.Description.text = $"High resolution is targetting {TargetFPS}fps with higher rendering";

#if UNITY_STANDALONE
                FullScreenResolutionValueChanged(FullScreenResolutionList[0]);
#endif
                QualitySettingsValueChanged(QualityEnum.High);
                HDRValueChanged(true);
                RenderScaleValueChanged(1.3f);
                RenderPipeline.shadowDistance = 50f;

                FullScreenResolution.Box.Hide();
                GameQuality.Box.Hide();
                HDR.Box.Hide();
                RenderScale.Box.Hide();
                break;
            case ResolutionEnum.Custom:
                GameResolution.Description.text = "Custom resolution let you choose your settings";

                FullScreenResolution.Box.Display();
                GameQuality.Box.Display();
                HDR.Box.Display();
                RenderScale.Box.Display();
                break;
            default:
                break;
        }

        Data.GameResolution = value;
        StoreResolutionData(Data);
    }

    public void FullScreenResolutionValueChanged(string value)
    {
        FullScreenResolutionsDropdown.SetValueWithoutNotify(value);
        var resolution = value.Split('x');
        int.TryParse(resolution[0], out var width);
        int.TryParse(resolution[1], out var height);
        if (Screen.fullScreen) Screen.SetResolution(width, height, true);
        Data.FullScreenResolution = value;
        StoreResolutionData(Data);
    }

    public void QualitySettingsValueChanged(QualityEnum value)
    {
        QualitySettingsSegmentedPicker.SetValueEnumWithoutNotify(value);
        switch (value)
        {
            case QualityEnum.VLow:
                QualitySettings.SetQualityLevel(0, false);
                break;
            case QualityEnum.Low:
                QualitySettings.SetQualityLevel(1, false);
                break;
            case QualityEnum.Medium:
                QualitySettings.SetQualityLevel(2, false);
                break;
            case QualityEnum.High:
                QualitySettings.SetQualityLevel(3, false);
                break;
            case QualityEnum.VHigh:
                QualitySettings.SetQualityLevel(4, false);
                break;
            case QualityEnum.Ultra:
                QualitySettings.SetQualityLevel(5, false);
                break;
            default:
                break;
        }
        RenderPipeline = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

        Data.Quality = value;
        StoreResolutionData(Data);
    }

    public void HDRValueChanged(bool value)
    {
        HDRToggle.SetValueWithoutNotify(value);
        RenderPipeline.supportsHDR = value;
        Data.HDR = value;
        StoreResolutionData(Data);
    }

    public void RenderScaleValueChanged(float value)
    {
        RenderScaleSlider.SetValueWithoutNotify(value);
        RenderPipeline.renderScale = value;
        Data.RenderScale = value;
        StoreResolutionData(Data);
    }

    public void UISizeValueChanged(UIZoom value)
    {
        UISizeSegmentedPicker.SetValueEnumWithoutNotify(value);
        switch (value)
        {
            case UIZoom.Small:
                DPIValueChanged(170f);
                DPI.Box.Hide();
                break;
            case UIZoom.Medium:
                DPIValueChanged(150f);
                DPI.Box.Hide();
                break;
            case UIZoom.Large:
                DPIValueChanged(120f);
                DPI.Box.Hide();
                break;
            case UIZoom.Custom:
                DPI.Box.Display();
                break;
            default:
                break;
        }

        Data.UISize = value;
        StoreResolutionData(Data);
    }

    public void DPIValueChanged(float value)
    {
        DPISlider.SetValueWithoutNotify(value);
        UIPanelSettings.referenceDpi = value;
        Data.DPI = value;
        StoreResolutionData(Data);
    }

    public void ReduceAnimationValueChanged(bool value)
    {
        ReduceAnimation.SetValueWithoutNotify(value);
        AnimatorManager.ReduceAnimation = value;
        Data.ReduceAnimation = value;
        StoreResolutionData(Data);
    }

#endregion
}
