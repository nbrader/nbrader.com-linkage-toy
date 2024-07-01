using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableHalfBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private DraggableJoint pivotJoint; // The pivot joint
    private DraggableJoint oppositeJoint; // The opposite end of the half bar

    private Vector3 initialOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Find the closest joint to the cursor
        pivotJoint = LinkageManager.Instance.FindClosestJoint(ScreenToWorldPoint(eventData.position));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pivotJoint != null && oppositeJoint != null)
        {
            // Convert screen position to world position
            Vector3 worldPoint = ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane

            // Calculate the direction from the joint to the new mouse position
            Vector3 direction = (worldPoint - pivotJoint.transform.position).normalized;

            // Maintain the distance between the joint and the opposite end
            oppositeJoint.transform.position = pivotJoint.transform.position + direction * Vector3.Distance(pivotJoint.transform.position, oppositeJoint.transform.position);

            LinkageManager.Instance.UpdateLinkage();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
        pivotJoint = null;
        oppositeJoint = null;
    }

    private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }

    public void Initialize(DraggableJoint pivotJoint, DraggableJoint oppositeJoint)
    {
        this.pivotJoint = pivotJoint;
        this.oppositeJoint = oppositeJoint;
    }
}
