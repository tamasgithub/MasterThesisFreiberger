using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject edgePrefab;
    public bool manuallyConnectable = true;

    private Layer layer;
    private Edge manuallyConnectingEdge;
    // these lists are taylored to simple NNs, where nodes can only be connected to nodes from neighboring layers!
    private List<Edge> incomingEdges = new List<Edge>();
    private List<Edge> outgoingEdges = new List<Edge>();
    // inside its layer
    private int nodeIndex;

    public int GetIncomingSize()
    {
        return incomingEdges.Count;
    }

    public int GetOutgoingSize()
    {
        return outgoingEdges.Count;
    }

    public void SetIncomingSize(int leftLayerSize)
    {
        SetEdgeListSize(incomingEdges, leftLayerSize);
    }

    public void SetOutgoingSize(int rightLayerSize)
    {
        SetEdgeListSize(outgoingEdges, rightLayerSize);
    }

    // TODO: with fancy property
    public void SetNodeIndex(int index)
    {
        nodeIndex = index;
    }

    public int GetNodeIndex()
    {
        return nodeIndex;
    }

    public void SetIncomingEdge(Edge edge, int sourceIndex)
    {
        incomingEdges[sourceIndex] = edge;
    }

    public void SetOutgoingEdge(Edge edge, int destinationIndex)
    {
        outgoingEdges[destinationIndex] = edge;
    }

    public void SetLayer(Layer layer)
    {
        this.layer = layer;
    }

    public int GetLayerIndex()
    {
        return layer.GetLayerIndex();
    }

    public bool HasIncomingEdge(int source)
    {
        return incomingEdges[source] != null;
    }

    public bool HasOutgoingEdge(int destination)
    {
        return outgoingEdges[destination] != null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetEdgeListSize(List<Edge> list, int size)
    {
        if (size < list.Count)
        {
            for (int i = list.Count - 1; i >= size; i--)
            {
                Edge edge = list[i];
                list.RemoveAt(i);
                if (edge != null)
                {
                    Destroy(edge.gameObject);
                }
            }
        } else if (size > list.Count)
        {
            for (int i = list.Count; i < size; i++)
            {
                list.Add(null);
            }
        }
    }

    private void OnMouseDown()
    {
        if (manuallyConnectable)
        {
            ManuallyConnect();
        }
    }

    private void ManuallyConnect()
    {
        manuallyConnectingEdge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
        manuallyConnectingEdge.isManuallyConnecting = true;
        manuallyConnectingEdge.SetFirstNode(this);
        manuallyConnectingEdge.transform.name = "edge";
    }

    private void OnMouseUp()
    {
        print("Mouse up");
        //RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, -Vector2.up);
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        if(hit.collider != null)
        {
            manuallyConnectingEdge.SetSecondNode(hit.collider.transform.GetComponent<Node>());
        }
        manuallyConnectingEdge.isManuallyConnecting = false;
        manuallyConnectingEdge = null;
    }
}
