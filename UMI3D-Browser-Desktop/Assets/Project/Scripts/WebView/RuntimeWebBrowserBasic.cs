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

using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Helper;

namespace BrowserDesktop
{
    public class RuntimeWebBrowserBasic : RawImageUwbClientInputHandler
    {
        void Awake()
        {
            TCPCommunicationLayer layer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();

            (int inPort, int outPort) = (WebViewFactory.Instance as WebViewFactory).GetPorts();

            layer.inPort = inPort;
            layer.outPort = outPort;

            browserClient.communicationLayer = layer;
            browserClient.CachePath = (WebViewFactory.Instance as WebViewFactory).GetCachePath();

            Debug.Log($"[ {nameof(RuntimeWebBrowserBasic)}] TCPCommunicationLayer created with ports { inPort }; { outPort } and cache path { browserClient.CachePath.FullName }");
        }
    }
}