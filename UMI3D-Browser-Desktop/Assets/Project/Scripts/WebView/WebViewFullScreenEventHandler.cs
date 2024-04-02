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
using UnityEngine;
using UnityEngine.EventSystems;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace BrowserDesktop
{
    [RequireComponent(typeof(RectTransform))]
    public class WebViewFullScreenEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField]
        private WebView webview = null;

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private RectTransform rectTransform = null;

        Vector3[] corners = new Vector3[4];

        Vector2 pointerPos = Vector3.zero;

        bool isHover = false;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!ProcessEvent())
                return;

            SendClick(eventData, MouseEventType.Down);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!ProcessEvent())
                return;

            ComputeCorners(eventData.pressEventCamera);

            if (webview.canInteract)
            {
                webview.browser.disableMouseInputs = true;
            }

            isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!ProcessEvent())
                return;

            if (webview.canInteract)
            {
                webview.browser.disableMouseInputs = false;
            }

            isHover = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!ProcessEvent())
                return;

            SendClick(eventData, MouseEventType.Up);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!ProcessEvent())
                return;

            ConvertPointerToLocalSpace(eventData);
            webview.browser.browserClient.SendMouseMove(pointerPos);
        }

        private MouseClickType GetClick(PointerEventData eventData)
        {
            return eventData.button switch
            {
                PointerEventData.InputButton.Left => MouseClickType.Left,
                PointerEventData.InputButton.Right => MouseClickType.Right,
                PointerEventData.InputButton.Middle => MouseClickType.Middle,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool ProcessEvent() => canvas.renderMode == RenderMode.ScreenSpaceOverlay;

        Vector3 topLeft, topRight, bottomLeft;

        private void ComputeCorners(Camera cam)
        {
            rectTransform.GetWorldCorners(corners);

            bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            topLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[1]);
            topRight = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
        }

        private void ConvertPointerToLocalSpace(PointerEventData eventData)
        {
            Vector3 localVector = (Vector3)eventData.position - topLeft;

            float widthRatio = Vector3.Project(localVector, topRight - topLeft).magnitude / Vector3.Distance(topLeft, topRight);
            float heightRatio = Vector3.Project(localVector, bottomLeft - topLeft).magnitude / Vector3.Distance(topLeft, bottomLeft);

            pointerPos.x = webview.textureSize.x * widthRatio;
            pointerPos.y = webview.textureSize.y * heightRatio;
        }

        private void SendClick(PointerEventData eventData, MouseEventType type)
        {
            ComputeCorners(eventData.pressEventCamera);
            ConvertPointerToLocalSpace(eventData);
            webview.browser.browserClient.SendMouseClick(pointerPos, 1, GetClick(eventData), type);
        }

        float scroll;

        private void Update()
        {
            if (!ProcessEvent() || !webview.browser.browserClient.IsConnected)
                return;

            scroll = webview.browser.inputHandler.GetScroll();
            scroll *= webview.browser.browserClient.BrowserTexture?.height ?? 0;

            if (scroll != 0)
                webview.browser.browserClient.SendMouseScroll(pointerPos, (int)scroll);

            if (!webview.browser.GetMousePosition(out Vector2 _) && isHover)
            {
                if (!webview.browser.disableKeyboardInputs)
                {
                    //Input
                    WindowsKey[] keysDown = webview.browser.inputHandler.GetDownKeys();
                    WindowsKey[] keysUp = webview.browser.inputHandler.GetUpKeys();
                    string inputBuffer = webview.browser.inputHandler.GetFrameInputBuffer();

                    if (keysDown.Length > 0 || keysUp.Length > 0 || inputBuffer.Length > 0)
                        webview.browser.browserClient.SendKeyboardControls(keysDown, keysUp, inputBuffer.ToCharArray());
                }
            }
        }
    }
}