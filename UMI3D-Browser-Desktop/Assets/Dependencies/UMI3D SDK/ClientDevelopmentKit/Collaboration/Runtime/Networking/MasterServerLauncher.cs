/*
Copyright 2019 - 2021 Inetum

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
using BeardedManStudios.SimpleJSON;
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Used to connect to a Master Server, when a World Controller is not used.
    /// </summary>
    public class MasterServerLauncher
    {
        const string requestInfoKey = "info";

        private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration;
        UMI3DClientLogger logger = new UMI3DClientLogger(mainTag: $"{nameof(MasterServerLauncher)}");

        /// <summary>
        /// The Master Server communicates over TCP
        /// </summary>
        private static TCPMasterClient client = new TCPMasterClient();

        #region Events

        /// <summary>
        /// Event raise when the connection failed.
        /// </summary>
        public event Action connectFailed;

        /// <summary>
        /// Event raise when the connection succeeded.
        /// </summary>
        public event Action connectSucceeded;

        /// <summary>
        /// Event raise when a request info has succeeded.
        /// </summary>
        public event Action<(string serverName, string icon)> requestInfSucceeded;

        #endregion

        Dictionary<string, (NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Text frame, NetWorker sender)> request = new Dictionary<string, (NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Text frame, NetWorker sender)>();

        public MasterServerLauncher()
        {
        }

        /// <summary>
        /// Try to connect to a master server asyncronously.
        /// 
        /// <para>
        /// The connection is established in another thread. The <see cref="connectFailed"/> and <see cref="connectSucceeded"/> events are raised in the main-thread.
        /// </para>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public AsyncOperation ConnectAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var asyncOperation = new AsyncOperation();

            asyncOperation.completed += operation =>
            {
                if (client.IsConnected)
                {
                    connectSucceeded?.Invoke();
                }
                else
                {
                    connectFailed?.Invoke();
                }
            };

            asyncOperation.Start(
                () =>
                {
                    string[] ip_port = url.Split(':');
                    if (ip_port.Length > 2)
                    {
                        logger.Assertion($"{nameof(ConnectAsync)}", $"url: {url} has not the right format. It should be: 000.000.000.000:00000");
                        return;
                    }

                    string ip = ip_port[0];
                    ushort port = 15940; // default port.

                    if (ip_port.Length == 2)
                    {
                        ushort.TryParse(ip_port[1], out port);
                    }

                    client.Connect(ip, port);
                }
            );

            return asyncOperation;
        }

        /// <summary>
        /// Get the requested information about this master server asyncronously.
        /// 
        /// <para>
        /// The request is performed in another thread.
        /// </para>
        /// </summary>
        /// <param name="requestFailed"></param>
        public void RequestInfo(Action requestFailed)
        {
            if (!client.IsConnected)
            {
                return;
            }

            var asyncOperation = new AsyncOperation();

            asyncOperation.completed += asyncOp =>
            {
                try
                {
                    if (!request.TryGetValue(requestInfoKey, out var response))
                    {

                    }

                    // Get the list of hosts to iterate through from the frame payload
                    var data = JSONNode.Parse(response.frame.ToString());
                    if (data["name"] != null)
                    {
                        requestInfSucceeded.Invoke((
                                serverName: data["name"],
                                icon: data["icon"]
                        ));
                    }
                }
                catch (Exception e)
                {
                    UMI3DLogger.LogException(e, scope);
                    client?.Disconnect(true);
                    MasterServerException.LogException(
                        "Trying to get the received information cause an exception.", 
                        inner: e, 
                        MasterServerException.ExceptionTypeEnum.ReceiveEcxeption
                    );
                }
            };

            asyncOperation.Start(
                async () =>
                {
                    // Create the get request with the desired filters
                    var sendData = JSONNode.Parse("{}");
                    sendData.Add(requestInfoKey, aItem: new JSONClass());

                    bool hasReceivedInfo = false;
                    void textMessageReceived(NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Text frame, NetWorker sender)
                    {
                        hasReceivedInfo = true;
                        request.Add(requestInfoKey, (player, frame, sender));
                    };

                    client.textMessageReceived += textMessageReceived;

                    try
                    {
                        client.Send(
                            frame: BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(
                                client.Time.Timestep,
                                message: sendData.ToString(),
                                useMask: true,
                                Receivers.Server,
                                MessageGroupIds.MASTER_SERVER_GET,
                                isStream: true
                            )
                        );

                        float delay = 0;
                        while (!hasReceivedInfo)
                        {
                            await Task.Delay(25);
                            delay += 25;

                            if (delay > 1000)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        client?.Disconnect(true);
                        requestFailed?.Invoke();
                        MasterServerException.LogException(
                        "Trying to send a request for information cause an exception.",
                        inner: e,
                        MasterServerException.ExceptionTypeEnum.SendException
                    );
                    }

                    client.textMessageReceived -= textMessageReceived;
                }
            );
        }

        public void SendDataSession(string sessionId, Action<MasterServerResponse.Server> UIcallback)
        {
            try
            {
                // The overall game id to select from
                string gameId = sessionId;

                // The game type to choose from, if "any" then all types will be returned
                string gameType = "any";

                // The game mode to choose from, if "all" then all game modes will be returned
                string gameMode = "all";

                // Create the get request with the desired filters
                var sendData = JSONNode.Parse("{}");
                var getData = new JSONClass
                {

                    // The id of the game to get
                    { "id", gameId },
                    { "type", gameType },
                    { "mode", gameMode }
                };

                sendData.Add("get", getData);

                // Send the request to the server
                //client.binaryMessageReceived += (x,y,z) => { UMI3DLogger.Log("bin massage received"); };
                client.textMessageReceived += (player, frame, sender) => { ReceiveMasterDatas(player, frame, sender, UIcallback); };
                client.Send(BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));

            }
            catch (Exception e)
            {
                UMI3DLogger.LogException(e, scope);
                // If anything fails, then this client needs to be disconnected
                client.Disconnect(true);
                client = null;

            }
        }

        

        private void ReceiveMasterDatas(NetworkingPlayer player, BeardedManStudios.Forge.Networking.Frame.Text frame, NetWorker sender, Action<MasterServerResponse.Server> UICallback)
        {
            try
            {
                // Get the list of hosts to iterate through from the frame payload
                var data = JSONNode.Parse(frame.ToString());
                if (data["hosts"] != null)
                {
                    // Create a C# object for the response from the master server
                    var response = new MasterServerResponse(data["hosts"].AsArray);

                    if (response != null && response.serverResponse.Count > 0)
                        // Go through all of the available hosts and add them to the server browser
                        foreach (MasterServerResponse.Server server in response.serverResponse)
                            // Update UI or something with the above data
                            UICallback.Invoke(server);
                }
            }
            catch (Exception e)
            {
                UMI3DLogger.LogException(e, scope);
                if (client != null)
                {
                    client.Disconnect(true);
                    client = null;
                }
            }
        }

        // disconnect TCPMasterClient
        ~MasterServerLauncher()
        {
            if (client != null)
            {
                // If anything fails, then this client needs to be disconnected
                client.Disconnect(true);
                client = null;
            }
        }
    }

    /// <summary>
    /// An exception class to deal with <see cref="MasterServerLauncher"/> issues.
    /// </summary>
    [Serializable]
    public class MasterServerException : Exception
    {
        static UMI3DClientLogger logger = new UMI3DClientLogger(mainTag: $"{nameof(MasterServerException)}");

        public enum ExceptionTypeEnum
        {
            Unknown,
            SendException,
            ReceiveEcxeption
        }

        public ExceptionTypeEnum exceptionType;

        public MasterServerException(string message, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}") 
        {
            this.exceptionType = exceptionType;
        }
        public MasterServerException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}", inner) 
        {
            this.exceptionType = exceptionType;
        }

        public static void LogException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown)
        {
            try
            {
                throw new MasterServerException(message, inner, exceptionType);
            }
            catch (Exception e)
            {
                logger.Exception(null, e);
            }
        }
    }
}