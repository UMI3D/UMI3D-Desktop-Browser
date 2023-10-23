﻿/*
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

using System;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.collaboration.dto.networking;
using umi3d.common.collaboration.dto.signaling;
using umi3d.common.interaction;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Handles the connection to a World Controller.
    /// </summary>
    /// Creates the <see cref="UMI3DEnvironmentClient"/>.
    public class UMI3DWorldControllerClient
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;

        private readonly MediaDto media;
        public string name => media?.name;

        private readonly GateDto gate;
        private string globalToken;
        private UMI3DEnvironmentClient environment;
        private PrivateIdentityDto privateIdentity;

        private readonly ThreadDeserializer deserializer = new ThreadDeserializer();

        /// <summary>
        /// Called to create a new Public Identity for this client.
        /// </summary>
        public PublicIdentityDto PublicIdentity => new PublicIdentityDto()
        {
            userId = privateIdentity.userId,
            login = privateIdentity.login,
            displayName = privateIdentity.displayName

        };

        /// <summary>
        /// Called to create a new Identity for this client.
        /// </summary>
        public IdentityDto Identity => new IdentityDto()
        {
            userId = privateIdentity.userId,
            login = privateIdentity.login,
            displayName = privateIdentity.displayName,
            guid = privateIdentity.guid,
            headerToken = privateIdentity.headerToken,
            localToken = privateIdentity.localToken,
            key = privateIdentity.key
        };

        private readonly bool isConnecting, isConnected;
        public bool IsConnected()
        {
            return isConnected;
        }

        public UMI3DWorldControllerClient(MediaDto media)
        {
            this.media = media;
            isConnecting = false;
            isConnected = false;
            privateIdentity = null;
        }

        public UMI3DWorldControllerClient(MediaDto media, GateDto gate) : this(media)
        {
            this.gate = gate;
        }

        public UMI3DWorldControllerClient(RedirectionDto redirection) : this(redirection.media, redirection.gate)
        {
        }

        public UMI3DWorldControllerClient(RedirectionDto redirection, string globalToken) : this(redirection)
        {
            this.globalToken = globalToken;
        }

        public ulong GetUserID() { return environment?.GetUserID() ?? 0; }

        public async Task<bool> Connect(bool downloadLibraryOnly = false)
        {
            if (!isConnected && !isConnecting)
                return await Connect(new ConnectionDto()
                {
                    globalToken = this.globalToken,
                    gate = this.gate,
                    libraryPreloading = downloadLibraryOnly
                });
            return false;
        }

        private async Task<bool> Connect(ConnectionDto dto)
        {
            if (UMI3DCollaborationClientServer.Exists && !string.IsNullOrEmpty(media.url))
            {
                UMI3DDto answerDto = await HttpClient.Connect(dto, media.url);
                if (answerDto is PrivateIdentityDto identity)
                {
                    Connected(identity);
                    return true;
                }
                else if (answerDto is ConnectionFormDto form)
                {
                    FormAnswerDto answer = await GetFormAnswer(form);
                    var _answer = new FormConnectionAnswerDto()
                    {
                        formAnswerDto = answer,
                        metadata = form.metadata,
                        globalToken = form.globalToken,
                        gate = dto.gate,
                        libraryPreloading = dto.libraryPreloading
                    };
                    return await Connect(_answer);
                }
            }
            return false;
        }

        private void Connected(PrivateIdentityDto identity)
        {
            globalToken = identity.globalToken;
            privateIdentity = identity;
        }

        private async Task<FormAnswerDto> GetFormAnswer(ConnectionFormDto form)
        {
            return await UMI3DCollaborationClientServer.Instance.Identifier.GetParameterDtos(form);
        }

        public UMI3DWorldControllerClient Redirection(RedirectionDto redirection)
        {
            if (media.url == redirection.media.url)
                return new UMI3DWorldControllerClient(redirection, globalToken);
            else
                return new UMI3DWorldControllerClient(redirection);
        }

        public async Task DownloadWorldLibraries()
        {
            MultiProgress libraryProgress = new("Download libraries");
            bool librariesUpdated = false;

            LibrariesDto LibrariesDto = await HttpClient.SendPostWorldLibraries(media.url, privateIdentity.connectionDto);
            bool shouldDownloadLibraries = await UMI3DCollaborationClientServer.Instance.Identifier.ShouldDownloadLibraries(UMI3DResourcesManager.LibrariesToDownload(LibrariesDto));

            if (!shouldDownloadLibraries) return;

            UMI3DCollaborationClientServer.Instance.OnDownloadingLibraries?.Invoke();
            libraryProgress.OnCompleteUpdated += e =>UMI3DCollaborationClientServer.Instance.OnDownloadingLibrariesUpdateValue?.Invoke(libraryProgress.completedPercent);
            libraryProgress.OnStatusUpdated += UMI3DCollaborationClientServer.Instance.OnDownloadingLibrariesUpdateMessage;

            libraryProgress.SetStatus("Downloading Libraries");
            try
            {
                await UMI3DResourcesManager.DownloadLibraries(LibrariesDto, name, libraryProgress);
                librariesUpdated = true;
            }
            catch (Exception e)
            {
                UMI3DLogger.LogException(e, scope);
            }

            while (!librariesUpdated)
                await UMI3DAsyncManager.Yield();
        }

        public async Task<UMI3DEnvironmentClient> ConnectToEnvironment(MultiProgress progress)
        {
            if (environment != null)
                await environment.Logout(false);

            environment = new UMI3DEnvironmentClient(privateIdentity.connectionDto, this, progress);
            if (environment.Connect())
                return environment;
            else
                return null;
        }

        /// <summary>
        /// Logout from the World Controller server.
        /// </summary>
        public void Logout()
        {

        }

        /// <summary>
        /// Logout from the World Controller server and clear infos.
        /// </summary>
        public void Clear()
        {
            Logout();
        }
    }
}