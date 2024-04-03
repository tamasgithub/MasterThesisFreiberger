using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN : MonoBehaviour
{
    public GameObject layerPrefab;

    // these are just helpers to modifiy the nn from the editor
    [Range(0, 10)]
    public int numLayers = 1;
    public List<int> layerSizes = new List<int>();
    [Range(1, 10)]
    public float layerDistance = 1.0f;
    [Range(1, 10)]
    public float nodeDistance = 1.0f;
    private float _nodeDistance = 1;

    public bool fullyConnected = false;

    public GameObject layerSizeUIPrefab;

    List<Layer> layers = new List<Layer>();

    public int GetLayerSize(int index)
    {
        if (index < 0 || index >= layers.Count)
        {
            return 0;
        }
        return layers[index].GetSize();
    }

    public int GetLayerCount()
    {
        return layers.Count;
    }

    public float GetLayerDistance()
    {
        return layerDistance;
    }

    public float GetNodeDistance()
    {
        return nodeDistance;
    }

    // for UI
    public void IncreaseLayerCount()
    {
        SetLayerCount(layers.Count + 1);

        if (layerSizeUIPrefab != null)
        {
            GameObject layerSizeUI = Instantiate(layerSizeUIPrefab, transform.parent);
            Transform plusButton = layerSizeUI.transform.GetChild(0);
            plusButton.GetComponent<NN_UI_Control>().underLayerWithIndex = layers.Count - 1;
            plusButton.GetComponent<NN_UI_Control>().offset = 0.5f;
            Transform minusButton = layerSizeUI.transform.GetChild(1);
            minusButton.GetComponent<NN_UI_Control>().underLayerWithIndex = layers.Count - 1;
            minusButton.GetComponent<NN_UI_Control>().offset = -0.5f;
        }
    }

    public void DecreaseLayerCount()
    {
        SetLayerCount(layers.Count - 1);

        NN_UI_Control[] layerSizeUIs = transform.parent.GetComponentsInChildren<NN_UI_Control>();
        if (layerSizeUIs.Length > 0)
        {
            Destroy(layerSizeUIs[layerSizeUIs.Length - 1].transform.parent.gameObject);
        }
    }

    public void IncreaseLayerSize(NN_UI_Control control)
    {
        int layerIndex = control.underLayerWithIndex;
        layerSizes[layerIndex]++;
        if (layerIndex >= layers.Count)
        {
            return;
        }
        layers[layerIndex].SetSize(layers[layerIndex].GetSize() + 1);
    }

    public void DecreaseLayerSize(NN_UI_Control control)
    {
        int layerIndex = control.underLayerWithIndex;
        layerSizes[layerIndex]--;
        if (layerIndex >= layers.Count)
        {
            return;
        }
        layers[layerIndex].SetSize(layers[layerIndex].GetSize() - 1);
    }
    // end for UI

    public void UpdateEdgeListsOfLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= layers.Count)
        {
            return;
        }
        // check whether to update incoming or outgoing edges or both
        if (layerIndex > 0)
        {
            layers[layerIndex].SetNodesIncomingSizes(layers[layerIndex - 1].GetSize());
        }
        if (layerIndex < layers.Count - 1)
        {
            layers[layerIndex].SetNodesOutgoingSizes(layers[layerIndex + 1].GetSize());
        }
    }

    public void ConnectEdgeToSource(Edge edge, int sourceLayer, int sourceNode, int destinationNode)
    {
        layers[sourceLayer].ConnectEdgeToSource(edge, sourceLayer, sourceNode, destinationNode);
    }

    public void ConnectEdgeToDestination(Edge edge, int destinationLayer, int destinationNode, int sourceNode)
    {
        layers[destinationLayer].ConnectEdgeToDestination(edge, destinationLayer, destinationNode, sourceNode);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // TODO: decide wether to put all checks into a single for loop for readabilty/less code, or to leave it like this for maximum performance
    void Update()
    {
        if (numLayers != layers.Count) {
            SetLayerCount(numLayers);
        }

        for (int i = 0; i < numLayers; i++)
        {
            if (layerSizes[i] != layers[i].GetSize())
            {
                layers[i].SetSize(layerSizes[i]);
            }
        }
        if (layers.Count > 1 && Vector2.Distance(transform.Find("layer0").localPosition, transform.Find("layer1").localPosition) != layerDistance)
        {
            for (int i = 0; i < numLayers; i++)
            {
                PositionLayer(i);
            }
        }
        if (nodeDistance != _nodeDistance)
        {
            for (int i = 0; i < numLayers; i++)
            {
                layers[i].SetNodeDistance(nodeDistance);
            }
            _nodeDistance = nodeDistance;
        }

        foreach (Layer layer in layers)
        {
            layer.SetFullyConnected(fullyConnected);
        }
    }

    private void SetLayerCount(int layerCount)
    {
        if (layerCount < layers.Count)
        {
            for (int i = layers.Count - 1; i >= layerCount; i--)
            {
                // set size of the layer to 0 first, so that the neigboring layers are updated correctly
                // (i.e. the outgoing edge list of the left neighboring layer is cleared)
                layers[i].SetSize(0);
                Destroy(layers[i].gameObject);
                // destroy the last layer
                layerSizes.RemoveAt(i);
                layers.RemoveAt(i);
            }
        }
        else if (layerCount > layerSizes.Count)
        {
            for (int i = layerSizes.Count; i < layerCount; i++)
            {
                layerSizes.Add(1);

                GameObject newLayer = Instantiate(layerPrefab, transform);
                newLayer.name = "layer" + i;
                Layer layer = newLayer.GetComponent<Layer>();
                layers.Add(layer);
                layer.SetLayerIndex(i);
                layer.SetSize(1);
                layer.SetNodeDistance(nodeDistance);
                layer.SetFullyConnected(fullyConnected);
                PositionLayer(i);
            }
        }
        numLayers = layerCount;
    }

    private void PositionLayer(int i)
    {
        transform.Find("layer" + i).localPosition = i * Vector2.right * layerDistance;
    }
}
