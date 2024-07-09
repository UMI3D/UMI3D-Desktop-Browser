/*
Copyright 2019 - 2024 Inetum

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

namespace inetum.unityUtils
{
    /// <summary>
    /// The notification send by the publisher to the subscribers.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Id of the notification.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// The publisher of the notification.
        /// </summary>
        public Object Publisher { get; private set; }

        /// <summary>
        /// Additional information.<br/>
        /// <br/>
        /// key: Id of the information, Value: the additional information.
        /// </summary>
        public Dictionary<string, Object> Info { get; private set; }

        Notification() { }

        /// <summary>
        /// Notification constructor. Initialize all the information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="publisher"></param>
        /// <param name="info"></param>
        public Notification(string id, object publisher, Dictionary<string, object> info)
        {
            ID = id;
            Publisher = publisher;
            Info = info;
        }
    }
}
