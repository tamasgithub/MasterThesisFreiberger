using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DecisionBoundary : MonoBehaviour
{
    public CoordinateSystem coordSys;
    public Transform functionUI;
    private Vector2 firstAnchor;
    private float[] coeffs;
    private Vector2 intersection1 = Vector2.zero;
    private Vector2 intersection2 = Vector2.zero;
    

    private void Start()
    {
        float halfEdge = coordSys.transform.localScale.x / 2f;
        float offset = coordSys.transform.localScale.x * coordSys.offset;
        Vector4 ranges = new Vector4(coordSys.transform.position.x - (halfEdge - offset), coordSys.transform.position.x + halfEdge,
            coordSys.transform.position.y - (halfEdge - offset), coordSys.transform.position.y + halfEdge);
        transform.GetComponent<SpriteRenderer>().material.SetVector("_Ranges", ranges);

        coeffs = new float[] { 1f, -1f, 0f };
        DrawDecisionBoundary();
        UpdateFunctionUI();
    }
    public void SetCoordSystem(CoordinateSystem coordSystem)
    {
        coordSys = coordSystem;
    }

    public void SetFirstAnchor(Vector2 anchor)
    {
        firstAnchor = anchor;
        print(anchor);
    }

    public void SetSecondAnchor(Vector2 anchor)
    {
        Vector2 systemPoint1 = GetSystemPos(firstAnchor);
        Vector2 systemPoint2 = GetSystemPos(anchor);
        // calculate the coefficients of the straight line a x1 + b x2 + c = 0 using the two given points
        Vector2 direction = systemPoint2 - systemPoint1;
        Vector2 normal = new Vector2(-direction.y, direction.x).normalized;
        coeffs[0] = normal.x;
        coeffs[1] = normal.y;
        coeffs[2] = -(normal.x * systemPoint1.x + normal.y * systemPoint1.y);
        DrawDecisionBoundary();
        UpdateFunctionUI();
    }
    private void Update()
    {
    }

    public void SetCoefficient(int index, float coefficient)
    {
        coeffs[index] = coefficient;
        DrawDecisionBoundary();
    }

    private Vector3 GetSystemPos(Vector3 localPos)
    {
        Vector3 scaledPos = localPos / (1 - 2 * coordSys.offset);
        Vector3 shiftedPos = scaledPos + 0.5f * Vector3.one;
        return shiftedPos;
    }

    private Vector3 GetLocalPos(Vector3 systemPos)
    {
        Vector3 shiftedPos = systemPos - 0.5f * Vector3.one;
        Vector3 scaledPos = shiftedPos * (1 - 2 * coordSys.offset);
        return scaledPos;
    }

    private void DrawDecisionBoundary()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        print("update");
        // anchors as intersections of the line with the axis-aligned square from 0|0 to 2|2

        List<Vector2> intersections = CalculateIntersections(2);
        if (intersections.Count < 1)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        intersection1 = intersections[0];
        intersection2 = new Vector2(intersection1.x - coeffs[1], intersection1.y + coeffs[0]);
        intersection1 = GetLocalPos(intersection1);
        intersection2 = GetLocalPos(intersection2);
        transform.localPosition = new Vector3((intersection1.x + intersection2.x) / 2f, (intersection1.y + intersection2.y) / 2f, - 1);
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, intersection2 - intersection1);


        coordSys.HighlightWronglyClassified(coeffs);
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
        for (int i = 0; i < 3; i++)
        {
            functionUI.GetChild(i + 1).GetChild(0).GetComponent<InputField>().SetTextWithoutNotify(RoundCoefficient(i));
        }
    }

    private string RoundCoefficient(int index)
    {
        float c = coeffs[index];
        return Mathf.Abs(c) < 10 ? c.ToString("0.0") : c.ToString("0");
    }


}
