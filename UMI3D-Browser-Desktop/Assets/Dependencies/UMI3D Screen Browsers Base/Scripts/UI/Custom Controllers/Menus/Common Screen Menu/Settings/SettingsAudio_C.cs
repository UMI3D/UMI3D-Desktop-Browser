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

using static umi3d.baseBrowser.preferences.SettingsPreferences;

namespace umi3d.commonScreen.menu
{
    public class SettingsAudio_C : CustomSettingsAudio
    {
        public SettingsAudio_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (Button_Back == null) Button_Back = new Displayer.Button_C();

            if (ScrollView == null) ScrollView = new Container.ScrollView_C();
            if (GeneralVolume_Visual == null) GeneralVolume_Visual = new Displayer.Slider_C();
            if (MicDropdown == null) MicDropdown = new Displayer.Dropdown_C();
            if (MicModeSegmentedPicker == null) MicModeSegmentedPicker = new Displayer.SegmentedPicker_C<MicModeEnum>();
            if (PushToTalkKeyDropdown == null) PushToTalkKeyDropdown = new Displayer.Dropdown_C();

            base.InitElement();
        }
    }
}
