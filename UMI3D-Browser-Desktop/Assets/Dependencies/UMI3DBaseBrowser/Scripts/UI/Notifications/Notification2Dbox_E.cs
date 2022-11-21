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
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class Notificationbox2D_E
    {
        public Func<int> MaxNotification;
        private Queue<NotificationDto> m_lowPriorityNotifications;
        private Queue<NotificationDto> m_mediumPriorityNotifications;
        private Queue<NotificationDto> m_HighPriorityNotifications;
        private int m_currentlyDisplayed;

        public VisualElement HighPriorityBox { get; protected set; }
        private VisualElement m_lowPriorityBox;
        protected ScrollView_E m_highPriorityScrollView;

        private bool m_processLowPriority => 
            (MaxNotification != null) 
            ? m_lowPriorityNotifications.Count > 0 && m_currentlyDisplayed < MaxNotification() 
            : m_lowPriorityNotifications.Count > 0;

        public void Add(NotificationDto dto)
        {
            switch (dto.priority)
            {
                case NotificationDto.NotificationPriority.Low:
                    m_lowPriorityNotifications.Enqueue(dto);
                    break;
                case NotificationDto.NotificationPriority.Medium:
                    m_mediumPriorityNotifications.Enqueue(dto);
                    break;
                case NotificationDto.NotificationPriority.High:
                    m_HighPriorityNotifications.Enqueue(dto);
                    break;
                default:
                    break;
            }
        }

        private IEnumerator DisplayLowPriorityNotifications()
        {
            while (true)
            {
                yield return new WaitUntil(() => m_processLowPriority);
                var dto = m_lowPriorityNotifications.Dequeue();

                var notification = new Notification2D_E(dto.title, dto.content, (int)dto.duration * 1000);
                notification.Complete += () =>
                {
                    notification.RemoveRootFromHierarchy();
                    --m_currentlyDisplayed;
                };
                notification.InsertRootAtTo(0, m_lowPriorityBox);

                ++m_currentlyDisplayed;
            }
        }

        private IEnumerator DisplayHighPriorityNotifications()
        {
            while (true)
            {
                yield return new WaitUntil(() => m_HighPriorityNotifications.Count > 0);
                var dto = m_HighPriorityNotifications.Dequeue();
                Action<bool> callback = (value) => 
                {
                    var callbackDto = new NotificationCallbackDto()
                    {
                        id = dto.id,
                        callback = value
                    };
                    UMI3DClientServer.SendData(callbackDto, true);
                };

                var notification = new Notification2D_E(dto.title, dto.content, dto.callback, callback);
                notification.Complete += () => notification.RemoveRootFromHierarchy();
                notification.InsertRootAtTo(0, m_highPriorityScrollView.Root);
            }
        }
    }

    public partial class Notificationbox2D_E : ISingleUI
    {
        public static Notificationbox2D_E Instance
        {
            get
            {
                if (s_instance == null) s_instance = new Notificationbox2D_E();
                return s_instance;
            }
        }
        public static void DestroySingleton()
        {
            if (s_instance == null) return;
            s_instance.Destroy();
            s_instance = null;
        }

        private static Notificationbox2D_E s_instance;
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

            HighPriorityBox = new VisualElement();
            HighPriorityBox.name = "highPriorityBox";
            HighPriorityBox.style.width = new Length(100, LengthUnit.Percent);
            Root.Add(HighPriorityBox);
            m_highPriorityScrollView = new ScrollView_E();
            m_highPriorityScrollView.InsertRootTo(HighPriorityBox);

            HighPriorityBox.style.marginBottom = 10f;

            m_lowPriorityBox = new VisualElement();
            m_lowPriorityBox.name = "lowPriorityBox";
            m_lowPriorityBox.style.width = new Length(100, LengthUnit.Percent);
            Root.Add(m_lowPriorityBox);

            m_lowPriorityNotifications = new Queue<NotificationDto>();
            m_mediumPriorityNotifications = new Queue<NotificationDto>();
            m_HighPriorityNotifications = new Queue<NotificationDto>();

            UIManager.StartCoroutine(DisplayLowPriorityNotifications());
            UIManager.StartCoroutine(DisplayHighPriorityNotifications());
        }

        private Notificationbox2D_E() :
            base("Notificationbox2D", null)
        { }
    }
}