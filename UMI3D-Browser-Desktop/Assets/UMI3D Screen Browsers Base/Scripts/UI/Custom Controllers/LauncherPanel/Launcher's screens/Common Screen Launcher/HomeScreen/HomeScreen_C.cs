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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class HomeScreen_C : CustomHomeScreen
    {
        public new class UxmlFactory : UxmlFactory<HomeScreen_C, UxmlTraits> { }

        public HomeScreen_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (Button_Back == null) Button_Back = new Displayer.Button_C();

            if (SavedServers__Title == null) SavedServers__Title = new Displayer.Text_C();
            if (SavedServers__Edit == null) SavedServers__Edit = new Displayer.Button_C();
            if (SavedServers__ScrollView == null) SavedServers__ScrollView = new Container.ScrollView_C();
            if (SavedServers_NewServer == null) SavedServers_NewServer = CreateServerItem();

            if (DirectConnect__Title == null) DirectConnect__Title = new Displayer.Text_C();
            if (DirectConnect__TextField == null) DirectConnect__TextField = new Displayer.Textfield_C();
            if (DirectConnect__Toggle == null) DirectConnect__Toggle = new Displayer.Toggle_C();

            if (Button_SwitchBoxes == null) Button_SwitchBoxes = new Displayer.Button_C();

            base.InitElement();
        }

        protected override CustomServerButton CreateServerItem()
            => new ServerButton_C();

        protected override CustomDialoguebox CreateDialogueBox()
            => new Displayer.Dialoguebox_C();

        protected override CustomTextfield CreateTextField()
            => new Displayer.Textfield_C();

        protected override CustomButton CreateButton()
            => new Displayer.Button_C();
    }
}
