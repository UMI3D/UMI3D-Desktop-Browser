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
using System.Security.Policy;
using System.Threading.Tasks;
using umi3d.common;
using UnityEngine;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    public class MediaDTOWebRequest : IMediaDTOWebRequest
    {
        public IConnectionStateData connectionStateData;
        public int maxCount;

        public MediaDTOWebRequest(IConnectionStateData connectionStateData, int maxCount = 3)
        {
            this.connectionStateData = connectionStateData;
            this.maxCount = maxCount;
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
            requestHandler.Execute();

            return requestHandler;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsUrlFormatValid(string url)
        {
            return url.EndsWith(UMI3DNetworkingKeys.media);
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

        async Task<MediaDto> TryRequest(string url, int count)
        {
            bool HasWWWError(UnityWebRequest www)
            {
#if UNITY_2020_1_OR_NEWER
                return www.result > UnityWebRequest.Result.Success;
#else
            return www.isNetworkError || www.isHttpError;
#endif
            }

            return null;
            //if (count >= maxCount)
            //{
            //    UnityEngine.Debug.LogError($"Max count reached");
            //    return;
            //}

            //using (UnityWebRequest www = UnityWebRequest.Get(url))
            //{
            //    UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            //    while (!operation.isDone)
            //    {
            //        await UMI3DAsyncManager.Yield();
            //    }

            //    //if (connectionStateData.ContainsStateByType<MasterServerSessionConnectionState>())
            //    //{
            //    //    connectionStateData.Add(new MediaDTOStoppedConnectionState());
            //    //    return;
            //    //}

            //    if (HasWWWError(www))
            //    {
            //        await TryRequest(url, count++);
            //    }
            //    else
            //    {
            //        if (www.downloadHandler.data == null)
            //        {
            //            return;
            //        }

            //        string json = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            //        //MediaDTO = UMI3DDtoSerializer.FromJson<MediaDto>(json, Newtonsoft.Json.TypeNameHandling.None);
            //    }
            //}
        }
    }
}
