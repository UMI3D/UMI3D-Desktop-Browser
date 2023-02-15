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
using System.Collections.Generic;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

namespace umi3d.commonScreen.menu
{
    public class SettingsGraphic_C : BaseSettingScreen_C
    {
        public struct ResolutionDisplayer
        {
            public VisualElement Displayer;
            public Text_C Description;
            public VisualElement Box;

            public ResolutionDisplayer(VisualElement displayer, LocalisationAttribute text)
            {
                Displayer = displayer;

                Description = new Text_C();
                Description.LocaliseText = text;
                Description.Color = TextColor.Menu;

                Box = new VisualElement { name = "box-displayer" };
                Box.AddToClassList(USSCustomClassBox());
                Box.Add(Displayer);
                Box.Add(Description);
            }

            public static System.Func<string> USSCustomClassBox;
        }

        public override string UssCustomClass_Emc => "setting-resolution";
        public virtual string USSCustomClassBox => $"{UssCustomClass_Emc}-box";

        #region Game Resolution

        public int MaxFPS;
        public int TargetFPS;
        public SegmentedPicker_C<ResolutionEnum> GameResolutionSegmentedPicker = new SegmentedPicker_C<ResolutionEnum>();
        public ResolutionDisplayer GameResolution;

        public List<string> FullScreenResolutionList = new List<string>();
        public Dropdown_C FullScreenResolutionsDropdown = new Dropdown_C { name = "full-screen-resolution" };
        public ResolutionDisplayer FullScreenResolution;

        public SegmentedPicker_C<QualityEnum> QualitySettingsSegmentedPicker = new SegmentedPicker_C<QualityEnum>();
        public ResolutionDisplayer GameQuality;

        public Toggle_C HDRToggle = new Toggle_C { name = "hdr" };
        public ResolutionDisplayer HDR;

        public Slider_C RenderScaleSlider = new Slider_C { name = "render-scale" };
        public ResolutionDisplayer RenderScale;

        #endregion

        #region UI Resolution

        public SegmentedPicker_C<UIZoom> UISizeSegmentedPicker = new SegmentedPicker_C<UIZoom>();
        public ResolutionDisplayer UISize;

        public Slider_C DPISlider = new Slider_C { name = "dpi" };
        public ResolutionDisplayer DPI;

        #endregion

        public Toggle_C ReduceAnimation = new Toggle_C { name = "reduce-animation" };

        public SettingsGraphic_C() { }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            ResolutionDisplayer.USSCustomClassBox = () => USSCustomClassBox;
        }

        protected override void InitElement()
        {
            base.InitElement();

            UIPanelSettings = Resources.Load<PanelSettings>("PanelSettings");
            Debug.Assert(UIPanelSettings != null, "Make sure to have a PanelSettings file at the root of Resources folder in your project");
            RenderPipeline_Low = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_low");
            RenderPipeline_Medium = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_medium");
            RenderPipeline_High = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset_high");

            MaxFPS = Screen.currentResolution.refreshRate;

            #region Game Resolution

            GameResolutionSegmentedPicker.LocalisedLabel = new LocalisationAttribute("Game resolution", "GraphicSettings", "Resolution_Label");
            GameResolutionSegmentedPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Low", "GraphicSettings", "Low"),
                new LocalisationAttribute("Medium", "GraphicSettings", "Medium"),
                new LocalisationAttribute("High", "GraphicSettings", "High"),
                new LocalisationAttribute("Custom", "GraphicSettings", "Custom"),
            };
            GameResolutionSegmentedPicker.ValueEnumChanged += value => GameResolutionValueChanged(value);
            GameResolution = new ResolutionDisplayer(GameResolutionSegmentedPicker, "");
            ScrollView.Add(GameResolution.Box);

