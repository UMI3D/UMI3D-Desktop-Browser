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
using System.Threading.Tasks;
using umi3d.cdk.collaboration;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionToMasterServer : IConnectionTo
    {
        LaucherOnMasterServer masterServer;
        public IWorldData WorldData { get; set; }
        public IConnectionStateData ConnectionStateData { get; set; }

        public ConnectionToMasterServer(LaucherOnMasterServer masterServer, IWorldData worldData, IConnectionStateData connectionStateData)
        { 
            this.masterServer = masterServer;
            this.WorldData = worldData;
            this.ConnectionStateData = connectionStateData;
        }

        public Task TryToConnect()
        {
            ConnectionStateData.Add(new MasterServerStartedConnectionState());
            masterServer.ConnectToMasterServer
            (
                callback: () =>
                {
                    if (ConnectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
                    {
                        ConnectionStateData.Add(new MasterServerStoppedConnectionState());
                        return;
                    }

                    masterServer.RequestInfo
                    (
                        UIcallback: (name, icon) =>
                        {
                            if (ConnectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
                            {
                                ConnectionStateData.Add(new MasterServerStoppedConnectionState());
                                return;
                            }

                            WorldData.World = new();
                            WorldData.World.serverName = name;
                            WorldData.World.serverIcon = icon;

                            //preferences.ServerPreferences.StoreUserData(currentServer);
                            //if (saveInfo) StoreServer();
                        },
                        failed: () =>
                        {
                            ConnectionStateData.Add(new MasterServerFailedConnectionState());
                        }
                    );

                    ConnectionStateData.Add(new MasterServerSessionConnectionState());
                    //DisplaySessions?.Invoke();
                },
                WorldData.World.serverUrl,
                failed: () =>
                {
                    ConnectionStateData.Add(new MasterServerFailedConnectionState());
                }
            );
            return Task.CompletedTask;
        }
    }
}
