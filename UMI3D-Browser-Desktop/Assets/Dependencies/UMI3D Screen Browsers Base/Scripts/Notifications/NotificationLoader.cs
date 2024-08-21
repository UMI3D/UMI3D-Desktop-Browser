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
using System.Collections.Specialized;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;

namespace umi3d.baseBrowser.notification
{
    [CreateAssetMenu(fileName = "NotificationLoader", menuName = "UMI3D/Notification Loader")]
    public class NotificationLoader : cdk.NotificationLoader
    {
        public Notification3D notification3DPrefab;
        public event System.Action<ulong,common.NotificationDto> Notification2DReceived;

        public override AbstractLoader GetNotificationLoader()
        {
            return new InternalNotificationLoader(this);
        }

        public void Notify(ulong environmentId, common.NotificationDto dto) { Notification2DReceived?.Invoke(environmentId,dto); }
    }

    public class InternalNotificationLoader : cdk.InternalNotificationLoader
    {
        NotificationLoader loader;

        public InternalNotificationLoader(NotificationLoader loader)
        {
            this.loader = loader;
        }

        public override async Task ReadUMI3DExtension(ReadUMI3DExtensionData value)
        {
            if (value.dto is common.NotificationOnObjectDto dto3D)
            {
                var notif3d = GameObject.Instantiate(loader.notification3DPrefab);
                notif3d.Parent = cdk.UMI3DEnvironmentLoader.GetNode(value.environmentId, dto3D.objectId)?.gameObject.transform;

                notif3d.Title = dto3D.title;
                notif3d.Content = dto3D.content;
                notif3d.SetNotificationTime(dto3D.duration);

                cdk.UMI3DEnvironmentLoader.RegisterNodeInstance(value.environmentId, dto3D.id, dto3D, notif3d.gameObject).NotifyLoaded();
                return;
            }
            else
            {
                await base.ReadUMI3DExtension(value);
                loader.Notify(value.environmentId,value.dto as NotificationDto);
            }
        }
    }
}