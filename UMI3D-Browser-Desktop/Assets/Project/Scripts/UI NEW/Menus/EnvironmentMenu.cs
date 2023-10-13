using System;
using umi3d.baseBrowser.connection;
using umi3d.baseBrowser.Controller;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentMenu : BaseMenu
{
    private FormScreen m_ConnectionScreen;
    private LoadingScreen m_LoadingScreen;

    private Progress m_Progress;

    protected override void Start()
    {
        m_ConnectionScreen = new FormScreen(m_UiDocument.rootVisualElement.Q("Form"));
        m_LstScreen.Add(m_ConnectionScreen);
        m_LoadingScreen = new LoadingScreen(m_UiDocument.rootVisualElement.Q("Loading"));
        m_LstScreen.Add(m_LoadingScreen);
        m_MainScreen = m_LoadingScreen;

        base.Start();

        BaseConnectionProcess.Instance.ConnectionSucces += (media) =>
        {
            ShowScreen(m_LoadingScreen);
            m_MainScreen = m_LoadingScreen;
        };
        BaseConnectionProcess.Instance.ConnectionFail += (message) =>
        {
            var buttonLeave = new Button(BaseConnectionProcess.Instance.Leave);
            buttonLeave.text = "Leave";

            m_ErrorBox.Show("Server error", message, buttonLeave);
        };
        BaseConnectionProcess.Instance.LoadedEnvironment += () =>
        {
            /*GamePanel.AddScreenToStack = GameViews.Game;
            m_isContextualMenuDown = false;
            BaseController.Instance.CurrentController.ResetInputsWhenEnvironmentLaunch();
            OnMenuObjectContentChange();*/
        };
        BaseConnectionProcess.Instance.Connecting += (state) => m_LoadingScreen.Message = state;
        BaseConnectionProcess.Instance.RedirectionStarted += () =>
        {
            ShowScreen(m_LoadingScreen);
            m_MainScreen = m_LoadingScreen;
        };
        BaseConnectionProcess.Instance.RedirectionEnded += () => Debug.Log("[Menu] RedirectionEnded (TODO)");
        BaseConnectionProcess.Instance.ConnectionLost += () =>
        {
            BaseController.CanProcess = false;

            var buttonReconnect = new Button(UMI3DCollaborationClientServer.Reconnect);
            buttonReconnect.text = "Reconnect";

            var buttonLeave = new Button(BaseConnectionProcess.Instance.Leave);
            buttonLeave.text = "Leave";

            m_ErrorBox.Show("Connection to the server lost", "Leave the environment or try to reconnect ?", buttonReconnect, buttonLeave);
        };
        BaseConnectionProcess.Instance.ForcedLeave += (message) =>
        {
            var buttonLeave = new Button(BaseConnectionProcess.Instance.Leave);
            buttonLeave.text = "Leave";

            m_ErrorBox.Show("Forced Deconnection", message, buttonLeave);
        };
        BaseConnectionProcess.Instance.AskForDownloadingLibraries += (count, callback) =>
        {
            var buttonAccept = new Button(() => callback?.Invoke(true));
            buttonAccept.text = "Accept";

            var buttonDeny = new Button(() => callback?.Invoke(false));
            buttonDeny.text = "Deny";

            m_InfoBox.Show((count == 1) ? $"One assets library is required" : $"{count} assets libraries are required",
                "Download libraries and connect to the server ?",
                buttonAccept, buttonDeny);
        };
        BaseConnectionProcess.Instance.GetParameterDtos += GetParameterDtos;
        BaseConnectionProcess.Instance.LoadingLauncher += (value) =>
        {
            ShowScreen(m_LoadingScreen);
            m_MainScreen = m_LoadingScreen;
            m_LoadingScreen.ProgressValue = value;
        };
        BaseConnectionProcess.Instance.DisplayPopUpAfterLoadingFailed += (title, message, action) =>
        {
            var buttonResume = new Button(() => action?.Invoke(0));
            buttonResume.text = "Resume";

            var buttonStop = new Button(() => action?.Invoke(1));
            buttonStop.text = "Stop";

            m_ErrorBox.Show(title, message, buttonResume, buttonStop);
        };
        BaseConnectionProcess.Instance.EnvironmentLoaded += () =>
        {/*;
            Menu.Libraries.InitLibraries();
            Menu.Tips.InitTips();
            EnvironmentSettings.Instance.AudioSetting.GeneralVolume = ((int)Menu.Settings.Audio.Data.GeneralVolume) / 10f;*/
        };
        BaseConnectionProcess.Instance.UserCountUpdated += count =>
        {/*
            Game.NotifAndUserArea.UserList.RefreshList();
            Game.NotifAndUserArea.OnUserCountUpdated(count);
            Menu.GameData.ParticipantCount = count;*/
        };

        UMI3DCollaborationClientServer.onProgress.AddListener(OnProgress);
    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto pForm, Action<FormAnswerDto> pCallback)
    {
        m_ConnectionScreen.GetParameterDtos(pForm, pCallback);
        ShowScreen(m_ConnectionScreen);
        m_MainScreen = m_ConnectionScreen;
    }

    void OnProgress(Progress pProgress)
    {
        if (m_Progress != null)
        {
            m_Progress.OnCompleteUpdated -= OnCompleteUpdated;
            m_Progress.OnFailedUpdated -= OnFailedUpdated;
            m_Progress.OnStatusUpdated -= OnStatusUpdated;
        }
        m_Progress = pProgress;
        void OnCompleteUpdated(float i)
        {
            ShowScreen(m_LoadingScreen);
            m_MainScreen = m_LoadingScreen;
            m_LoadingScreen.ProgressValue = m_Progress.progressPercent;
            m_LoadingScreen.ProgressValueText = m_Progress.progressPercent.ToString("0.00") + "%";
        }
        void OnFailedUpdated(float i)
        {
            m_LoadingScreen.ProgressValue = m_Progress.progressPercent;
        }
        void OnStatusUpdated(string i)
        {
            ShowScreen(m_LoadingScreen);
            m_MainScreen = m_LoadingScreen;
            m_LoadingScreen.Message = m_Progress.currentState;
        }

        m_Progress.OnCompleteUpdated += OnCompleteUpdated;
        m_Progress.OnFailedUpdated += OnFailedUpdated;
        m_Progress.OnStatusUpdated += OnStatusUpdated;

        m_LoadingScreen.ProgressValue = m_Progress.progressPercent;
        m_LoadingScreen.Message = m_Progress.currentState;
    }
}