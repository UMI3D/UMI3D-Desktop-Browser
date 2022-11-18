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
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomNotificationCenter : VisualElement, ICustomElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<NotificationFilter> m_filter = new UxmlEnumAttributeDescription<NotificationFilter>
        {
            name = "filter",
            defaultValue = NotificationFilter.All,
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomNotificationCenter;

            custom.Set
            (
                m_filter.GetValueFromBag(bag, cc)
            );
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

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/notificationCenter";
    public virtual string USSCustomClassName => "notification__center";
    public virtual string USSCustomClassNoNotif => $"{USSCustomClassName}-no__notif__text";

    
    public static List<CustomNotification> Notifications = new List<CustomNotification>();
    public List<CustomNotification> FilteredNotifications = new List<CustomNotification>();
    public CustomSegmentedPicker<NotificationFilter> FilterPicker;
    public CustomScrollView ScrollView;
    public CustomText NoNotificationVisual;

    protected bool m_hasBeenInitialized;
    protected bool m_isSet = false;
    protected NotificationFilter m_filter;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);
        NoNotificationVisual.AddToClassList(USSCustomClassNoNotif);

        FilterPicker.Category = ElementCategory.Game;
        FilterPicker.ValueEnumChanged += (value) => Filter = value;

        ScrollView.Category = ElementCategory.Game;

        WillUpdateFilter += UpdateFilter;

        Add(FilterPicker);
        Add(ScrollView);
    }

    public virtual void Set() => Set(NotificationFilter.All);

    public virtual void Set(NotificationFilter filter)
    {
        m_isSet = false;

        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        Filter = filter;

        m_isSet = true;
    }

    ~CustomNotificationCenter()
    {
        WillUpdateFilter -= UpdateFilter;
    }

    public override VisualElement contentContainer => m_isSet ? ScrollView.contentContainer : this;

    protected static System.Func<CustomNotification> CreateNotification;

    #region Implementation

    public static Stack<string> NotificationTitleStack = new Stack<string>();

    protected static event System.Action WillUpdateFilter;
    protected static List<NotificationDto> m_notificationDtos = new List<NotificationDto>();
    protected static List<NotificationDto> m_newNotificationDtos = new List<NotificationDto>();

    public static CustomNotification AddNotification(NotificationDto dto)
    {
        m_notificationDtos.Insert(0, dto);
        m_newNotificationDtos.Insert(0, dto);

        var notification = CreateNotification();
        notification.Category = ElementCategory.Game;
        notification.Size = ElementSize.Custom;
        notification.Title = dto.title;
        notification.Timestamp = "Now";
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
        };

        Notifications.Add(notification);
        WillUpdateFilter?.Invoke();
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
        m_isSet = false;
        ScrollView.Clear();
        FilteredNotifications.Clear();
        
        switch (m_filter)
        {
            case NotificationFilter.All:
                FilteredNotifications.AddRange(Notifications);
                NoNotificationVisual.text = "No notification";
                if (m_notificationDtos.Count > 0) NoNotificationVisual.RemoveFromHierarchy();
                else Insert(0, NoNotificationVisual);
                break;
            case NotificationFilter.New:
                FilteredNotifications = Notifications.FindAll(notification => m_newNotificationDtos.Contains(notification.DTO));
                NoNotificationVisual.text = "No new notification";
                if (m_newNotificationDtos.Count > 0) NoNotificationVisual.RemoveFromHierarchy();
                else Insert(0, NoNotificationVisual);
                break;
            default:
                break;
        }
        m_isSet = true;

        if (this.FindRoot() == null) return;
        foreach (var notification in FilteredNotifications) Add(notification);
    }

    #endregion
}
