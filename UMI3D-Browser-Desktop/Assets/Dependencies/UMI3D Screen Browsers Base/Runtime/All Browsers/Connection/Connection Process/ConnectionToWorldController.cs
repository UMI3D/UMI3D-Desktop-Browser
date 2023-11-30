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
    public class ConnectionToWorldController
    {
        public IWorldData worldData;
        public IConnectionStateData connectionStateData;
        public MediaDTOWebRequest mediaDTOWebRequest;

        public ConnectionToWorldController(IWorldData worldData, IConnectionStateData connectionStateData)
        {
            this.worldData = worldData;
            this.connectionStateData = connectionStateData;
            this.mediaDTOWebRequest = new(connectionData);
        }

        public async Task TryToConnect()
        {
            await mediaDTOWebRequest.RequestMediaDto(worldData.World.serverUrl);

            if (mediaDTOWebRequest.MediaDTO != null)
            {
                UMI3DCollaborationClientServer.Connect(mediaDTOWebRequest.MediaDTO, s =>
                {
                    //ConnectionFail?.Invoke(s);
                });
            }
        }
    }
}
