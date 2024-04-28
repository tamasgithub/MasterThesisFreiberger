using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerDownHandler
{
    public GameObject edgePrefab;
    public GameObject labelPrefab;
    public GameObject editorPrefab;
    public bool manuallyConnectable = true;
    public float inputValue;
    public ActivationFunction activationFunction;
    public Gradient colorGradient;

    private float value;
    private Layer layer;
    private Edge manuallyConnectingEdge;
    // these lists are taylored to simple NNs, where nodes can only be connected to nodes from neighboring layers!
    private List<Edge> incomingEdges = new List<Edge>();
    private List<Edge> outgoingEdges = new List<Edge>();


    // inside its layer
    private int nodeIndex;

    private float bias;
    private GameObject hoverLabel;
    private bool hoveringEnabled;
    private GameObject editor;
    private bool editingEnabled;
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
                Edge edge = CreateNewEdge();
                layer.ConnectEdgeToSource(edge, layer.GetLayerIndex() - 1, i, nodeIndex);
            }
        }
        for (int i = 0; i < outgoingEdges.Count; i++)
        {
            if (outgoingEdges[i] == null)
            {
                Edge edge = CreateNewEdge();
                layer.ConnectEdgeToDestination(edge, layer.GetLayerIndex() + 1, i, nodeIndex);
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

    public float GetBias()
    {
        return bias;
    }

    public void SetInputValue(float inputValue)
    {
        this.inputValue = inputValue;
        // format the value to two decimals
        Text text = transform.GetChild(0).GetComponent<Text>();
        text.text = inputValue.ToString("0.00");
        text.color = colorGradient.Evaluate((inputValue + 1) / 2f);
    }

    public void SetBias(float bias)
    {
        this.bias = bias;
    }

    public void SetUISettings(bool showValue, bool colorEdges, bool edgeHoveringEnabled, bool nodeHoveringEnabled, bool editingEnabled)
    {
        transform.GetChild(0).gameObject.SetActive(showValue);
        foreach (Edge edge in incomingEdges)
        {
            if (edge == null)
            {
                continue;
            }
            edge.SetColorEdges(colorEdges);
            edge.SetHoveringEnabled(edgeHoveringEnabled);
            edge.SetEditingEnabled(editingEnabled);
        }
        foreach (Edge edge in outgoingEdges) {
            if (edge == null)
            {
                continue;
            }
            edge.SetColorEdges(colorEdges);
            edge.SetHoveringEnabled(edgeHoveringEnabled);
            edge.SetEditingEnabled(editingEnabled);
        }
        this.hoveringEnabled = nodeHoveringEnabled;
        if (hoverLabel != null)
        {
            hoverLabel.SetActive(nodeHoveringEnabled);
        }
        this.editingEnabled = editingEnabled;
        if (editor != null)
        {
            editor.SetActive(editingEnabled);
        }
    }

    public void EdgeHovered()
    {
        layer.EdgeHovered();
    }

    public float GetWeightTo(int neighborNodeIndex, bool incoming)
    {
        if (incoming)
        {
            return incomingEdges[neighborNodeIndex].GetWeight();
        } else
        {
            return outgoingEdges[neighborNodeIndex].GetWeight();
        }
    }
    public void RandomizeBias()
    {
        bias = UnityEngine.Random.value * 2 - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // recalculate position every frame bc the text is a UI element. TODO: replace with a textmesh, then setting localposition to zero once.
        transform.GetChild(0).GetComponent<RectTransform>().position = cam.WorldToScreenPoint(transform.position);

        if (hoverLabel != null)
        {
            hoverLabel.transform.position = Input.mousePosition + Vector3.up * 80;
        }

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

        // format the value to two decimals
        text.text = value.ToString("0.00");
        text.color = colorGradient.Evaluate((value + 1) / 2f);
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
        manuallyConnectingEdge = CreateNewEdge();
        manuallyConnectingEdge.isManuallyConnecting = true;
    }

    private Edge CreateNewEdge()
    {
        Edge edge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
        edge.SetColorEdges(layer.GetColorEdges());
        if (layer.AreWeightsRandom())
        {
            edge.RandomizeWeight();
        }
        edge.SetEditingEnabled(layer.GetEditingEnabled());
        edge.SetHoveringEnabled(layer.GetEdgeHoveringEnabled());
        edge.SetFirstNode(this);
        edge.transform.name = "edge";
        return edge;
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
        if (GetLayerIndex() > 0 && hoveringEnabled)
        {
            hoverLabel = Instantiate(labelPrefab, GameObject.Find("Canvas").transform);
            //tempLabel.GetComponent<RectTransform>().position = Vector3.up*50;
            hoverLabel.GetComponentInChildren<Text>().text = "Bias:\n" + bias.ToString("0.00");
            layer.NodeHovered();
        }   
    }

    private void OnMouseExit()
    {
        if (hoverLabel != null)
        {
            Destroy(hoverLabel);
        }
        if (editor != null)
        {
            Destroy(editor);
        }
    }

    private void OnDestroy()
    {
        if (hoverLabel != null)
        {
            Destroy(hoverLabel);
        }
        if (editor != null)
        {
            Destroy(editor);
        }
    }

    // only registers on UI elements, that means the value of the node has to be displayed to
    // edit the input value or the bias
    public void OnPointerDown(PointerEventData eventData)
    {
        // right click
        if (Input.GetMouseButtonDown(1) && editingEnabled)
        {
            if (hoverLabel != null)
            {
                Destroy(hoverLabel);
            }
            InputField input;
            if (editor != null)
            {
                input = editor.GetComponentInChildren<InputField>();
                input.Select();
                return;
            }
            Vector3 position = cam.WorldToScreenPoint(transform.position) + Vector3.up * 100;
            editor = Instantiate(editorPrefab, position, Quaternion.identity, transform);
            Text label = editor.transform.GetChild(0).GetComponent<Text>();
            label.text = GetLayerIndex() == 0 ? "Input" : "Bias";
            input = editor.GetComponentInChildren<InputField>();
            input.Select();
            input.SetTextWithoutNotify(GetLayerIndex() == 0 ? inputValue.ToString("0.00") : bias.ToString("0.00"));
            // how to fucus? input.MoveTextStart(true);
        }
    }
}
