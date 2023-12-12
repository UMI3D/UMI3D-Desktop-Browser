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

using umi3d.common;

namespace umi3d.browserRuntime.connection
{
    public static class URLFormat
    {
        /// <summary>
        /// Combine Ip and Port to get a URL.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string IpPortToURL(string ip, string port)
        {
            if (string.IsNullOrEmpty(ip))
            {
                UnityEngine.Debug.LogError($"Ip is empty or null.");
                return null;
            }

            string url;

            if (string.IsNullOrEmpty(port))
            {
                url = ip;
            }
            else
            {
                url = $"{ip}:{port}";
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = $"http://{url}";
            }

            return url;
        }

        /// <summary>
        /// Format the url to get a mediaDTO url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string URLToMediaURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                UnityEngine.Debug.LogError($"url is null or empty.");
                return null;
            }

            string mediaURL;
            if (url.EndsWith(UMI3DNetworkingKeys.media))
            {
                mediaURL = IpPortToURL(url, null);
            }
            else
            {
                mediaURL = $"{IpPortToURL(url, null)}{UMI3DNetworkingKeys.media}";
            }

            return mediaURL;
        }
    }
}
