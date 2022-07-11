/*
Copyright 2019 - 2021 Inetum

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
using UnityEngine;

namespace umi3d.baseBrowser.notification
{
    [CreateAssetMenu(fileName = "NotificationLoader", menuName = "UMI3D/Notification Loader")]
    public class NotificationLoader : cdk.NotificationLoader
    {
        public Notification3D notification3DPrefab;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dto"></param>
        public override void Load(common.NotificationDto dto)
        {
            if (dto is common.NotificationOnObjectDto dto3D)
            {
                var notif3d = Instantiate(notification3DPrefab);
                notif3d.Parent = cdk.UMI3DEnvironmentLoader.GetNode(dto3D.objectId)?.gameObject.transform;

                notif3d.Title = dto3D.title;
                notif3d.Content = dto3D.content;
                notif3d.SetNotificationTime(dto3D.duration);

                cdk.UMI3DEnvironmentLoader.RegisterNodeInstance(dto.id, dto, notif3d.gameObject).NotifyLoaded();
            }
            else ui.viewController.Notificationbox2D_E.Instance.Add(dto);
        }
    }
}