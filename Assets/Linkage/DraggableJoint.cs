using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableJoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private DraggableJoint closestJoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Find the closest joint to the cursor
        closestJoint = LinkageManager.Instance.FindClosestJoint(ScreenToWorldPoint(eventData.position));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (closestJoint != null)
        {
            // Convert screen position to world position
            Vector3 worldPoint = ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane

            // Move the joint via the LinkageManager
            LinkageManager.Instance.MoveJoint(closestJoint, worldPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
        closestJoint = null;
    }

    private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
