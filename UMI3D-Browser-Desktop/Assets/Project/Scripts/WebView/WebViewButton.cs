using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WebViewButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent OnButtonPerformed = new();

    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonPerformed.Invoke();
    }
}
