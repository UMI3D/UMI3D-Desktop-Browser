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
using umi3d.common;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the display of 2D notification.
/// </summary>
public class NotificationDisplayer : Singleton<NotificationDisplayer>
{
    public PanelRenderer panelRenderer;

    public VisualTreeAsset notifTreeAsset;

    public int maxNotificationsDisplayed;

    private int notificationCurrentlyDisplayed = 0;

    VisualElement notificationContainer;

    Queue<NotificationDto> notificationsToDisplay;

    private void Start()
    {
        Debug.Assert(panelRenderer != null);
        Debug.Assert(notifTreeAsset != null);

        notificationContainer = panelRenderer.visualTree.Q<VisualElement>("notification-container");

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

            notificationContainer.Insert(0, notification);
            notificationCurrentlyDisplayed++;
        }
    }
}
