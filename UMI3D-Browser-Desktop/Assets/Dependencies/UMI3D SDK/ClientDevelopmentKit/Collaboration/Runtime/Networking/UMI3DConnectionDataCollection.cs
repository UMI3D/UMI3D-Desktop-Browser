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
using System.Security.Policy;
using umi3d.common;
using umi3d.common.collaboration.dto.networking;

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// The collection of <see cref="UMI3DConnectionData"/>.
    /// </summary>
    public static partial class UMI3DConnectionDataCollection
    {
        static UMI3DClientLogger logger = new UMI3DClientLogger(mainTag: $"{nameof(UMI3DConnectionDataCollection)}");

        static List<UMI3DConnectionData> connections;

        public static bool hasUnsavedModification = false;

        /// <summary>
        /// At the end of the method <see cref="connections"/> is not null.
        /// </summary>
        static void LazyInitCollection()
        {
            if (connections == null)
            {
                connections = Fetch();
            }
        }

        /// <summary>
        /// Whether or not <paramref name="connectionData"/> is part of the collection.
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns></returns>
        public static bool Contains(UMI3DConnectionData connectionData) 
        {
            if (connectionData == null)
            {
                return false;
            }

            LazyInitCollection();

            return connections.Contains(connectionData);
        }

        /// <summary>
        /// Whether or not there is a <see cref="UMI3DConnectionData"/> with this url in the collection.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool Contains(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            LazyInitCollection();

            return connections.Find(connection =>
            {
                return connection.url == url;
            }) != null;
        }

        /// <summary>
        /// Adds a new connection to the collection.
        /// 
        /// <para>
        /// Return:
        /// <list type="bullet">
        /// <item>True: if the connection has been added.</item>
        /// <item>False: if the connection has not been added (because it is already present in the collection or the connection is null).</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns></returns>
        public static bool Add(UMI3DConnectionData connectionData)
        {
            if (connectionData == null)
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null", 
                    inner: null, 
                    ConnectionCollectionException.ExceptionTypeEnum.ConnectionNullException
                );
                return false;
            }

            LazyInitCollection();

            if (Contains(connectionData))
            {
                return false;
            }

            connections.Add(connectionData);
            hasUnsavedModification = true;
            return true;
        }

        /// <summary>
        /// Removes <paramref name="connectionData"/> from the collection.
        /// 
        /// <para>
        /// Return:
        /// <list type="bullet">
        /// <item>True: if the connection has been removed.</item>
        /// <item>False: if the connection has not been removed (because it is not present in the collection or the connection is null).</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns></returns>
        public static bool Remove(UMI3DConnectionData connectionData)
        {
            if (connectionData == null)
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null", 
                    inner: null, 
                    ConnectionCollectionException.ExceptionTypeEnum.ConnectionNullException
                );
                return false;
            }

            LazyInitCollection();

            if (!Contains(connectionData))
            {
                return false;
            }

            connections.Remove(connectionData);
            hasUnsavedModification = true;
            return true;
        }

        /// <summary>
        /// Removes the <see cref="UMI3DConnectionData"/> corresponding to <paramref name="url"/> from the collection.
        /// 
        /// <para>
        /// Return:
        /// <list type="bullet">
        /// <item>True: if the connection has been removed.</item>
        /// <item>False: if the connection has not been removed (because it is not present in the collection or the url is null).</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns></returns>
        public static bool Remove(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null", 
                    inner: null, 
                    ConnectionCollectionException.ExceptionTypeEnum.URLNullOrEmptyException
                );
                return false;
            }

            return Remove(FindConnection(url));
        }

        /// <summary>
        /// Updates the connections stored 
        /// </summary>
        /// <param name="connectionData"></param>
        /// <returns></returns>
        public static bool Update(UMI3DConnectionData connectionData)
        {
            if (connectionData == null)
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null",
                    inner: null,
                    ConnectionCollectionException.ExceptionTypeEnum.ConnectionNullException
                );
                return false;
            }

            LazyInitCollection();

            int connectionIndex = connections.FindIndex(connection =>
            {
                return connection.url == connectionData.url;
            });
            if (connectionIndex < 0)
            {
                ConnectionCollectionException.LogException(
                    "There is no connection to update.",
                    inner: null,
                    ConnectionCollectionException.ExceptionTypeEnum.ConnectionNullException
                );
                return false;
            }

            connections.RemoveAt(connectionIndex);
            connections.Insert(connectionIndex, connectionData);

            return true;
        }

        /// <summary>
        /// Fetch the current state of the connection collection.
        /// </summary>
        /// <returns></returns>
        public static List<UMI3DConnectionData> Fetch()
        {
            logger.DebugTodo($"{nameof(Fetch)}", $"Implement the fetching of the connections");
            return null;
        }

        /// <summary>
        /// Save the current state of the connection collection.
        /// </summary>
        public static void Save()
        {
            logger.DebugTodo($"{nameof(Save)}", $"Implement the saving of the connections");

            hasUnsavedModification = false;
        }

        /// <summary>
        /// Find the <see cref="UMI3DConnectionData"/> that correspond to the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static UMI3DConnectionData FindConnection(Predicate<UMI3DConnectionData> predicate)
        {
            if (predicate == null)
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null", 
                    inner: null, 
                    ConnectionCollectionException.ExceptionTypeEnum.PredicateNullException
                );
                return null;
            }

            LazyInitCollection();

            return connections.Find(predicate);
        }

        /// <summary>
        /// Find the <see cref="UMI3DConnectionData"/> that match the <paramref name="url"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static UMI3DConnectionData FindConnection(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null",
                    inner: null,
                    ConnectionCollectionException.ExceptionTypeEnum.URLNullOrEmptyException
                );
                return null;
            }

            return FindConnection(connection =>
            {
                return connection.url == url;
            });
        }

        /// <summary>
        /// Queries the connections that correspond to the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<UMI3DConnectionData> Query(Predicate<UMI3DConnectionData> predicate, Comparison<UMI3DConnectionData> sortBy = null)
        {
            if (predicate == null)
            {
                ConnectionCollectionException.LogException(
                    "Parameter is null", 
                    inner: null, 
                    ConnectionCollectionException.ExceptionTypeEnum.PredicateNullException
                );
                return null;
            }

            LazyInitCollection();

            List<UMI3DConnectionData> result = connections.FindAll(predicate);

            if (sortBy != null)
            {
                result.Sort(sortBy);
            }

            return result;
        }
    }

    public static partial class UMI3DConnectionDataCollection
    {
        /// <summary>
        /// Returns favorite connection sorted by <paramref name="sortBy"/>.
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<UMI3DConnectionData> GetFavorites(Comparison<UMI3DConnectionData> sortBy = null)
        {
            return Query(
                predicate: connection =>
                {
                    return connection.isFavorite;
                },
                sortBy
            );
        }
    }

    /// <summary>
    /// An exception class to deal with <see cref="UMI3DConnectionDataCollection"/> issues.
    /// </summary>
    [Serializable]
    public class ConnectionCollectionException : Exception
    {
        static UMI3DClientLogger logger = new UMI3DClientLogger(mainTag: $"{nameof(ConnectionCollectionException)}");

        public enum ExceptionTypeEnum
        {
            Unknown,
            ConnectionNullException,
            PredicateNullException,
            URLNullOrEmptyException
        }

        public ExceptionTypeEnum exceptionType;

        public ConnectionCollectionException(string message, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}")
        {
            this.exceptionType = exceptionType;
        }
        public ConnectionCollectionException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown) : base($"{exceptionType}: {message}", inner)
        {
            this.exceptionType = exceptionType;
        }

        public static void LogException(string message, Exception inner, ExceptionTypeEnum exceptionType = ExceptionTypeEnum.Unknown)
        {
            try
            {
                throw new ConnectionCollectionException(message, inner, exceptionType);
            }
            catch (Exception e)
            {
                logger.Exception(null, e);
            }
        }
    }
}
