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
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.emotes;
using umi3d.baseBrowser.Navigation;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using umi3d.mobileBrowser.interactions;
using UnityEngine;

namespace umi3d.baseBrowser.connection
{
    public partial class BaseGamePanelController
    {
        [Header("Object Menu")]
        public ObjectMenuFormContainer ObjectMenu;
        public cdk.menu.view.MenuDisplayManager ObjectMenuDisplay;

        public CustomGame Game => GamePanel.Game;
        public CustomTopArea TopArea => Game.TopArea;
        public CustomBottomArea BottomArea => Game.BottomArea;
        public CustomLeadingArea LeadingArea => Game.LeadingArea;
        public CustomTrailingArea TrailingArea => Game.TrailingArea;

        protected System.Action m_contextualMenuActionUp;
        protected System.Action m_contextualMenuActionDown;
        protected bool m_isContextualMenuDown;

        protected virtual void InitGame()
        {
            BaseCursor.Instance.UpdateCursor += UpdateCursor;

            InitGame_Audio();
            InitGame_ButtonsArea();
            InitGame_UserList();
            InitGame_Notification();
            InitGame_Emote();
            InitGame_ObjectMenu();
        }

        protected virtual void InitGame_ButtonsArea()
        {
            var buttonsArea = TrailingArea.ButtonsArea;
            buttonsArea.Jump.ClickedDown += () => BaseFPSNavigation.Instance.WantToJump = true;
            buttonsArea.Jump.ClickedUp += () => BaseFPSNavigation.Instance.WantToJump = false;
            buttonsArea.Crouch.ClickedDown += () => BaseFPSNavigation.Instance.WantToCrouch = true;
            buttonsArea.Crouch.ClickedUp += () => BaseFPSNavigation.Instance.WantToCrouch = false;
        }

        protected virtual void InitGame_UserList()
        {
            UMI3DEnvironmentClient.EnvironementJoinned.AddListener(Game.NotifAndUserArea.UserList.OnEnvironmentChanged);
            UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAvatarStatusUpdated.AddListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAttentionStatusUpdated.AddListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnRemoveUser.AddListener(Game.NotifAndUserArea.UserList.RemoveUser);
        }

        protected virtual void InitGame_Notification()
        {
            NotificationLoader.Notification2DReceived += dto =>
            {
                var notification = CustomNotificationCenter.AddNotification(dto);

                root.schedule.Execute(() =>
                {
                    notification.Timestamp = "0min";
                    root.schedule.Execute(() =>
                    {
                        var time = notification.Timestamp.Substring(0, notification.Timestamp.Length - 3);
                        notification.Timestamp = $"{int.Parse(time) + 1}min";
                    }).Every(60000);
                }).ExecuteLater(60000);
            };
        }

        protected virtual void InitGame_Emote()
        {
            EmoteManager.Instance.EmoteConfigReceived += emotes =>
            {
                Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = true;
                CustomEmoteWindow.OnEmoteConfigReceived(emotes);
            };
            EmoteManager.Instance.NoEmoteConfigReeived += () =>
            {
                Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = false;
                CustomEmoteWindow.Reset();
            };
            EmoteManager.Instance.EmoteUpdated += CustomEmoteWindow.OnUpdateEmote;
        }

        protected virtual void InitGame_ObjectMenu()
        {
            ObjectMenuDisplay.menu.onContentChange.AddListener(OnMenuObjectContentChange);

            ObjectMenu.GetContainer = () => TrailingArea.ObjectMenu;
            ObjectMenu.DisplayObjectMenu = value =>
            {
                if (GamePanel.CurrentView != CustomGamePanel.GameViews.Game) return;
                TrailingArea.DisplayObjectMenu = value;
            };
            ObjectMenu.InsertDisplayer = (index, displayer) => TrailingArea.ObjectMenu.Insert(index, displayer);
            ObjectMenu.RemoveDisplayer = displayer => TrailingArea.ObjectMenu.Remove(displayer);

            Game.LeadingAndTrailingAreaClicked += worldPosition =>
            {
                if (Game.Controller != ControllerEnum.MouseAndKeyboard) return;
            };
        }

        protected virtual void InitGame_Audio()
        {
            var envAudio = EnvironmentSettings.Instance.AudioSetting;
            var envMic = EnvironmentSettings.Instance.MicSetting;

            var infArea = TopArea.InformationArea;
            envAudio.StatusChanged += (value) => infArea.IsSoundOn = value;
            envMic.StatusChanged += (value) => infArea.IsMicOn = value;
            infArea.SoundStatusChanged += () => envAudio.Toggle();
            infArea.MicStatusChanged += () => envMic.Toggle();

            var bottomArea = BottomArea;
            envAudio.StatusChanged += (value) => bottomArea.IsSoundOn = value;
            envMic.StatusChanged += (value) => bottomArea.IsMicOn = value;
            bottomArea.Sound.clicked += () => envAudio.Toggle();
            bottomArea.Mic.clicked += () => envMic.Toggle();
        }

