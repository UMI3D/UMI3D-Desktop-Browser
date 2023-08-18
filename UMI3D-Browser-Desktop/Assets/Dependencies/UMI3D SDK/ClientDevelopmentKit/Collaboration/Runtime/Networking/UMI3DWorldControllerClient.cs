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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Security.Policy;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.collaboration.dto.networking;
using umi3d.common.collaboration.dto.signaling;
using umi3d.common.interaction;
using umi3d.debug;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Handles the connection to a World Controller.
    /// </summary>
    /// Creates the <see cref="UMI3DEnvironmentClient"/>.
    public class UMI3DWorldControllerClient
    {
        static debug.UMI3DLogger logger = new debug.UMI3DLogger(mainTag: $"{nameof(UMI3DWorldControllerClient)}");

        private readonly MediaDto media;
        public string name => media?.name;

        private readonly GateDto gate;
        private string globalToken;
        private UMI3DEnvironmentClient environment;
        private PrivateIdentityDto privateIdentity;

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

        public UMI3DWorldControllerClient(MediaDto media, GateDto gate = null, string globalToken = null)
        {
            this.media = media;
            this.gate = gate;
            this.globalToken = globalToken;

            isConnecting = false;
            isConnected = false;
            privateIdentity = null;
        }

        static string mediaDtoURL;

        public static IEnumerator RequestMediaDto(
            string RawURL, 
            Action<MediaDto> requestSucceeded, Action<int> requestFailed, Func<bool> shouldCleanAbort, 
            int tryCount = 0
        )
        {
            if (shouldCleanAbort?.Invoke() ?? false)
            {
                logger.Debug($"{nameof(RequestMediaDto)}", $"Caller requests to abort the connection with MediaDto in a clean way.");
                yield break;
            }

            string curentUrl = RawURL;
            
            if (!curentUrl.EndsWith(UMI3DNetworkingKeys.media))
            {
                curentUrl += UMI3DNetworkingKeys.media;
            }

            if (!curentUrl.StartsWith("http://") && !curentUrl.StartsWith("https://"))
            {
                curentUrl = "http://" + curentUrl;
            }

            mediaDtoURL = curentUrl;

            var tabReporter = logger.GetReporter("RequestMediaDTOTab");
            var assertReporter = logger.GetReporter("RequestMediaDTOAssert");

            yield return UMI3DNetworking.Get_WR(
                (null, null),
                mediaDtoURL,
                shouldCleanAbort,
                op =>
                {
                    var uwr = op.webRequest;

                    if (uwr?.downloadHandler.data == null)
                    {
                        logger.DebugAssertion($"{nameof(RequestMediaDto)}", $"downloadHandler.data == null.");
                        return;
                    }

                    string json = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
                    requestSucceeded?.Invoke(UMI3DDtoSerializer.FromJson<MediaDto>(json, Newtonsoft.Json.TypeNameHandling.None));
                    logger.Default($"{nameof(RequestMediaDto)}", $"Request at: {RawURL} is a success.");
                    tabReporter.Clear();
                    assertReporter.Clear();
                },
                op =>
                {
                    if (shouldCleanAbort?.Invoke() ?? false)
                    {
                        logger.Debug($"{nameof(RequestMediaDto)}", $"Caller requests to abort the connection with MediaDto in a clean way.");
                        return;
                    }

                    logger.Assertion(
                            $"{nameof(RequestMediaDto)}",
                            $"MediaDto failed:   " +
                            $"{op.webRequest.result}".FormatString(19) +
                            "   " +
                            $"{mediaDtoURL}".FormatString(40) +
                            "   " +
                            $"{tryCount}" +
                            $"\n{op.webRequest.error}",
                            report: assertReporter
                        );

                    switch (op.webRequest.result)
                    {
                        case UnityEngine.Networking.UnityWebRequest.Result.InProgress:
                        case UnityEngine.Networking.UnityWebRequest.Result.Success:
                            break;
                        case UnityEngine.Networking.UnityWebRequest.Result.ConnectionError:
                            break;
                        case UnityEngine.Networking.UnityWebRequest.Result.ProtocolError:
                            break;
                        case UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError:
                            logger.Error($"{nameof(RequestMediaDto)}", $"{nameof(UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError)}:   {op.webRequest.url.FormatString(40)}   \n{op.webRequest.error}.");
                            break;
                        default:
                            break;
                    }

                    if (tryCount < 3)
                    {
                        CoroutineManager.Instance.AttachCoroutine(RequestMediaDto(RawURL, requestSucceeded, requestFailed, shouldCleanAbort, tryCount + 1));
                    }
                    else
                    {
                        logger.Error($"{nameof(RequestMediaDto)}", $"MediaDto failed more than 3 times. Connection has been aborted.");
                        tabReporter.Report();
                        assertReporter.Report();
                    }

                    requestFailed?.Invoke(tryCount);
                },
                tabReporter
            );
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
                return new UMI3DWorldControllerClient(redirection.media, redirection.gate, globalToken);
            else
                return new UMI3DWorldControllerClient(redirection.media, redirection.gate);
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