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

namespace umi3d.commonMobile.game
{
    public class InformationArea_C : CustomInformationArea
    {
        public new class UxmlFactory : UxmlFactory<InformationArea_C, UxmlTraits> { }

        public InformationArea_C() => Set();

        public override void InitElement()
        {
            if(UserList == null) UserList = new commonScreen.game.UserList_C();
            if (NotificationCenter == null) NotificationCenter = new commonScreen.game.NotificationCenter_C();
            if (ShortInf == null) ShortInf = new commonScreen.Displayer.Text_C();

            base.InitElement();
        }
    }
}
