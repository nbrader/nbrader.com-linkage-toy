using System.Collections.Generic;
using UnityEngine;

public class LinkageManager : MonoBehaviour
{
    public static LinkageManager Instance;

    public List<Transform> joints;
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
            Transform currentJoint = joints[i];
            Transform nextJoint = joints[nextIndex];
            Transform prevJoint = joints[prevIndex];

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
}
