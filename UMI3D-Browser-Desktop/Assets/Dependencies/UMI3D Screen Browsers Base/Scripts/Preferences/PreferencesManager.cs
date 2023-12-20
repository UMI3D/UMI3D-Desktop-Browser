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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace umi3d.baseBrowser.preferences
{
    public static class PreferencesManager
    {
        public static void StoreData<T>(T data, string dataType, string directories = null)
        {
            try
            {
                string path;

                if (!string.IsNullOrEmpty(directories))
                {
                    string directoriesPath = inetum.unityUtils.Path.Combine(Application.persistentDataPath, directories);
                    if (!Directory.Exists(directoriesPath)) Directory.CreateDirectory(directoriesPath);
                    path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, $"{directories}/{dataType}");
                }
                else path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, dataType);

                FileStream file;
                if (File.Exists(path)) file = File.OpenWrite(path);
                else file = File.Create(path);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, data);

                file.Close();

            } catch(Exception ex)
            {
                Debug.LogError("PreferencesManager.StoreData " + ex.GetType());
            }
        }

        public static bool TryGet<T>(out T data, string dataType, string directories = null) where T : new()
        {
            string path;

            if (!string.IsNullOrEmpty(directories))
                path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, $"{directories}/{dataType}");
            else path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, dataType);

            if (!File.Exists(path))
            {
                data = new T();
                return false;
            }

            try
            {
                FileStream file = File.OpenRead(path);

                BinaryFormatter bf = new BinaryFormatter();
                data = (T)bf.Deserialize(file);

                file.Close();
            }
            catch
            {
                data = new T();
                return false;
            }

            return true;
        }

        public static T GetData<T>(string dataType) where T : new()
        {
            string path = inetum.unityUtils.Path.Combine(Application.persistentDataPath, dataType);

            if (!File.Exists(path)) return new T();

            T data;

            try
            {
                FileStream file = File.OpenRead(path);

                BinaryFormatter bf = new BinaryFormatter();
                data = (T)bf.Deserialize(file);

                file.Close();
            }
            catch
            {
                data = new T();
            }

            return data;
        }
    }
}
