using System.Collections;
using System.Collections.Generic;
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
