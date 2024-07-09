using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
                    DeeplinkingNotificationKey.ArgumentsReceived,
                    typeof(DeeplinkingArgumentCatcher).FullName
                );
            } while (observers == 0);
        }).Start(TaskScheduler.FromCurrentSynchronizationContext());
    }
}
