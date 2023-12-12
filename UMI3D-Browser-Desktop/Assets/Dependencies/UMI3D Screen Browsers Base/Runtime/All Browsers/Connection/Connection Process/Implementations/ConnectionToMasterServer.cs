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
using BeardedManStudios.Forge.Networking;
using System;
using System.Threading.Tasks;
using umi3d.cdk.collaboration;
using UnityEditor.Experimental.GraphView;
using URL = System.String;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionToMasterServer : IConnectionTo
    {
        const string requestServerInfoKey = "info";
        const string requestSessionInfoKey = "get";
        static debug.UMI3DLogger logger = new debug.UMI3DLogger(mainTag: $"{nameof(ConnectionToMasterServer)}");

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionTo> Connected;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionTo> Disconnected;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IConnectionTo> Canceled;

        LaucherOnMasterServer masterServer;
        IWorldData worldData;
        IConnectionStateData connectionStateData;
        /// <summary>
        /// The Master Server communicates over TCP
        /// </summary>
        TCPMasterClient client;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IConnectionTo.ConnectionToResult Result { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task ConnectionOrDisconnectionTask { get; private set; }

        public ConnectionToMasterServer(LaucherOnMasterServer masterServer, IWorldData worldData, IConnectionStateData connectionStateData)
        { 
            this.masterServer = masterServer;
            this.worldData = worldData;
            this.connectionStateData = connectionStateData;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task Connect(URL url)
        {
            //connectionStateData.Add(new MasterServerStartedConnectionState());
            //masterServer.ConnectToMasterServer
            //(
            //    callback: () =>
            //    {
            //        //if (connectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
            //        //{
            //        //    connectionStateData.Add(new MasterServerStoppedConnectionState());
            //        //    return;
            //        //}

            //        masterServer.RequestInfo
            //        (
            //            UIcallback: (name, icon) =>
            //            {
            //                //if (connectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
            //                //{
            //                //    connectionStateData.Add(new MasterServerStoppedConnectionState());
            //                //    return;
            //                //}

            //                worldData.World = new();
            //                worldData.World.serverName = name;
            //                worldData.World.serverIcon = icon;

            //                //preferences.ServerPreferences.StoreUserData(currentServer);
            //                //if (saveInfo) StoreServer();
            //            },
            //            failed: () =>
            //            {
            //                //connectionStateData.Add(new MasterServerFailedConnectionState());
            //            }
            //        );

            //        //connectionStateData.Add(new MasterServerSessionConnectionState());
            //        //DisplaySessions?.Invoke();
            //    },
            //    worldData.World.serverUrl,
            //    failed: () =>
            //    {
            //        //connectionStateData.Add(new MasterServerFailedConnectionState());
            //    }
            //);

            if (Result == IConnectionTo.ConnectionToResult.Processing || Result == IConnectionTo.ConnectionToResult.Connected)
            {
                logger.Error(
                    $"{nameof(Connect)}",
                    IConnectionTo.ConnectionAlreadyInProgress
                );
            }
            Result = IConnectionTo.ConnectionToResult.Processing;

            string[] ip_port = url.Split(':');
            if (ip_port.Length != 2)
            {
                logger.Error(
                    $"{nameof(Connect)}",
                    $"url: {url} has not the right format. It should be: 000.000.000.000:00000"
                );
                return;
            }

            var ConnectionTask = Task.Factory.StartNew(() =>
            {
                client.Connect(ip_port[0], ushort.Parse(ip_port[1]));
            });

            while (!ConnectionTask.IsCompleted)
            {
                await Task.Yield();
            }

            if (!client.IsConnected)
            {
                Result = IConnectionTo.ConnectionToResult.Disconnected;
                Error = $"Connection process failed.";
                Connected?.Invoke(this);
            }
            else
            {
                Result = IConnectionTo.ConnectionToResult.Connected;
            }



        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task Disconnect()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public URL URLToFormattedURL(URL url)
        {
            string[] ip_port = url.Split(':');
            if (ip_port.Length > 2)
            {
                logger.Error(
                    $"{nameof(URLToFormattedURL)}", 
                    $"url: {url} has not the right format. It should be: 000.000.000.000:00000"
                );
                return null;
            }

            ushort port; 
            if (ip_port.Length == 2)
            {
                if (!ushort.TryParse(ip_port[1], out port))
                {
                    logger.Error(
                    $"{nameof(URLToFormattedURL)}",
                    $"url: {url} has not the right format. The port is not a number."
                );
                    return null;
                }
            }
            else
            {
                // default port.
                port = 15940; 
            }

            return $"{ip_port[0]}:{port}";
        }
    }
}
