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
            Vector3 mousePosDiff = prevMousePos - currentMousePos;
            
            transform.RotateAround(rotationPivot, Vector3.up, Vector3.Dot(mousePosDiff, Camera.main.transform.right) / 100);
            transform.RotateAround(rotationPivot, Vector3.right, Vector3.Dot(mousePosDiff, -Camera.main.transform.up) / 100);
            transform.RotateAround(rotationPivot, Vector3.forward, Vector3.Dot(mousePosDiff, -Camera.main.transform.up) / 100);
        }
        prevMousePos = currentMousePos;
    }

    public override Vector3 SystemToLocalPoint(Vector3 systemPos)
    {

        return new Vector3(systemPos.y, systemPos.z, -systemPos.x);
    }

    public override Vector3 LocalToSystemPoint(Vector3 localPos)
    {
        return new Vector3(-localPos.z, localPos.x, localPos.y);
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
