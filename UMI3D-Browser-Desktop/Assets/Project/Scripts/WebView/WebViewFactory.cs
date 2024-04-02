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
using System.Threading.Tasks;
using umi3d.cdk;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Helper;
using System.Net.Sockets;
using System.Net;

using Process = System.Diagnostics.Process;

namespace BrowserDesktop
{
    public class WebViewFactory : AbstractWebViewFactory
    {
        #region

        /// <summary>
        /// Webview template.
        /// </summary>
        public GameObject template = null;

        /// <summary>
        /// Last time a webview was created.
        /// </summary>
        private static float lastTimeWebViewCreated = 0;

        /// <summary>
        /// Name of process launched in background by webviews.
        /// </summary>
        private const string webEngineProcessName = "UnityWebBrowser.Engine.Cef";

        /// <summary>
        /// Delay in seconds between web view creation.
        /// </summary>
        private readonly float creationDelay = 3f;

        #endregion

        public override async Task<AbstractUMI3DWebView> CreateWebView()
        {
            while (lastTimeWebViewCreated != 0 && lastTimeWebViewCreated + creationDelay > Time.time)
            {
                await UMI3DAsyncManager.Yield();
            }

            lock (this)
            {
                lastTimeWebViewCreated = Time.time;

                GameObject go = Instantiate(template);
                WebView view = go.GetComponent<WebView>();

                return view;
            }
        }

        /// <summary>
        /// Returns unique in and out ports for WebViews.
        /// </summary>
        /// <returns></returns>
        public (int, int) GetPorts()
        {
            return (FindFreeTcpPort(), FindFreeTcpPort());
        }

        /// <summary>
        /// Returns a webview folder path.
        /// </summary>
        /// <returns></returns>
        public System.IO.FileInfo GetCachePath() => new (WebBrowserUtils.GetAdditionFilesDirectory() + "/cache-path/" + System.Guid.NewGuid().ToString());

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Process[] processes = Process.GetProcessesByName(webEngineProcessName);

            Debug.Log($"{ nameof(WebViewFactory) } : process to kill " + processes.Length);

#if UNITY_EDITOR
            foreach (Process process in Process.GetProcessesByName("UnityWebBrowser.Engine.Cef"))
            {
                process.Kill();
            }
#endif
        }

        /// <summary>
        /// Default port buffer used if <see cref="FindFreeTcpPort"/> fails.
        /// </summary>
        private static int basePort = 55000;

        /// <summary>
        /// Find a tcp port open.
        /// </summary>
        /// <returns></returns>
        static int FindFreeTcpPort()
        {
            int port;
            TcpListener l = new (IPAddress.Loopback, 0);
            try
            {
                l.Start();
                port = ((IPEndPoint)l.LocalEndpoint).Port;
            }
            catch (Exception e)
            {
                Debug.LogError("Error during webview tcp port generation." + e.Message);

                port = basePort++;
            }
            finally
            {
                l.Stop();
            }

            return port;
        }
    }
}