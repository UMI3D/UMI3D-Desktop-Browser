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
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class NotifAndUsersArea_C : CustomNotifAndUsersArea
    {
        public new class UxmlFactory : UxmlFactory<NotifAndUsersArea_C, UxmlTraits> { }

        public NotifAndUsersArea_C() => Set();

        public override void InitElement()
        {
            if (SegmentedPicker == null) SegmentedPicker = new Displayer.SegmentedPicker_C<NotificationsOrUsers>();
            if (notificationCenter == null) notificationCenter = new NotificationCenter_C();
            if (UserList == null) UserList = new UserList_C();

            base.InitElement();
        }
    }
}
