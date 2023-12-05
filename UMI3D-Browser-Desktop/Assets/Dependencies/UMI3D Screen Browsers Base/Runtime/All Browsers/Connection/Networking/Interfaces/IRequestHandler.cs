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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    public interface IRequestHandler<T> 
    {
        /// <summary>
        /// Event raised when the request is done.
        /// </summary>
        event Action<IRequestHandler<T>> Completed;

        /// <summary>
        /// Whether or not this handler is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Whether or not the request done.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Number of try before abandoning the request.
        /// </summary>
        int CountTry { get; set; }

#if UNITY_2020_1_OR_NEWER
        /// <summary>
        /// Result of the request
        /// </summary>
        UnityWebRequest.Result Result { get; }
#else
        public enum Result
        {
            //
            // R�sum�:
            //     The request hasn't finished yet.
            InProgress,
            //
            // R�sum�:
            //     The request succeeded.
            Success,
            //
            // R�sum�:
            //     Failed to communicate with the server. For example, the request couldn't connect
            //     or it could not establish a secure channel.
            ConnectionError,
            //
            // R�sum�:
            //     The server returned an error response. The request succeeded in communicating
            //     with the server, but received an error as defined by the connection protocol.
            ProtocolError,
            //
            // R�sum�:
            //     Error processing data. The request succeeded in communicating with the server,
            //     but encountered an error when processing the received data. For example, the
            //     data was corrupted or not in the correct format.
            DataProcessingError
        }
        Result Result { get; }
#endif

        /// <summary>
        /// The value of type <typeparamref name="T"/> requested by the request.
        /// </summary>
        T RequestValue { get; }

        /// <summary>
        /// Return a Task object to wait on when using async await.
        /// </summary>
        Task<T> Task { get; }
    }
}