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

namespace umi3d.baseBrowser.connection
{
    public partial class BaseGamePanelController
    {
        public CustomGameMenu Menu => GamePanel.Menu;
        public CustomSettingsContainer Settings => Menu.Settings;

        protected virtual void InitMenu()
        {
            Menu.Leave.clicked += () =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = "Do you want to leave the environment ?";
                dialoguebox.Message = "";
                dialoguebox.ChoiceAText = "Stay";
                dialoguebox.ChoiceBText = "Leave";
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.Callback = (index) =>
                {
                    if (index != 0) BaseConnectionProcess.Instance.Leave();
                };
                dialoguebox.EnqueuePriority(root);
            };

            InitMenu_Audio();
        }

        protected virtual void InitMenu_Audio()
        {
            var envAudioSettings = EnvironmentSettings.Instance.AudioSetting;
            var menuAudioSettings = Settings.Audio;
            menuAudioSettings.GeneralVolumeValeChanged += value => envAudioSettings.GeneralVolume = value;
            envAudioSettings.StatusChanged += isOn => menuAudioSettings.SetGeneralVolumeValueWithoutNotify(envAudioSettings.GeneralVolume * 10f);

            Menu.Settings.Audio.SetAudio();
        }
    }
}