            FullScreenResolutionsDropdown.LocalisedLabel = new LocalisationAttribute("Full screen resolution", "GraphicSettings", "FullScreenResolution_Label");
            foreach (var resolution in Screen.resolutions) FullScreenResolutionList.Insert(0, $"{resolution.width}x{resolution.height}");
            FullScreenResolutionsDropdown.LocalisedOptions = FullScreenResolutionList;
            FullScreenResolutionsDropdown.ValueChanged += (index, ce) => FullScreenResolutionValueChanged(ce.newValue);
            FullScreenResolution = new ResolutionDisplayer
            (
                FullScreenResolutionsDropdown, 
                new LocalisationAttribute("Will impact Game and UI Resolutions", "GraphicSettings", "FullScreenResolution_description")
            );
#if UNITY_STANDALONE
            ScrollView.Add(FullScreenResolution.Box);
#endif

            QualitySettingsSegmentedPicker.LocalisedLabel = new LocalisationAttribute("Quality Settings", "GraphicSettings", "Quality_Label");
            QualitySettingsSegmentedPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("VLow", "GraphicSettings", "VLow"),
                new LocalisationAttribute("Low", "GraphicSettings", "Low"),
                new LocalisationAttribute("Medium", "GraphicSettings", "Medium"),
                new LocalisationAttribute("High", "GraphicSettings", "High"),
                new LocalisationAttribute("VHigh", "GraphicSettings", "VHigh"),
                new LocalisationAttribute("Ultra", "GraphicSettings", "Ultra"),
            };
            QualitySettingsSegmentedPicker.ValueEnumChanged += value => QualitySettingsValueChanged(value);
            GameQuality = new ResolutionDisplayer(QualitySettingsSegmentedPicker, "");
            ScrollView.Add(GameQuality.Box);

            HDRToggle.LocaliseLabel = new LocalisationAttribute("Enable HDR", "GraphicSettings", "HDR_Label");
            HDRToggle.RegisterValueChangedCallback(value => HDRValueChanged(value.newValue));
            HDR = new ResolutionDisplayer
            (
                HDRToggle,
                new LocalisationAttribute("High Dynamic Range", "GraphicSettings", "HDR_Description")
            );
            ScrollView.Add(HDR.Box);

            RenderScaleSlider.LocaliseLabel = new LocalisationAttribute("Render Scale", "GraphicSettings", "RenderScale_Label");
            RenderScaleSlider.DirectionDisplayer = ElemnetDirection.Leading;
            RenderScaleSlider.lowValue = 0f;
            RenderScaleSlider.highValue = 2f;
            RenderScaleSlider.showInputField = true;
            RenderScaleSlider.RegisterValueChangedCallback(value => RenderScaleValueChanged(value.newValue));
            RenderScale = new ResolutionDisplayer
            (
                RenderScaleSlider,
                new LocalisationAttribute("Lower means less resolution and more FPS.", "GraphicSettings", "RenderScale_Description")
            );
            ScrollView.Add(RenderScale.Box);

            #endregion

            #region UI Resolution

            UISizeSegmentedPicker.LocalisedLabel = new LocalisationAttribute("UI Size", "GraphicSettings", "UISize_Label");
            UISizeSegmentedPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Small", "GraphicSettings", "Small"),
                new LocalisationAttribute("Medium", "GraphicSettings", "Medium"),
                new LocalisationAttribute("Large", "GraphicSettings", "Large"),
                new LocalisationAttribute("Custom", "GraphicSettings", "Custom"),
            };
            UISizeSegmentedPicker.ValueEnumChanged += value => UISizeValueChanged(value);
            UISize = new ResolutionDisplayer
            (
                UISizeSegmentedPicker,
                new LocalisationAttribute("Size of the user interface", "GraphicSettings", "UISize_Description")
            );

            DPISlider.LocaliseLabel = "DPI";
            DPISlider.DirectionDisplayer = ElemnetDirection.Leading;
            DPISlider.lowValue = 90f;
            DPISlider.highValue = 200f;
            DPISlider.value = UIPanelSettings?.referenceDpi ?? 100f;
            DPISlider.showInputField = true;
            DPISlider.RegisterValueChangedCallback(value => DPIValueChanged(value.newValue));
            DPI = new ResolutionDisplayer
            (
                DPISlider,
                new LocalisationAttribute("Dots per inch: Lower means the UI will be larger.", "GraphicSettings", "DPI_Description")
            );

