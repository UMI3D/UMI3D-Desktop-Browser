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
using umi3d.baseBrowser.connection;
using umi3d.browserRuntime.connection;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace umi3d.browserRuntime.managers
{
    public class ConnectionManager : MonoBehaviour
    {
        public SerializedAddressableT<UMI3DCollabLoadingParameters> loadingParametersRef;
        public SerializedAddressableT<BaseClientIdentifier> identifierRef;

        public WorldData worldData;
        public AssetFormat assetFormat;
        public ConnectionEvents connectionEvents;

        [HideInInspector]
        public LaucherOnMasterServer masterServer;

        private void Awake()
        {
            loadingParametersRef.LoadAssetAsync().Completed += LoadingParametersLoaded;
            identifierRef.LoadAssetAsync().Completed += IdentifierLoaded;
            
            worldData = new();
            
            masterServer = new();
        }

        private void Start()
        {
            StartCoroutine(connectionEvents.SetManagerEvent());
        }

        private void LoadingParametersLoaded(AsyncOperationHandle<UMI3DCollabLoadingParameters> handler)
        {
            if (handler.IsDone && handler.Status == AsyncOperationStatus.Succeeded)
            {
                assetFormat = new(handler.Result);
            }
        }

        private void IdentifierLoaded(AsyncOperationHandle<BaseClientIdentifier> handler)
        {
            if (handler.IsDone && handler.Status == AsyncOperationStatus.Succeeded)
            {
                connectionEvents = new(handler.Result);
            }
        }
    }
}
