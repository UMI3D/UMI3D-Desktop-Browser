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

using System.Drawing;
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;
//using VoltstroStudios.UnityWebBrowser;
//using VoltstroStudios.UnityWebBrowser.Input;

namespace BrowserDesktop
{

    public class WebView : AbstractUMI3DWebView
    {
        private RawImage texture;

        private WebBrowserUIBasic browser;

        int templateId = 0;

        protected virtual void Awake()
        {
            var template = (WebViewFactory.Instance as WebViewFactory).GetTemplate();

            if (template == null)
            {
                Debug.LogError("Impossible to load web view.");
            }

            templateId = template.GetInstanceID();

            GameObject canvas = Instantiate(template, transform);
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
            canvas.transform.localRotation = Quaternion.identity;
            canvas.transform.localPosition = Vector3.zero;

            browser = GetComponentInChildren<WebBrowserUIBasic>();

            texture = canvas.GetComponentInChildren<RawImage>();
        }

        protected override void OnCanInteractChanged(bool canInteract)
        {
            browser.disableMouseInputs = !canInteract;
            browser.disableKeyboardInputs = !canInteract;
        }

        protected override void OnSyncViewChanged(bool syncView)
        {
            Debug.Log("TODO");
        }

        protected override async void OnTextureSizeChanged(Vector2 size)
        {
            while (!browser.browserClient.ReadySignalReceived && !browser.browserClient.IsConnected)
            {
                await UMI3DAsyncManager.Yield();
            }

            await UMI3DAsyncManager.Yield();

            browser.browserClient.Resize(new VoltstroStudios.UnityWebBrowser.Shared.Resolution ((uint) size.x, (uint) size.y));
        }

        protected override async void OnUrlChanged(string url)
        {
            while (!browser.browserClient.ReadySignalReceived && !browser.browserClient.IsConnected)
            {
                await UMI3DAsyncManager.Yield();
            }

            await UMI3DAsyncManager.Yield();

            browser.browserClient.LoadUrl(url);
        }

        private void OnDestroy()
        {
            (WebViewFactory.Instance as WebViewFactory).ReleaseObject(templateId);
        }
    }
}