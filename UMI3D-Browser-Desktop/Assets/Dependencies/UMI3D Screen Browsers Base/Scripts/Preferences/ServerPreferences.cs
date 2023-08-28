/*
Copyright 2019 - 2022 Inetum

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

namespace umi3d.baseBrowser.preferences
{
    public class ServerPreferences
    {
        /// <summary>
        /// Contains : environment name, ip and port.
        /// </summary>
        [Serializable]
        public class Data
        {
            public string environmentName;
            public string ip;
            public string port;

            public override string ToString() => $"name = {environmentName}, ip = {ip}, port = {port}";
        }

        /// <summary>
        /// Contains : server name, url and icon.
        /// </summary>
        [Serializable]
        public class ServerData
        {
            public string serverName;
            public string serverUrl;
            public string serverIcon;
            public string dateFirstConnection;
            public string dateLastConnection;
            public bool isFavorite;
        }

        public const string registeredServer = "registeredServerData";
        public const string previousServer = "previusServerData";
        public const string dataFile = "userData";
        public const string favoriteDataFile = "favoriteUserData";

        #region Store Data.

        /// <summary>
        /// Write a previous userInfo data.
        /// </summary>
        /// <param name="data">DataFile to write.</param>
        /// <param name="directory">Directory to write the file into.</param>
        public static void StoreUserData(Data data)
            => PreferencesManager.StoreData(data, dataFile);

        /// <summary>
        /// Write a previous userInfo data server.
        /// </summary>
        /// <param name="data">ServerData to write.</param>
        public static void StoreUserData(ServerData data)
            => PreferencesManager.StoreData(data, previousServer);

        /// <summary>
        /// Stores the connection data about the registered servers.
        /// </summary>
        public static void StoreRegisteredServerData(List<ServerData> savedServers) 
            => PreferencesManager.StoreData(savedServers, registeredServer);

        #endregion

        #region Getter for Server

        /// <summary>
        /// Read a userInfo data in a directory.
        /// </summary>
        /// <returns>A DataFile if the directory containe one, null otherwhise.</returns>
        public static Data GetPreviousConnectionData()
            => PreferencesManager.GetData<Data>(dataFile);

        /// <summary>
        /// Read a userInfo data in a directory.
        /// </summary>
        /// <returns>A ServerData if the directory containe one, null otherwhise.</returns>
        public static ServerData GetPreviousServerData()
            => PreferencesManager.GetData<ServerData>(previousServer);

        /// <summary>
        /// get the connection data about the favorite server.
        /// </summary>
        /// <returns></returns>
        public static List<ServerData> GetRegisteredServerData()
            => PreferencesManager.GetData<List<ServerData>>(registeredServer);

        #endregion
    }
}