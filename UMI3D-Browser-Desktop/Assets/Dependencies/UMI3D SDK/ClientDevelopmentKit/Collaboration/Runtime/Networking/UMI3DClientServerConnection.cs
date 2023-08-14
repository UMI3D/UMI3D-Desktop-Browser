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
using System.Threading;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Class responsible to connect the client to a server.
    /// 
    /// <para>
    /// When the connection succeed the active scene should switch from the <see cref="launcherScene"/> to the <see cref="environmentScene"/>.
    /// </para>
    /// </summary>
    public class UMI3DClientServerConnection
    {
        #region Public

        #region Connection events

        /// <summary>
        /// Event raise when a master server has been found.
        /// </summary>
        public static event Action<UMI3DClientServerConnection> masterServerFound;
        /// <summary>
        /// Event raise when a media dto has been found.
        /// </summary>
        public static event Action<UMI3DClientServerConnection> mediaDtoFound;

        /// <summary>
        /// Event raise when the environment scene start loading.
        /// </summary>
        public static event Action<UMI3DClientServerConnection> environmentSceneStartLoading;
        /// <summary>
        /// Event raise when the loading of the environment scene progresses.
        /// </summary>
        public static event Action<float> environmentSceneLoadingProgress;

        public static event Action<UMI3DClientServerConnection> connectionSucceeded;
        public static event Action<UMI3DClientServerConnection> connectionFailed;

        /// <summary>
        /// Event raise when the connection process has been aborted.
        /// </summary>
        public static event Action<UMI3DClientServerConnection> connectionAborted;

        #endregion

        [Header("Scene names")]
        /// <summary>
        /// The launcher scene name.
        /// </summary>
        public string launcherScene = "LauncherScene";
        /// <summary>
        /// The environment scene name.
        /// </summary>
        public string environmentScene = "EnvironmentScene";

        /// <summary>
        /// The <see cref="UMI3DConnectionData"/> of the current succeeded connection.
        /// </summary>
        public UMI3DConnectionData currentConnection
        {
            get
            {
                return connection;
            }
        }
        /// <summary>
        /// The <see cref="MediaDto"/> of the current succeeded connection.
        /// </summary>
        public MediaDto currentMediaDto
        {
            get
            {
                return mediaDto;
            }
        }

        /// <summary>
        /// Create a <see cref="UMI3DClientServerConnection"/> with a context.
        /// </summary>
        /// <param name="context"></param>
        public UMI3DClientServerConnection(UnityEngine.Object context = null)
        {
            logger = new UMI3DClientLogger(mainTag: nameof(UMI3DClientServerConnection), mainContext: context, isThreadDisplayed: true);
            ConnectionReport = logger.GetReporter("InitConnection");
        }

        /// <summary>
        /// Connect with a <paramref name="url"/> and a <paramref name="isFavorite"/> connection parameters.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isFavorite"></param>
        /// <exception cref="ClientServerConnectionException"></exception>
        public void Connect(string url, bool isFavorite)
        {
            this.connect(url, isFavorite);
        }

        /// <summary>
        /// Connect with stored <paramref name="connectionData"/>.
        /// </summary>
        /// <param name="connectionData"></param>
        /// <exception cref="ClientServerConnectionException"></exception>
        public void Connect(UMI3DConnectionData connectionData)
        {
            this.connect(connectionData);
        }

        #endregion


        #region Private or Protected

        /// <summary>
        /// The state of the connection process.
        /// </summary>
        enum ConnectionState
        {
            /// <summary>
            /// No connection is currently in progress.
            /// </summary>
            Iddle,
            /// <summary>
            /// A connection is currently in progress.
            /// </summary>
            Processing,
            /// <summary>
            /// A connection has been established.
            /// </summary>
            Succes,
            /// <summary>
            /// A connection has failed.
            /// </summary>
            Fail
        }

        [SerializeField]
        UMI3DClientLogger logger;
        UMI3DLogReport ConnectionReport;

        UMI3DConnectionData connection;
        MediaDto mediaDto;

        #region Connection life cycle data

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

        void connect(string url, bool isFavorite)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ClientServerConnectionException(
                    $"URL is null when trying to connect.",
                ClientServerConnectionException.ExceptionTypeEnum.URLNullOrEmptyException
                );
            }

            UMI3DConnectionData connection = UMI3DConnectionDataCollection.FindConnection(url);

            if (connection == null)
            {
                connection = new UMI3DConnectionData()
                {
                    url = url,
                    isFavorite = isFavorite
                };

                UMI3DConnectionDataCollection.Add(connection);
            }
            else
            {
                connection.isFavorite = isFavorite;
                UMI3DConnectionDataCollection.Update(connection);
            }

            InitConnect(connection);
        }

        void connect(UMI3DConnectionData connectionData)
        {
            if (connectionData == null)
            {
                throw new ClientServerConnectionException(
                    $"Connection data is null when trying to connect.",
                ClientServerConnectionException.ExceptionTypeEnum.ConnectionNullException
                );
            }

            InitConnect(connectionData);
        }

        void InitConnect(UMI3DConnectionData connectionData)
        {
            logger.DebugTab(
                tabName: $"{nameof(InitConnect)}",
                new[]
                {
                    new UMI3DLogCell(
                        header: "URL",
                        message: connectionData.url,
                        stringFormatSize: 40
                    ),
                    new UMI3DLogCell(
                        header: "Name",
                        message: connectionData.name,
                        stringFormatSize: 20
                    ),
                    new UMI3DLogCell(
                        header: "Is Favorite",
                        message: connectionData.isFavorite,
                        stringFormatSize: 13
                    ),
                },
                report: ConnectionReport
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
                    connectionData.url,
                    requestSucceeded: mediaDto =>
                    {
                        mediaDtoState = ConnectionState.Succes;
                        masterServerTokenSource.Cancel();
                        connectionData.name = mediaDto.name;

                        connection = connectionData;
                        UMI3DConnectionDataCollection.Update(connectionData);
                        UMI3DConnectionDataCollection.Save();

                        mediaDtoFound?.Invoke(this);
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
            MasterServerLauncher masterServer = new MasterServerLauncher();
            masterServerTokenSource = new CancellationTokenSource();
            CancellationToken masterServerToken = masterServerTokenSource.Token;

            masterServerState = ConnectionState.Processing;
            masterServer.connectFailed += () =>
            {
                masterServerState = ConnectionState.Fail;
                masterServerTokenSource.Cancel();
            };
            masterServer.connectSucceeded += () =>
            {
                masterServerState = ConnectionState.Succes;
                mediaDtoTokenSource.Cancel();

                masterServer.requestInfSucceeded += info =>
                {
                    if (connectionTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    connectionData.name = info.serverName;
                    connectionData.icon = info.icon;
                    connectionData.lastConnection = DateTime.Now;
                    connectionData.numberOfConnection = connectionData.numberOfConnection + 1;

                    connection = connectionData;
                    UMI3DConnectionDataCollection.Update(connectionData);
                    UMI3DConnectionDataCollection.Save();

                    masterServerFound?.Invoke(this);
                };

                masterServer.RequestInfo(
                    requestFailed: () =>
                    {
                        masterServerTokenSource.Cancel();
                    }
                );
            };
            masterServer.ConnectAsync(connectionData.url);

            var loadingReport = logger.GetReporter("loading");
            loadingReport.Clear();
            CoroutineManager.Instance.AttachCoroutine(LoadEnvironmentScene(
                isConnectionInProgress: () =>
                {
                    bool result = mediaDtoState != ConnectionState.Succes;

                    if (!result)
                    {
                        mediaDtoState = ConnectionState.Iddle;
                        masterServerState = ConnectionState.Iddle;
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

        IEnumerator LoadEnvironmentScene(
            Func<bool> isConnectionInProgress, Func<bool> shouldStopLoading,
            UMI3DLogReport report = null
        )
        {
            logger.Debug($"{nameof(LoadEnvironmentScene)}", $"Start {nameof(LoadEnvironmentScene)}: wait until media dto or master server is found.", report: report);
            while (isConnectionInProgress?.Invoke() ?? false)
            {
                if (shouldStopLoading?.Invoke() ?? false)
                {
                    logger.Debug($"{nameof(LoadEnvironmentScene)}", $"Stop loading before it started.");
                    connectionAborted?.Invoke(this);
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
                    logger.Debug($"{nameof(LoadEnvironmentScene)}", $"Loading progress: {progress}", report: progressReport);
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

                    logger.Assert(UMI3DCollaborationClientServer.Exists, $"{nameof(LoadEnvironmentScene)}", $"UMI3DCollaborationClientServer does not exist.");
                    UMI3DCollaborationClientServer.Instance.Clear();

                    try
                    {
                        logger.Assert(currentMediaDto != null, $"{nameof(LoadEnvironmentScene)}", "Media dto null when loading environment scene.");
                        UMI3DCollaborationClientServer.Connect(currentMediaDto, s =>
                        {
                            logger.Error($"{nameof(InitConnect)}", $"{s}");
                            connectionFailed?.Invoke(this);
                        });
                    }
                    catch (Exception e)
                    {
                        logger.Exception($"{nameof(InitConnect)}", e);
                        connectionFailed?.Invoke(this);
                        return;
                    }

                    connectionSucceeded?.Invoke(this);
                },

                loadingFail: () =>
                {
                    report.Report();
                    progressReport.Report();
                },
                report: report
            );

        }

        #endregion
    }


    /// <summary>
    /// An exception class to deal with <see cref="UMI3DClientServerConnection"/> issues.
    /// </summary>
    [Serializable]
    public class ClientServerConnectionException : Exception
    {
        static UMI3DClientLogger logger = new UMI3DClientLogger(mainTag: $"{nameof(ClientServerConnectionException)}");

        public enum ExceptionTypeEnum
        {
            Unknown,
            ConnectionNullException,
            URLNullOrEmptyException
        }

        public ExceptionTypeEnum exceptionType;

        public ClientServerConnectionException(string message, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}")
        {
            this.exceptionType = exceptionType;
        }
        public ClientServerConnectionException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}", inner)
        {
            this.exceptionType = exceptionType;
        }

        public static void LogException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown)
        {
            try
            {
                throw new ClientServerConnectionException(message, inner, exceptionType);
            }
            catch (Exception e)
            {
                logger.Exception(null, e);
            }
        }
    }
}
