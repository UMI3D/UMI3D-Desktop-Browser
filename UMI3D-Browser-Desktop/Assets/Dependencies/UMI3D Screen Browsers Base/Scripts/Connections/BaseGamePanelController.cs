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
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.emotes;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using umi3d.baseBrowser.notification;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using umi3d.mobileBrowser.Controller;
using umi3d.mobileBrowser.interactions;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.Controller.BaseCursor;

namespace umi3d.baseBrowser.connection
{
    public abstract partial class BaseGamePanelController : inetum.unityUtils.SingleBehaviour<BaseGamePanelController>
    {
        #region Field

        public UIDocument document;

        [HideInInspector]
        public NotificationLoader NotificationLoader;

        public CustomGamePanel GamePanel;

        protected VisualElement root => document.rootVisualElement;
        protected VisualElement logo;

        protected DateTime m_time_Start;

        protected System.Action m_next;
        

        #endregion

        #region Initialization of the Connection Process

        protected virtual void InitConnectionProcess()
        {
            BaseConnectionProcess.Instance.ConnectionSucces += (media) =>
            {
                GamePanel.CurrentView = CustomGamePanel.GameViews.Loader;
                Loader.Loading.Title = "Loading environment";
                Loader.Loading.Value = 0;
                Menu.GameData.WorldName = media.name;
            };
            BaseConnectionProcess.Instance.ConnectionFail += (message) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = "Server error";
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = "Leave";
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.LoadedEnvironment += () => GamePanel.AddScreenToStack = CustomGamePanel.GameViews.Game;
            BaseConnectionProcess.Instance.Connecting += (state) => Loader.Loading.Message = state;
            BaseConnectionProcess.Instance.RedirectionStarted += () =>
            {
                GamePanel.AddScreenToStack = CustomGamePanel.GameViews.Loader;
                Loader.CurrentScreen = LoaderScreens.Loading;
            };
            BaseConnectionProcess.Instance.RedirectionEnded += () => GamePanel.AddScreenToStack = CustomGamePanel.GameViews.Game;
            BaseConnectionProcess.Instance.ConnectionLost += () =>
            {
                BaseController.CanProcess = false;

                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = "Connection to the server lost";
                dialoguebox.Message = "Leave the environment or try to reconnect ?";
                dialoguebox.ChoiceAText = "Reconnect";
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = "Leave";
                dialoguebox.Callback = (index) =>
                {
                    BaseController.CanProcess = true;
                    if (index == 0) cdk.collaboration.UMI3DCollaborationClientServer.Reconnect();
                    else BaseConnectionProcess.Instance.Leave();
                };
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.ForcedLeave += (message) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = "Forced Deconnection";
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = "Leave";
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.AskForDownloadingLibraries += (count, callback) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = (count == 1) ? $"One assets library is required" : $"{count} assets libraries are required";
                dialoguebox.Message = "Download libraries and connect to the server ?";
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceAText = "Accept";
                dialoguebox.ChoiceB.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = "Deny";
                dialoguebox.Callback = (index) => callback?.Invoke(index == 0);
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.GetParameterDtos += GetParameterDtos;
            BaseConnectionProcess.Instance.LoadingLauncher += (value) =>
            {
                GamePanel.AddScreenToStack = CustomGamePanel.GameViews.Loader;
                Loader.CurrentScreen = LoaderScreens.Loading;
                Loader.Loading.Title = "Leaving environment";
                Loader.Loading.Value = value / 100f;
            };
            BaseConnectionProcess.Instance.DisplayPopUpAfterLoadingFailed += (title, message, action) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = title;
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = "Resume";
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = "Stop";
                dialoguebox.Callback = action;
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.EnvironmentLoaded += () =>
            {
                Menu.Libraries.InitLibraries();
                EnvironmentSettings.Instance.AudioSetting.GeneralVolume = ((int)Menu.Settings.Audio.Data.GeneralVolume) / 10f;
            };
            BaseConnectionProcess.Instance.UserCountUpdated += count =>
            {
                Game.NotifAndUserArea.UserList.RefreshList();
                Game.NotifAndUserArea.OnUserCountUpdated(count);
                Menu.GameData.ParticipantCount = count;
            };
        }

        #endregion

        #region Initialization of the Controls

        protected virtual void InitControls()
        {
            KeyboardShortcut.AddUpListener(ShortcutEnum.MuteUnmute, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"mute unmute");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.PushToTalk, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"push to talk");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.GeneraVolume, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"GeneraVolume");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.IncreaseVolume, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"IncreaseVolume");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.DeacreaseVolue, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"DeacreaseVolue");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.Cancel, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"Cancel");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.Submit, () => 
            {
                m_next?.Invoke();
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.GameMenu, () =>
            {
                if (GamePanel.CurrentView == CustomGamePanel.GameViews.GameMenu)
                {
                    GamePanel.AddScreenToStack = CustomGamePanel.GameViews.Game;
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Center);
                    CloseGameWindows();
                }
                else
                {
                    GamePanel.AddScreenToStack = CustomGamePanel.GameViews.GameMenu;
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);
                }
            });

            KeyboardShortcut.AddDownListener(ShortcutEnum.ContextualMenu, () =>
            {
                if
                (
                    GamePanel.CurrentView == CustomGamePanel.GameViews.GameMenu
                    || GamePanel.CurrentView == CustomGamePanel.GameViews.Loader
                ) return;

                if (BaseCursor.Movement == CursorMovement.Free || m_contextualMenuAction == null) return;
                BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

                CloseGameWindows();

                m_contextualMenuAction?.Invoke();
            });

            KeyboardShortcut.AddDownListener(ShortcutEnum.Notification, () =>
            {
                if
                (
                    GamePanel.CurrentView == CustomGamePanel.GameViews.GameMenu
                    || GamePanel.CurrentView == CustomGamePanel.GameViews.Loader
                ) return;

                if
                (
                    !Game.DisplayNotifUsersArea
                    || Game.NotifAndUserArea.AreaPanel != CustomNotifAndUsersArea.NotificationsOrUsers.Notifications)
                {
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

                    if (!Game.DisplayNotifUsersArea) Game.DisplayNotifUsersArea = true;
                    Game.NotifAndUserArea.AreaPanel = CustomNotifAndUsersArea.NotificationsOrUsers.Notifications;
                }
                else
                {
                    CloseGameWindows();
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Center);
                }
            });

            KeyboardShortcut.AddDownListener(ShortcutEnum.UserList, () =>
            {
                if
                (
                    GamePanel.CurrentView == CustomGamePanel.GameViews.GameMenu
                    || GamePanel.CurrentView == CustomGamePanel.GameViews.Loader
                ) return;

                if 
                (
                    !Game.DisplayNotifUsersArea 
                    || Game.NotifAndUserArea.AreaPanel != CustomNotifAndUsersArea.NotificationsOrUsers.Users)
                {
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

                    if (!Game.DisplayNotifUsersArea) Game.DisplayNotifUsersArea = true;
                    Game.NotifAndUserArea.AreaPanel = CustomNotifAndUsersArea.NotificationsOrUsers.Users;
                }
                else
                {
                    CloseGameWindows();
                    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Center);
                }
            });

            KeyboardEmote.EmotePressed += index =>
            {
                if
                (
                    GamePanel.CurrentView == CustomGamePanel.GameViews.GameMenu
                    || GamePanel.CurrentView == CustomGamePanel.GameViews.Loader
                    || BaseCursor.Movement == CursorMovement.Free
                ) return;

                if (CustomEmoteWindow.Emotes == null || CustomEmoteWindow.Emotes.Count <= index) return;
                var emote = CustomEmoteWindow.Emotes[index];
                emote.PlayEmote(emote);
            };

            BaseConnectionProcess.Instance.EnvironmentLeave += () => NotifAndUsersArea_C.Instance = null;

            Game.TrailingArea.ButtonsArea.MainActionDown = MainMobileAction.OnClickedDown;
            Game.TrailingArea.ButtonsArea.MainActionUp = MainMobileAction.OnClickedUp;
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(document != null);
            Debug.Assert(FormMenu != null);
            Debug.Assert(formMenuDisplay != null);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            NotificationLoader = Resources.Load<NotificationLoader>("Scriptables/GamePanel/NotificationLoader");
            m_time_Start = DateTime.Now;
        }

        protected virtual void Start()
        {
            BaseConnectionProcess.Instance.ResetEnvironmentEvents();

            GamePanel = root.Q<CustomGamePanel>();

            InitConnectionProcess();

            InitLoader();
            InitMenu();
            InitGame();

            InitControls();

            GamePanel.CurrentView = CustomGamePanel.GameViews.Loader;
        }

        #region OnDestroy

        protected virtual void ConcludeGame_UserList()
        {
            UMI3DEnvironmentClient.EnvironementJoinned.RemoveListener(Game.NotifAndUserArea.UserList.OnEnvironmentChanged);
            UMI3DUser.OnUserMicrophoneStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAvatarStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAttentionStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnRemoveUser.RemoveListener(Game.NotifAndUserArea.UserList.RemoveUser);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ConcludeGame_UserList();
        }

        #endregion

        float fps = 30;
        private void Update()
        {
            var time = DateTime.Now - m_time_Start;
            Menu.GameData.Time = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");

            //FrameManagement();
        }

        protected void FrameManagement()
        {
            float newFPS = 1.0f / Time.deltaTime;
            fps = Mathf.Lerp(fps, newFPS, Time.deltaTime);

            UnityEngine.Debug.Log($"fps = {fps}, {Menu.Settings.Resolution.RenderScaleSlider.value}, {QualitySettings.names[QualitySettings.GetQualityLevel()]}");

            if (GamePanel.CurrentView != CustomGamePanel.GameViews.Game || Menu.Settings.Resolution.GameResolutionSegmentedPicker.ValueEnum == preferences.SettingsPreferences.ResolutionEnum.Custom) return;

            var renderScaleValue = Menu.Settings.Resolution.RenderScaleSlider.value;

            if (fps < Menu.Settings.Resolution.TargetFPS - 20 && renderScaleValue > 0.01) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 0.95f);
            else if (fps > Menu.Settings.Resolution.TargetFPS - 5) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 1.05f);
        }

        public abstract CustomDialoguebox CreateDialogueBox();

        public void UpdateCursor(CursorState state)
        {
            switch (state)
            {
                case CursorState.Default:
                    Game.Cursor.State = ElementPseudoState.Enabled;
                    break;
                case CursorState.Hover:
                    Game.Cursor.State = ElementPseudoState.Hover;
                    break;
                case CursorState.Clicked:
                    Game.Cursor.State = ElementPseudoState.Hover;
                    break;
                case CursorState.FollowCursor:
                    Game.Cursor.State = ElementPseudoState.Enabled;
                    break;
                default:
                    break;
            }
        }

        
    }
}
