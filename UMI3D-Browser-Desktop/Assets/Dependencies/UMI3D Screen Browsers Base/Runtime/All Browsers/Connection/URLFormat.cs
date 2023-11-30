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
        public static string IpPortToURL(string ip, string port)
        {
            if (string.IsNullOrEmpty(ip) && string.IsNullOrEmpty(port))
            {
                UnityEngine.Debug.LogError($"Ip and port are empty or null.");
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

        public static string URLToMediaURL(string url)
        {
            string mediaURL;
            if (url.EndsWith(UMI3DNetworkingKeys.media))
            {
                mediaURL = url;
            }
            else
            {
                mediaURL = $"{url}{UMI3DNetworkingKeys.media}";
            }

            return mediaURL;
        }
    }
}