#if UNITY_STANDALONE
            ScrollView.Add(UISize.Box);
            ScrollView.Add(DPI.Box);
#endif
            #endregion

            ReduceAnimation.LocaliseLabel = new LocalisationAttribute("Reduce animation", "GraphicSettings", "ReduceAnimation");
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

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Graphics", "GenericStrings", "Graphics");
        }

        #region Implementation

        public PanelSettings UIPanelSettings;
        public UniversalRenderPipelineAsset RenderPipeline;
        protected UniversalRenderPipelineAsset RenderPipeline_Low;
        protected UniversalRenderPipelineAsset RenderPipeline_Medium;
        protected UniversalRenderPipelineAsset RenderPipeline_High;
        public ResolutionData Data;

        /// <summary>
        /// Update the value of the game resolution and notify.
        /// </summary>
        /// <param name="value"></param>
        public void GameResolutionValueChanged(ResolutionEnum value)
        {
            GameResolutionSegmentedPicker.SetValueEnumWithoutNotify(value);
            switch (value)
            {
                case ResolutionEnum.Low:
                    TargetFPS = MaxFPS;
                    Application.targetFrameRate = TargetFPS;
                    GameResolution.Description.LocaliseText = new LocalisationAttribute
                    (
                        $"Low resolution is targetting {TargetFPS}fps with lower rendering",
                        "GraphicSettings",
                        "Description_Low",
                        new string[] { TargetFPS.ToString() }
                    );

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
                    GameResolution.Description.LocaliseText = new LocalisationAttribute
                    (
                        $"Medium resolution is targetting {TargetFPS}fps with midium rendering",
                        "GraphicSettings",
                        "Description_Medium",
                        new string[] { TargetFPS.ToString() }
                    );

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
                    GameResolution.Description.LocaliseText = new LocalisationAttribute
                    (
                        $"High resolution is targetting {TargetFPS}fps with higher rendering",
                        "GraphicSettings",
                        "Description_High",
                        new string[] { TargetFPS.ToString() }
                    );

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
                    GameResolution.Description.LocaliseText = new LocalisationAttribute
                    (
                        $"Custom resolution let you choose your settings",
                        "GraphicSettings",
                        "Description_Custom"
                    );

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

        /// <summary>
        /// Update the value of the full screen resolution and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the quality settings and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the HDR and notify.
        /// </summary>
        /// <param name="value"></param>
        public void HDRValueChanged(bool value)
        {
            HDRToggle.SetValueWithoutNotify(value);
            RenderPipeline.supportsHDR = value;
            Data.HDR = value;
            StoreResolutionData(Data);
        }

        /// <summary>
        /// Update the value of the render scale and notify.
        /// </summary>
        /// <param name="value"></param>
        public void RenderScaleValueChanged(float value)
        {
            RenderScaleSlider.SetValueWithoutNotify(value);
            RenderPipeline.renderScale = value;
            Data.RenderScale = value;
            StoreResolutionData(Data);
        }

        /// <summary>
        /// Update the value of the ui size and notify.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Update the value of the dpi and notify.
        /// </summary>
        /// <param name="value"></param>
        public void DPIValueChanged(float value)
        {
            DPISlider.SetValueWithoutNotify(value);
            UIPanelSettings.referenceDpi = value;
            Data.DPI = value;
            StoreResolutionData(Data);
        }

        /// <summary>
        /// Update the value of the reduce animation and notify.
        /// </summary>
        /// <param name="value"></param>
        public void ReduceAnimationValueChanged(bool value)
        {
            ReduceAnimation.SetValueWithoutNotify(value);
            AnimatorManager.ReduceAnimation = value;
            Data.ReduceAnimation = value;
            StoreResolutionData(Data);
        }

        #endregion
    }
}
