using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableJoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform closestJoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Find the closest joint to the cursor
        closestJoint = FindClosestJoint(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (closestJoint != null)
        {
            // Convert screen position to world position
            Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
            worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane
            closestJoint.position = worldPoint;

            // Update the linkage
            LinkageManager.Instance.UpdateLinkage();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
        closestJoint = null;
    }

    private Transform FindClosestJoint(PointerEventData eventData)
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        // Convert screen position to world position
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        foreach (Transform joint in LinkageManager.Instance.joints)
        {
            float distance = Vector3.Distance(worldPoint, joint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = joint;
            }
        }

        return closest;
    }
}
