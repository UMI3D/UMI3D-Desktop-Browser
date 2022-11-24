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

namespace umi3d.commonMobile.game
{
    public class EmoteWindow_C : CustomEmoteWindow
    {
        public static EmoteWindow_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = new EmoteWindow_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static EmoteWindow_C s_instance;

        public EmoteWindow_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new commonScreen.Displayer.Text_C();
            if (VScroll == null) VScroll = new commonScreen.Container.ScrollView_C();

            base.InitElement();
        }

        protected override CustomButton CreateButton()
            => new commonScreen.Displayer.Button_C();
    }
}
