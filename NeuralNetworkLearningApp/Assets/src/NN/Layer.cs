using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    private int layerIndex;
    private float nodeDistance = 1.0f;
    //private bool fullyConnected = false;
    public GameObject nodePrefab;

    private List<Node> nodes = new List<Node>();
    private NN nn;

    // TODO: fancy c# property instead
    public int GetSize()
    {
        return nodes.Count;
    }
    public void SetSize(int size)
    {
        if (size == nodes.Count)
        {
            return;
        }
        nn = transform.parent.GetComponent<NN>();
        if (size < nodes.Count)
        {
            for (int i = nodes.Count - 1; i >= size; i--)
            {
                Node node = nodes[i];
                nodes.RemoveAt(i);
                Destroy(node.gameObject);
            }
        }
        else if (size > nodes.Count)
        {
            int leftLayerSize = nn.GetLayerSize(layerIndex - 1);
            int rightLayerSize = nn.GetLayerSize(layerIndex + 1);
            for (int i = nodes.Count; i < size; i++)
            {
                GameObject newNode = Instantiate(nodePrefab, transform);
                newNode.name = "node" + i;
                Node node = newNode.GetComponent<Node>();
                nodes.Add(node);
                node.SetLayer(this);
                node.SetNodeIndex(i);
                node.SetIncomingSize(leftLayerSize);
                node.SetOutgoingSize(rightLayerSize);
                /*if (fullyConnected)
                {
                    //TODO: newNode.GetComponent<Node>().fullyConnect();
                }*/
            }
        }

        // reposition all nodes according to the new size of the layer (and the node distance)
        foreach (Node node in nodes)
        {
            PositionNode(node);
        }


        // for all nodes from the left layer, set outgoing size to size
        // for all nodes from the right layer, set incoming size to size
        // TODO: maybe just get the references of the layers and directly call the set...size methods on them?
        nn.UpdateEdgeListsOfLayer(layerIndex - 1);
        nn.UpdateEdgeListsOfLayer(layerIndex + 1);
    }

    public void SetNodesIncomingSizes(int size)
    {
        if (nodes[0].GetIncomingSize() == size)
        {
            return;
        }
        foreach (Node node in nodes)
        {
            node.SetIncomingSize(size);
        }
    }

    public void SetNodesOutgoingSizes(int size)
    {
        if (nodes[0].GetOutgoingSize() == size)
        {
            return;
        }
        foreach (Node node in nodes)
        {
            node.SetOutgoingSize(size);
        }
    }

    public void SetNodeDistance(float nodeDistance)
    {
        if (this.nodeDistance == nodeDistance)
        {
            return;
        }
        this.nodeDistance = nodeDistance;
        foreach (Node node in nodes)
        {
            PositionNode(node);
        }
    }

    public void SetLayerIndex(int index)
    {
        layerIndex = index;
    }

    public int GetLayerIndex()
    {
        return layerIndex;
    }

    private void PositionNode(Node node)
    {
        node.transform.localPosition = Vector2.down * nodeDistance * (nodes.IndexOf(node) - (nodes.Count - 1) / 2.0f);
    }


}
