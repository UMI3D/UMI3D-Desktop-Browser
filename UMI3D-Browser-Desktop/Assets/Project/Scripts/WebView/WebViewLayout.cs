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

using System.Collections.Generic;
using UnityEngine;

namespace BrowserDesktop
{
    /// <summary>
    /// Class to switch <see cref="WebView"/> layout from 2d to 3d space.
    /// </summary>
    public class WebViewLayout : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private RenderMode layoutType = RenderMode.WorldSpace;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private List<RectTransform> containerToStrech = new();

        [SerializeField]
        private WebView webView;

        [SerializeField]
        private RectTransform rawImage;

        [SerializeField]
        private UnityEngine.UI.Text searchText;

        [SerializeField]
        List<RectTransformData> worldLayout = new();

        [SerializeField]
        List<RectTransformData> overlayLayout = new();

        [SerializeField]
        private GameObject fullScreenBtn, reduceBtn;

        #endregion

        #region Methods

        [ContextMenu("Init World Layout")]
        public void InitWorldLayout()
        {
            worldLayout.Clear();

            foreach (RectTransform rect in GetComponentsInChildren<RectTransform>(true))
            {
                worldLayout.Add(new RectTransformData(rect));
            }

            Debug.Log(worldLayout.Count + " RectTransform saved.");
        }

        [ContextMenu("Init Overlay Layout")]
        public void InitOverlayLayout()
        {
            overlayLayout.Clear();

            foreach (RectTransform rect in GetComponentsInChildren<RectTransform>(true))
            {
                overlayLayout.Add(new RectTransformData(rect));
            }

            Debug.Log(overlayLayout.Count + " RectTransform saved.");
        }

        [ContextMenu("Switch Layout Type")]
        public async void SwitchLayoutType()
        {
            List<RectTransformData> listToUse = null;

            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                listToUse = overlayLayout;

                searchText.fontSize = 14;

            } else
            {
                canvas.renderMode = RenderMode.WorldSpace;
                listToUse = worldLayout;

                searchText.fontSize = 8;
            }

            foreach (var entry in listToUse)
            {
                entry.SetRecTransform();
            }

            if (canvas.renderMode == RenderMode.WorldSpace)
                webView.SetWorldSpace();
            else
            {
                await UMI3DAsyncManager.Delay(100);
                SetTextureRatio();
            }

            fullScreenBtn.SetActive(canvas.renderMode == RenderMode.WorldSpace);
            reduceBtn.SetActive(canvas.renderMode != RenderMode.WorldSpace);
        }

        private Vector2 currentResolution = default;

        private void Update()
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (Mathf.Abs(currentResolution.x - Screen.width) > 1 || Mathf.Abs(currentResolution.y - Screen.height) > 1)
                {
                    SetTextureRatio();
                }
            }
        }

        [ContextMenu("Set Texture Ratio")]
        private void SetTextureRatio()
        {
            currentResolution = new Vector2(Screen.width, Screen.height);

            float ratio = webView.size.x / webView.size.y;
            float width = rawImage.rect.width;
            float height = rawImage.rect.height;

            float delta = -ratio * height + width; // Lydie's Formula (thank you very much)

            containerToStrech.ForEach(c =>
            {
                c.sizeDelta = new Vector2(c.sizeDelta.x - delta, c.sizeDelta.y);
            });
        }

        #endregion
    }

    [System.Serializable]
    public class RectTransformData
    {
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private Vector2 anchorMin, anchorMax, anchoredPosition, sizeDelta, pivot;
        [SerializeField]
        private Vector3 localScale;

        public RectTransformData(RectTransform rect)
        {
            this.rect = rect;

            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
            anchoredPosition = rect.anchoredPosition;
            sizeDelta = rect.sizeDelta;
            pivot = rect.pivot;
            localScale = rect.localScale;
        }

        public void SetRecTransform()
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.pivot = pivot;
            rect.localScale = localScale;
        }
    }
}