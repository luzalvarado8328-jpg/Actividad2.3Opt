using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(mouseWorldPos.x + offset.x, mouseWorldPos.y + offset.y, transform.position.z);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Se suelta el objeto
    }
}
