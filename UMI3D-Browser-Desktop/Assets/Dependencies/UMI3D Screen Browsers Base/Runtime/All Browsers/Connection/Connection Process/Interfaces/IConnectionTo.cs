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

namespace umi3d.browserRuntime.connection
{
    /// <summary>
    /// Interface for classes that have the purpose to connecte this browser to something.
    /// </summary>
    public interface IConnectionTo 
    {
        enum ConnectionToResult
        {
            /// <summary>
            /// The browser is disconnected from this element.
            /// </summary>
            Disconnected,
            /// <summary>
            /// The browser is in the process of being connected.
            /// </summary>
            Processing,
            /// <summary>
            /// The browser is connected to this element.
            /// </summary>
            Connected,
            /// <summary>
            /// The connection to this element has been canceled.
            /// </summary>
            Aborted,
            /// <summary>
            /// An error occurs while trying to connect to this element.
            /// </summary>
            Error
        }

        const string ConnectionAlreadyInProgress = "Cannot connect when the element is in the process of connecting or already connected.";

        /// <summary>
        /// Event raised when the connection to this element has succeeded.
        /// </summary>
        event Action<IConnectionTo> Connected;
        /// <summary>
        /// Event raised when the disconnection to this element has succeeded.
        /// </summary>
        event Action<IConnectionTo> Disconnected;
        /// <summary>
        /// Event raised when the connection or the disconnection has been canceled.
        /// </summary>
        event Action<IConnectionTo> Canceled;

        /// <summary>
        /// Error message.
        /// </summary>
        string Error { get; }
        /// <summary>
        /// Result of the connection.
        /// </summary>
        ConnectionToResult Result { get; }
        /// <summary>
        /// The task relative to the connection or disconnection of this element.<br/>
        /// You can wait for this task to end or listen to <see cref="Connected"/>, <see cref="Disconnected"/> and <see cref="Canceled"/> events.
        /// </summary>
        Task ConnectionOrDisconnectionTask { get; }

        /// <summary>
        /// connect to this element.
        /// </summary>
        /// <returns></returns>
        Task Connect(string url);
        /// <summary>
        /// Disconnect if it was connected.
        /// </summary>
        /// <returns></returns>
        Task Disconnect();
        /// <summary>
        /// Format the url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string URLToFormattedURL(string url);
    }
}