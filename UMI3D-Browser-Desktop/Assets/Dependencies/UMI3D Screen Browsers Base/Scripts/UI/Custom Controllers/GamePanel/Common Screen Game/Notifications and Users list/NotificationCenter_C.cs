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
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class NotificationCenter_C : Visual_C
    {
        public new class UxmlFactory : UxmlFactory<NotificationCenter_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<NotificationFilter> m_filter = new UxmlEnumAttributeDescription<NotificationFilter>
            {
                name = "filter",
                defaultValue = NotificationFilter.All,
            };

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as NotificationCenter_C;

                custom.Filter = m_filter.GetValueFromBag(bag, cc);
            }
        }

        public virtual NotificationFilter Filter
        {
            get => m_filter;
            set
            {
                m_filter = value;
                FilterPicker.SetValueEnumWithoutNotify(value);
                UpdateFilter();
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/notificationCenter";

        public override string UssCustomClass_Emc => "notification__center";
        public virtual string USSCustomClassNoNotif => $"{UssCustomClass_Emc}-no__notif__text";

        public static List<Notification_C> Notifications = new List<Notification_C>();
        public List<Notification_C> FilteredNotifications = new List<Notification_C>();
        public SegmentedPicker_C<NotificationFilter> FilterPicker = new SegmentedPicker_C<NotificationFilter>();
        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };
        public Text_C NoNotificationVisual = new Text_C();

        protected NotificationFilter m_filter;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            NoNotificationVisual.AddToClassList(USSCustomClassNoNotif);
        }

        protected override void InitElement()
        {
            base.InitElement();
            FilterPicker.Category = ElementCategory.Game;
            FilterPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("All", "LibrariesScreen", "Filter_All"),
                new LocalisationAttribute("New", "GenericStrings", "New"),
            };
            FilterPicker.ValueEnumChanged += (value) => Filter = value;

            ScrollView.Category = ElementCategory.Game;

            Add(FilterPicker);
            Add(ScrollView);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Filter = NotificationFilter.All;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);
            WillUpdateFilter += UpdateFilter;
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);
            WillUpdateFilter -= UpdateFilter;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet ? ScrollView.contentContainer : this;

        #region Implementation

        public static event System.Action WillUpdateFilter;
        public static event System.Action<int> NotificationCountUpdate;
        public static Stack<string> NotificationTitleStack = new Stack<string>();

        protected static List<NotificationDto> m_notificationDtos = new List<NotificationDto>();
        protected static List<NotificationDto> m_newNotificationDtos = new List<NotificationDto>();

        public static Notification_C AddNotification(NotificationDto dto)
        {
            m_notificationDtos.Insert(0, dto);
            m_newNotificationDtos.Insert(0, dto);

            var notification = new Notification_C();
            notification.Category = ElementCategory.Game;
            notification.Size = ElementSize.Custom;
            notification.Title = dto.title;
            notification.Timestamp = new LocalisationAttribute("Now", "NotificationCenter", "Timestamp_Now");
            notification.Message = dto.content;
            notification.DTO = dto;

            NotificationTitleStack.Push(notification.Title);

            if (dto.callback == null || dto.callback.Length == 0)
            {
                notification.Type = NotificationType.Default;
                notification.ChoiceALabel = "OK";
            }
            else if (dto.callback.Length == 1)
            {
                notification.Type = NotificationType.Confirmation;
                notification.ChoiceALabel = dto.callback[0];
            }
            else if (dto.callback.Length == 2)
            {
                notification.Type = NotificationType.Confirmation;
                notification.ChoiceALabel = dto.callback[0];
                notification.ChoiceBLabel = dto.callback[1];
            }
            notification.Callback = (index) =>
            {
                var callbackDto = new NotificationCallbackDto()
                {
                    id = dto.id,
                    callback = index == 0
                };
                UMI3DClientServer.SendData(callbackDto, true);

                Notifications.Remove(notification);
                m_notificationDtos.Remove(dto);
                if (m_newNotificationDtos.Contains(dto)) m_newNotificationDtos.Remove(dto);
                WillUpdateFilter?.Invoke();
                NotificationCountUpdate?.Invoke(m_notificationDtos.Count);
            };

            Notifications.Add(notification);
            WillUpdateFilter?.Invoke();
            NotificationCountUpdate?.Invoke(m_notificationDtos.Count);
            return notification;
        }

        public virtual void RemoveNotification(NotificationDto dto)
        {

        }

        public void ResetNewNotificationFilter()
        {
            m_newNotificationDtos?.Clear();
            UpdateFilter();
        }

        public void UpdateFilter()
        {
            IsSet = false;
            ScrollView.Clear();
            FilteredNotifications.Clear();

            switch (m_filter)
            {
                case NotificationFilter.All:
                    FilteredNotifications.AddRange(Notifications);
                    NoNotificationVisual.LocaliseText = new LocalisationAttribute("No notification", "NotificationCenter", "No_Notif");
                    if (m_notificationDtos.Count > 0) NoNotificationVisual.RemoveFromHierarchy();
                    else Insert(0, NoNotificationVisual);
                    break;
                case NotificationFilter.New:
                    FilteredNotifications = Notifications.FindAll(notification => m_newNotificationDtos.Contains(notification.DTO));
                    NoNotificationVisual.LocaliseText = new LocalisationAttribute("No new notification", "NotificationCenter", "No_NewNotif");
                    if (m_newNotificationDtos.Count > 0) NoNotificationVisual.RemoveFromHierarchy();
                    else Insert(0, NoNotificationVisual);
                    break;
                default:
                    break;
            }
            IsSet = true;

            if (this.FindRoot() == null) return;
            foreach (var notification in FilteredNotifications) Add(notification);
        }

        #endregion
    }
}
