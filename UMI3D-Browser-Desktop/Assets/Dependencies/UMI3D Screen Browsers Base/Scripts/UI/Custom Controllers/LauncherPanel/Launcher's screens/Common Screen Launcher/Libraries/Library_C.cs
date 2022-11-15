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
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class Library_C : CustomLibrary
    {
        public new class UxmlFactory : UxmlFactory<Library_C, UxmlTraits> { }

        public Library_C() => Set();

        public override void InitElement()
        {
            if (DropDown_Button == null) DropDown_Button = new Displayer.Button_C();
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (SizeLabel == null) SizeLabel = new Displayer.Text_C();
            if (Delete == null) Delete = new Displayer.Button_C();
            if (DropDown_Date == null) DropDown_Date = new Displayer.Text_C();
            if (DropDown_Message == null) DropDown_Message = new Displayer.Text_C();

            base.InitElement();
        }

        protected override CustomDialoguebox CreateDialogueBox()
            => new Displayer.Dialoguebox_C();
    }
}