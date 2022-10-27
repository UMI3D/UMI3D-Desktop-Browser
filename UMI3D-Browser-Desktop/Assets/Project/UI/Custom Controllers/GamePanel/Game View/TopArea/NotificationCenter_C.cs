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

namespace umi3d.mobileBrowser.game
{
    public class NotificationCenter_C : CustomNotificationCenter
    {
        public new class UxmlFactory : UxmlFactory<NotificationCenter_C, UxmlTraits> { }

        public NotificationCenter_C() => Set();

        public override void InitElement()
        {
            if (NoNotificationVisual == null) NoNotificationVisual = new Displayer.Text_C();
            if (FilterPicker == null) FilterPicker = new Displayer.SegmentedPicker_C<NotificationFilter>();
            if (ScrollView == null) ScrollView = new Container.ScrollView_C();

            base.InitElement();
        }

        protected override CustomNotification CreateNotification()
            => new Displayer.Notification_C();
    }
}
