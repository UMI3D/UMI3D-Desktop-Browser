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
        public static NotifAndUsersArea_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                
                s_instance = new NotifAndUsersArea_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static NotifAndUsersArea_C s_instance;

        public class NotifAndUsersSegmentedPicker: Displayer.SegmentedPicker_C<NotificationsOrUsers>
        {
            public override string Value 
            { 
                get => base.Value;
                set
                {
                    if (!string.IsNullOrEmpty(value)) value = value.Split(':')[0];
                    base.Value = value;
                }
            }

            public override void SetValueWithoutNotify(string newValue)
            {
                m_value = newValue;

                if (string.IsNullOrEmpty(newValue)) return;
                var tps = m_options.Find(str => str.StartsWith(newValue));
                if (!m_options.Contains(tps)) return;

                var index = m_options.IndexOf(tps);
                SelectedValueBox.AddAnimation
                (
                    this,
                    () => SelectedValueBox.style.left = SelectedValueBox.style.left,
                    () => SelectedValueBox.style.left = Length.Percent(m_textWidth.value * index),
                    "left",
                    0.5f
                );
            }
        }

        public new class UxmlFactory : UxmlFactory<NotifAndUsersArea_C, UxmlTraits> { }

        public NotifAndUsersArea_C() => Set();

        public override void InitElement()
        {
            if (SegmentedPicker == null) SegmentedPicker = new NotifAndUsersSegmentedPicker();
            if (notificationCenter == null) notificationCenter = new NotificationCenter_C();
            if (UserList == null) UserList = new UserList_C();

            base.InitElement();
        }
    }
}
