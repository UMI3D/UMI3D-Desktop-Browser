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

namespace umi3d.baseBrowser.connection
{
    public class BaseConnectionProcess : inetum.unityUtils.PersistentSingleBehaviour<BaseConnectionProcess>
    {
        protected const string LauncherPanelScene = "Connection";
        protected const string GamePanelScene = "Environment";

        [SerializeField]
        UMI3DClientLogger logger;

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
        public UMI3DCollabLoadingParameters LoadingParameters;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            logger = new UMI3DClientLogger(mainTag: nameof(BaseConnectionProcess), mainContext: Instance, isThreadDisplayed: true);
            logger.Debug($"{nameof(Awake)}", $"Awake");

            currentServer = preferences.ServerPreferences.GetPreviousServerData() ?? new preferences.ServerPreferences.ServerData();
            currentConnectionData = preferences.ServerPreferences.GetPreviousConnectionData() ?? new preferences.ServerPreferences.Data();
            savedServers = preferences.ServerPreferences.GetRegisteredServerData() ?? new List<preferences.ServerPreferences.ServerData>();
            masterServer = new cdk.collaboration.LaucherOnMasterServer();

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
            logger.Debug($"{nameof(Start)}", $"Start");

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

        public bool IsConnectionProcessInProgress = false;
        // Base connection token.
        CancellationTokenSource connectionTokenSource;
        CancellationToken connectionToken;
        // Master server token
        CancellationTokenSource masterServerTokenSource;
        CancellationToken masterServerToken;
        // Media dto token.
        CancellationTokenSource mediaDtoTokenSource;
        CancellationToken mediaDtoToken;
        // Scene coroutine
        Coroutine LoadGameSceneCoroutine;
        readonly object lockObj = new object();

