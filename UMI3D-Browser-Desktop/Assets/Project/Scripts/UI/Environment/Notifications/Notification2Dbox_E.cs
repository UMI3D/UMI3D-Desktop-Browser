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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Notificationbox2D_E
    {
        public Func<int> MaxNotification;
        private Queue<NotificationDto> m_notifications;
        private int m_currentlyDisplayed;

        private bool m_process 
            => (MaxNotification != null) 
            ? m_notifications.Count > 0 && m_currentlyDisplayed < MaxNotification() 
            : m_notifications.Count > 0;

        public void Add(NotificationDto dto)
            => m_notifications.Enqueue(dto);

        public IEnumerator DisplayNotifications()
        {
            while (true)
            {
                yield return new WaitUntil(() => m_process);
                var dto = m_notifications.Dequeue();

                var notification = new Notification2D_E(dto.title, dto.content, (int)dto.duration * 1000);
                //var notification = new Notification2D_E(dto.title, dto.content, 0);
                notification.Complete += () =>
                {
                    notification.RemoveRootFromHierarchy();
                    --m_currentlyDisplayed;
                };
                notification.InsertRootAtTo(0, Root);

                ++m_currentlyDisplayed;
            }
        }
    }

    public partial class Notificationbox2D_E : ISingleUI
    {
        public static Notificationbox2D_E Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Notificationbox2D_E();
                return m_instance;
            }
        }

        private static Notificationbox2D_E m_instance;
    }

    public partial class Notificationbox2D_E : Box_E
    {
        public override void Insert(int index, View_E child)
        {
            base.Insert(index, child);
            Root.Insert(index, child.Root);
        }

        protected override void Initialize()
        {
            base.Initialize();

            Root.name = "notification2Dbox";
            Root.style.position = Position.Absolute;
            Root.style.right = 0f;
            Root.style.top = 0f;
            Root.style.bottom = 0f;
            m_notifications = new Queue<NotificationDto>();

            UIManager.StartCoroutine(DisplayNotifications());
        }

        private Notificationbox2D_E() :
            base("Notificationbox2D", null)
        { }
    }
}
