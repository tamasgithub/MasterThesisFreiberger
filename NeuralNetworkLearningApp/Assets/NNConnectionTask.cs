using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NNConnectionTask : Task
{
    public NN network;
    public bool fullyConnect;
    public int leftLayerToConnect;
    public int rightLayerToConnect;
    public int[] leftNodeToConnect;
    public int[] rightNodeToConnect;

    public override void StartTask()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: events!
        if (fullyConnect)
        {
            if (network.IsFullyConnected(false))
            {
                TaskFinished();
            }
        } else if (rightLayerToConnect == leftLayerToConnect + 1)
        {
            throw new NotImplementedException();
        } else if (leftNodeToConnect != null && rightNodeToConnect != null && rightNodeToConnect[0] == leftNodeToConnect[0] + 1 && network.GetLayerCount() > rightNodeToConnect[0]) {
            if (network.AreNodesConnected(leftNodeToConnect[0], rightNodeToConnect[0], leftNodeToConnect[1], rightNodeToConnect[1])) {
                TaskFinished();
            }
        }
    }

    public override void TaskFinished()
    {
        base.TaskFinished();
        gameObject.SetActive(false);
    }
}
