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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using umi3d.commonDesktop.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class Game_C : BaseVisual_C, IGameView
    {
        public new class UxmlFactory : UxmlFactory<Game_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
            {
                name = "controller",
                defaultValue = ControllerEnum.MouseAndKeyboard
            };

            protected UxmlBoolAttributeDescription m_leftHand = new UxmlBoolAttributeDescription
            {
                name = "left-hand",
                defaultValue = false
            };

            protected UxmlBoolAttributeDescription m_displayNotifUserArea = new UxmlBoolAttributeDescription
            {
                name = "display-notif-users-area",
                defaultValue = false
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
                var custom = ve as Game_C;

                custom.Controller = m_controller.GetValueFromBag(bag, cc);
                custom.DisplayNotifUsersArea = m_displayNotifUserArea.GetValueFromBag(bag, cc);
                Game_C.LeftHand = m_leftHand.GetValueFromBag(bag, cc);
            }
        }

        public ControllerEnum Controller
        {
            get => m_controller;
            set
            {
                m_controller = value;
                TopArea.Controller = value;
                LeadingArea.Controller = value;
                TrailingArea.Controller = value;
                NotifAndUserArea.Controller = value;
                if (value == ControllerEnum.MouseAndKeyboard) Add(BottomArea);
                else BottomArea.RemoveFromHierarchy();
            }
        }

        public virtual bool DisplayNotifUsersArea
        {
            get => S_displayNotifUserArea;
            set
            {
                if (S_displayNotifUserArea == value) return;
                S_displayNotifUserArea = value;
                switch (m_controller)
                {
                    case ControllerEnum.MouseAndKeyboard:
                        DisplayMouseAndKeyboardNotifAndUsers(value);
                        break;
                    case ControllerEnum.Touch:
                        DisplayTouchNotifAndUsers(value);
                        break;
                    case ControllerEnum.GameController:
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual bool DisplayEmoteWindow
        {
            get => S_displayEmoteWindow;
            set
            {
                if (S_displayEmoteWindow == value) return;
                S_displayEmoteWindow = value;
                switch (m_controller)
                {
                    case ControllerEnum.MouseAndKeyboard:
                        DisplayMouseAndKeyboardEmoteWindow(value);
                        break;
                    case ControllerEnum.Touch:
                        DisplayTouchEmoteWindow(value);
                        break;
                    case ControllerEnum.GameController:
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool LeftHand
        {
            get => m_leftHand;
            set
            {
                m_leftHand = value;
                LeftHandModeUpdated?.Invoke(value);
            }
        }

        public override string StyleSheetPath_MainTheme => $"USS/game";
        public static string StyleSheetPath_MainStyle => $"{ElementExtensions.StyleSheetGamesFolderPath}/game";

        public override string UssCustomClass_Emc => "game";
        public virtual string USSCustomClassLeadingAndTrailingBox => $"{UssCustomClass_Emc}-leading__trailing__box";

        /// <summary>
        /// Event raised if the if the user clicke on the trailing and leading area but not in the objectMenu.
        /// Take in parameter the world position of the click.
        /// </summary>
        public event System.Action<Vector2> LeadingAndTrailingAreaClicked;

        public Cursor_C Cursor = new Cursor_C { name = "cursor" };
        public TopArea_C TopArea = new TopArea_C { name = "top-area" };
        public LeadingArea_C LeadingArea = new LeadingArea_C { name = "leading-area" };
        public TrailingArea_C TrailingArea = new TrailingArea_C { name = "trailing-area" };
        public BottomArea_C BottomArea = new BottomArea_C { name = "bottom-area" };
        public VisualElement LeadingAndTrailingBox = new VisualElement { name = "leading-trailing-area" };

        public NotifAndUsersArea_C NotifAndUserArea;

        public static bool S_displayNotifUserArea;
        public static bool S_displayEmoteWindow;

        public TouchManipulator2 TouchManipulator = new TouchManipulator2(null, 0, 0);

        protected ControllerEnum m_controller;
        public static System.Action<bool> LeftHandModeUpdated;
        protected static bool m_leftHand;

        protected override void InstanciateChildren()
        {
            base.InstanciateChildren();
            if (NotifAndUserArea == null)
            {
                if (Application.isPlaying) NotifAndUserArea = NotifAndUsersArea_C.Instance;
                else NotifAndUserArea = new NotifAndUsersArea_C();
                NotifAndUserArea.name = "notif-and-user-area";
            }
        }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            LeadingAndTrailingBox.AddToClassList(USSCustomClassLeadingAndTrailingBox);
        }

        protected override void InitElement()
        {
            base.InitElement();
            TopArea.InformationArea.NotificationTitleClicked += () =>
            {
                DisplayNotifUsersArea = true;
                NotifAndUserArea.AreaPanel = NotifAndUsersArea_C.NotificationsOrUsers.Notifications;
                NotifAndUserArea.notificationCenter.Filter = NotificationFilter.New;
            };
            BottomArea.NotifUsersValueChanged = value => DisplayNotifUsersArea = value;
            BottomArea.EmoteClicked += () => DisplayEmoteWindow = !DisplayEmoteWindow;
            TopArea.InformationArea.ExpandUpdate += value => DisplayNotifUsersArea = value;
            TrailingArea.ButtonsArea.Emote.clicked += () => DisplayEmoteWindow = !DisplayEmoteWindow;

            LeftHandModeUpdated = (value) =>
            {
                LeadingArea.LeftHand = value;
                TrailingArea.LeftHand = value;
            };
            LeftHandModeUpdated(m_leftHand);

            this.AddManipulator(TouchManipulator);
            TouchManipulator.ClickedDownWithInfo += (evnt, localPosition) =>
            {
                var worldPosition = this.LocalToWorld(localPosition);

                //if (IsLeadingAndtrailingClicked(worldPosition)) LeadingAndTrailingAreaClicked?.Invoke(worldPosition);
            };

            TrailingArea.ToolsWindow.Pinned = (isPinned, menu) =>
            {
                if (isPinned) LeadingArea.PinnedToolsArea.AddMenu(menu);
                else LeadingArea.PinnedToolsArea.RemoveMenu(menu);
            };

            Add(Cursor);
            Add(TopArea);
            Add(LeadingAndTrailingBox);
            LeadingAndTrailingBox.Add(LeadingArea);
            LeadingAndTrailingBox.Add(TrailingArea);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Controller = ControllerEnum.MouseAndKeyboard;
            DisplayNotifUsersArea = S_displayNotifUserArea;
            LeftHand = m_leftHand;
        }

        protected override void AttachedToPanel(AttachToPanelEvent evt)
        {
            base.AttachedToPanel(evt);
            TopArea.InformationArea.Toolbox.clicked += ToolboxButtonClicked;

            LeadingArea.PinnedToolsArea.ToolClicked += PinnedToolClicked;
        }

        protected override void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            base.DetachedFromPanel(evt);
            TopArea.InformationArea.Toolbox.clicked -= ToolboxButtonClicked;

            LeadingArea.PinnedToolsArea.ToolClicked -= PinnedToolClicked;
        }

        #region Implementation

        /// <summary>
        /// Whether or not the leading and trailing area are being clicked.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool IsLeadingAndtrailingClicked(Vector2 worldPosition)
        {
            var leadingAndTrailingLocal = LeadingAndTrailingBox.WorldToLocal(worldPosition);
            if (!LeadingAndTrailingBox.ContainsPoint(leadingAndTrailingLocal)) return false;

            var objectMenuLocal = TrailingArea.ObjectMenu.WorldToLocal(worldPosition);
            if (TrailingArea.ObjectMenu.ContainsPoint(objectMenuLocal)) return false;

            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="persistentVisual"></param>
        public void TransitionIn(VisualElement persistentVisual)
            => Transition(persistentVisual, false);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="persistentVisual"></param>
        public void TransitionOut(VisualElement persistentVisual)
            => Transition(persistentVisual, true);

        protected virtual void Transition(VisualElement persistentVisual, bool revert)
        {
            this.AddAnimation
            (
                persistentVisual,
                () => style.opacity = 0,
                () => style.opacity = 1,
                "opacity",
                0.5f,
                revert: revert,
                callback: revert ? RemoveFromHierarchy : null
            );
        }

        protected void DisplayMouseAndKeyboardNotifAndUsers(bool value)
        {
            BottomArea.DisplayNotifUsersArea = value;
            TrailingArea.DisplayNotifUsersArea = value;
        }

        protected void DisplayTouchNotifAndUsers(bool value)
        {
            if (value)
            {
                this.AddIfNotInHierarchy(NotifAndUserArea);
                NotifAndUserArea.style.visibility = Visibility.Hidden;
                NotifAndUserArea.notificationCenter.UpdateFilter();
                NotifAndUserArea.style.width = StyleKeyword.Null;
            }
            else NotifAndUserArea.notificationCenter.ResetNewNotificationFilter();

            if
            (
                !value
                && NotifAndUserArea.FindRoot() == null
            ) return;

            NotifAndUserArea.schedule.Execute(() =>
            {
                NotifAndUserArea.style.visibility = StyleKeyword.Null;
                NotifAndUserArea.AddAnimation
                (
                    this,
                    () => NotifAndUserArea.style.opacity = 0f,
                    () => NotifAndUserArea.style.opacity = 1f,
                    "opacity",
                    0.5f,
                    revert: !value,
                    callback: value ? null : NotifAndUserArea.RemoveFromHierarchy
                );
            });
        }

        protected void DisplayMouseAndKeyboardEmoteWindow(bool value)
        {
            BottomArea.ButtonSelected = value ? BottomArea_C.BottomBarButton.Emote : BottomArea_C.BottomBarButton.None;
        }

        protected void DisplayTouchEmoteWindow(bool value)
        {
            TrailingArea.ActiveWindow = value ? TrailingArea_C.WindowsEnum.EmoteWindow : TrailingArea_C.WindowsEnum.None;
        }

        protected virtual void ToolboxButtonClicked()
        {
            TrailingArea.ActiveWindow = TrailingArea.ActiveWindow != TrailingArea_C.WindowsEnum.ToolsWindow 
                ? TrailingArea_C.WindowsEnum.ToolsWindow
                : TrailingArea.ActiveWindow = TrailingArea_C.WindowsEnum.None;
        }

        protected virtual void PinnedToolClicked(bool isSelected, AbstractMenuItem menu)
        {
            if (isSelected) TrailingArea.ToolsItemsWindow.AddMenu(menu);
            TrailingArea.ActiveWindow = isSelected ? TrailingArea_C.WindowsEnum.ToolsItemsWindow : TrailingArea_C.WindowsEnum.None;
        }

        #endregion
    }
}
