using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    private NN network;
    private int layerIndex;
    private float nodeDistance = 1.0f;
    private bool fullyConnected = false;
    public GameObject nodePrefab;

    private List<Node> nodes = new List<Node>();

    public void SetNetwork(NN network)
    {
        this.network = network;
    }

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
        NN nn = transform.parent.GetComponent<NN>();
        int leftLayerSize = nn.GetLayerSize(layerIndex - 1);
        int rightLayerSize = nn.GetLayerSize(layerIndex + 1);
        int initialSize = nodes.Count;

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
            for (int i = nodes.Count; i < size; i++)
            {
                CreateNewNode(i, leftLayerSize, rightLayerSize);
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

        // have to fully connect here, after the neighboring layers' nodes' edge lists have been updated,
        // instead of already connecting in the loop above at instantiation
        // alternatively one could update neighboring layers' nodes' edge lists after each single change of
        // this layer's size in the loop above and then also fully connect the new node (if the change was adding)
        if (fullyConnected && size > initialSize)
        {
            for (int i = initialSize; i < size; i++)
            {
                nodes[i].FullyConnect();
            } 
        }
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

    public void SetFullyConnected(bool fullyConnected)
    {
        if (this.fullyConnected == fullyConnected)
        {
            return;
        }

        this.fullyConnected = fullyConnected;
        if (fullyConnected)
        {
            foreach (Node node in nodes)
            {
                node.FullyConnect();
            }
        }
    }

    public void ConnectEdgeToSource(Edge edge, int sourceLayer, int sourceNode, int destinationNode)
    {
        if (sourceLayer == layerIndex)
        {
            nodes[sourceNode].ConnectOutgoingEdge(edge, destinationNode);
        } else
        {
            transform.parent.GetComponent<NN>().ConnectEdgeToSource(edge, sourceLayer, sourceNode, destinationNode);
        }
    }

    public void ConnectEdgeToDestination(Edge edge, int destinationLayer, int destinationNode, int sourceNode)
    {
        if (destinationLayer == layerIndex)
        {
            nodes[destinationNode].ConnectIncomingEdge(edge, sourceNode);
        } else
        {
            transform.parent.GetComponent<NN>().ConnectEdgeToDestination(edge, destinationLayer, destinationNode, sourceNode);
        }
    }


    public bool AreNodesConnected(int nodeIndex, int neighborNodeIndex, bool incoming)
    {
        Node node = nodes[nodeIndex];
        return incoming ? node.HasIncomingEdge(neighborNodeIndex) : node.HasOutgoingEdge(neighborNodeIndex);
    }

    public void SetUISettings(bool showValue, bool colorEdges, bool edgeHoveringEnabled, bool nodeHoveringEnabled, bool editingEnabled)
    {
        foreach (Node node in nodes)
        {
            node.SetUISettings(showValue, colorEdges, edgeHoveringEnabled, nodeHoveringEnabled, editingEnabled);
        }
    }

    public bool GetColorEdges()
    {
        return network.colorEdges;
    }

    public bool AreWeightsRandom()
    {
        return network.randomWeights;
    }

    public bool GetEdgeHoveringEnabled()
    {
        return network.edgeHoveringEnabled;
    }
    public bool GetEditingEnabled()
    {
        return network.editingEnabled;
    }

    public int GetNodeCount()
    {
        return nodes.Count;
    }

    public bool IsFullyConnected(bool incoming)
    {
        foreach (Node node in nodes)
        {
            if (incoming)
            {
                int incomingSize = node.GetIncomingSize();
                for (int i = 0; i < incomingSize; i++)
                {
                    if (!node.HasIncomingEdge(i))
                    {
                        return false;
                    }
                }
            } else
            {
                int ougoingSize = node.GetOutgoingSize();
                for (int i = 0; i < ougoingSize; i++)
                {
                    if (!node.HasOutgoingEdge(i))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public float GetNodeValue(int nodeIndex)
    {
        return nodes[nodeIndex].GetValue();
    }

    public float GetNodeBias(int nodeIndex)
    {
        return nodes[nodeIndex].GetBias();
    }

    public float GetEdgeWeight(int nodeIndex, int neighborNodeIndex, bool incoming)
    {
        return nodes[nodeIndex].GetWeightTo(neighborNodeIndex, incoming);
    }

    public void EdgeHovered()
    {
        network.EdgeHovered();
    }

    public void NodeHovered()
    {
        network.NodeHovered();
    }

    private void CreateNewNode(int index, int leftLayerSize, int rightLayerSize)
    {
        GameObject newNode = Instantiate(nodePrefab, transform);
        newNode.name = "node" + index;
        Node node = newNode.GetComponent<Node>();
        nodes.Add(node);
        node.SetLayer(this);
        node.SetNodeIndex(index);
        node.SetIncomingSize(leftLayerSize);
        node.SetOutgoingSize(rightLayerSize);
        node.SetUISettings(network.showValues, network.colorEdges, network.edgeHoveringEnabled, network.nodeHoveringEnabled, network.editingEnabled);
        if (network.randomBiases)
        {
            node.RandomizeBias();
        }
    }

    private void PositionNode(Node node)
    {
        node.transform.localPosition = Vector2.down * nodeDistance * (nodes.IndexOf(node) - (nodes.Count - 1) / 2.0f);
    }
}
