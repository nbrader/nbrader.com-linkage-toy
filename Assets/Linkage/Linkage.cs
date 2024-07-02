using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum LinkagePartType
{
    Joint = 0,
    HalfBar,
}

public class Linkage : MonoBehaviour
{
    public List<Joint> joints;
    public GameObject barPrefab;  // Prefab for a bar
    public GameObject halfBarPrefab;  // Prefab for a half bar
    float barVisibleThickness = 0.1f;
    float barColliderThickness = 10f;

    private GameObject[] bars;
    private HalfBar[] halfBars;

    private void Awake()
    {
        if (joints == null || joints.Count != 4)
        {
            Debug.LogError("4 joints are required.");
            return;
        }

        foreach (var joint in joints)
        {
            joint.parentLinkage = this;
        }

        // Initialize bars and halfBars arrays
        bars = new GameObject[joints.Count];
        halfBars = new HalfBar[joints.Count * 2];

        // Create Bar objects
        for (int i = 0; i < joints.Count; i++)
        {
            // Create full bar
            GameObject barObj = Instantiate(barPrefab, transform);
            bars[i] = barObj;

            // Create half bars
            GameObject halfBarObj1 = Instantiate(halfBarPrefab, transform);
            GameObject halfBarObj2 = Instantiate(halfBarPrefab, transform);
            halfBars[i * 2] = halfBarObj1.GetComponent<HalfBar>();
            halfBars[i * 2 + 1] = halfBarObj2.GetComponent<HalfBar>();

            // Assign DraggableHalfBar components and their references
            HalfBar halfBar1 = halfBarObj1.GetComponent<HalfBar>();
            HalfBar halfBar2 = halfBarObj2.GetComponent<HalfBar>();

            // Ensure half bars are created properly with the necessary components
            if (halfBar1 != null && halfBar2 != null)
            {
                halfBar1.Initialize(this, joints[i], (i + 1 < joints.Count) ? joints[i + 1] : joints[0]);
                halfBar2.Initialize(this, joints[i], (i - 1 >= 0) ? joints[i - 1] : joints[joints.Count - 1]);
            }
            else
            {
                Debug.LogError("DraggableHalfBar component missing on half bar prefab.");
            }
        }

        UpdateLinkage();
    }

    public void UpdateLinkage()
    {
        if (joints == null || joints.Count < 2) return;

        for (int i = 0; i < joints.Count; i++)
        {
            int nextIndex = (i + 1) % joints.Count;
            int prevIndex = (i - 1 + joints.Count) % joints.Count;
            Transform currentJoint = joints[i].transform;
            Transform nextJoint = joints[nextIndex].transform;
            Transform prevJoint = joints[prevIndex].transform;

            // Calculate the position and scale for the full bar
            Vector3 direction = nextJoint.position - currentJoint.position;
            float distance = direction.magnitude;

            // Update full bar position, rotation, and scale
            bars[i].transform.position = currentJoint.position;
            bars[i].transform.right = direction;
            bars[i].transform.localScale = new Vector3(distance, barVisibleThickness, 1);
            bars[i].GetComponent<BoxCollider2D>().size = new Vector2(1, barColliderThickness);

            // Calculate and update half bar positions, rotations, and scales
            Vector3 nextDirection = (nextJoint.position - currentJoint.position) / 2;
            Vector3 prevDirection = (prevJoint.position - currentJoint.position) / 2;
            float nextHalfDistance = nextDirection.magnitude;
            float prevHalfDistance = prevDirection.magnitude;

            halfBars[i * 2].transform.position = currentJoint.position;
            halfBars[i * 2].transform.right = nextDirection;
            halfBars[i * 2].transform.localScale = new Vector3(nextHalfDistance, barVisibleThickness, 1);
            halfBars[i * 2].GetComponent<BoxCollider2D>().size = new Vector2(1, barColliderThickness);

            halfBars[i * 2 + 1].transform.position = currentJoint.position;
            halfBars[i * 2 + 1].transform.right = prevDirection;
            halfBars[i * 2 + 1].transform.localScale = new Vector3(prevHalfDistance, barVisibleThickness, 1);
            halfBars[i * 2 + 1].GetComponent<BoxCollider2D>().size = new Vector2(1, barColliderThickness);
        }
    }

    public void MoveJoint(Joint joint, Vector3 position)
    {
        if (joint != null)
        {
            joint.transform.position = position;
            UpdateLinkage();
        }
    }

    public (Joint closestJoint, float closestDistance) FindClosestJoint(Vector3 position)
    {
        Joint closest = null;
        float minDistance = float.MaxValue;

        foreach (Joint joint in joints)
        {
            float distance = Vector3.Distance(position, joint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = joint;
            }
        }

        return (closestJoint : closest, closestDistance : minDistance);
    }

