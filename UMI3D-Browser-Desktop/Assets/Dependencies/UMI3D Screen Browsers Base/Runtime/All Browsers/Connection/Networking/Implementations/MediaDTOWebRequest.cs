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
using Newtonsoft.Json;
using System.Threading.Tasks;
using umi3d.common;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    public class MediaDTOWebRequest : IMediaDTOWebRequest
    {
        IConnectionStateData connectionStateData;
        int maxTryCount;

        public MediaDTOWebRequest(IConnectionStateData connectionStateData, int maxTryCount = 3)
        {
            this.connectionStateData = connectionStateData;
            this.maxTryCount = maxTryCount;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IAsyncRequestHandler RequestMediaDto(string url)
        {
            if (!IsUrlFormatValid(url))
            {
                UnityEngine.Debug.LogError($"Url [{url}] is not valid");
            }

            IAsyncRequestHandler requestHandler = new AsyncRequestedHandler(webRequestFactory: tries =>
            {
                return Task.FromResult(UnityWebRequest.Get(url));
            });
            requestHandler.IsCancellationRequired = () =>
            {
                if (connectionStateData.Contains(new MasterServerSessionConnectionState().Id))
                {
                    var stopConnection = new MediaDTOStoppedConnectionState();
                    connectionStateData.Add(stopConnection, stopConnection.Id);
                    return true;
                }
                else
                {
                    return false;
                }
            };
            requestHandler.Completed += handler =>
            {
                if (handler.Result > UnityWebRequest.Result.Success && handler.TryCount < maxTryCount)
                {
                    handler.Retry();
                }
                else if (handler.Result > UnityWebRequest.Result.Success && handler.TryCount >= maxTryCount)
                {
                    UnityEngine.Debug.LogError($"MediaDTO request failed after {maxTryCount} tries.");
                }
            };
            requestHandler.Execute();

            return requestHandler;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <returns></returns>
        public MediaDto ConvertToMediaDTO(IAsyncRequestHandler requestHandler)
        {
            return requestHandler.GetDownloadedData<MediaDto>(TypeNameHandling.None);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsUrlFormatValid(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            if (!url.EndsWith(UMI3DNetworkingKeys.media))
            {
                return false;
            }
            else
            {
                if (url.Equals(UMI3DNetworkingKeys.media))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string URLToMediaURL(string url)
        {
            return URLFormat.URLToMediaURL(url);
        }
    }
}
