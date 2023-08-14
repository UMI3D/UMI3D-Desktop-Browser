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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEngine;
using umi3d.baseBrowser.cursor;
using System.Threading;
using System.Collections;
using System;
using inetum.unityUtils;
using UnityEngine.Networking;

namespace umi3d.baseBrowser.connection
{
    public class BaseConnectionProcess : PersistentSingleBehaviour<BaseConnectionProcess>
    {
        protected const string LauncherPanelScene = "Connection";
        protected const string GamePanelScene = "Environment";

        [SerializeField]
        UMI3DClientLogger logger;

        #region Data

        [HideInInspector]
        public BaseClientIdentifier Identifier;
        [HideInInspector]
        public UMI3DCollabLoadingParameters LoadingParameters;

        #endregion

        UMI3DClientServerConnection clientServerConnection;

        protected override void Awake()
        {
            base.Awake();
            logger = new UMI3DClientLogger(mainTag: nameof(BaseConnectionProcess), mainContext: Instance, isThreadDisplayed: true);

            Identifier = Resources.Load<BaseClientIdentifier>("Scriptables/Connections/BaseClientIdentifier");
            Identifier.ShouldDownloadLib = ShouldDownloadLibraries;
            Identifier.GetParameters = (form, callback) => GetParameterDtos?.Invoke(form, callback);

            LoadingParameters =  Resources.Load<UMI3DCollabLoadingParameters>("Scriptables/GamePanel/CollabLoadingParameters");
            LoadingParameters.supportedformats.Clear();
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.gltf);
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.obj);
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.fbx);
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.png);
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.jpg);
#if UNITY_STANDALONE || UNITY_EDITOR
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_standalone_urp);
#elif UNITY_ANDROID
            LoadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
