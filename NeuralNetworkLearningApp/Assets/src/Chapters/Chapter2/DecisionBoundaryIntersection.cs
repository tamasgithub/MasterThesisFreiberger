using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionBoundaryIntersection : MonoBehaviour
{
    private bool dragging;
    private Camera cam;
    public DecisionBoundary[] decisionBoundaries;
    private CoordinateSystem coordinateSystem;
    // Start is called before the first frame update
    void Start()
    {
        coordinateSystem = transform.GetComponentInParent<CoordinateSystem>();
        cam = Camera.main;
        foreach (DecisionBoundary boundary in decisionBoundaries)
        {
            boundary.TransformIntoRay(coordinateSystem.LocalToSystemPoint(transform.localPosition));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = -1;
            transform.position = newPos;
            newPos = transform.localPosition;
            newPos.x = Mathf.Clamp(newPos.x, -0.5f, 0.5f);
            newPos.y = Mathf.Clamp(newPos.y, -0.5f, 0.5f);
            transform.localPosition = newPos;
            foreach (DecisionBoundary boundary in decisionBoundaries)
            {
                boundary.TransformIntoRay(coordinateSystem.LocalToSystemPoint(transform.localPosition));
            }
        }
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }
}
