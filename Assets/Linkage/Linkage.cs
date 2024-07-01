using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Linkage : MonoBehaviour
{
    public static Linkage Instance;

    public List<Joint> joints;
    public GameObject barPrefab;  // Prefab for a bar
    public GameObject halfBarPrefab;  // Prefab for a half bar
    float barThickness = 0.1f;  // Bar thickness

    private GameObject[] bars;
    private GameObject[] halfBars;

    private void Awake()
    {
        Instance = this;

        if (joints == null || joints.Count < 2)
        {
            Debug.LogError("Insufficient joints provided. At least 2 joints are required.");
            return;
        }

        // Initialize bars and halfBars arrays
        bars = new GameObject[joints.Count];
        halfBars = new GameObject[joints.Count * 2];

        // Create Bar objects
        for (int i = 0; i < joints.Count; i++)
        {
            // Create full bar
            GameObject barObj = Instantiate(barPrefab, transform);
            bars[i] = barObj;

            // Create half bars
            GameObject halfBarObj1 = Instantiate(halfBarPrefab, transform);
            GameObject halfBarObj2 = Instantiate(halfBarPrefab, transform);
            halfBars[i * 2] = halfBarObj1;
            halfBars[i * 2 + 1] = halfBarObj2;

            // Assign DraggableHalfBar components and their references
            HalfBar halfBar1 = halfBarObj1.GetComponent<HalfBar>();
            HalfBar halfBar2 = halfBarObj2.GetComponent<HalfBar>();

            // Ensure half bars are created properly with the necessary components
            if (halfBar1 != null && halfBar2 != null)
            {
                halfBar1.Initialize(joints[i], (i + 1 < joints.Count) ? joints[i + 1] : joints[0]);
                halfBar2.Initialize(joints[i], (i - 1 >= 0) ? joints[i - 1] : joints[joints.Count - 1]);
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
            bars[i].transform.localScale = new Vector3(distance, barThickness, barThickness);

            // Calculate and update half bar positions, rotations, and scales
            Vector3 nextDirection = (nextJoint.position - currentJoint.position) / 2;
            Vector3 prevDirection = (prevJoint.position - currentJoint.position) / 2;
            float nextHalfDistance = nextDirection.magnitude;
            float prevHalfDistance = prevDirection.magnitude;

            halfBars[i * 2].transform.position = currentJoint.position;
            halfBars[i * 2].transform.right = nextDirection;
            halfBars[i * 2].transform.localScale = new Vector3(nextHalfDistance, barThickness, barThickness);

            halfBars[i * 2 + 1].transform.position = currentJoint.position;
            halfBars[i * 2 + 1].transform.right = prevDirection;
            halfBars[i * 2 + 1].transform.localScale = new Vector3(prevHalfDistance, barThickness, barThickness);
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

    public Joint FindClosestJoint(Vector3 position)
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

        return closest;
    }
}
