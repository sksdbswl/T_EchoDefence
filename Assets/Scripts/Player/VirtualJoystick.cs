using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 100f;

    private Vector2 inputVector;
    private Vector2 startPosition;

    public Vector2 Input => inputVector;

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position; // 터치 시작 위치 = 원점
        handle.position = startPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = eventData.position - startPosition;
        inputVector = Vector2.ClampMagnitude(offset / handleRange, 1f);
        inputVector = new Vector2(inputVector.x, 0f);
        handle.position = startPosition + inputVector * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.position = startPosition; // 원래 자리로 복귀
    }
}