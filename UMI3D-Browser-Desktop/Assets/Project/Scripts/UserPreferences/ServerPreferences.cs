/*
Copyright 2019 Gfi Informatique

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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BrowserDesktop.preferences
{
    public class ServerPreferences
    {
        [Serializable]
        public class Data
        {
            public string environmentName;
            public string ip;
            public string port;
        }

        [Serializable]
        public class ServerData
        {
            public string serverName;
            public string serverUrl;
            public string serverIcon;
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
        {
            StoreData<Data>(data, dataFile);
        }

        /// <summary>
        /// Write a previous userInfo data server.
        /// </summary>
        /// <param name="data">ServerData to write.</param>
        public static void StoreUserData(ServerData data)
        {
            StoreData<ServerData>(data, previousServer);
        }

        /// <summary>
        /// Stores the connection data about the registered servers.
        /// </summary>
        public static void StoreRegisteredServerData(List<ServerData> favorites)
        {
            StoreData<List<ServerData>>(favorites, registeredServer);
        }

        private static void StoreData<T>(T data, string dataType)
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, dataType);

            FileStream file;
            if (File.Exists(path)) file = File.OpenWrite(path);
            else file = File.Create(path);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);

            file.Close();
        }

        #endregion

        #region Getter for Server

        /// <summary>
        /// Read a userInfo data in a directory.
        /// </summary>
        /// <returns>A DataFile if the directory containe one, null otherwhise.</returns>
        public static Data GetPreviousConnectionData()
        {
            return GetData<Data>(dataFile);
        }

        /// <summary>
        /// Read a userInfo data in a directory.
        /// </summary>
        /// <returns>A ServerData if the directory containe one, null otherwhise.</returns>
        public static ServerData GetPreviousServerData()
        {
            return GetData<ServerData>(previousServer);
        }

        /// <summary>
        /// get the connection data about the favorite server.
        /// </summary>
        /// <returns></returns>
        public static List<ServerData> GetRegisteredServerData()
        {
            return GetData<List<ServerData>>(registeredServer);
        }

        private static T GetData<T>(string dataType) where T : new()
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, dataType);

            if (File.Exists(path))
            {
                FileStream file;
                file = File.OpenRead(path);

                BinaryFormatter bf = new BinaryFormatter();
                T data;

                try
                {
                    data = (T)bf.Deserialize(file);
                }
                catch
                {
                    data = new T();
                }

                file.Close();
                return data;
            }

            return new T();
        }

        #endregion

        /// <summary>
        /// Stores the connection data about the registered servers.
        /// </summary>
        public static void AddRegisterdeServerData(ServerData newFavorite)
        {
            var favorites = GetRegisteredServerData();
            favorites.Add(newFavorite);
            StoreRegisteredServerData(favorites);
        }
    }
}