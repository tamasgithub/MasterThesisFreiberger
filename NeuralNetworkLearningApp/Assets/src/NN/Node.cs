using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public GameObject edgePrefab;
    public GameObject labelPrefab;
    public bool manuallyConnectable = true;
    public float inputValue;
    public ActivationFunction activationFunction;
    public Gradient gradient;

    public float value;

    private Layer layer;
    private Edge manuallyConnectingEdge;
    // these lists are taylored to simple NNs, where nodes can only be connected to nodes from neighboring layers!
    private List<Edge> incomingEdges = new List<Edge>();
    private List<Edge> outgoingEdges = new List<Edge>();
    // inside its layer
    private int nodeIndex;

    private float bias;
    private GameObject tempLabel;
    private Camera cam;

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

    public void RemoveIncomingEdge(int sourceIndex)
    {
        incomingEdges[sourceIndex] = null;
    }

    public void RemoveOutgoingEdge(int destinationIndex)
    {
        outgoingEdges[destinationIndex] = null;
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

    public void FullyConnect()
    {
        for (int i = 0; i < incomingEdges.Count; i++)
        {
            if (incomingEdges[i] == null)
            {
                Edge edge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
                edge.SetFirstNode(this);
                layer.ConnectEdgeToSource(edge, layer.GetLayerIndex() - 1, i, nodeIndex);
                
                edge.transform.name = "edge";
            }
        }
        for (int i = 0; i < outgoingEdges.Count; i++)
        {
            if (outgoingEdges[i] == null)
            {
                Edge edge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
                edge.SetFirstNode(this);
                layer.ConnectEdgeToDestination(edge, layer.GetLayerIndex() + 1, i, nodeIndex);
                
                edge.transform.name = "edge";
            }
        }
    }

    public void ConnectOutgoingEdge(Edge edge, int destination)
    {
        edge.SetSecondNode(this);
        outgoingEdges[destination] = edge;
    }

    public void ConnectIncomingEdge(Edge edge, int source)
    {
        edge.SetSecondNode(this);
        incomingEdges[source] = edge;
    }

    public float GetValue()
    {
        return value;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        bias = UnityEngine.Random.value * 2 - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (tempLabel != null)
        {
            tempLabel.transform.position = Input.mousePosition + Vector3.up * 30;
        }
        transform.GetChild(0).GetComponent<RectTransform>().position = cam.WorldToScreenPoint(transform.position);

        ComputeValue(); 
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

    private void ComputeValue()
    {
        Text text = transform.GetChild(0).GetComponent<Text>();
        if (GetLayerIndex() == 0)
        {
            value = inputValue;
        } else
        {
            value = 0f;
            foreach (Edge incoming in incomingEdges)
            {
                // check for null bc not existing edges are still in the list as null values
                if (incoming != null)
                {
                    value += incoming.GetWeight() * incoming.GetLeftNodeValue();
                }
            }
            value += bias;

            switch (activationFunction)
            {

                case ActivationFunction.SIGMOID:
                    value = 1 / (1 + Mathf.Exp(-value));
                    break;

                // in case of identity or unknow function, do nothing
                case ActivationFunction.IDENTITY:
                default:
                    break;
            }
        }

        text.text = value.ToString("0.00");
        text.color = gradient.Evaluate(value);
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
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        if(hit.collider != null)
        {
            manuallyConnectingEdge.SetSecondNode(hit.collider.transform.GetComponent<Node>());
        }
        manuallyConnectingEdge.isManuallyConnecting = false;
        manuallyConnectingEdge = null;
    }
    private void OnMouseEnter()
    {
        if (GetLayerIndex() > 0)
        {
            tempLabel = Instantiate(labelPrefab, GameObject.Find("Canvas").transform);
            tempLabel.GetComponent<RectTransform>().position = Input.mousePosition;
            tempLabel.GetComponent<Text>().text = "Bias:\n" + bias.ToString("0.00");
        }   
    }

    private void OnMouseExit()
    {
        if (tempLabel != null)
        {
            Destroy(tempLabel);
        }
    }

    private void OnDestroy()
    {
        if (tempLabel != null)
        {
            Destroy(tempLabel);
        }
    }
}
