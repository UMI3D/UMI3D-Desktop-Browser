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

using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

namespace umi3d.commonScreen.menu
{
    public class SettingsNotification_C : BaseSettingScreen_C
    {
        public override string UssCustomClass_Emc => "setting-notification";

        public Toggle_C HideNotification = new Toggle_C();
        public Text_C HideNotificationDescription = new Text_C { name = "hide-notifications" };

        public SettingsNotification_C() { }

        protected override void InitElement()
        {
            base.InitElement();
            HideNotification.LocaliseLabel = new LocalisationAttribute("Hide notifications", "NotificationSettings", "Hide_Label");
            HideNotification.Direction = ElemnetDirection.Leading;
            HideNotification.RegisterValueChangedCallback(ce => HideNotificationValueChanged(ce.newValue));

            HideNotificationDescription.Color = TextColor.Menu;

            ScrollView.Add(HideNotification);
            ScrollView.Add(HideNotificationDescription);

            if (TryGetNotificationData(out Data))
            {
                HideNotificationValueChanged(Data.HideNotification);
            }
            else
            {
                HideNotificationValueChanged(false);
            }
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Notifications", "GenericStrings", "Notifications");
        }

        #region Implementation

        public NotificationData Data;

        /// <summary>
        /// Update Hide notification value and notify.
        /// </summary>
        /// <param name="value"></param>
        public void HideNotificationValueChanged(bool value)
        {
            HideNotification.SetValueWithoutNotify(value);
            HideNotificationDescription.LocaliseText = value 
            ? new LocalisationAttribute("Notification will still be availlable in the notification center but you won't be notified when you recieve them.", "NotificationSettings", "Hide_On") 
            : new LocalisationAttribute("You will be notified when you receive a notification.", "NotificationSettings", "Hide_Off");
            InformationArea_C.HideNotification = value;
        }

        #endregion
    }
}
