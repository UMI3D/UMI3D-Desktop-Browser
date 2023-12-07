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
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    /// <summary>
    /// Asynchronous request handler.<br/>
    /// You can be notify when the request finished with the <see cref="IAsyncRequestHandler.Completed"/> event.
    /// </summary>
    public interface IAsyncRequestHandler 
    {
        /// <summary>
        /// Event raised when the request is done.
        /// </summary>
        event Action<IAsyncRequestHandler> Completed;

        /// <summary>
        /// Whether or not this handler is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Whether or not the request done.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Returns a floating-point value between 0.0 and 1.0, indicating the progress of completion.
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Whether or not the request has been canceled.
        /// </summary>
        bool HasBeenCanceled { get; }

#if UNITY_2020_1_OR_NEWER
        /// <summary>
        /// Result of the request
        /// </summary>
        UnityWebRequest.Result Result { get; }
#else
        public enum Result
        {
            //
            // Résumé :
            //     The request hasn't finished yet.
            InProgress,
            //
            // Résumé :
            //     The request succeeded.
            Success,
            //
            // Résumé :
            //     Failed to communicate with the server. For example, the request couldn't connect
            //     or it could not establish a secure channel.
            ConnectionError,
            //
            // Résumé :
            //     The server returned an error response. The request succeeded in communicating
            //     with the server, but received an error as defined by the connection protocol.
            ProtocolError,
            //
            // Résumé :
            //     Error processing data. The request succeeded in communicating with the server,
            //     but encountered an error when processing the received data. For example, the
            //     data was corrupted or not in the correct format.
            DataProcessingError
        }
        Result Result { get; }
#endif
        /// <summary>
        /// A human-readable string describing any system errors encountered by the requests or responses.
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Returns the bytes from data interpreted as a UTF8 string.
        /// </summary>
        string DownloadedText { get; }

        /// <summary>
        /// Execute the request. This method should only be called once.
        /// </summary>
        /// <returns></returns>
        void Execute();
        /// <summary>
        /// Abort the request as soon as possible.
        /// </summary>
        void Abort();
        /// <summary>
        /// Return the downloaded data.
        /// </summary>
        /// <param name="typeNameHandling"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        T GetDownloadedData<T>(TypeNameHandling typeNameHandling = TypeNameHandling.All, IList<JsonConverter> converters = null);
    }
}