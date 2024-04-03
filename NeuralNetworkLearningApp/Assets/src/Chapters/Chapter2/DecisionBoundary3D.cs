using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

/**
 *  This class was not finished and is nowhere functional.
 */
public class DecisionBoundary3D : DecisionBoundary
{

    // Start is called before the first frame update
    void Start()
    {
        RecalculateVisibleArea();

        arrows.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = coordSys.colorOfClass[separatingClasses[0]];
        arrows.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = coordSys.colorOfClass[separatingClasses[0]];
        arrows.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = coordSys.colorOfClass[separatingClasses[1]];
        arrows.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = coordSys.colorOfClass[separatingClasses[1]];

        Vector3 normal = Random.onUnitSphere;
        if (Vector3.Dot(normal, Vector3.one * 0.5f) < 0)
        {
            normal *= -1;
        }
        coeffs[0] = normal.x;
        coeffs[1] = normal.y;
        coeffs[2] = normal.z;
        coeffs[3] = -Vector3.Dot(normal, Vector3.one * 0.5f);
        
        if (functionUI != null)
        {
            UpdateFunctionUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //DrawDecisionBoundary();
    }

    public void RecalculateVisibleArea()
    {
        float edge = coordSys.transform.localScale.x * 0.9f;
        Vector4 xyRanges = new Vector4(coordSys.transform.position.x, coordSys.transform.position.x + edge,
            coordSys.transform.position.y, coordSys.transform.position.y + edge);
        Vector3 zRange = new Vector3(coordSys.transform.position.z - edge, coordSys.transform.position.z, 0);
        transform.GetComponent<Renderer>().material.SetVector("_XYRange", xyRanges);
        transform.GetComponent<Renderer>().material.SetVector("_ZRange", zRange);
        transform.GetChild(0).GetComponent<Renderer>().material.SetVector("_XYRange", xyRanges);
        transform.GetChild(0).GetComponent<Renderer>().material.SetVector("_ZRange", zRange);
    }

    protected override void DrawDecisionBoundary()
    {
        Vector3 normal = new Vector3(coeffs[0], coeffs[1], coeffs[2]);
        print("Look at normal " + normal);
        transform.LookAt(transform.TransformPoint(transform.localPosition + normal));
    }
}
