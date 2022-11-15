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

namespace umi3d.commonScreen.game
{
    public sealed class TopArea_C : CustomTopArea
    {
        public new class UxmlFactory : UxmlFactory<TopArea_C, UxmlTraits> { }

        public TopArea_C() => Set();

        public override void InitElement()
        {
            if (Menu == null) Menu = new Displayer.Button_C();
            if (InformationArea == null) InformationArea = new commonMobile.game.InformationArea_C();
            if (Toolbox == null) Toolbox = new Displayer.Button_C();

            base.InitElement();
        }
    }
}
