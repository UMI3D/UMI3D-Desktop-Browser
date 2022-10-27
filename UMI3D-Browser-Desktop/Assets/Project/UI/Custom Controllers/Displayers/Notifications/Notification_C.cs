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

namespace umi3d.mobileBrowser.Displayer
{
    public class Notification_C : CustomNotification
    {
        public new class UxmlFactory : UxmlFactory<Notification_C, UxmlTraits> { }

        public Notification_C() => Set();

        public Notification_C(ElementCategory category, ElementSize size, NotificationType type, string title, string author, string timestamp, string message, string choiceA, string choiceB) 
            => Set(category, size, type, title, author, timestamp, message, choiceA, choiceB);

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Text_C();
            if (AuthorLabel == null) AuthorLabel = new Text_C();
            if (TimestampLabel == null) TimestampLabel = new Text_C();
            if (MessageLabel == null) MessageLabel = new Text_C();
            if (ChoiceA == null) ChoiceA = new Button_C();
            if (ChoiceB == null) ChoiceB = new Button_C();
            base.InitElement();
        }
    }
}