    public (HalfBar closestHalfBar, float closestDistance)? FindClosestHalfBar(Vector3 inputPosition)
    {
        Edge3D[] edges = halfBars.Select(bar => new Edge3D("", bar.pivotJoint.transform.position, (bar.pivotJoint.transform.position + bar.oppositeJoint.transform.position)/2)).ToArray();

        // Check _EDGES_ for nearest point
        Vector3 nearestPointOnEdge = Vector3.zero;
        float nearestPointDistanceOnEdge = float.PositiveInfinity;
        bool nearestPointFoundOnEdge = false;
        int? nearestEdgeIndex = null;
        for (int i = 0; i < edges.Count(); i++)
        {
            Edge3D edge = edges[i];
            Geometry.BasisDir projectionDir = Geometry.BasisDir.Y;
            Maybe<Geometry.BasisDir> maybePreferredProjectionDir = edge.GetBestProjectionDir();
            if (maybePreferredProjectionDir.exists)
            {
                projectionDir = maybePreferredProjectionDir.value;
            }
            else
            {
                Debug.LogError("Degenerate edge found.");
            }

            Vector3 inputPointPosToEdge = Geometry.NearestPointOfLineFromPoints(inputPosition, edge.p1, edge.p2);
            Interval projectedInterval = edge.ProjectToDir(projectionDir);

            float inputPointPosToDir = Geometry.ProjectToDir(inputPointPosToEdge, projectionDir);

            bool edgeContainsInputPointPosToEdge = projectedInterval.Contains(inputPointPosToDir);

            if (edgeContainsInputPointPosToEdge)
            {
                float thisPointDistance = (inputPointPosToEdge - inputPosition).magnitude;

                if (nearestPointFoundOnEdge)
                {
                    if (thisPointDistance < nearestPointDistanceOnEdge)
                    {
                        nearestPointOnEdge = inputPointPosToEdge;
                        nearestPointDistanceOnEdge = thisPointDistance;
                        nearestEdgeIndex = i;
                    }
                }
                else
                {
                    nearestPointFoundOnEdge = true;
                    nearestPointOnEdge = inputPointPosToEdge;
                    nearestPointDistanceOnEdge = thisPointDistance;
                    nearestEdgeIndex = i;
                }
            }
        }

        if (nearestEdgeIndex.HasValue)
        {
            return (closestHalfBar: halfBars[nearestEdgeIndex.Value], closestDistance: nearestPointDistanceOnEdge);
        }
        else
        {
            return null;
        }
    }

    private Joint closestJoint;
    private HalfBar closestHalfBar;
    private LinkagePartType latestDraggedPartType = LinkagePartType.Joint;
    public void OnBeginDrag(PointerEventData eventData)
    {
        var closestJointData = FindClosestJoint(ScreenToWorldPoint(eventData.position));
        closestJoint = closestJointData.closestJoint;
        var closestJointDistance = closestJointData.closestDistance;

        var closestHalfBarData = FindClosestHalfBar(ScreenToWorldPoint(eventData.position));

        if (closestHalfBarData == null)
        {
            latestDraggedPartType = LinkagePartType.Joint;
            OnBeginDragJoint(eventData);
        }
        else
        {
            closestHalfBar = closestHalfBarData.Value.closestHalfBar;
            var closestHalfBarDistance = closestHalfBarData.Value.closestDistance;

            if (closestJointDistance <= closestHalfBarDistance)
            {
                latestDraggedPartType = LinkagePartType.Joint;
                OnBeginDragJoint(eventData);
            }
            else
            {
                latestDraggedPartType = LinkagePartType.HalfBar;
                OnBeginDragHalfBar(eventData);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (latestDraggedPartType == LinkagePartType.Joint)
        {
            OnDragJoint(eventData);
        }
        else
        {
            OnDragHalfBar(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (latestDraggedPartType == LinkagePartType.Joint)
        {
            OnEndDragJoint(eventData);
        }
        else
        {
            OnEndDragHalfBar(eventData);
        }
    }

    public void OnBeginDragJoint(PointerEventData eventData)
    {

    }

    public void OnDragJoint(PointerEventData eventData)
    {
        if (closestJoint != null)
        {
            // Convert screen position to world position
            Vector3 worldPoint = ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane

            // Move the joint via the LinkageManager
            MoveJoint(closestJoint, worldPoint);
        }
    }

    public void OnEndDragJoint(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
        closestJoint = null;
    }

    public void OnBeginDragHalfBar(PointerEventData eventData)
    {

    }

    public void OnDragHalfBar(PointerEventData eventData)
    {
        if (closestHalfBar != null)
        {
            // Convert screen position to world position
            Vector3 worldPoint = ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0; // Ensure the joint stays on the z = 0 plane

            // Calculate the direction from the joint to the new mouse position
            Vector3 direction = (worldPoint - closestHalfBar.pivotJoint.transform.position).normalized;

            // Maintain the distance between the joint and the opposite end
            closestHalfBar.oppositeJoint.transform.position = closestHalfBar.pivotJoint.transform.position + direction * Vector3.Distance(closestHalfBar.pivotJoint.transform.position, closestHalfBar.oppositeJoint.transform.position);

            UpdateLinkage();
        }
    }

    public void OnEndDragHalfBar(PointerEventData eventData)
    {
        // Optional: Handle end drag logic if needed
        closestHalfBar = null;
    }

    private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
