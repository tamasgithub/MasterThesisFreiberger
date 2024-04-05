using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNAdjustSizeTask : Task
{
    public NN network;
    public bool nodesAnywhere;
    public int numberOfNodesAnywhere;
    
    public int desiredLayerCount;
    public int[] desiredLayerSize;
    public override void StartTask()
    {
    }

    private void Update()
    {
        if (nodesAnywhere)
        {
            if (network.GetNodeCount() == numberOfNodesAnywhere)
            {
                TaskFinished();
            }
           
        } else if (network.GetLayerCount() == desiredLayerCount && AllLayerSizesCorrect())
        {
            TaskFinished();
            gameObject.SetActive(false);
        }
    }

    private bool AllLayerSizesCorrect()
    {
        if (desiredLayerSize.Length > network.GetLayerCount())
        {
            return false;
            
        }
        for (int i = 0; i < desiredLayerSize.Length; i++)
        {
            if (network.GetLayerSize(i) != desiredLayerSize[i])
            {
                return false;
            }
        }
        return true;
    }

    public override void TaskFinished()
    {
        base.TaskFinished();
        gameObject.SetActive(false);
    }
}
