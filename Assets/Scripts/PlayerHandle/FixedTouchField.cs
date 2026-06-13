using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    public Vector2 TouchDist;

    private Vector2 pointerOld;
    private bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        pointerOld = eventData.position;
        TouchDist = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pressed) return;

        TouchDist = eventData.position - pointerOld;
        pointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        TouchDist = Vector2.zero;
    }
}
