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
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

namespace umi3d.commonScreen.menu
{
    public class SettingsResolution_C : CustomSettingsResolution
    {
        public SettingsResolution_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (Button_Back == null) Button_Back = new Displayer.Button_C();

            if (ScrollView == null) ScrollView = new Container.ScrollView_C();
            if (GameResolutionSegmentedPicker == null) GameResolutionSegmentedPicker = new Displayer.SegmentedPicker_C<ResolutionEnum>();
            if (FullScreenResolutionsDropdown == null) FullScreenResolutionsDropdown = new Displayer.Dropdown_C();
            if (QualitySettingsSegmentedPicker == null) QualitySettingsSegmentedPicker = new Displayer.SegmentedPicker_C<QualityEnum>();
            if (HDRToggle == null) HDRToggle = new Displayer.Toggle_C();
            if (RenderScaleSlider == null) RenderScaleSlider = new Displayer.Slider_C();
            if (ReduceAnimation == null) ReduceAnimation = new Displayer.Toggle_C();


            if (DPISlider == null) DPISlider = new Displayer.Slider_C();
            if (UISizeSegmentedPicker == null) UISizeSegmentedPicker = new Displayer.SegmentedPicker_C<UIZoom>();

            ResolutionDisplayer.CreateText = () => new Displayer.Text_C();
            ResolutionDisplayer.USSCustomClassBox = () => USSCustomClassBox;

            base.InitElement();
        }
    }
}
