using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableJoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Convert screen position to world position
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        offset = transform.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen position to world position
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint) + offset;
        worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane
        transform.position = worldPoint;

        // Update the linkage
        LinkageManager.Instance.UpdateLinkage();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
    }
}
