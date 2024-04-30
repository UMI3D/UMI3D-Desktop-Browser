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
using System.Collections;
using System.Text.RegularExpressions;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BrowserDesktop
{
    public class WebView : AbstractUMI3DWebView, IPointerEnterHandler, IPointerExitHandler
    {
        public static bool IsWebViewFocused { get; private set; } = false;

        #region Fields

        [SerializeField]
        private RectTransform textureTransform = null;

        [SerializeField]
        public RuntimeWebBrowserBasic browser = null;

        [SerializeField]
        private RectTransform container = null;

        [Header("Bottom bar")]

        [SerializeField]
        private RectTransform bottomBarContainer = null;

        [SerializeField]
        private RectTransform nextRectTransform = null;

        [SerializeField]
        private RectTransform previousRectTransform = null;

        [SerializeField]
        private RectTransform homeRectTransform = null;

        [SerializeField]
        private RectTransform synchronizeRectTransform = null;

        [SerializeField]
        private RectTransform fullScreenRectTransform = null;

        [SerializeField]
        private RectTransform reduceScreenRectTransform = null;

        [Header("Top bar")]
        [SerializeField]
        private RectTransform topBarContainer = null;

        [SerializeField]
        private RectTransform urlRectTransform = null;

        [SerializeField]
        private RectTransform searchRectTransform = null;

        [Space]

        [SerializeField]
        Canvas canvas = null;

        [SerializeField]
        private InputField urlText = null;

        [Space]
        [SerializeField, Tooltip("Delay to send current url and scroll offset when user synchronizes his content. In seconds")]
        float synchronizationDelay = 2f;

        [SerializeField, Tooltip("A feedback to show user he's currently sharing its content")]
        GameObject syncFeedback;

        private string previousUrl;

        private UMI3DWebViewDto dto;

        private bool isSynchronizing;
        private bool IsSynchronizing
        {
            get => isSynchronizing;
            set
            {
                isSynchronizing = value;
                syncFeedback.SetActive(value);
            }
        }

        private int currentScrollPosition;

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            canvas.worldCamera = Camera.main;
            canvas.transform.localRotation = Quaternion.identity;
            canvas.transform.localPosition = Vector3.zero;

            canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 3;
            canvas.sortingOrder = 1;

            browser.browserClient.OnUrlChanged += OnUrlLoaded;

            autoRefreshCoroutine = StartCoroutine(AutoRefresh());

            synchronizeRectTransform.gameObject.SetActive(false);

            StartCoroutine(SynchronizationCoroutine());

            IsSynchronizing = false;
        }

        /// <summary>
        /// If user is synchronizing his view, send his current url and scroll offset.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SynchronizationCoroutine()
        {
            var wait = new WaitForSeconds(synchronizationDelay);

            while (true)
            {
                if (isAdmin && IsSynchronizing && browser.browserClient.ReadySignalReceived && browser.browserClient.IsConnected)
                {
                    int scroll = (int) browser.browserClient.GetScrollPosition().y;

                    if (currentScrollPosition != scroll)
                    {
                        currentScrollPosition = scroll;

                        var request = new WebViewUrlChangedRequestDto
                        {
                            url = previousUrl,
                            webViewId = dto.id,
                            scrollOffset = new() { X = 0, Y = currentScrollPosition }
                        };

                        UMI3DClientServer.SendRequest(request, true);
                    }
                }

                yield return wait;
            }
        }

        /// <summary>
        /// Called when web browser loads a new url.
        /// </summary>
        /// <param name="url"></param>
        private void OnUrlLoaded(string url)
        {
            if (autoRefreshCoroutine is not null)
                StopCoroutine(autoRefreshCoroutine);

            autoRefreshCoroutine = StartCoroutine(AutoRefresh());

            if (url == previousUrl)
            {
                return;
            }

            //Check if url is authorized.
            if (CheckIfUrlValid(url))
            {
                if (url.StartsWith("data:"))
                {
                    string title = Regex.Match(url, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                    urlText.text = "HTML data:Title : " + title;
                }
                else
                {
                    previousUrl = url;
                    urlText.text = url;

                    // If true send to server url loaded.

                    var request = new WebViewUrlChangedRequestDto
                    {
                        url = url,
                        webViewId = dto.id
                    };

                    UMI3DClientServer.SendRequest(request, true);
                }
            }
            else
            {
                LoadNotAccessibleWebPage(url);
            }
        }

        public override void Init(UMI3DWebViewDto dto)
        {
            base.Init(dto);

            this.dto = dto;
        }

        protected override void OnCanInteractChanged(bool canInteract)
        {
            browser.disableMouseInputs = !canInteract;
            browser.disableKeyboardInputs = !canInteract;

            bottomBarContainer.gameObject.SetActive(canInteract);
            topBarContainer.gameObject.SetActive(canInteract);
        }

        protected override void OnSizeChanged(Vector2 size)
        {
            try
            {
                container.localScale = new Vector3(size.x, size.y, 1);

                Vector3[] corners = new Vector3[4];

                textureTransform.GetWorldCorners(corners);

                bottomBarContainer.position = (corners[0] + corners[3]) / 2f;
                topBarContainer.position = (corners[1] + corners[2]) / 2f;

                topBarContainer.localScale = new Vector3(topBarContainer.localScale.x,
                    topBarContainer.localScale.y / container.localScale.y, topBarContainer.localScale.z);

                bottomBarContainer.localScale = new Vector3(bottomBarContainer.localScale.x,
                    bottomBarContainer.localScale.y / container.localScale.y, bottomBarContainer.localScale.z);

                urlRectTransform.localScale = new Vector3(urlRectTransform.localScale.x / container.localScale.x,
                    urlRectTransform.localScale.y, urlRectTransform.localScale.z);

                searchRectTransform.localScale = new Vector3(searchRectTransform.localScale.x / container.localScale.x,
                    searchRectTransform.localScale.y, searchRectTransform.localScale.z);

                nextRectTransform.localScale = new Vector3(nextRectTransform.localScale.x / container.localScale.x,
                    nextRectTransform.localScale.y, nextRectTransform.localScale.z);

                previousRectTransform.localScale = new Vector3(previousRectTransform.localScale.x / container.localScale.x,
                    previousRectTransform.localScale.y, previousRectTransform.localScale.z);

                homeRectTransform.localScale = new Vector3(homeRectTransform.localScale.x / container.localScale.x,
                    homeRectTransform.localScale.y, homeRectTransform.localScale.z);

                synchronizeRectTransform.localScale = new Vector3(synchronizeRectTransform.localScale.x / container.localScale.x,
                    synchronizeRectTransform.localScale.y, synchronizeRectTransform.localScale.z);

                fullScreenRectTransform.localScale = new Vector3(fullScreenRectTransform.localScale.x / container.localScale.x,
                    fullScreenRectTransform.localScale.y, fullScreenRectTransform.localScale.z);

                reduceScreenRectTransform.localScale = new Vector3(reduceScreenRectTransform.localScale.x / container.localScale.x,
                    reduceScreenRectTransform.localScale.y, reduceScreenRectTransform.localScale.z);

            } catch (Exception e)
            {
                UMI3DLogger.LogException(e, DebugScope.CDK);
            }
        }

        protected override void OnAdminStatusChanged(bool status)
        {
            synchronizeRectTransform.gameObject.SetActive(status);
        }

        protected override async void OnTextureSizeChanged(Vector2 size)
        {
            while (!browser.browserClient.ReadySignalReceived && !browser.browserClient.IsConnected)
            {
                await UMI3DAsyncManager.Yield();
            }

            await UMI3DAsyncManager.Yield();

            try
            {
                browser.browserClient.Resize(new VoltstroStudios.UnityWebBrowser.Shared.Resolution((uint)size.x, (uint)size.y));
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to resize WebView.");
                Debug.LogException(ex);
            }
        }

        protected override async void OnUrlChanged(string url)
        {
            IsSynchronizing = false;

            while (!browser.browserClient.ReadySignalReceived && !browser.browserClient.IsConnected)
            {
                await UMI3DAsyncManager.Yield();
            }

            await UMI3DAsyncManager.Yield();

            try
            {
                if (CheckIfUrlValid(url))
                    browser.browserClient.LoadUrl(url);
                else
                    LoadNotAccessibleWebPage(url);
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to load url " + url);
                Debug.LogException(ex);

                await UMI3DAsyncManager.Delay(5000);

                browser.browserClient.LoadUrl(url);
            }
        }

        protected override async void OnScrollOffsetChanged(Vector2 scroll)
        {
            while (!browser.browserClient.ReadySignalReceived && !browser.browserClient.IsConnected)
            {
                await UMI3DAsyncManager.Yield();
            }

            await UMI3DAsyncManager.Yield();

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    browser.browserClient.SendMouseScroll(Vector2.one, (int)(browser.browserClient.GetScrollPosition().y - scroll.y));
                    await UMI3DAsyncManager.Yield();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to set offset url " + url);
                Debug.LogException(ex);

                await UMI3DAsyncManager.Delay(5000);

                browser.browserClient.LoadUrl(url);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsWebViewFocused = true;

            BaseKeyInteraction.IsEditingTextField = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsWebViewFocused = false;

            BaseKeyInteraction.IsEditingTextField = false;
        }

        public void SetWorldSpace()
        {
            OnSizeChanged(size);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Check if <paramref name="url"/> is allowed regarding whitelist/blacklist parameter.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool CheckIfUrlValid(string url)
        {
            if (url.StartsWith("data:"))
                return true;

            try
            {
                Uri uriToCheck = new(url);

                Debug.Log(url + " " + uriToCheck.Host);

                if (useBlackList)
                {
                    if (blackList.Contains(uriToCheck.Host))
                    {
                        UMI3DLogger.LogError("Trying to load a blacklisted url " + url, DebugScope.Networking);

                        return false;
                    }
                }

                if (useWhiteList)
                {
                    if (!whiteList.Contains(uriToCheck.Host))
                    {
                        UMI3DLogger.LogError("Trying to load an url not whitelisted " + url, DebugScope.Networking);

                        return false;
                    }
                }
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Displays a web page explaining that a <paramref name="notAuthorizedUrl"/> was loaded but it was not allowed.
        /// </summary>
        /// <param name="notAuthorizedUrl"></param>
        public void LoadNotAccessibleWebPage(string notAuthorizedUrl)
        {
            browser.LoadHtml("<html><head><meta charset=\"utf-8\"><title>Not authorized</title></head><body>Impossible to load " + notAuthorizedUrl + ", this url is either blacklisted or not white listed. Contact your administrator.</body></html>");
        }

        private Coroutine autoRefreshCoroutine;

        /// <summary>
        /// Auto refresh to fix a time out bug.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoRefresh()
        {
            var wait = new WaitForSeconds(60 * 4);

            while (true)
            {
                yield return wait;
                browser.Refresh();
            }
        }

        public void ToggleSynchronization()
        {
            IsSynchronizing = !IsSynchronizing;

            var request = new WebViewSynchronizationRequestDto
            {
                webViewId = dto.id
            };

            UMI3DClientServer.SendRequest(request, true);
        }

        #endregion
    }
}