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
using System.ComponentModel;
using umi3d.commonScreen.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonDesktop.menu
{
    public class AppHeader_C : CustomAppHeader
    {
        public new class UxmlFactory : UxmlFactory<AppHeader_C, UxmlTraits> { }

        public AppHeader_C() => Set();

        public override void InitElement()
        {
            if (Minimize == null) Minimize = new commonScreen.Displayer.Button_C();
            if (Maximize == null) Maximize = new commonScreen.Displayer.Button_C();
            if (Close == null) Close = new commonScreen.Displayer.Button_C();

            base.InitElement();
        }
    }
}
