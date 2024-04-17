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
using umi3d.baseBrowser.Navigation;
using umi3d.cdk.collaboration;
using umi3d.cdk.collaboration.emotes;
using umi3d.commonDesktop.game;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using umi3d.mobileBrowser.Controller;
using umi3d.mobileBrowser.interactions;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.commonScreen.game.GamePanel_C;

namespace umi3d.baseBrowser.connection
{
    public partial class BaseGamePanelController
    {
        [Header("Object Menu")]
        public ObjectMenuFormContainer ObjectMenu;
        public cdk.menu.view.MenuDisplayManager ObjectMenuDisplay;

        public BaseFPSData fPSData;

        public Game_C Game => GamePanel.Game;
        public TopArea_C TopArea => Game.TopArea;
        public BottomArea_C BottomArea => Game.BottomArea;
        public LeadingArea_C LeadingArea => Game.LeadingArea;
        public TrailingArea_C TrailingArea => Game.TrailingArea;

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
            InitGame_InteractableMapping();
        }

        protected virtual void InitGame_ButtonsArea()
        {
            Debug.Assert(TrailingArea != null, "TrailingArea null");
            var buttonsArea = TrailingArea.ButtonsArea;
            Debug.Assert(buttonsArea != null, "buttonsArea null");


            Debug.Assert(buttonsArea.Jump != null, "buttonsArea.Jump null");
            buttonsArea.Jump.ClickedDown += () => fPSData.WantToJump = true;
            buttonsArea.Jump.ClickedUp += () => fPSData.WantToJump = false;

            Debug.Assert(buttonsArea.Crouch != null, "buttonsArea.Crouch null");
            buttonsArea.Crouch.ClickedDown += () => fPSData.WantToCrouch = true;
            buttonsArea.Crouch.ClickedUp += () => fPSData.WantToCrouch = false;

            //var mobileNavigation = BaseFPSNavigation.Instance.Navigations.Find(navigation => navigation is MobileFpsNavigation) as MobileFpsNavigation;
            //Debug.Assert(mobileNavigation != null, "mobileNavigation.Crouch null");
            Debug.Assert(Game != null, "Game null");
            Debug.Assert(Game.TrailingArea != null, "Game.TrailingArea null");
            //mobileNavigation.CameraDirection = () => Game.TrailingArea.Direction;
            Debug.Assert(Game.LeadingArea != null, "Game.LeadingArea null");
            //mobileNavigation.MoveDirection = () => Game.LeadingArea.JoystickArea.Joystick.Direction;
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
                var notification = NotificationCenter_C.AddNotification(dto);

                root.schedule.Execute(() =>
                {
                    notification.Timestamp = "0min";
                    root.schedule.Execute(() =>
                    {
                        var value = notification.Timestamp.Value;
                        var time = value.Substring(0, value.Length - 3);
                        notification.Timestamp = $"{int.Parse(time) + 1}min";
                    }).Every(60000);
                }).ExecuteLater(60000);
            };
        }

        protected virtual void InitGame_Emote()
        {
            EmoteManager.Instance.EmotesLoaded += emotes =>
            {
                if (emotes is not null && emotes.Count > 0)
                {
                    Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = true;
                    EmoteWindow_C.Instance.OnEmoteConfigReceived(emotes);
                }
                else
                {
                    Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = false;
                    EmoteWindow_C.Reset();
                }
            };
            EmoteManager.Instance.EmoteUpdated += EmoteWindow_C.OnUpdateEmote;
        }

        protected virtual void InitGame_ObjectMenu()
        {
            ObjectMenuDisplay.menu.onContentChange.AddListener(OnMenuObjectContentChange);

            ObjectMenu.GetContainer = () => TrailingArea.ObjectMenu;
            ObjectMenu.DisplayObjectMenu = value =>
            {
                if (GamePanel.CurrentView != GameViews.Game) return;
                TrailingArea.ActiveWindow = value ? TrailingArea_C.WindowsEnum.ContextualMenu : TrailingArea_C.WindowsEnum.None;
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

        protected virtual void InitGame_InteractableMapping()
        {
            LeadingArea.InteractableMapping.MappingAdded += () =>
            {

                if (string.IsNullOrEmpty(BaseController.Instance?.mouseData.CurrentHovered?.name))
                {
                    LeadingArea.InteractableMapping.InteractableName = BaseController.Instance?.mouseData.OldHovered?.name?? "Interaction";
                }
                else
                {
                    LeadingArea.InteractableMapping.InteractableName = BaseController.Instance.mouseData.CurrentHovered.name;

                }
                //LeadingArea.InteractableMapping
                //    .SetLeft(0)
                //    .WithAnimation();
                //UnityEngine.Debug.Log("<color=red>Fix for Laval: </color>" + $"To be updated");
                LeadingArea.InteractableMapping.Display();
            };
            LeadingArea.InteractableMapping.MappingRemoved += () =>
            {
                //LeadingArea.InteractableMapping
                //    .SetLeft(Length.Percent(-50f))
                //    .WithAnimation()
                //    .SetCallback(() => LeadingArea.InteractableMapping.InteractableName = null);
                //UnityEngine.Debug.Log("<color=red>Fix for Laval: </color>" + $"To be updated");
                LeadingArea.InteractableMapping.Hide();
                LeadingArea.InteractableMapping.InteractableName = null;
            };
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
                    if (TrailingArea.ActiveWindow != TrailingArea_C.WindowsEnum.ContextualMenu || forceUpdate) OpenContextualMenu(callbackOpen);
                    break;
                case ContextualMenuEnum.Close:
                    if (TrailingArea.ActiveWindow == TrailingArea_C.WindowsEnum.ContextualMenu || forceUpdate) CloseContextualMenu();
                    break;
                case ContextualMenuEnum.OpenOrClose:
                    if (TrailingArea.ActiveWindow == TrailingArea_C.WindowsEnum.ContextualMenu) CloseContextualMenu();
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
            //// If action is down don't update.
            //if (m_isContextualMenuDown) return;

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
                    UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"here");
                    Game.Cursor.Action = new LocalisationAttribute("Edit text", "Other", "EditText");
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
                //else UpdateContextualMenuActions(ContextualMenuActionEnum.OpenOrClose);
                else
                {
                    UnityEngine.Debug.Log("<color=red>Fix for Laval: </color>" + $"To be updated");
                    Game.Cursor.Action = null;
                    UpdateContextualMenuActions(ContextualMenuActionEnum.Null);
                }

                if (!ButtonsArea.IsActionButtonDisplayed) ButtonsArea.IsActionButtonDisplayed = true;
            }
            else
            {
                string CursorAction = null;
                if (BaseController.Exists && BaseController.Instance.mouseData.CurrentHovered != null) 
                    CursorAction = BaseController.Instance.mouseData.CurrentHovered.dto.name;

                if (string.IsNullOrEmpty(CursorAction) || CursorAction == "new tool") 
                    Game.Cursor.ActionText.LocalisedText = new LocalisationAttribute("Display contextual Menu", "Other", "DisplayInteractionsMenu");
                else 
                    Game.Cursor.Action = CursorAction;

                if (!ButtonsArea.IsActionButtonDisplayed) ButtonsArea.IsActionButtonDisplayed = true;

                //UpdateContextualMenuActions(ContextualMenuActionEnum.OpenOrClose);

                UnityEngine.Debug.Log("<color=red>Fix for Laval: </color>" + $"To be updated");
                ButtonDisplayer button = ObjectMenu.FirstOrDefault<ButtonDisplayer>();
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