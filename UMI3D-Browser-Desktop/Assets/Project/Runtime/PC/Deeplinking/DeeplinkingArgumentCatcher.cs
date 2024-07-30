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
using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.pc.deeplinking
{
    public static class DeeplinkingArgumentCatcher
    {
        [RuntimeInitializeOnLoadMethod]
        static void CatchArg()
        {
            UnityEngine.Debug.Log($"deeplinking catch");


            new Task(async () =>
            {
                // The number of objects that wait for a 'DeeplinkingNotificationKey.ArgumentsReceived' notification.
                int observers = 0;
                do
                {
                    // Wait one frame while no one is waiting for this notification.
                    await Task.Yield();

                    // Notify the observers that the browser was open thanks to a deeplink.
                    observers = NotificationHub.Default.Notify(
                        typeof(DeeplinkingArgumentCatcher).FullName,
                        DeeplinkingNotificationKey.ArgumentsReceived
                    );
                } while (observers == 0);
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
