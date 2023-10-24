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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.collaboration.dto.networking;
using umi3d.common.collaboration.dto.signaling;
using UnityEngine.Networking;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Send HTTP requests to the environment server.
    /// </summary>
    /// Usually used before connection or to retrieve DTOs.
    public class HttpClientEnvironment
    {
        private const DebugScope k_Scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Networking;

        internal string HeaderToken;

        private string m_HttpUrl => m_EnvironmentClient.connectionDto.httpUrl;

        private readonly ThreadDeserializer m_Deserializer;
        private readonly UMI3DEnvironmentClient m_EnvironmentClient;

        /// <summary>
        /// Init HttpClientWorldController.
        /// </summary>
        /// <param name="UMI3DClientServer"></param>
        public HttpClientEnvironment(UMI3DEnvironmentClient pEnvironmentClient)
        {
            m_EnvironmentClient = pEnvironmentClient;
            UMI3DLogger.Log($"Init HttpClient(Environment)", k_Scope | DebugScope.Connection);
            m_Deserializer = new ThreadDeserializer();
        }

        public void Stop()
        {
            m_Deserializer?.Stop();
        }

        public void SetToken(string token)
        {
            UMI3DLogger.Log($"SetToken {token}", k_Scope | DebugScope.Connection);
            HeaderToken = UMI3DNetworkingKeys.bearer + token;
        }

        private static bool DefaultShouldTryAgain(RequestFailedArgument argument)
        {
            return argument.count < 3;
        }

        /// <summary>
        /// Send request using GET method to get the user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<UserConnectionDto> SendGetIdentity(Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send Get Identity", k_Scope | DebugScope.Connection);

            using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.connectionInfo, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received Get Identity", k_Scope | DebugScope.Connection);
                UMI3DDto dto = await m_Deserializer.FromBson(uwr?.downloadHandler.data);
                return dto as UserConnectionDto;
            }
        }

        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<PendingTransactionDto> SendPostUpdateIdentity(UserConnectionAnswerDto answer, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            PendingTransactionDto result = null;
            UMI3DLogger.Log($"Send PostUpdateIdentity", k_Scope | DebugScope.Connection);
            using (UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.connection_information_update, null, answer.ToBson(), (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                try
                {
                    var b = uwr?.downloadHandler.data;
                    if (b != null)
                    {
                        result = UMI3DDtoSerializer.FromBson<PendingTransactionDto>(b);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
            UMI3DLogger.Log($"Received PostUpdateIdentity", k_Scope | DebugScope.Connection);
            return result;
        }
        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostUpdateStatus(StatusType status, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostUpdateStatus", k_Scope | DebugScope.Connection);
            UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.status_update, null, new StatusDto() { status = status }.ToBson(), (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            uwr.Dispose();
            UMI3DLogger.Log($"Received PostUpdateStatus", k_Scope | DebugScope.Connection);
        }

        /// <summary>
        /// Send request using POST method to logout of the server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostLogout(Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostLogout", k_Scope | DebugScope.Connection);
            UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.logout, null, new UMI3DDto().ToBson(), (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            uwr.Dispose();
            UMI3DLogger.Log($"Received PostLogout", k_Scope | DebugScope.Connection);
        }


        /// <summary>
        /// Send request using GET.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<LibrariesDto> SendGetLibraries(Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetLibraries", k_Scope | DebugScope.Connection);
            using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.libraries, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received GetLibraries", k_Scope | DebugScope.Connection);
                UMI3DDto dto = await m_Deserializer.FromBson(uwr?.downloadHandler.data);
                return dto as LibrariesDto;
            }
        }

        /// <summary>
        /// Get a LoadEntityDto
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="onError"></param>
        /// <param name="shouldTryAgain"></param>
        public async Task<LoadEntityDto> SendPostEntity(EntityRequestDto id, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostEntity", k_Scope | DebugScope.Connection);
            using (UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.entity, null, id.ToBson(), (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received PostEntity", k_Scope | DebugScope.Connection);
                UMI3DDto dto = await m_Deserializer.FromBson(uwr?.downloadHandler.data);
                return dto as LoadEntityDto;
            }
        }

        /// <summary>
        /// Send request using GET
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<byte[]> SendGetPublic(string url, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetPublic {url}", k_Scope | DebugScope.Connection);
            using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, url, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), false))
            {
                UMI3DLogger.Log($"received getPublic {url}", k_Scope | DebugScope.Connection);
                return uwr?.downloadHandler.data;
            }
        }

        /// <summary>
        /// Send request using GET method to get the a private file.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <param name="useParameterInsteadOfHeader">If true, sets authorization via parameters instead of header</param>
        public async Task<byte[]> SendGetPrivate(string url, bool useParameterInsteadOfHeader, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetPrivate {url}", k_Scope | DebugScope.Connection);

            if (useParameterInsteadOfHeader)
            {
                if (UMI3DResourcesManager.HasUrlGotParameters(url))
                    url += "&" + UMI3DNetworkingKeys.ResourceServerAuthorization + "=" + HeaderToken;
                else
                    url += "?" + UMI3DNetworkingKeys.ResourceServerAuthorization + "=" + HeaderToken;
            }
            int i = 0;
            while (i < 10)
            {
                i++;
                using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, url, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), !useParameterInsteadOfHeader))
                {
                    UMI3DLogger.Log($"Received GetPrivate {url}\n{uwr?.responseCode}\n{uwr?.url}", k_Scope | DebugScope.Connection);
                    if (uwr?.responseCode != 204)
                        return uwr?.downloadHandler.data;
                    UMI3DLogger.Log($"Resend GetPrivate Because responce code was 204 {url}", k_Scope | DebugScope.Connection);
                    await UMI3DAsyncManager.Delay(1000);
                }
            }
            return null;
        }

        /// <summary>
        /// Send request using GET method to get the Environement.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<GlTFEnvironmentDto> SendGetEnvironment(Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetEnvironment", k_Scope | DebugScope.Connection);
            using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.environment, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received GetEnvironment", k_Scope | DebugScope.Connection);
                UMI3DDto dto = await m_Deserializer.FromBson(uwr?.downloadHandler.data);
                return dto as GlTFEnvironmentDto;
            }
        }

        /// <summary>
        /// Send request using POST method to Join server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<EnterDto> SendPostJoin(JoinDto join, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostJoin", k_Scope | DebugScope.Connection);

            using (UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.join, null, join.ToBson(), (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received PostJoin", k_Scope | DebugScope.Connection);
                UMI3DDto dto = await m_Deserializer.FromBson(uwr?.downloadHandler.data);
                return dto as EnterDto;
            }
        }

        /// <summary>
        /// Send request using POST method to request the server to send a Scene.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task SendPostSceneRequest(Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostSceneRequest", k_Scope | DebugScope.Connection);
            UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, m_HttpUrl + UMI3DNetworkingKeys.scene, null, null, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            uwr.Dispose();
            UMI3DLogger.Log($"Received PostSceneRequest", k_Scope | DebugScope.Connection);
        }

        /// <summary>
        /// Send request using POST method to update user Identity.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async void SendPostUpdateStatusAsync(StatusType status, bool throwError = false, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            try
            {
                await SendPostUpdateStatus(status, shouldTryAgain);
            }
            catch (UMI3DAsyncManagerException)
            {

            }
            catch
            {
                if (throwError)
                    throw;
            }
        }

        /// <summary>
        /// Send request using POST method to send to the server Local Info.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <param name="key">Local data file key.</param>
        public async Task SendPostLocalInfo(string key, byte[] bytes, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send PostLocalInfo {key}", k_Scope | DebugScope.Connection);
            string url = System.Text.RegularExpressions.Regex.Replace(m_HttpUrl + UMI3DNetworkingKeys.localData, ":param", key);
            UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, url, null, bytes, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true);
            uwr.Dispose();
            UMI3DLogger.Log($"Received PostLocalInfo {key}", k_Scope | DebugScope.Connection);
        }

        /// <summary>
        /// Send request using GET method to get datas from server then save its in local file.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        public async Task<byte[]> SendGetLocalInfo(string key, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            UMI3DLogger.Log($"Send GetLocalInfo {key}", k_Scope | DebugScope.Connection);
            string url = System.Text.RegularExpressions.Regex.Replace(m_HttpUrl + UMI3DNetworkingKeys.localData, ":param", key);

            using (UnityWebRequest uwr = await HttpClient.GetRequest(HeaderToken, url, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true))
            {
                UMI3DLogger.Log($"Received GetLocalInfo {key}", k_Scope | DebugScope.Connection);
                return uwr?.downloadHandler.data;
            }
        }

        /// <summary>
        /// Send request using POST method to send file to the server.
        /// </summary>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <param name="token">Authorization token, given by the server.</param>
        /// <param name="fileName">Name of the uploaded file.</param>
        /// <param name="bytes">the file in bytes.</param>
        /// <param name="shouldTryAgain"></param>
        public async Task SendPostFile(string token, string fileName, byte[] bytes, Func<RequestFailedArgument, bool> shouldTryAgain = null)
        {
            string url = System.Text.RegularExpressions.Regex.Replace(m_HttpUrl + UMI3DNetworkingKeys.uploadFile, ":param", token);
            var headers = new List<(string, string)>
            {
                (UMI3DNetworkingKeys.contentHeader, fileName)
            };
            UnityWebRequest uwr = await HttpClient.PostRequest(HeaderToken, url, null, bytes, (e) => shouldTryAgain?.Invoke(e) ?? DefaultShouldTryAgain(e), true, headers);
            uwr.Dispose();
        }
    }
}