#endif

            logger.DebugTodo($"{nameof(Awake)}", $"Not force mono speaker mode. For now forced for bluetooth headset.");
            var settings = AudioSettings.GetConfiguration();
            settings.speakerMode = AudioSpeakerMode.Mono;
            AudioSettings.Reset(settings);
        }

        void Start()
        {
            UMI3DEnvironmentClient.ConnectionState.AddListener((state) => Connecting?.Invoke(state));
            UMI3DCollaborationClientServer.Instance.OnRedirectionStarted.AddListener(() => RedirectionStarted?.Invoke());
            UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.AddListener(() => RedirectionEnded?.Invoke());
            UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(() => ConnectionLost?.Invoke());
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener((s) => ForcedLeave?.Invoke(s));

            UMI3DCollaborationClientServer.EnvironmentProgress = () =>
            {
                logger.Debug($"{nameof(UMI3DCollaborationClientServer.EnvironmentProgress)}", $"Progess of joining an environment.");

                var p = new MultiProgress("Join Environement");

                //p.ResumeAfterFail = ResumeAfterFail;
                p.ResumeAfterFail = async (e) =>
                {
                    await Task.Delay(10000);
                    logger.DebugHack($"{nameof(UMI3DCollaborationClientServer.EnvironmentProgress)}", $"Join environment fail: {e}");
                    return true;
                };

                return p;
            };

            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(() => LeaveWithoutNotify());

            UMI3DEnvironmentClient.EnvironementLoaded.AddListener(() => EnvironmentLoaded?.Invoke());

            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinnedUserList += () => UserCountUpdated?.Invoke(UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count());
        }

        #region Launcher

        [HideInInspector]
        public event Action<string> ConnectionInitialized;
        [HideInInspector]
        public event Action<string> ConnectionInitializationFailled;
        [HideInInspector]
        public event Action DisplaySessions;
        [HideInInspector]
        public event Action<float> LoadingEnvironment;
        [HideInInspector]
        public event Action LoadedLauncher;

        static string url = null;

        public void ResetLauncherEvent()
        {
            ConnectionInitialized = null;
            ConnectionInitializationFailled = null;
            DisplaySessions = null;
            LoadingEnvironment = null;
            LoadedLauncher = null;
        }

        #endregion

        #region Environment

        [HideInInspector]
        public event System.Action<int, System.Action<bool>> AskForDownloadingLibraries;
        [HideInInspector]
        public event System.Action<common.interaction.ConnectionFormDto, System.Action<common.interaction.FormAnswerDto>> GetParameterDtos;
        [HideInInspector]
        public event System.Action<common.MediaDto> ConnectionSucces;
        [HideInInspector]
        public event System.Action<string> ConnectionFail;
        [HideInInspector]
        public event System.Action<string> Connecting;
        [HideInInspector]
        public event System.Action LoadedEnvironment;
        [HideInInspector]
        public event System.Action RedirectionStarted;
        [HideInInspector]
        public event System.Action RedirectionEnded;
        [HideInInspector]
        public event System.Action ConnectionLost;
        [HideInInspector]
        public event System.Action EnvironmentLeave;
        [HideInInspector]
        public event System.Action<string> ForcedLeave;
        [HideInInspector]
        public event System.Action<float> LoadingLauncher;
        [HideInInspector]
        public event System.Action<string, string, System.Action<int>> DisplayPopUpAfterLoadingFailed;
        [HideInInspector]
        public event System.Action EnvironmentLoaded;
        [HideInInspector]
        public event System.Action<int> UserCountUpdated;

        bool rememberNe = false;
        bool rememberLe = false;
        bool rememberUe = false;

        async Task<bool> ResumeAfterFail(System.Exception e)
        {
            switch (e)
            {
                case Umi3dNetworkingException ne:
                    if (rememberNe) return true;
                    (bool, bool) c = (false, false);
                    switch (ne.errorCode)
                    {
                        case 404:
                            //c = await DisplayPopUp("Resources not found",ne.Message +"\n"+ne.url);
                            Debug.Log("TODO : display pop up again");
                            return true;
                            break;
                        case 204:
                            c = await DisplayPopUp("Resources Downloaded was empty", ne.Message + "\n" + ne.url);
                            break;
                        case 501:
                            c = await DisplayPopUp("Resources acess was denied", ne.Message + "\n" + ne.url);
                            break;
                        default:
                            c = await DisplayPopUp($"Networking error {ne.errorCode}", $"{ne.Message}\n{ne.url}");
                            break;
                    }
                    rememberNe = c.Item2;
                    return c.Item1;
                case Umi3dLoadingException le:
                    if (rememberLe) return true;
                    (bool, bool) lc = await DisplayPopUp("Loading error", $"{le.Message}");
                    rememberLe = lc.Item2;
                    return lc.Item1;
                case Umi3dException ue:
                    if (rememberUe) return true;
                    (bool, bool) uc = await DisplayPopUp("Error", $"{ue.Message}");
                    rememberUe = uc.Item2;
                    return uc.Item1;
            }
            (bool, bool) u = await DisplayPopUp("Other Error", $"{e.Message}");
            rememberUe = u.Item2;
            return true;
        }

        async Task<(bool, bool)> DisplayPopUp(string title, string message)
        {
            UnityEngine.Debug.Log(title + " \n" + message + " \n" + "Ignore and resume loading ? ");

            bool? choise = null;
            System.Action<int> action = (index) => { choise = index == 0; };

            DisplayPopUpAfterLoadingFailed?.Invoke(title, message, action);

            while (choise == null) await UMI3DAsyncManager.Yield();
            return (choise.Value, false);
        }

        public void ResetEnvironmentEvents()
        {
            ConnectionSucces = null;
            ConnectionFail = null;
            ConnectionSucces = null;
            Connecting = null;
            LoadedEnvironment = null;
            RedirectionStarted = null;
            RedirectionEnded = null;
            ConnectionLost = null;
            EnvironmentLeave = null;
            ForcedLeave = null;
            AskForDownloadingLibraries = null;
            GetParameterDtos = null;
            DisplayPopUpAfterLoadingFailed = null;
            EnvironmentLoaded = null;
            UserCountUpdated = null;
        }

        /// <summary>
        /// Checks if users need to download libraries to join the environement.
        /// If yes, a screen is displayed to explain that.
        /// </summary>
        protected void ShouldDownloadLibraries(List<string> ids, System.Action<bool> callback)
        {
            if (ids.Count == 0) callback.Invoke(true);
            else AskForDownloadingLibraries?.Invoke(ids.Count, callback);
        }


        public void Leave() => UMI3DCollaborationClientServer.Logout();

        /// <summary>
        /// Clears the environment and goes back to the launcher.
        /// </summary>
        public void LeaveWithoutNotify()
        {
            cdk.UMI3DEnvironmentLoader.Clear();
            cdk.UMI3DResourcesManager.Instance.ClearCache();

            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);

            EnvironmentLeave?.Invoke();

            LoadLauncher();
        }

        public void TryReconnecting() => cdk.collaboration.UMI3DCollaborationClientServer.Reconnect();

        protected async void LoadLauncher()
        {
            var loadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(LauncherPanelScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (!loadAsync.isDone)
            {
                LoadingLauncher?.Invoke(loadAsync.progress);
                await UMI3DAsyncManager.Yield();
            }

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GamePanelScene);
        }

        #endregion
    }
}
