using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Script for Pickup Button
public class MobilePickupButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPickupPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPickupPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPickupPressed = false;
    }

    public bool IsPickupPressed()
    {
        return isPickupPressed;
    }
}

