using System;
using umi3d.baseBrowser.connection;
using umi3d.baseBrowser.Controller;
using umi3d.cdk.collaboration;
using umi3d.common.interaction;
using UnityEngine.UIElements;

public class EnvironmentMenu : BaseMenu
{
    private FormScreen m_ConnectionScreen;
    private LoadingScreen m_LoadingScreen;

    protected override void Start()
    {
        base.Start();

        m_ConnectionScreen = new FormScreen(m_UiDocument.rootVisualElement.Q("Form"));
        m_LstScreen.Add(m_ConnectionScreen);
        m_LoadingScreen = new LoadingScreen(m_UiDocument.rootVisualElement.Q("Loading"));
        m_LstScreen.Add(m_LoadingScreen);

        BaseConnectionProcess.Instance.ConnectionSucces += (media) =>
        {
            ShowScreen(m_LoadingScreen);
        };
        BaseConnectionProcess.Instance.ConnectionFail += (message) =>
        {
            var buttonLeave = new Button(BaseConnectionProcess.Instance.Leave);
            buttonLeave.text = "Leave";

            m_ErrorBox.Show("Server error", message, buttonLeave);
        };
        /*BaseConnectionProcess.Instance.LoadedEnvironment += () =>
        {
            GamePanel.AddScreenToStack = GameViews.Game;
            m_isContextualMenuDown = false;
            BaseController.Instance.CurrentController.ResetInputsWhenEnvironmentLaunch();
            OnMenuObjectContentChange();
        };*/
        /*BaseConnectionProcess.Instance.Connecting += (state) => Loader.Loading.Message = state;*/
        BaseConnectionProcess.Instance.RedirectionStarted += () =>
        {
            ShowScreen(m_LoadingScreen);
        };
        /*BaseConnectionProcess.Instance.RedirectionEnded += () => GamePanel.AddScreenToStack = GameViews.Game;*/
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
            // TODO : Update Value
        };
        BaseConnectionProcess.Instance.DisplayPopUpAfterLoadingFailed += (title, message, action) =>
        {
            var buttonResume = new Button(() => action?.Invoke(0));
            buttonResume.text = "Resume";

            var buttonStop = new Button(() => action?.Invoke(1));
            buttonStop.text = "Stop";

            m_ErrorBox.Show(title, message, buttonResume, buttonStop);
        };
        /*BaseConnectionProcess.Instance.EnvironmentLoaded += () =>
        {
            Menu.Libraries.InitLibraries();
            Menu.Tips.InitTips();
            EnvironmentSettings.Instance.AudioSetting.GeneralVolume = ((int)Menu.Settings.Audio.Data.GeneralVolume) / 10f;
        };*/
        /*BaseConnectionProcess.Instance.UserCountUpdated += count =>
        {
            Game.NotifAndUserArea.UserList.RefreshList();
            Game.NotifAndUserArea.OnUserCountUpdated(count);
            Menu.GameData.ParticipantCount = count;
        };*/
    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto pForm, Action<FormAnswerDto> pCallback)
    {
        m_ConnectionScreen.GetParameterDtos(pForm, pCallback);
        m_ConnectionScreen.Show();
    }
}