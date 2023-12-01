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
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;

namespace umi3d.browserRuntime.connection
{
    /// <summary>
    /// Interface to access world stored data.
    /// </summary>
    public interface IWorldData
    {
        /// <summary>
        /// Current World
        /// </summary>
        public ServerPreferences.ServerData World { get; set; }
        /// <summary>
        /// List of all the worlds.
        /// </summary>
        public List<ServerPreferences.ServerData> Worlds { get; set; }
        /// <summary>
        /// List of the favorite worlds.
        /// </summary>
        public List<ServerPreferences.ServerData> FavoriteWorlds { get; set; }
    }
}
