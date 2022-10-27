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

namespace umi3d.mobileBrowser.menu
{
    public class AdvConnection_C : CustomAdvConnection
    {
        public new class UxmlFactory : UxmlFactory<AdvConnection_C, UxmlTraits> { }

        public AdvConnection_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();

            if (IP == null) IP = new Displayer.Textfield_C();
            if (Port == null) Port = new Displayer.Textfield_C();

            if (Button_Back == null) Button_Back = new Displayer.Button_C();
            if (Button_Submit == null) Button_Submit = new Displayer.Button_C();

            base.InitElement();
        }
    }
}
