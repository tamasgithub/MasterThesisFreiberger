using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DecisionBoundary : MonoBehaviour
{
    public CoordinateSystem coordSys;
    public Transform functionUI;
    public GameObject arrows;
    private Vector2 firstAnchor = new Vector2(0,0);
    private Vector2 secondAnchor = new Vector2(1, 1);
    private float[] coeffs = new float[] { 1f, -1f, 0f };
    private Vector4 fullVisibleRanges;
    private Vector4 visibleRanges;

    private void Start()
    {
        float halfEdge = coordSys.transform.localScale.x / 2f;
        float offset = coordSys.transform.localScale.x * coordSys.offset;
        fullVisibleRanges = new Vector4(coordSys.transform.position.x - (halfEdge - offset), coordSys.transform.position.x + halfEdge,
            coordSys.transform.position.y - (halfEdge - offset), coordSys.transform.position.y + halfEdge);
        ResetVisibleRanges();
        arrows = Instantiate(arrows);
        arrows.transform.parent = transform;
        arrows.transform.localPosition = Vector3.back * 3;
        DrawDecisionBoundary();
        if (functionUI != null)
        {
            UpdateFunctionUI();
        }
        
    }
    public void SetCoordSystem(CoordinateSystem coordSystem)
    {
        coordSys = coordSystem;
    }

    public void SetFirstAnchor(Vector2 anchor)
    {
        firstAnchor = anchor;
    }

    public void SetSecondAnchor(Vector2 anchor, bool isRay)
    {
        secondAnchor = anchor;  

        // calculate the coefficients of the straight line a x1 + b x2 + c = 0 using the two given points
        Vector2 direction = secondAnchor - firstAnchor;
        Vector2 normal = new Vector2(-direction.y, direction.x).normalized;
        if (Vector3.Dot(normal, firstAnchor) < 0)
        {
            normal *= -1;
        }
        coeffs[0] = normal.x;
        coeffs[1] = normal.y;
        coeffs[2] = -(normal.x * firstAnchor.x + normal.y * firstAnchor.y);
        DrawDecisionBoundary();
        if (functionUI != null)
        {
            UpdateFunctionUI();
        }
        if (isRay)
        {
            UpdateRayVisibleRange();
        }
    }
    private void Update()
    {
    }

    public float[] GetCoefficients()
    {
        return coeffs;
    }

    public void SetCoefficient(int index, float coefficient)
    {
        coeffs[index] = coefficient;
        DrawDecisionBoundary();
    }

    public void TransformIntoRay(Vector2 initialPoint)
    {
        print(initialPoint);
        Vector2 direction = secondAnchor - firstAnchor;
        SetFirstAnchor(initialPoint);
        SetSecondAnchor(initialPoint + direction, true);
    }

    private void DrawDecisionBoundary()
    {
        foreach (Renderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = true;
        }

        List<Vector2> intersections = CalculateIntersections((1 - coordSys.offset) / (1 - 2 * coordSys.offset));
        if (intersections.Count < 2)
        {
            foreach (Renderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.enabled = false;
            }
            return;
        }

        Vector3 intersection1 = coordSys.SystemToLocalPoint(intersections[0]);
        Vector3 intersection2 = coordSys.SystemToLocalPoint(intersections[1]);
        if (intersection1.x > intersection2.x)
        {
            Vector2 temp = intersection1;
            intersection1 = intersection2;
            intersection2 = temp;
        }
        Vector2 centerOfLine = new Vector3((intersection1.x + intersection2.x) / 2f, (intersection1.y + intersection2.y) / 2f, -1);
        transform.localPosition = centerOfLine;
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, new Vector2(coeffs[0], coeffs[1]));
        coordSys.HighlightWronglyClassified();
    }

    // intersections of the line with the axis-aligned square from 0|0 to Ssl|Ssl
    private List<Vector2> CalculateIntersections(float squareSideLength)
    {
        float ssl = squareSideLength;
        HashSet<Vector2> intersections = new HashSet<Vector2>();
        if (coeffs[1] != 0)
        {
            float yAtX0 = -coeffs[2] / coeffs[1];
            if (yAtX0 >= 0 && yAtX0 <= ssl)
            {
                intersections.Add(new Vector2(0, yAtX0));
            }
            float yAtXSsl = (-ssl * coeffs[0] - coeffs[2]) / coeffs[1];
            if (yAtXSsl >= 0 && yAtXSsl <= ssl)
            {
                intersections.Add(new Vector2(ssl, yAtXSsl));
            }
        }
        if (coeffs[0] != 0)
        {
            float xAtY0 = -coeffs[2] / coeffs[0];
            if (xAtY0 >= 0 && xAtY0 <= ssl)
            {
                intersections.Add(new Vector2(xAtY0, 0));
            }
            float xAtYSsl = (-ssl * coeffs[1] - coeffs[2]) / coeffs[0];
            if (xAtYSsl >= 0 && xAtYSsl <= ssl)
            {
                intersections.Add(new Vector2(xAtYSsl, ssl));
            }
        }
        return intersections.ToList();
    }
    private void UpdateFunctionUI()
    {
        InputField[] inputFields = functionUI.GetComponentsInChildren<InputField>();
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].SetTextWithoutNotify(RoundCoefficient(i));
        }
    }

    private string RoundCoefficient(int index)
    {
        float c = coeffs[index];
        return Mathf.Abs(c) < 10 ? c.ToString("0.0") : c.ToString("0");
    }
    private void ResetVisibleRanges()
    {
        visibleRanges = fullVisibleRanges;
    }
    private void UpdateRayVisibleRange()
    {
        ResetVisibleRanges();
        Vector3 initialPointWorld = coordSys.transform.TransformPoint(coordSys.SystemToLocalPoint(firstAnchor));
        Vector2 direction = secondAnchor - firstAnchor;
        if (direction.x > 0)
        {
            visibleRanges.x = initialPointWorld.x;
        }
        if (direction.x < 0)
        {
            visibleRanges.y = initialPointWorld.x;
        }
        if (direction.y > 0)
        {
            visibleRanges.z = initialPointWorld.y;
        }
        if (direction.y < 0)
        {
            visibleRanges.w = initialPointWorld.y;
        }
        print("visible ranges " + visibleRanges);
        transform.GetComponent<SpriteRenderer>().material.SetVector("_Ranges", visibleRanges);
    }
}
