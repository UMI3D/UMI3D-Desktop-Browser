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
using umi3d.common.interaction;

namespace umi3d.browserRuntime.connection
{
    public interface IConnectionEvents
    {
        /// <summary>
        /// Event raised when the connection state has changed.
        /// </summary>
        event Action<IConnectionStateParam> ConnectionStateChanged;
        /// <summary>
        /// Event raised when the connection has started.
        /// </summary>
        event Action<IConnectionStartedParam> ConnectionOrRedirectionStarted;
        /// <summary>
        /// Event raised when the media dto has been found.
        /// </summary>
        event Action<IMediaDtoFoundParam> MediaDtoFound;
        /// <summary>
        /// Event raised when the connection has succeeded.
        /// </summary>
        event Action<IConnectionSucceededParam> ConnectionOrRedirectionSucceeded;
        /// <summary>
        /// Event raised when the connection has failed. <br/>
        /// Has an error message as parameter.
        /// </summary>
        event Action<IConnectionFailedParam> ConnectionOrRedirectionFailed;
        /// <summary>
        /// Event raised when the environment has been loaded.
        /// </summary>
        event Action EnvironmentLoaded;
        /// <summary>
        /// event raised when the connection has been lost.
        /// </summary>
        event Action ConnectionLost;
        /// <summary>
        /// Event raised when the user asks to leave the environment.
        /// </summary>
        event Action UserAsksToLeave;
        /// <summary>
        /// Event raised when the browser receives a force disconnection.
        /// </summary>
        event Action<string> BrowserForceDisconnection;

        /// <summary>
        /// Event raised when the server asks the browser to download libraries.
        /// </summary>
        event Action<List<string>, Action<bool>> ServerAsksToLoadLib;
        /// <summary>
        /// Event raised when the server has sent a connection form.
        /// </summary>
        event Action<ConnectionFormDto, Action<FormAnswerDto>> ServerAsksForConnectionForm;
    }
}
