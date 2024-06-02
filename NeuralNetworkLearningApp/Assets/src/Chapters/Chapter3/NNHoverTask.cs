using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNHoverTask : Task
{
    public NN network;
    public int edgesToHover;
    public int nodesToHover;
    private int edgesHovered = 0;
    private int nodesHovered = 0;

    public override void StartTask()
    {
        network.edgeHovered += EdgeHovered;
        network.nodeHovered += NodeHovered;
    }

    private void EdgeHovered()
    {
        edgesHovered++;
        if (edgesHovered == edgesToHover && nodesHovered >= nodesToHover) {
            TaskFinished();
            gameObject.SetActive(false);
        }
    }

    private void NodeHovered()
    {
        nodesHovered++;
        if (nodesHovered == nodesToHover && edgesHovered >= edgesToHover)
        {
            TaskFinished();
            gameObject.SetActive(false);
        }
    }
}
