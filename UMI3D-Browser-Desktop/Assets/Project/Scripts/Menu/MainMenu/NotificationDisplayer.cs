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

using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the display of 2D notification.
/// </summary>
public class NotificationDisplayer : Singleton<NotificationDisplayer>
{
    public UIDocument uiDocument;

    public VisualTreeAsset notifTreeAsset;

    public int maxNotificationsDisplayed;

    private int notificationCurrentlyDisplayed = 0;

    VisualElement notificationContainer;

    Queue<NotificationDto> notificationsToDisplay;

    private void Start()
    {
        Debug.Assert(uiDocument != null);
        Debug.Assert(notifTreeAsset != null);

        notificationContainer = uiDocument.rootVisualElement.Q<VisualElement>("notification-container");

        notificationsToDisplay = new Queue<NotificationDto>();
    }


    public void DisplayNotification(NotificationDto dto)
    {
        notificationsToDisplay.Enqueue(dto);
    }

    private void Update()
    {
        if (notificationsToDisplay.Count > 0 && notificationCurrentlyDisplayed < maxNotificationsDisplayed)
        {
            var notifDto = notificationsToDisplay.Dequeue();
            NotificationElement notification = notifTreeAsset.CloneTree().Q<NotificationElement>();
            notification.Setup(notifDto.title, notifDto.content, (int)notifDto.duration * 1000, () => notificationCurrentlyDisplayed--);
            
            if(notifDto.icon2D != null)
            {
                Texture2D icon = LoadIcon2d(notifDto.id,notifDto.icon2D);
                Debug.Log("Icon " + (icon == null));
            }

            notificationContainer.Insert(0, notification);
            notificationCurrentlyDisplayed++;
        }
    }

    Texture2D LoadIcon2d(ulong id, ResourceDto dto)
    {

        if (dto.variants.Count == 0)
            return null;

        Texture2D res = null;
        FileDto fileToLoad = UMI3DEnvironmentLoader.Parameters.ChooseVariante(dto.variants);

        if (fileToLoad != null)
        {
            string url = fileToLoad.url;
            string ext = fileToLoad.extension;
            string authorization = fileToLoad.authorization;
            IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(ext);

            if (loader != null)
            {
                UMI3DResourcesManager.LoadFile(
                    id,
                    fileToLoad,
                    loader.UrlToObject,
                    loader.ObjectFromCache,
                    (o) =>
                    {
                        res = o as Texture2D;
                    },
                    (Umi3dException error) =>
                    {
                        Debug.LogWarning($"Icon not loadable : {url} [{error.errorCode}:{error.Message}]");
                    },
                    loader.DeleteObject
                    );
            }
            else
                Debug.LogWarning("No loader was found to load this icon " + url);
        }
        else
        {
            Debug.LogWarning("Impossible to load " + dto.ToString());
        }

        return res;
    }
}
