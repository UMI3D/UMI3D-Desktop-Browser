/*
Copyright 2019 Gfi Informatique

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

using System;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;

[CreateAssetMenu(fileName = "NotificationLoader", menuName = "UMI3D/Notification Loader")]

public class NotificationLoader : umi3d.cdk.NotificationLoader
{
    public Notification notificationPrefab;
    public Notification3D notification3DPrefab;

    public override void Load(NotificationDto dto)
    {
        AbstractNotification notif;
        if (dto is NotificationOnObjectDto)
        {
            var notif3d = Instantiate(notification3DPrefab, NotificationContainer.Instance.transform);
            notif = notif3d;
            var Odto = dto as NotificationOnObjectDto;
            notif3d.Parent = UMI3DEnvironmentLoader.GetNode(Odto.objectId)?.gameObject.transform;

            notif.Title = dto.title;
            notif.Content = dto.content;
            notif.SetNotificationTime(dto.duration);

            loadIcon3d(dto.icon3D);
        }
        else
        {
            notif = Instantiate(notificationPrefab, NotificationContainer.Instance.transform);
            NotificationDisplayer.Instance.DisplayNotification(dto);
        }

        //LoadIcon2d(dto.icon2D);
        
        UMI3DEnvironmentLoader.RegisterNodeInstance(dto.id, dto,notif.gameObject).NotifyLoaded();

    }

    public override bool SetUMI3DProperty(UMI3DEntityInstance entity, SetEntityPropertyDto property)
    {
        var node = entity as UMI3DNodeInstance;
        var notification = node?.gameObject?.GetComponent<Notification>();
        if (notification == null) return base.SetUMI3DProperty(entity, property);
        var dto = entity.dto as NotificationDto;
        if (dto == null) return false;
        switch (property.property)
        {
            case UMI3DPropertyKeys.NotificationTitle:
                notification.Title = dto.title = (string)property.value;
                break;
            case UMI3DPropertyKeys.NotificationContent:
                notification.Content = dto.content = (string)property.value;
                break;
            case UMI3DPropertyKeys.NotificationDuration:
                notification.SetNotificationTime(dto.duration = (float)(Double)property.value);
                break;
            case UMI3DPropertyKeys.NotificationIcon2D:
                loadIcon2d(dto.icon2D = (ResourceDto)property.value);
                break;
            case UMI3DPropertyKeys.NotificationIcon3D:
                loadIcon3d(dto.icon3D = (ResourceDto)property.value);
                break;
            case UMI3DPropertyKeys.NotificationObjectId:
                var Odto = dto as NotificationOnObjectDto;
                if (Odto == null) return false;
                Odto.objectId = (ulong)(UInt64)property.value;
                break;
            default:
                return false;
        }
        return true;
    }

    void loadIcon3d(ResourceDto dto) { }
    void loadIcon2d(ResourceDto dto) { }
}
