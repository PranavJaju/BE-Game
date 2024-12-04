using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileJumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public bool IsJumpPressed()
    {
        return isPressed;
    }
}
