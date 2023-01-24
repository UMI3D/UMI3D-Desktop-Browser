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
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonDesktop.game
{
    public class BottomArea_C : CustomBottomArea
    {
        public new class UxmlFactory : UxmlFactory<BottomArea_C, UxmlTraits> { }

        public BottomArea_C() => Set();

        public override void InitElement()
        {
            if (Avatar == null) Avatar = new commonScreen.Displayer.Button_C();
            if (Emote == null) Emote = new commonScreen.Displayer.Button_C();
            if (Mic == null) Mic = new commonScreen.Displayer.Button_C();
            if (Sound == null) Sound = new commonScreen.Displayer.Button_C();
            if (NotifAndUsers == null) NotifAndUsers = new commonScreen.Displayer.Button_C();

            if (EmoteWindow == null)
            {
                if (Application.isPlaying) EmoteWindow = EmoteWindow_C.Instance;
                else EmoteWindow = new EmoteWindow_C();
            }

            base.InitElement();
        }
    }
}
