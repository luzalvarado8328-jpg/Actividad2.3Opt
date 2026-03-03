using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        dragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    void OnMouseDrag()
    {
        if (dragging)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            transform.position = new Vector3(mouseWorldPos.x + offset.x, mouseWorldPos.y + offset.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
    }
}
