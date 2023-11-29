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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.connection
{
    public class ConnectionToMasterServer
    {
        LaucherOnMasterServer masterServer;
        IWorldData worldData;
        IConnectionData connectionData;

        public ConnectionToMasterServer(IWorldData worldData, IConnectionData connectionData, LaucherOnMasterServer masterServer) 
        { 

        }

        public void TryToConnect()
        {
            masterServer.ConnectToMasterServer
            (
                () =>
                {
                    if (connectionData.ContainsState(typeof(MediaDTOConnectionState))) return;

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
        }
    }
}
