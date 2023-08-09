/*
Copyright 2019 - 2023 Inetum

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
using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// 
    /// </summary>
    public class UMI3DClientServerConnection
    {
        enum ConnectionState
        {
            Iddle,
            Processing,
            Succes,
            Fail
        }

        [SerializeField]
        UMI3DClientLogger logger;
        UMI3DLogReport initConnectionReport;

        [Header("Scene names")]
        [SerializeField]
        string launcherScene = "Connection";
        [SerializeField]
        string environmentScene = "Environment";

        #region Connection events

        // master server found
        public event Action<UMI3DClientServerConnection> masterServerFound;

        // media dto found

        /// <summary>
        /// Event raise when the environment scene start loading.
        /// </summary>
        public event Action<UMI3DClientServerConnection> environmentSceneStartLoading;
        public event Action<float> environmentSceneLoadingProgress;

        public event Action<UMI3DClientServerConnection> connectionSucceeded;
        public event Action<UMI3DClientServerConnection> connectionFailed;

        #endregion

        #region Connection data

        public UMI3DConnectionData connectionData { get; protected set; }


        #endregion


        #region Connection life cycle data

        MasterServerLauncher masterServer;
        UMI3DWorldControllerClient worldController;

        // Connection token.
        CancellationTokenSource connectionTokenSource;

        ConnectionState mediaDtoState;
        // Media dto token.
        CancellationTokenSource mediaDtoTokenSource;

        ConnectionState masterServerState;
        // Master server token
        CancellationTokenSource masterServerTokenSource;

        #endregion


        public UMI3DClientServerConnection(UnityEngine.Object context = null)
        {
            logger = new UMI3DClientLogger(mainTag: nameof(UMI3DClientServerConnection), mainContext: context, isThreadDisplayed: true);
            initConnectionReport = logger.GetReporter("InitConnection");

            masterServer = new MasterServerLauncher();
            worldController = new UMI3DWorldControllerClient(null);

            masterServer.requestInfSucceded += info =>
            {
                if (connectionTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (connectionData != null)
                {
                    connectionData.name = info.serverName;
                    connectionData.icon = info.icon;
                }

                preferences.ServerPreferences.StoreUserData(currentServer);
                StoreServer();
            };
        }

        public void Connect(UMI3DConnectionData connectionData)
        {
            this.connectionData = connectionData;

            InitConnect(connectionData.url);
        }

        public void Connect(string url)
        {
            this.connectionData = new UMI3DConnectionData()
            {
                url = url
            };

            Connect(connectionData);
        }

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
        public void InitConnect(string url)
        {
            logger.DebugTab(
                tabName: "InitConnection",
                new[]
                {
                    new UMI3DLogCell(
                        header: "URL",
                        message: url,
                        stringFormatSize: 40
                    ),
                    new UMI3DLogCell(
                        header: "SaveInfo",
                        message: saveInfo,
                        stringFormatSize: 10
                    ),
                },
                report: initConnectionReport
            );

            // Generic connection data.
            connectionTokenSource?.Cancel();

            connectionTokenSource = new CancellationTokenSource();
            var connectionToken = connectionTokenSource.Token;

            // world controller data and request.
            mediaDtoTokenSource?.Cancel();

            mediaDtoState = ConnectionState.Processing;
            mediaDtoTokenSource = new CancellationTokenSource();
            CancellationToken mediaDtoToken = mediaDtoTokenSource.Token;

            CoroutineManager.Instance.AttachCoroutine(
                UMI3DWorldControllerClient.RequestMediaDto(
                    url,
                    requestSucced: mediaDto =>
                    {
                        mediaDtoState = ConnectionState.Succes;
                        masterServerTokenSource.Cancel();
                    },
                    requestFailed: tryCount =>
                    {
                        if (tryCount >= 3)
                        {
                            mediaDtoTokenSource.Cancel();
                            mediaDtoState = ConnectionState.Fail;
                        }
                    },
                    shouldCleanAbort: () =>
                    {
                        return mediaDtoToken.IsCancellationRequested || connectionToken.IsCancellationRequested;
                    }
            ));

            // Master server data and request.
            masterServerTokenSource?.Cancel();

            masterServerState = ConnectionState.Processing;
            masterServerTokenSource = new CancellationTokenSource();
            CancellationToken masterServerToken = masterServerTokenSource.Token;

            masterServerState = ConnectionState.Processing;
            masterServer.connectFailed += () =>
            {
                masterServerState = ConnectionState.Fail;
                masterServerTokenSource.Cancel();
            };
            masterServer.connectSucceded += () =>
            {
                masterServerState = ConnectionState.Succes;
                mediaDtoTokenSource.Cancel();

                masterServer.RequestInfo(
                    requestFailed: () =>
                    {
                        masterServerTokenSource.Cancel();
                    }
                );

                masterServerFound?.Invoke(this);
            };
            masterServer.ConnectAsync(url);

            var loadingReport = logger.GetReporter("loading");
            loadingReport.Clear();
            CoroutineManager.Instance.AttachCoroutine(LoadGameScene(
                isConnectionInProgress: () =>
                {
                    bool result = mediaDtoState != ConnectionState.Succes;

                    if (!result)
                    {
                        mediaDtoState = ConnectionState.Iddle;
                        masterServerState = ConnectionState.Iddle;
                        ConnectionInitialized?.Invoke(url);
                    }

                    return result;
                },

                shouldStopLoading: () =>
                {
                    return connectionToken.IsCancellationRequested || mediaDtoState == ConnectionState.Fail;
                },
                report: loadingReport
            ));
        }

        IEnumerator LoadGameScene(
            Func<bool> isConnectionInProgress, Func<bool> shouldStopLoading,
            UMI3DLogReport report = null
        )
        {
            logger.Debug($"{nameof(LoadGameScene)}", $"Start {nameof(LoadGameScene)}: wait until media dto or master server is found.", report: report);
            while (isConnectionInProgress?.Invoke() ?? false)
            {
                if (shouldStopLoading?.Invoke() ?? false)
                {
                    logger.Debug($"{nameof(LoadGameScene)}", $"Stop loading before it started.");
                    ConnectionInitializationFailled?.Invoke(currentServer.serverUrl);
                    yield break;
                }

                yield return null;
            }

            environmentSceneStartLoading?.Invoke(this);
            var progressReport = logger.GetReporter("progress");
            UMI3DClientSceneManager.LoadSceneAsync(
                environmentScene,

                shouldStopLoading,

                loadingProgress: environmentSceneLoadingProgress != null
                ? environmentSceneLoadingProgress
                : progress =>
                {
                    logger.Debug($"{nameof(LoadGameScene)}", $"Loading progress: {progress}", report: progressReport);
                },

                loadingSucced: () =>
                {
                    UMI3DClientSceneManager.UnloadSceneAsync(
                        launcherScene,
                        unloadingProgress: null,
                        unloadingSucced: null
                    );

                    //logger.Assert(UMI3DEnvironmentLoader.Exists, $"{nameof(LoadGameScene)}", $"UMI3DEnvironmentLoader does not exist.");
                    //UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => LoadedEnvironment?.Invoke());

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
                },

                loadingFail: () =>
                {
                    report.Report();
                    progressReport.Report();
                },
                report: report
            );

        }
    } 
}
