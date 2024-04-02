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
    public int[] decisionBetweenClasses;
    private Vector2 firstAnchor = new Vector2(0,0);
    private Vector2 secondAnchor = new Vector2(1, 1);
    private float[] coeffs = new float[] { 1f, -1f, 0f };
    private Vector4 fullVisibleRanges;
    private Vector4 visibleRanges;
    private bool isRay;

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
        arrows.transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = coordSys.colorOfClass[decisionBetweenClasses[0]];
        arrows.transform.GetChild(1).GetComponent<SpriteRenderer>().material.color = coordSys.colorOfClass[decisionBetweenClasses[1]];
        isRay = transform.parent.GetComponentsInChildren<DecisionBoundary>().Length > 1;
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

    public int[] GetDecisionBetweenClasses()
    {
        return decisionBetweenClasses;
    }
    public void TransformIntoRay(Vector2 initialPoint)
    {
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
        Vector3 endPoint1;
        Vector3 endPoint2;
        List<Vector2> intersections = CalculateIntersections((1 - coordSys.offset) / (1 - 2 * coordSys.offset));
        if (intersections.Count < 2)
        {
            foreach (Renderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.enabled = false;
            }
            return;
        }

        if (isRay)
        {
            endPoint1 = firstAnchor;
            Vector2 direction = secondAnchor - firstAnchor;
            if (direction.x != 0)
            {
                if (Mathf.Sign((intersections[0] - firstAnchor).x) == Mathf.Sign(direction.x))
                {
                    endPoint2 = intersections[0];
                }
                else
                {
                    endPoint2 = intersections[1];
                }
            } else
            {
                if (Mathf.Sign((intersections[0] - firstAnchor).y) == Mathf.Sign(direction.y))
                {
                    endPoint2 = intersections[0];
                }
                else
                {
                    endPoint2 = intersections[1];
                }
            }
        }
        else
        {
            endPoint1 = intersections[0];
            endPoint2 = intersections[1];
        }
        if (endPoint1.x > endPoint2.x)
        {
            Vector2 temp = endPoint1;
            endPoint1 = endPoint2;
            endPoint2 = temp;
        }
        if (Vector2.Distance(endPoint1, endPoint2) <= 0.00001f)
        {
            foreach (Renderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.enabled = false;
            }
            return;
        }
        endPoint1 = coordSys.SystemToLocalPoint(endPoint1);
        endPoint2 = coordSys.SystemToLocalPoint(endPoint2);
        Vector2 centerOfLine = new Vector3((endPoint1.x + endPoint2.x) / 2f, (endPoint1.y + endPoint2.y) / 2f, -1);
        transform.localPosition = centerOfLine;

        // scaling the boundary to be exactly from one endpoint to the other
        Vector3 localScale = transform.localScale;
        Vector3 arrowLocalScale = transform.GetChild(0).localScale;
        float factor = Vector2.Distance(endPoint1, endPoint2) / localScale.x;
        localScale.x *= factor;
        arrowLocalScale.x /= factor;
        transform.localScale = localScale;
        transform.GetChild(0).localScale = arrowLocalScale;

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
        // to not make lines narrower near the culling line
        float buffer = 0.01f;
        if (direction.x > 0)
        {
            visibleRanges.x = initialPointWorld.x - buffer;
        }
        if (direction.x < 0)
        {
            visibleRanges.y = initialPointWorld.x + buffer;
        }
        if (direction.y > 0)
        {
            visibleRanges.z = initialPointWorld.y - buffer;
        }
        if (direction.y < 0)
        {
            visibleRanges.w = initialPointWorld.y + buffer;
        }
        transform.GetComponent<SpriteRenderer>().material.SetVector("_Ranges", visibleRanges);
    }
}
