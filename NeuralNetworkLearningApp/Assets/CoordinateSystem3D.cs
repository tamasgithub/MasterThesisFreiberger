using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 
 *
 */
public class CoordinateSystem3D : CoordinateSystem
{
    private bool rotating;
    private Vector3 rotationPivot;
    private Vector3 prevMousePos;

    protected new void Start()
    {
        base.Start();
        rotationPivot = transform.TransformPoint(new Vector3(0.4f, 0.4f, -0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentMousePos = Input.mousePosition;
        
        if (rotating)
        {
            Vector3 mousePosDiff = currentMousePos - prevMousePos;
            
            transform.Rotate(transform.up, -Vector3.Dot(mousePosDiff, Camera.main.transform.right) / 10, Space.World);
            transform.Rotate(Camera.main.transform.right, -Vector3.Dot(mousePosDiff, -Camera.main.transform.up) / 10, Space.World);
        }
        prevMousePos = currentMousePos;

        foreach (PlottableData plottableData in data)
        {
            plottableData.transform.localPosition = SystemToLocalPoint(ArrayAsVector3(plottableData.GetFeatureValues()));
            if (plottableData.GetFeatureValues().Length < 0)
            {
                Vector3 localPosition = plottableData.transform.localPosition;
                localPosition.z = -1;
                plottableData.transform.localPosition = localPosition;
            }
        }

    }

    public override Vector3 SystemToLocalPoint(Vector3 systemPos)
    {

        return new Vector3(systemPos.x, systemPos.y, -systemPos.z) * .8f;
    }

    public override Vector3 LocalToSystemPoint(Vector3 localPos)
    {
        return new Vector3(localPos.x, localPos.y, -localPos.z) / .8f;
    }

    protected override void OnMouseDown()
    {
        rotating = true;
    }

    protected override void OnMouseUp()
    {
        rotating = false;
    }

}
