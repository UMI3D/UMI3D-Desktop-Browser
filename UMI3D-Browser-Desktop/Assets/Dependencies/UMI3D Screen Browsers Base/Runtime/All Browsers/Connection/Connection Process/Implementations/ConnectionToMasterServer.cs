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
    public class ConnectionToMasterServer /*: IConnectionTo*/
    {
        LaucherOnMasterServer masterServer;
        IWorldData worldData;
        IConnectionStateData connectionStateData;

        public ConnectionToMasterServer(LaucherOnMasterServer masterServer, IWorldData worldData, IConnectionStateData connectionStateData)
        { 
            this.masterServer = masterServer;
            this.worldData = worldData;
            this.connectionStateData = connectionStateData;
        }

        public Task TryToConnect()
        {
            //connectionStateData.Add(new MasterServerStartedConnectionState());
            masterServer.ConnectToMasterServer
            (
                callback: () =>
                {
                    //if (connectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
                    //{
                    //    connectionStateData.Add(new MasterServerStoppedConnectionState());
                    //    return;
                    //}

                    masterServer.RequestInfo
                    (
                        UIcallback: (name, icon) =>
                        {
                            //if (connectionStateData.ContainsStateByType<MediaDTOFoundConnectionState>())
                            //{
                            //    connectionStateData.Add(new MasterServerStoppedConnectionState());
                            //    return;
                            //}

                            worldData.World = new();
                            worldData.World.serverName = name;
                            worldData.World.serverIcon = icon;

                            //preferences.ServerPreferences.StoreUserData(currentServer);
                            //if (saveInfo) StoreServer();
                        },
                        failed: () =>
                        {
                            //connectionStateData.Add(new MasterServerFailedConnectionState());
                        }
                    );

                    //connectionStateData.Add(new MasterServerSessionConnectionState());
                    //DisplaySessions?.Invoke();
                },
                worldData.World.serverUrl,
                failed: () =>
                {
                    //connectionStateData.Add(new MasterServerFailedConnectionState());
                }
            );
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            throw new System.NotImplementedException();
        }
    }
}
