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

using BeardedManStudios.Forge.Networking;
using System;
using UnityEngine;

namespace umi3d.browserRuntime.connection
{
    public interface IConnectionToMasterServer 
    {
        /// <summary>
        /// Event raised when a request info has succeeded.
        /// </summary>
        event Action<(string serverName, string icon)> requestServerInfSucceeded;
        /// <summary>
        /// Event raised when a request info has succeeded.
        /// </summary>
        event Action<MasterServerResponse.Server> requestSessionInfSucceeded;
    }
}