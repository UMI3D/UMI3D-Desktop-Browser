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
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsNotification : CustomSettingScreen
{
    public override string USSCustomClassName => "setting-notification";

    public CustomToggle HideNotification;
    public CustomText HideNotificationDescription;

    public override void InitElement()
    {
        base.InitElement();

        HideNotification.label = "Hide notifications";
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

    public override void Set() => Set("Notification");

    #region Implementation

    public NotificationData Data;

    public void HideNotificationValueChanged(bool value)
    {
        HideNotification.SetValueWithoutNotify(value);
        HideNotificationDescription.text = 
            value 
            ? "Notification will still be availlable in the notification center but you won't be notified when you recieve them."
            : "You will be notified when you receive a notification.";
        CustomInformationArea.HideNotification = value;
    }

    #endregion
}
