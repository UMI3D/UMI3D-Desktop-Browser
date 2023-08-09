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

namespace umi3d.cdk.collaboration
{
    /// <summary>
    /// A data wrapper for the connection process.
    /// </summary>
    public class UMI3DConnectionData
    {
        /// <summary>
        /// The url of this connection.
        /// </summary>
        public string url;
        /// <summary>
        /// The name of this connection (received from the server).
        /// </summary>
        public string name;
        /// <summary>
        /// The name of this connection (chose by the user).
        /// </summary>
        public string nickname;
        /// <summary>
        /// The icon of this connection.
        /// </summary>
        public string icon;
        /// <summary>
        /// The date of the first connection.
        /// </summary>
        public DateTime firstConnection;
        /// <summary>
        /// The date of the last connection.
        /// </summary>
        public DateTime lastConnection;
        /// <summary>
        /// The number of succeeded connection.
        /// </summary>
        public int numberOfConnection;

        public UMI3DConnectionData(string name, string nickname, string url, string icon, DateTime firstConnection, DateTime lastConnection, int numberOfConnection)
        {
            this.name = name;
            this.nickname = nickname;
            this.url = url;
            this.icon = icon;
            this.firstConnection = firstConnection;
            this.lastConnection = lastConnection;
            this.numberOfConnection = numberOfConnection;
        }

        public UMI3DConnectionData()
        {
        }

        /// <summary>
        /// The ip part of the <see cref="url"/>
        /// </summary>
        public string ip
        {
            get
            {
                return url.Split(':')[0];
            }
        }

        /// <summary>
        /// The port part of the <see cref="url"/>
        /// </summary>
        public ushort? port
        {
            get
            {
                if (ushort.TryParse(url.Split(':')[1], out ushort port))
                {
                    return port;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
