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

public abstract class CustomNotifAndUsersArea : VisualElement, ICustomElement
{
    public enum NotificationsOrUsers { Notifications, Users }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<NotificationsOrUsers> m_panel = new UxmlEnumAttributeDescription<NotificationsOrUsers>
        {
            name = "area-panel",
            defaultValue = NotificationsOrUsers.Notifications,
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomNotifAndUsersArea;

            custom.Set
            (
                m_panel.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual NotificationsOrUsers AreaPanel
    {
        get => m_panel;
        set
        {
            m_panel = value;
            switch (value)
            {
                case NotificationsOrUsers.Notifications:
                    Add(notificationCenter);
                    UserList.RemoveFromHierarchy();
                    break;
                case NotificationsOrUsers.Users:
                    Add(UserList);
                    notificationCenter.RemoveFromHierarchy();
                    break;
                default:
                    break;
            }
        }
    }

    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/notifAndUsers";
    public virtual string USSCustomClassName => "notif__users";

    public CustomSegmentedPicker<NotificationsOrUsers> SegmentedPicker;
    public CustomNotificationCenter notificationCenter;
    public CustomUserList UserList;

    protected bool m_hasBeenInitialized;
    protected NotificationsOrUsers m_panel;

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

        SegmentedPicker.ValueEnumChanged += value => AreaPanel = value;
        SegmentedPicker.Category = ElementCategory.Game;

        Add(SegmentedPicker);
    }

    public virtual void Set() => Set(NotificationsOrUsers.Notifications);

    public virtual void Set(NotificationsOrUsers panel)
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }

        AreaPanel = panel;
    }
}
