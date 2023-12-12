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
    public interface IMediaDTOWebRequest
    {
        /// <summary>
        /// Return a <see cref="IAsyncRequestHandler"/> for the request of a MediaDTO.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IAsyncRequestHandler RequestMediaDto(string url);
        /// <summary>
        /// Convert an <see cref="IAsyncRequestHandler"/> result in a <see cref="MediaDto"/>.
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <returns></returns>
        MediaDto ConvertToMediaDTO(IAsyncRequestHandler requestHandler);
        /// <summary>
        /// Whether or not the format of <paramref name="url"/> is valid for requesting a mediaDTO.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsUrlFormatValid(string url);
        /// <summary>
        /// Format the url to get a mediaDTO url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string URLToMediaURL(string url);
    }
}
