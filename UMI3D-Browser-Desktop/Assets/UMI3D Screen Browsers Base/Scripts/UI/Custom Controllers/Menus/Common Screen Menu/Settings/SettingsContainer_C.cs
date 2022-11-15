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

namespace umi3d.commonScreen.menu
{
    public class SettingsContainer_C : CustomSettingsContainer
    {
        public new class UxmlFactory : UxmlFactory<SettingsContainer_C, UxmlTraits> { }

        public SettingsContainer_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (Button_Back == null) Button_Back = new Displayer.Button_C();

            if (FirstScreen == null) FirstScreen = new SettingsFirstScreen_C();
            if (General == null) General = new SettingsGeneral_C();
            if (Resolution == null) Resolution = new SettingsResolution_C();
            if (Player == null) Player = new SettingsPlayer_C();
            if (Controller == null) Controller = new SettingsController_C();
            if (Audio == null) Audio = new SettingsAudio_C();
            if (Notification == null) Notification = new SettingsNotification_C();

            base.InitElement();
        }
    } 
}
