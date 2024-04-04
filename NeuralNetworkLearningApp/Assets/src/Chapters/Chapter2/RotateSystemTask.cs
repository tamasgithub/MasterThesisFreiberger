using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSystemTask : Task
{
    public CoordinateSystem3D coordSystem;
    public float minTotalMouseDistance;
    private float rotatedTotal;

    public override void StartTask()
    {
        coordSystem.rotatedEvent += SystemRotated;
    }

    private void SystemRotated(float mouseDistance)
    {
        rotatedTotal += mouseDistance;
        if (rotatedTotal > minTotalMouseDistance) {
            coordSystem.rotatedEvent -= SystemRotated;
            TaskFinished();
        }
    }
}
