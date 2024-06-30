using System.Collections.Generic;
using UnityEngine;

public class LinkageManager : MonoBehaviour
{
    public static LinkageManager Instance;

    public List<Transform> joints;
    public GameObject lineRendererPrefab;  // Prefab with a LineRenderer component
    public float lineThickness = 0.2f;  // Line thickness to be set

    private LineRenderer[] lineRenderers;

    private void Awake()
    {
        Instance = this;

        if (joints == null || joints.Count < 2)
        {
            Debug.LogError("Insufficient joints provided. At least 2 joints are required.");
            return;
        }

        // Initialize lineRenderers array
        lineRenderers = new LineRenderer[joints.Count];

        // Create LineRenderer objects
        for (int i = 0; i < joints.Count; i++)
        {
            GameObject lineObj = Instantiate(lineRendererPrefab, transform);
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
            lineRenderer.startWidth = lineThickness;
            lineRenderer.endWidth = lineThickness;
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
            lineRenderers[i] = lineRenderer;
        }

        UpdateLinkage();
    }

    public void UpdateLinkage()
    {
        if (joints == null || joints.Count < 2) return;

        for (int i = 0; i < joints.Count; i++)
        {
            int nextIndex = (i + 1) % joints.Count;
            lineRenderers[i].SetPosition(0, joints[i].position);
            lineRenderers[i].SetPosition(1, joints[nextIndex].position);
        }
    }
}
