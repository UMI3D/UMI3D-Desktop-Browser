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
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace umi3d.baseBrowser.connection
{
    public class BaseConnectionProcess : inetum.unityUtils.PersistentSingleBehaviour<BaseConnectionProcess>
    {
        protected const string LauncherPanelScene = "Connection";
        protected const string GamePanelScene = "Environment";

        #region Data
        [HideInInspector]
        public preferences.ServerPreferences.ServerData currentServer;
        [HideInInspector]
        public List<preferences.ServerPreferences.ServerData> savedServers;
        [HideInInspector]
        public preferences.ServerPreferences.Data currentConnectionData;
        [HideInInspector]
        public cdk.collaboration.LaucherOnMasterServer masterServer;
        [HideInInspector]
        public common.MediaDto mediaDto;

        [HideInInspector]
        public BaseClientIdentifier Identifier;
        [HideInInspector]
        public UniversalRenderPipelineAsset RenderPipeline;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            currentServer = preferences.ServerPreferences.GetPreviousServerData() ?? new preferences.ServerPreferences.ServerData();
            currentConnectionData = preferences.ServerPreferences.GetPreviousConnectionData() ?? new preferences.ServerPreferences.Data();
            savedServers = preferences.ServerPreferences.GetRegisteredServerData() ?? new List<preferences.ServerPreferences.ServerData>();
            masterServer = new cdk.collaboration.LaucherOnMasterServer();

            Identifier = Resources.Load<BaseClientIdentifier>("Scriptables/Connections/BaseClientIdentifier");
            Identifier.ShouldDownloadLib = ShouldDownloadLibraries;
            Identifier.GetParameters = (form, callback) => GetParameterDtos?.Invoke(form, callback);
            RenderPipeline = Resources.Load<UniversalRenderPipelineAsset>("Scriptables/Rendering/UniversalRenderPipelineAsset");
        }

        void Start()
        {
            cdk.collaboration.UMI3DEnvironmentClient.ConnectionState.AddListener((state) => Connecting?.Invoke(state));
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnRedirectionStarted.AddListener(() => RedirectionStarted?.Invoke());
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.AddListener(() => RedirectionEnded?.Invoke());
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnConnectionLost.AddListener(() => ConnectionLost?.Invoke());
            cdk.collaboration.UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener((s) => ForcedLeave?.Invoke(s));
        }

        #region Launcher

        [HideInInspector]
        public event System.Action DisplaySessions;
        [HideInInspector]
        public event System.Action<float> LoadingEnvironment;
        [HideInInspector]
        public event System.Action LoadedLauncher;

        static bool onlyOneConnection = false;
        static bool? _mediaDtoFound = null;
        static bool? _masterServerFound = null;
        static string url = null;

        static bool mediaDtoFound
        {
            get => _mediaDtoFound ?? false;
            set => _mediaDtoFound = value;
        }

        static bool masterServerFound
        {
            get => _masterServerFound ?? false;
            set => _masterServerFound = value;
        }

        public void ResetLauncherEvent()
        {
            DisplaySessions = null;
            LoadingEnvironment = null;
            LoadedLauncher = null;
        }

        /// <summary>
        /// Initiates the connection, if a connection is already in process return.
        /// </summary>
        public async Task InitConnect(bool saveInfo = false)
        {
            if (onlyOneConnection)
            {
                Debug.Log("Only one connection at a time");
                return;
            }
            await ConnectWithMasterServerOrMediaDto(saveInfo);
            while (onlyOneConnection) await UMI3DAsyncManager.Yield();
        }

        protected async Task ConnectWithMasterServerOrMediaDto(bool saveInfo = false)
        {
            if (onlyOneConnection)
            {
                Debug.Log("Only one connection at a time");
                return;
            }

            onlyOneConnection = true;
            _mediaDtoFound = null;
            _masterServerFound = null;

            WaitForError();

            //1. Try to find a master server, if it found show sessions.
            masterServer.ConnectToMasterServer
            (
                () =>
                {
                    if (mediaDtoFound) return;

                    masterServer.RequestInfo
                    (
                        (name, icon) =>
                        {
                            if (mediaDtoFound) return;
                            masterServerFound = true;

                            currentServer.serverName = name;
                            currentServer.serverIcon = icon;
                            preferences.ServerPreferences.StoreUserData(currentServer);
                            if (saveInfo) StoreServer();
                        },
                        () => masterServerFound = false
                    );

                    DisplaySessions?.Invoke();
                },
                currentServer.serverUrl,
                () => masterServerFound = false
            );

            //2. try to get a mediaDto
            mediaDto = await GetMediaDto();
            if (mediaDto == null || masterServerFound)
            {
                mediaDtoFound = false;
                return;
            }
            mediaDtoFound = true;
            ConnectWithMediaDto(mediaDto, saveInfo);
        }

        public void ConnectWithMediaDto(string ip, string port)
        {
            var curentUrl = FormatUrl(currentServer.serverUrl, null) + common.UMI3DNetworkingKeys.media;
            ConnectWithMediaDto(new common.MediaDto { url = curentUrl }, false);
        }

        protected void ConnectWithMediaDto(common.MediaDto media, bool saveInfo)
        {
            mediaDto = media;

            currentServer.serverName = media.name;
            currentServer.serverIcon = media?.icon2D?.variants?.FirstOrDefault()?.url;
            preferences.ServerPreferences.StoreUserData(currentServer);
            if (saveInfo) StoreServer();

            currentConnectionData.environmentName = media.name;
            currentConnectionData.ip = media.url;
            currentConnectionData.port = null;
            StoreCurrentConnectionDataAndConnect();
        }

        protected void StoreServer()
        {
            if (savedServers.Find((server) => server.serverName == currentServer.serverName) == null) 
                savedServers.Add(currentServer);
            preferences.ServerPreferences.StoreRegisteredServerData(savedServers);
        }

        public async Task<common.MediaDto> GetMediaDto()
        {
            var curentUrl = FormatUrl(currentServer.serverUrl, null) + common.UMI3DNetworkingKeys.media;
            url = curentUrl;
            try
            {
                return await cdk.collaboration.UMI3DCollaborationClientServer.GetMedia
                (
                    url, 
                    (e) => url == curentUrl && e.count < 3
                );
            }
            catch
            {
                return null;
            }
        }

        private async void WaitForError()
        {
            while (onlyOneConnection)
            {
                await UMI3DAsyncManager.Yield();
                if (masterServerFound || mediaDtoFound)
                    return;
                if (_masterServerFound != null && _mediaDtoFound != null)
                {
                    onlyOneConnection = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Store current connection data and Load environment.
        /// </summary>
        protected void StoreCurrentConnectionDataAndConnect()
        {
            preferences.ServerPreferences.StoreUserData(currentConnectionData);
            LoadEnvironment();
        }

        protected async void LoadEnvironment()
        {
            var loadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GamePanelScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (!loadAsync.isDone || !cdk.collaboration.UMI3DCollaborationClientServer.Exists)
            {
                LoadingEnvironment?.Invoke(loadAsync.progress);
                await UMI3DAsyncManager.Yield();
            }

            cdk.UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => LoadedEnvironment?.Invoke());

            cdk.collaboration.UMI3DCollaborationClientServer.Instance.Clear();
            Connect();

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(LauncherPanelScene);
            onlyOneConnection = false;
        }

        /// <summary>
        /// Uses the connection data to connect to te server.
        /// </summary>
        /// <param name="connectionData"></param>
        protected void Connect()
        {
            try
            {
                //succes
                cdk.collaboration.UMI3DCollaborationClientServer.Connect(mediaDto, (s) => ConnectionFail?.Invoke(s));
                ConnectionSucces?.Invoke(mediaDto);
            }
            catch (System.Exception e)
            {
                ConnectionFail?.Invoke(e.Message);
            }
        }

        protected static string FormatUrl(string ip, string port)
        {
            string url = ip + (string.IsNullOrEmpty(port) ? "" : (":" + port));

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "http://" + url;
            return url;
        }

        #endregion

        #region Environment

        [HideInInspector]
        public event System.Action<int, System.Action<bool>> AskForDownloadingLibraries;
        [HideInInspector]
        public event System.Action<common.interaction.FormDto, System.Action<common.interaction.FormAnswerDto>> GetParameterDtos;
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
        public event System.Action<string> ForcedLeave;
        [HideInInspector]
        public event System.Action<float> LoadingLauncher;

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
            ForcedLeave = null;
            AskForDownloadingLibraries = null;
            GetParameterDtos = null;
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

        /// <summary>
        /// Clears the environment and goes back to the launcher.
        /// </summary>
        public void Leave()
        {
            cdk.UMI3DEnvironmentLoader.Clear();
            cdk.UMI3DResourcesManager.Instance.ClearCache();
            cdk.collaboration.UMI3DCollaborationClientServer.Logout();
            //Destroy(cdk.UMI3DClientServer.Instance.gameObject);

            Controller.BaseCursor.SetMovement(this, Controller.BaseCursor.CursorMovement.Free);

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
