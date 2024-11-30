using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileThrowButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isThrowPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isThrowPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isThrowPressed = false;
    }

    public bool IsThrowPressed()
    {
        return isThrowPressed;
    }
}