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

public class UserPreferencesManager 
{
    [Serializable]
    public class Data
    {
        public string environmentName;
        public string ip;
    }

    public const string dataFile = "userData";
    public const string favoriteDataFile = "favoriteUserData";

    /// <summary>
    /// Write a previous userInfo data.
    /// </summary>
    /// <param name="data">DataFile to write.</param>
    /// <param name="directory">Directory to write the file into.</param>
    public static void StoreUserData(Data data)
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Read a userInfo data in a directory.
    /// </summary>
    /// <returns>A DataFile if the directory containe one, null otherwhise.</returns>
    public static Data GetPreviousConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();

            Data data;
            try
            {
                data = (Data)bf.Deserialize(file);
            } catch
            {
                data = new Data();
            }
           
            file.Close();
            return data;
        }
        return new Data();
    }

    /// <summary>
    /// get the connection data about the favorite environments.
    /// </summary>
    /// <returns></returns>
    public static List<Data> GetFavoriteConnectionData()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, favoriteDataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            List<Data> data;
            try
            {
                data = (List<Data>)bf.Deserialize(file);
            } catch
            {
                data = new List<Data>();
            }
            file.Close();
            return data;
        }
        return new List<Data>();
    }

    /// <summary>
    /// Stores the connection data about the favorite environments.
    /// </summary>
    public static void StoreFavoriteConnectionData(List<Data> favorites)
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, favoriteDataFile);
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, favorites);
        file.Close();
    }
}
