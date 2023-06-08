using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestPointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer handler enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer handler exit");
    }
}
