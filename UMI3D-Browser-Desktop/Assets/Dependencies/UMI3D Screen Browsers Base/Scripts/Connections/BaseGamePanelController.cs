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
using umi3d.baseBrowser.Navigation;
using umi3d.baseBrowser.notification;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.Container;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.Controller.BaseCursor;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseGamePanelController : inetum.unityUtils.SingleBehaviour<BaseGamePanelController>
    {
        #region Field

        public UIDocument document;

        [Header("Form Loader")]
        public cdk.menu.MenuAsset FormMenu;
        public cdk.menu.view.MenuDisplayManager formMenuDisplay;
        public LoaderFormContainer FormContainer;

        [Header("Object Menu")]
        public ObjectMenuFormContainer ObjectMenu;
        public cdk.menu.view.MenuDisplayManager ObjectMenuDisplay;

        [HideInInspector]
        public NotificationLoader NotificationLoader;

        public CustomGamePanel GamePanel;
        public CustomLoader Loader => GamePanel.Loader;
        public CustomGame Game => GamePanel.Game;
        public CustomGameMenu Menu => GamePanel.Menu;

        protected VisualElement root => document.rootVisualElement;
        protected VisualElement logo;

        protected DateTime m_time_Start;

        #endregion

        protected virtual void InitLoader()
        {
            Loader.Version = UMI3DVersion.version;
            Loader.Loading.Title = "Connection";
            Loader.Loading.BackText = "Leave";
            Loader.Loading.Button_Back.clicked += BaseConnectionProcess.Instance.Leave;
            Loader.Loading.LoadingBar.highValue = 1;
            Loader.Form.BackText = "Leave";
            Loader.Form.Button_Back.clicked += BaseConnectionProcess.Instance.Leave;
            Loader.CurrentScreen = LoaderScreens.Loading;
            Loader.ControllerCanProcess = (value) => BaseController.CanProcess = value;
            Loader.SetMovement = (value) => SetMovement(value, CursorMovement.Free);
            Loader.UnSetMovement = (value) => UnSetMovement(value);

            umi3d.cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded?.AddListener(() => Loader.ControllerCanProcess?.Invoke(true));

            FormContainer.GetContainer = () => Loader.Form.ScrollView;
            FormContainer.InsertDisplayer = (index, displayer) => Loader.Form.Insert(index, displayer);
            FormContainer.RemoveDisplayer = displayer => Loader.Form.Remove(displayer);

            ObjectMenuDisplay.menu.onContentChange.AddListener(OnMenuObjectContentChange);
        }

        protected virtual void InitGame()
        {
            BaseCursor.Instance.UpdateCursor += UpdateCursor;

            var buttonsArea = Game.TrailingArea.ButtonsArea;
            buttonsArea.Jump.ClickedDown += () => BaseFPSNavigation.Instance.IsJumping = true;
            buttonsArea.Jump.ClickedUp += () => BaseFPSNavigation.Instance.IsJumping = false;
            buttonsArea.Crouch.ClickedDown += () => BaseFPSNavigation.Instance.WantToCrouch = true;
            buttonsArea.Crouch.ClickedUp += () => BaseFPSNavigation.Instance.WantToCrouch = false;

            Menu.Leave.clicked += () =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = "Do you want to leave the environment ?";
                dialoguebox.Message = "";
                dialoguebox.ChoiceAText = "Stay";
                dialoguebox.ChoiceBText = "Leave";
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.Callback = (index) =>
                {
                    if (index != 0) BaseConnectionProcess.Instance.Leave();
                };
                dialoguebox.AddToTheRoot(root);
            };
            var envAudioSettings = EnvironmentSettings.Instance.AudioSetting;
            Menu.Settings.Audio.GeneralVolumeValeChanged += value => envAudioSettings.GeneralVolume = value;
            envAudioSettings.StatusChanged += isOn => Menu.Settings.Audio.SetGeneralVolumeValueWithoutNotify(envAudioSettings.GeneralVolume * 10f);

            var infArea = Game.TopArea.InformationArea;
            EnvironmentSettings.Instance.AudioSetting.StatusChanged += (value) => infArea.IsSoundOn = value;
            EnvironmentSettings.Instance.MicSetting.StatusChanged += (value) => infArea.IsMicOn = value;
            infArea.SoundStatusChanged += () => EnvironmentSettings.Instance.AudioSetting.Toggle();
            infArea.MicStatusChanged += () => EnvironmentSettings.Instance.MicSetting.Toggle();

            NotificationLoader.Notification2DReceived += dto => infArea.AddNotification(dto);

            Game.TrailingArea.ButtonsArea.Emote.clicked += () => Game.TrailingArea.DisplayEmoteWindow = !Game.TrailingArea.DisplayEmoteWindow;
            Game.BottomArea.Emote.clicked += () => Game.BottomArea.ButtonSelected = CustomBottomArea.BottomBarButton.Emote;
            EmoteManager.Instance.EmoteReceived += emotes => 
            {
                Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = true;
                Game.TrailingArea.EmoteWindow.OnEmoteReceived(emotes);
                Game.BottomArea.EmoteWindow.OnEmoteReceived(emotes);
            };
            EmoteManager.Instance.NoEmoteReeived += () => 
            {
                Game.TrailingArea.ButtonsArea.IsEmoteButtonDisplayed = false;
                Game.TrailingArea.EmoteWindow.Reset();
                Game.BottomArea.EmoteWindow.Reset();
            };
            EmoteManager.Instance.EmoteUpdated += emote =>
            {
                Game.TrailingArea.EmoteWindow.OnUpdateEmote(emote);
                Game.BottomArea.EmoteWindow.OnUpdateEmote(emote);
            };

            ObjectMenu.GetContainer = () => Game.TrailingArea.ObjectMenu;
            ObjectMenu.DisplayObjectMenu = value =>
            {
                if (GamePanel.CurrentView != CustomGamePanel.GameViews.Game) return;
                Game.TrailingArea.DisplayObjectMenu = value;
            };
            ObjectMenu.InsertDisplayer = (index, displayer) => Game.TrailingArea.ObjectMenu.Insert(index, displayer);
            ObjectMenu.RemoveDisplayer = displayer => Game.TrailingArea.ObjectMenu.Remove(displayer);
        }
        
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(document != null);
            Debug.Assert(FormMenu != null);
            Debug.Assert(formMenuDisplay != null);

            NotificationLoader = Resources.Load<NotificationLoader>("Scriptables/GamePanel/NotificationLoader");
            m_time_Start = DateTime.Now;
        }

        protected virtual void Start()
        {
            BaseConnectionProcess.Instance.ResetEnvironmentEvents();

            GamePanel = root.Q<CustomGamePanel>();

            InitLoader();
            InitGame();

            GamePanel.CurrentView = CustomGamePanel.GameViews.Loader;

            BaseConnectionProcess.Instance.ConnectionSucces += (media) =>
            {
                GamePanel.CurrentView = CustomGamePanel.GameViews.Loader;
                Loader.Loading.Title = "Loading environment";
                Loader.Loading.Value = 0;
                Game.TopArea.InformationArea.EnvironmentName = media.name;
            };
            BaseConnectionProcess.Instance.ConnectionFail += (message) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = "Server error";
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = "Leave";
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.AddToTheRoot(root);
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
                dialoguebox.AddToTheRoot(root);
            };
            BaseConnectionProcess.Instance.ForcedLeave += (message) =>
            {
                var dialoguebox = CreateDialogueBox();
                dialoguebox.Type = DialogueboxType.Default;
                dialoguebox.Title = "Forced Deconnection";
                dialoguebox.Message = message;
                dialoguebox.ChoiceAText = "Leave";
                dialoguebox.Callback = (index) => BaseConnectionProcess.Instance.Leave();
                dialoguebox.AddToTheRoot(root);
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
                dialoguebox.AddToTheRoot(root);
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
                dialoguebox.AddToTheRoot(root);
            };
            BaseConnectionProcess.Instance.EnvironmentLoaded += () =>
            {
                Menu.Libraries.InitLibraries();
                EnvironmentSettings.Instance.AudioSetting.GeneralVolume = ((int)Menu.Settings.Audio.Data.GeneralVolume) / 10f;
            };
            BaseConnectionProcess.Instance.UserCountUpdated += count =>
            {
                Game.TopArea.InformationArea.UserList.RefreshList();
                Menu.GameData.ParticipantCount = count;
            };
        }

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

            UnityEngine.Debug.Log($"fps = {fps}, {Menu.Settings.Resolution.RenderScale.value}, {QualitySettings.names[QualitySettings.GetQualityLevel()]}");

            if (GamePanel.CurrentView != CustomGamePanel.GameViews.Game || Menu.Settings.Resolution.SegmentedResolution.ValueEnum == preferences.SettingsPreferences.ResolutionEnum.Custom) return;

            var renderScaleValue = Menu.Settings.Resolution.RenderScale.value;

            if (fps < Menu.Settings.Resolution.TargetFPS - 20 && renderScaleValue > 0.01) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 0.95f);
            else if (fps > Menu.Settings.Resolution.TargetFPS - 5) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 1.05f);
        }

        public abstract CustomDialoguebox CreateDialogueBox();

        /// <summary>
        /// Asks users some parameters when they join the environement.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="callback"></param>
        protected void GetParameterDtos(common.interaction.FormDto form, System.Action<common.interaction.FormAnswerDto> callback)
        {
            Loader.CurrentScreen = LoaderScreens.Form;

            if (form == null) callback.Invoke(null);
            else
            {
                common.interaction.FormAnswerDto answer = new common.interaction.FormAnswerDto()
                {
                    boneType = 0,
                    hoveredObjectId = 0,
                    id = form.id,
                    toolId = 0,
                    answers = new List<common.interaction.ParameterSettingRequestDto>()
                };

                FormMenu.menu.RemoveAll();
                formMenuDisplay.CreateMenuAndDisplay(true, false);
                FormMenu.menu.Name = form.name;

                foreach (var param in form.fields)
                {
                    var c = cdk.interaction.GlobalToolMenuManager.GetInteractionItem(param);
                    FormMenu.menu.Add(c.Item1);
                    answer.answers.Add(c.Item2);
                }

                ButtonMenuItem send = new ButtonMenuItem() { Name = "Join" };
                UnityEngine.Events.UnityAction<bool> action = (bool b) =>
                {
                    formMenuDisplay.Hide(false);
                    FormMenu.menu.RemoveAll();
                    callback.Invoke(answer);
                    Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Center);
                    cdk.collaboration.LocalInfoSender.CheckFormToUpdateAuthorizations(form);
                };
                send.Subscribe(action);
                FormMenu.menu.Add(send);
            }
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
        
        protected virtual void OnMenuObjectContentChange()
        {
            var count = ObjectMenuDisplay.menu.Count;

            if (count == 0)
            {
                ObjectMenuDisplay.Collapse(false);
                if (Game.TrailingArea.ButtonsArea.IsActionButtonDisplayed) Game.TrailingArea.ButtonsArea.IsActionButtonDisplayed = false;
            }
            else
            {
                if (!Game.TrailingArea.ButtonsArea.IsActionButtonDisplayed) Game.TrailingArea.ButtonsArea.IsActionButtonDisplayed = true;
                if (Game.TrailingArea.ButtonsArea.MainActionUp != null) return;
                Game.TrailingArea.ButtonsArea.MainActionUp = () =>
                {
                    if (ObjectMenuDisplay.isDisplaying) ObjectMenuDisplay.Collapse(true);
                    else ObjectMenuDisplay.Expand(false);
                };
            }
        }
    }
}
