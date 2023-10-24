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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;
using UnityEngine.Networking;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// Send HTTP requests.
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// Ienumerator to send GET request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <returns></returns>
        public static async Task<UnityWebRequest> GetRequest(string HeaderToken, string url, Func<RequestFailedArgument, bool> ShouldTryAgain, bool UseCredential = false, List<(string, string)> headers = null, int tryCount = 0)
        {
            var www = UnityWebRequest.Get(url);
            if (UseCredential) www.SetRequestHeader(UMI3DNetworkingKeys.Authorization, HeaderToken);
            if (headers != null)
            {
                foreach ((string, string) item in headers)
                {
                    www.SetRequestHeader(item.Item1, item.Item2);
                }
            }
            DateTime date = DateTime.UtcNow;
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone)
                await UMI3DAsyncManager.Yield();

#if UNITY_2020_1_OR_NEWER
            if (www.result > UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {

                if (UMI3DClientServer.Exists && await UMI3DClientServer.Instance.TryAgainOnHttpFail(new RequestFailedArgument(www, tryCount, date, ShouldTryAgain)))
                    return await GetRequest(HeaderToken, url, ShouldTryAgain, UseCredential, headers, tryCount + 1);
                else
                    throw new Umi3dNetworkingException(www, "Failed to get ");
            }
            return www;
        }

        /// <summary>
        /// Ienumerator to send POST Request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="bytes">Data send via post method.</param>
        /// <param name="callback">Action to be call when the request succeed.</param>
        /// <param name="onError">Action to be call when the request fail.</param>
        /// <returns></returns>
        public static async Task<UnityWebRequest> PostRequest(string HeaderToken, string url, string contentType, byte[] bytes, Func<RequestFailedArgument, bool> ShouldTryAgain, bool UseCredential = false, List<(string, string)> headers = null, int tryCount = 0)
        {

            UnityWebRequest www = CreatePostRequest(url, bytes, contentType, true);
            if (UseCredential) www.SetRequestHeader(UMI3DNetworkingKeys.Authorization, HeaderToken);
            if (headers != null)
            {
                foreach ((string, string) item in headers)
                {
                    www.SetRequestHeader(item.Item1, item.Item2);
                }
            }
            DateTime date = DateTime.UtcNow;

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone)
                await UMI3DAsyncManager.Yield();

#if UNITY_2020_1_OR_NEWER
            if (www.result > UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                if (UMI3DClientServer.Exists && await UMI3DClientServer.Instance.TryAgainOnHttpFail(new RequestFailedArgument(www, tryCount, date, ShouldTryAgain)))
                    return await PostRequest(HeaderToken, url, contentType, bytes, ShouldTryAgain, UseCredential, headers, tryCount + 1);
                else
                {
                    UnityEngine.Debug.Log(System.Text.Encoding.ASCII.GetString(bytes));
                    throw new Umi3dNetworkingException(www, " Failed to post\n" + www.downloadHandler.text);
                }
            }
            return www;
        }

        /// <summary>
        /// Util function to create POST request.
        /// </summary>
        /// <param name="url">Url to send the request at.</param>
        /// <param name="bytes">Data send via post method.</param>
        /// <param name="withResult">require a result</param>
        /// <returns></returns>
        private static UnityWebRequest CreatePostRequest(string url, byte[] bytes, string contentType = null, bool withResult = false)
        {
            var requestU = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            var uH = new UploadHandlerRaw(bytes);
            if (contentType != null)
                uH.contentType = contentType;
            requestU.uploadHandler = uH;
            if (withResult)
                requestU.downloadHandler = new DownloadHandlerBuffer();
            //requestU.SetRequestHeader("access_token", UMI3DClientServer.GetToken(null));
            return requestU;
        }
    }
}