        /// <summary>
        /// Close Object menu, Notif and Users area and Emote window, only if they are displayed.
        /// </summary>
        protected void CloseGameWindows()
        {
            if (ObjectMenuDisplay.isDisplaying) ObjectMenuDisplay.Collapse(true);
            if (Game.DisplayNotifUsersArea) Game.DisplayNotifUsersArea = false;
            if (Game.DisplayEmoteWindow) Game.DisplayEmoteWindow = false;
        }

        protected enum ContextualMenuEnum { Open, Close, OpenOrClose }
        protected void OpenOrCloseContextualMenu(ContextualMenuEnum value = ContextualMenuEnum.OpenOrClose, bool forceUpdate = false, System.Action callbackOpen = null)
        {
            switch (value)
            {
                case ContextualMenuEnum.Open:
                    if (!ObjectMenuDisplay.isDisplaying || forceUpdate) OpenContextualMenu(callbackOpen);
                    break;
                case ContextualMenuEnum.Close:
                    if (ObjectMenuDisplay.isDisplaying || forceUpdate) CloseContextualMenu();
                    break;
                case ContextualMenuEnum.OpenOrClose:
                    if (ObjectMenuDisplay.isDisplaying) CloseContextualMenu();
                    else OpenContextualMenu(callbackOpen);
                    break;
                default:
                    break;
            }
        }
        protected void OpenContextualMenu(System.Action callbackOpen = null)
        {
            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);
            CloseGameWindows();
            ObjectMenuDisplay.Expand(false);
            callbackOpen?.Invoke();
        }
        protected void CloseContextualMenu()
        {
            ObjectMenuDisplay.Collapse(true);
            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Center);
        }

        protected virtual void OnMenuObjectContentChange()
        {
            var count = ObjectMenuDisplay.menu.Count;
            MainMobileAction.MenuCount = count;

            var ButtonsArea = TrailingArea.ButtonsArea;
            // If action is down don't update.
            if (m_isContextualMenuDown) return;

            if (count == 0)
            {
                Game.Cursor.Action = null;
                ObjectMenuDisplay.Collapse(false);
                if (ButtonsArea.IsActionButtonDisplayed) ButtonsArea.IsActionButtonDisplayed = false;
                UpdateContextualMenuActions(ContextualMenuActionEnum.Null);
            }
            else if (count == 1)
            {
                if (ObjectMenu[0] is TextfieldDisplayer textfield)
                {
                    Game.Cursor.Action = "Edit Text";
                    UpdateContextualMenuActions(ContextualMenuActionEnum.OpenOrClose, () => textfield.Focus());
                }
                else if (ObjectMenu[0] is ButtonDisplayer button)
                {
                    Game.Cursor.Action = button.Text;
                    UpdateContextualMenuActions(ContextualMenuActionEnum.UpAndDown, null,
                    () =>
                    {
                        m_isContextualMenuDown = true;
                        button.menuItem.NotifyValueChange(true);
                    },
                    () =>
                    {
                        m_isContextualMenuDown = false;
                        button.menuItem.NotifyValueChange(false);
                    });
                }
                else UpdateContextualMenuActions(ContextualMenuActionEnum.OpenOrClose);

                if (!ButtonsArea.IsActionButtonDisplayed) ButtonsArea.IsActionButtonDisplayed = true;
            }
            else
            {
                string CursorAction = null;
                if
                (
                    BaseController.Exists
                    && BaseController.Instance.mouseData.CurrentHovered != null
                ) CursorAction = BaseController.Instance.mouseData.CurrentHovered.dto.name;
                if
                (
                    string.IsNullOrEmpty(CursorAction)
                    || CursorAction == "new tool"
                ) CursorAction = $"Display interactions menu";
                Game.Cursor.Action = CursorAction;

                if (!ButtonsArea.IsActionButtonDisplayed) ButtonsArea.IsActionButtonDisplayed = true;

                UpdateContextualMenuActions(ContextualMenuActionEnum.OpenOrClose);
            }
        }

        protected enum ContextualMenuActionEnum { OpenOrClose, UpAndDown, Null }
        protected void UpdateContextualMenuActions(ContextualMenuActionEnum value, System.Action callbackOpen = null, System.Action callbackDown = null, System.Action callbackUp = null)
        {
            switch (value)
            {
                case ContextualMenuActionEnum.OpenOrClose:
                    m_contextualMenuActionDown = null;
                    m_contextualMenuActionUp = ()
                        => OpenOrCloseContextualMenu(ContextualMenuEnum.OpenOrClose, false, callbackOpen);
                    break;
                case ContextualMenuActionEnum.UpAndDown:
                    m_contextualMenuActionDown = callbackDown;
                    m_contextualMenuActionUp = callbackUp;
                    break;
                case ContextualMenuActionEnum.Null:
                    m_contextualMenuActionDown = null;
                    m_contextualMenuActionUp = null;
                    break;
                default:
                    break;
            }
        }
    }
}