        /// <summary>
        /// Initiates the connection, if a connection is already in process return.
        /// 
        /// <para>
        /// There is two types of connection to an environment in UMI3D for now:
        /// <list type="bullet">
        /// <item>Via a master server.</item>
        /// <item>Via a world controller.</item>
        /// </list>
        /// </para>
        /// </summary>
        public void InitConnect(bool saveInfo = false)
        {
            logger.Debug($"{nameof(InitConnect)}", $"Init connection, save info: {saveInfo}");

            StartCoroutine(UMI3DWorldControllerClient.RequestMediaDto(currentServer.serverUrl, mediaDto => this.mediaDto = mediaDto));

            return;

            #region Multi-Thread

            if (IsConnectionProcessInProgress)
            {
                logger.Debug($"{nameof(InitConnect)}", $"Connection process is already in progress.");
                // Cancel the previous connection token sources.
                connectionTokenSource.Cancel();
                masterServerTokenSource.Cancel();
                mediaDtoTokenSource.Cancel();
            }

            // Start or Restart token for new connection.
            connectionTokenSource = new CancellationTokenSource();
            connectionToken = connectionTokenSource.Token;
            masterServerTokenSource = new CancellationTokenSource();
            masterServerToken = masterServerTokenSource.Token;
            mediaDtoTokenSource = new CancellationTokenSource();
            mediaDtoToken = mediaDtoTokenSource.Token;
            IsConnectionProcessInProgress = true;

            var connectionTask = Task.Factory.StartNew(async () =>
            {
                UnityEngine.Debug.Log($"connection task on thread {Thread.CurrentThread.ManagedThreadId}");

                // Connection to the master server.
                var masterServerTask = Task.Factory.StartNew(async () =>
                {
                    UnityEngine.Debug.Log($"masterServerTask task on thread {Thread.CurrentThread.ManagedThreadId}");

                    masterServer.ConnectToMasterServer(() =>
                    {
                        if (masterServerTokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        masterServer.RequestInfo
                        (
                            (name, icon) =>
                            {
                                if (masterServerTokenSource.IsCancellationRequested)
                                {
                                    return;
                                }

                                mediaDtoTokenSource.Cancel();

                                currentServer.serverName = name;
                                currentServer.serverIcon = icon;
                                preferences.ServerPreferences.StoreUserData(currentServer);
                                if (saveInfo)
                                {
                                    StoreServer();
                                }
                            },
                            () => masterServerTokenSource.Cancel()
                        );

                        DisplaySessions?.Invoke();
                    },
                    currentServer.serverUrl,
                    () => masterServerTokenSource.Cancel());
                },
                masterServerToken).Unwrap();

                // Connection via mediaDto.
                var mediaDtoTask = Task.Factory.StartNew(async () =>
                {
                    logger.Debug($"{nameof(InitConnect)}", $"Start looking for a media dto.");
                    var curentUrl = currentServer.serverUrl + UMI3DNetworkingKeys.media;

                    if (!curentUrl.StartsWith("http://") && !curentUrl.StartsWith("https://"))
                    {
                        curentUrl = "http://" + curentUrl;
                    }

                    url = curentUrl;
                    try
                    {
                        Debug.Assert(UMI3DCollaborationClientServer.Exists, "UMI3DCollaborationClientServer does not exist when trying to connect via media dto.");

                        var _mediaDto = await HttpClient.SendGetMedia
                        (
                            url,
                            e => url == curentUrl && e.count < 3
                        );

                        lock (lockObj)
                        {
                            mediaDto = _mediaDto;
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log($"media dto fail to get. {e}");

                        lock (lockObj)
                        {
                            mediaDto = null;
                        }
                        mediaDtoTokenSource.Cancel();
                        return;
                    }

                    UnityEngine.Debug.Log($"media dto succed to get");

                    masterServerTokenSource.Cancel();

                    lock (lockObj)
                    {
                        currentServer.serverName = mediaDto.name;
                        currentServer.serverIcon = mediaDto?.icon2D?.variants?.FirstOrDefault()?.url;
                        preferences.ServerPreferences.StoreUserData(currentServer);
                        if (saveInfo)
                        {
                            StoreServer();
                        }

                        currentConnectionData.environmentName = mediaDto.name;
                        currentConnectionData.ip = mediaDto.url;
                        currentConnectionData.port = null;

                        preferences.ServerPreferences.StoreUserData(currentConnectionData);
                    }
                },
                mediaDtoToken).Unwrap();

                Task processes = Task.WhenAll(new[] { masterServerTask, mediaDtoTask });
                await processes;

                if (connectionTokenSource.IsCancellationRequested)
                {
                    UnityEngine.Debug.Log($"connection task cancelled on thread {Thread.CurrentThread.ManagedThreadId}");
                    return;
                }
                else
                {
                    UnityEngine.Debug.Log($"connection task finish on thread {Thread.CurrentThread.ManagedThreadId}");
                }

                IsConnectionProcessInProgress = false;
            },
            connectionToken);

            #endregion

            LoadGameSceneCoroutine = StartCoroutine(LoadGameScene());

            //while (onlyOneConnection)
            //{
            //    await UMI3DAsyncManager.Yield();
            //    if (masterServerFound || mediaDtoFound) return;
            //    if (_masterServerFound != null && _mediaDtoFound != null)
            //    {
            //        ConnectionInitializationFailled?.Invoke(currentServer.serverUrl);
            //        onlyOneConnection = false;
            //    }
            //}

        }

        protected void StoreServer()
        {
            if (savedServers.Find((server) => server.serverName == currentServer.serverName) == null)
                savedServers.Add(currentServer);
            preferences.ServerPreferences.StoreRegisteredServerData(savedServers);
        }

        IEnumerator LoadGameScene()
        {
            logger.Debug($"{nameof(LoadGameScene)}", $"Start {nameof(LoadGameScene)}.");

            while (IsConnectionProcessInProgress)
            {
                yield return null;
            }

            logger.Assert(masterServerTokenSource != null, $"{nameof(LoadGameScene)}", "Master server token source null when loading game scene.");
            logger.Assert(mediaDtoTokenSource != null, $"{nameof(LoadGameScene)}", "Media dto token source null when loading game scene.");
            if (masterServerTokenSource.IsCancellationRequested && mediaDtoTokenSource.IsCancellationRequested)
            {
                logger.Assertion($"{nameof(LoadGameScene)}", $"No connection process have been found.");
                yield break;
            }
            else if (masterServerTokenSource.IsCancellationRequested)
            {
                logger.Debug($"{nameof(LoadGameScene)}", $"Connection to a worldController via media dto.");
            }
            else
            {
                logger.Debug($"{nameof(LoadGameScene)}", $"Connection to a master server.");
            }

            ConnectionInitialized?.Invoke(currentServer.serverUrl);

            var loadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GamePanelScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            logger.Debug($"{nameof(LoadGameScene)}", $"Start to load game scene async.");
            while (!loadAsync.isDone || !UMI3DCollaborationClientServer.Exists)
            {
                LoadingEnvironment?.Invoke(loadAsync.progress);
                yield return null;
            }
            logger.Debug($"{nameof(LoadGameScene)}", $"Game scene has finished loaded.");

            logger.Assert(UMI3DEnvironmentLoader.Exists, $"{nameof(LoadGameScene)}", $"UMI3DEnvironmentLoader does not exist.");
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => LoadedEnvironment?.Invoke());

            logger.Assert(UMI3DCollaborationClientServer.Exists, $"{nameof(LoadGameScene)}", $"UMI3DCollaborationClientServer does not exist.");
            UMI3DCollaborationClientServer.Instance.Clear();

            try
            {
                logger.Assert(mediaDto != null, $"{nameof(LoadGameScene)}", "Media dto null when loading game scene.");
                UMI3DCollaborationClientServer.Connect(mediaDto, s => ConnectionFail?.Invoke(s));
                ConnectionSucces?.Invoke(mediaDto);
            }
            catch (System.Exception e)
            {
                ConnectionFail?.Invoke(e.Message);
            }

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(LauncherPanelScene);
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
