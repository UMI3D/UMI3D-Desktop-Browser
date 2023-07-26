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
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.notification;
using umi3d.cdk.collaboration;
using umi3d.commonScreen;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.cursor.BaseCursor;
using static umi3d.commonScreen.game.GamePanel_C;

namespace umi3d.baseBrowser.connection
{
    public abstract partial class BaseGamePanelController : inetum.unityUtils.SingleBehaviour<BaseGamePanelController>
    {
        #region Field

        public UIDocument document;
        [HideInInspector]
        public PanelSettings PanelSettings;

        [HideInInspector]
        public NotificationLoader NotificationLoader;

        public GamePanel_C GamePanel;

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
                GamePanel.CurrentView = GameViews.Loader;
                Loader.Loading.TitleLabel.LocalisedText = new LocalisationAttribute("Loading environment","Other", "LoadingEnv");
                Loader.Loading.Value = 0;
                Menu.GameData.WorldName = media.name;
            };
            BaseConnectionProcess.Instance.ConnectionFail += (message) =>
            {
                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = new LocalisationAttribute("Server error", "ErrorStrings", "ServerError");
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.LoadedEnvironment += () =>
            {
                GamePanel.AddScreenToStack = GameViews.Game;
                m_isContextualMenuDown = false;
                BaseController.Instance.CurrentController.ResetInputsWhenEnvironmentLaunch();
                OnMenuObjectContentChange();
            };
            BaseConnectionProcess.Instance.Connecting += (state) => Loader.Loading.Message = state;
            BaseConnectionProcess.Instance.RedirectionStarted += () =>
            {
                GamePanel.AddScreenToStack = GameViews.Loader;
                Loader.CurrentScreen = LoaderScreens.Loading;
            };
            BaseConnectionProcess.Instance.RedirectionEnded += () => GamePanel.AddScreenToStack = GameViews.Game;
            BaseConnectionProcess.Instance.ConnectionLost += () =>
            {
                BaseController.CanProcess = false;

                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = new LocalisationAttribute("Connection to the server lost", "ErrorStrings", "ConnectionLost");
                dialoguebox.Message = new LocalisationAttribute
                (
                    "Leave the environment or try to reconnect ?",
                    "ErrorStrings", "LeaveOrTry"
                );
                dialoguebox.ChoiceAText = new LocalisationAttribute("Reconnect", "GenericStrings", "Reconnect");
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
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
                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = new LocalisationAttribute("Forced Deconnection", "ErrorStrings", "ForcedDeco");
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.AskForDownloadingLibraries += (count, callback) =>
            {
                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = new LocalisationAttribute
                (
                    (count == 1) ? $"One assets library is required" : $"{count} assets libraries are required",
                    "ErrorStrings", 
                    (count == 1) ? "AssetsLibRequired1" : "AssetsLibRequired", 
                    (count == 1) ? null : new string[1] { count.ToString() }
                );
                dialoguebox.Message = new LocalisationAttribute("Download libraries and connect to the server ?", "ErrorStrings", "DownloadAndConnect");
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceAText = new LocalisationAttribute("Accept", "GenericStrings", "Accept");
                dialoguebox.ChoiceB.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = new LocalisationAttribute("Deny", "GenericStrings", "Deny");
                dialoguebox.Callback = (index) => callback?.Invoke(index == 0);
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.GetParameterDtos += GetParameterDtos;
            BaseConnectionProcess.Instance.LoadingLauncher += (value) =>
            {
                GamePanel.AddScreenToStack = GameViews.Loader;
                Loader.CurrentScreen = LoaderScreens.Loading;
                Loader.Loading.Title = new LocalisationAttribute("Leave environment", "Other", "LeaveEnvironment");
                Loader.Loading.Value = value / 100f;
            };
            BaseConnectionProcess.Instance.DisplayPopUpAfterLoadingFailed += (title, message, action) =>
            {
                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = title;
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = new LocalisationAttribute("Resume", "GenericStrings", "Resume");
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = new LocalisationAttribute("Stop", "GenericStrings", "Stop");
                dialoguebox.Callback = action;
                dialoguebox.Enqueue(root);
            };
            BaseConnectionProcess.Instance.EnvironmentLoaded += () =>
            {
                Menu.Libraries.InitLibraries();
                Menu.Tips.InitTips();
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

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(document != null);
            Debug.Assert(FormMenu != null);
            Debug.Assert(formMenuDisplay != null);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            PanelSettings = Resources.Load<PanelSettings>("PanelSettings");
            NotificationLoader = Resources.Load<NotificationLoader>("Scriptables/GamePanel/NotificationLoader");
            m_time_Start = DateTime.Now;
        }

        protected virtual void Start()
        {
            BaseConnectionProcess.Instance.ResetEnvironmentEvents();

            root.Add(TooltipsLayer_C.Instance);
            GamePanel = root.Q<GamePanel_C>();

            InitConnectionProcess();

            InitLoader();
            InitMenu();
            InitGame();

            InitControls();

            GamePanel.CurrentView = GameViews.Loader;

            BaseConnectionProcess.Instance.EnvironmentLeave += () =>
            {
                UnityEngine.Debug.Log($"leave");
                var clh = UMI3DCollaborationLoadingHandler.Instance;
                UMI3DCollaborationLoadingHandler.Instance = null;
                Destroy(clh.gameObject);
            };
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

            if (GamePanel.CurrentView != GameViews.Game || Menu.Settings.Resolution.GameResolutionSegmentedPicker.ValueEnum == preferences.SettingsPreferences.ResolutionEnum.Custom) return;

            var renderScaleValue = Menu.Settings.Resolution.RenderScaleSlider.value;

            if (fps < Menu.Settings.Resolution.TargetFPS - 20 && renderScaleValue > 0.01) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 0.95f);
            else if (fps > Menu.Settings.Resolution.TargetFPS - 5) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 1.05f);
        }

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