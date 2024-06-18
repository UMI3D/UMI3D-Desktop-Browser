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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.runtimeBrowser.networkTest
{
    [System.Serializable]
    public class PingTest
    {
        public string hostName = "www.google.com";
        [Tooltip("Time in s.")]
        /// <summary>
        /// Duration of the ping process before declaring time out.
        /// </summary>
        public int timeout = 5;

        [Tooltip("Time in s.")]
        /// <summary>
        /// Time between tests.
        /// </summary>
        public int timeBetweenTests = 60 * 10;

        public IEnumerator TestConnection()
        {
            while (true)
            {
                yield return Ping();
                yield return new WaitForSeconds(timeBetweenTests);
            }
        }

        public IEnumerator Ping()
        {
            Ping ping = new Ping(IPFounder.GetIPV74(hostName).ToString());
            float _timeOut = timeout;
            while (!ping.isDone && _timeOut > 0)
            {
                _timeOut -= Time.deltaTime;
                yield return null;
            }

            if (ping.isDone)
            {
                Debug.Log("[NetworkTest] Ping time: " + ping.time + " ms");
            }
            else
            {
                Debug.LogError("[NetworkTest] Ping timed out, bad internet connection.");
            }

            ping.DestroyPing();
        }
    } 
}
