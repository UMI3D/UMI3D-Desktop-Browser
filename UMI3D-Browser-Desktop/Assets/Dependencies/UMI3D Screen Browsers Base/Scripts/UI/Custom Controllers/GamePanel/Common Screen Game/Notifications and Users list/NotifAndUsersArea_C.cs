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
using System;
using System.Collections.Generic;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class NotifAndUsersArea_C : BaseVisual_C
    {
        public enum NotificationsOrUsers { Notifications, Users }

        public static NotifAndUsersArea_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                
                s_instance = new NotifAndUsersArea_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static NotifAndUsersArea_C s_instance;

        public class NotifAndUsersSegmentedPicker: SegmentedPicker_C<NotificationsOrUsers>
        {
            protected override bool TryGetEnumValue(LocalisationAttribute value, out NotificationsOrUsers result)
            {
                result = NotificationsOrUsers.Notifications;
                if (value.DefaultText == null) return false;
                var _value = value.DefaultText.Split(':');
                return Enum.TryParse<NotificationsOrUsers>(_value[0], out result);
            }
        }

        public new class UxmlFactory : UxmlFactory<NotifAndUsersArea_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };

            UxmlEnumAttributeDescription<NotificationsOrUsers> m_panel = new UxmlEnumAttributeDescription<NotificationsOrUsers>
            {
                name = "area-panel",
                defaultValue = NotificationsOrUsers.Notifications,
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
                var custom = ve as NotifAndUsersArea_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.AreaPanel = m_panel.GetValueFromBag(bag, cc);
            }
        }

        public ControllerEnum Controller
        {
            get => m_controller;
            set
            {
                RemoveFromClassList(USSCustomClassController(m_controller));
                m_controller = value;
                AddToClassList(USSCustomClassController(value));
                switch (value)
                {
                    case ControllerEnum.MouseAndKeyboard:
                        Insert(0, SegmentedPicker);
                        break;
                    case ControllerEnum.Touch:
                        SegmentedPicker.RemoveFromHierarchy();
                        Add(UserList);
                        Add(notificationCenter);
                        break;
                    case ControllerEnum.GameController:
                        break;
                    default:
                        break;
                }
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
                        notificationCenter.UpdateFilter();
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

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/notifAndUsers";

        public override string UssCustomClass_Emc => "notif__users";
        public virtual string USSCustomClassController(ControllerEnum controller) => $"{UssCustomClass_Emc}-{controller.ToString().ToLower()}";

        public SegmentedPicker_C<NotificationsOrUsers> SegmentedPicker = new NotifAndUsersSegmentedPicker();
        public NotificationCenter_C notificationCenter = new NotificationCenter_C { name = "notification-center" };
        public UserList_C UserList = new UserList_C { name = "users-list" };

        protected ControllerEnum m_controller;
        protected NotificationsOrUsers m_panel;

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void InitElement()
        {
            base.InitElement();

            SegmentedPicker.Category = ElementCategory.Game;
            SegmentedPicker.LocalisedOptions = new List<LocalisationAttribute>
            {
                new LocalisationAttribute("Notifications: 0", "NotificationCenter", "NbNotifs", new string[] { "0" }),
                new LocalisationAttribute("Users: 0", "NotificationCenter", "NbUsers", new string[] { "0" }),
            };
            SegmentedPicker.ValueEnumChanged += value => AreaPanel = value;

            NotificationCenter_C.NotificationCountUpdate += value =>
            {
                SegmentedPicker.LocalisedOptions[0].Arguments[0] = value.ToString();
                SegmentedPicker.UpdateTranslation();
            };
            Add(SegmentedPicker);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            AreaPanel = NotificationsOrUsers.Notifications;
        }

        #region Implementation

        /// <summary>
        /// Update the user count ui.
        /// </summary>
        /// <param name="count"></param>
        public virtual void OnUserCountUpdated(int count)
        {
            SegmentedPicker.LocalisedOptions[1].Arguments[0] = count.ToString();
            SegmentedPicker.UpdateTranslation();
        }

        #endregion
    }
}
