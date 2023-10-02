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
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BrowserDesktop
{
    [RequireComponent(typeof(RawImage))]
    public class WebViewButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnButtonPerformed = new();

        [SerializeField]
        private Texture hoverTexture;

        private Texture defaultTexture;

        private RawImage rawImage;

        private void Start()
        {
            rawImage = GetComponent<RawImage>();
            defaultTexture = rawImage.texture;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnButtonPerformed.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverTexture != null)
            {
                rawImage.texture = hoverTexture;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            rawImage.texture = defaultTexture;
        }
    }
}
