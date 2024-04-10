using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNSetParametersTask : Task
{
    public NN network;
    public int[] inputNodesIndices;
    public float[] inputNodesValues;
    public int[] layersForBiases;
    public int[] indicesForBiases;
    public float[] biases;
    public int[] leftLayersForEdgeWeights;
    public int[] leftIndicesForEdgeWeights;
    public int[] rightIndicesForEdgeWeights;
    public float[] edgeWeights;

    public override void StartTask()
    {
        /*
        network.nodeValueChanged += NNParamteresUpdated;
        network.nodeBiasChanged += NNParamteresUpdated;
        network.edgeWeightChanged += NNParamteresUpdated;
        */
    }

    //private void NNParamteresUpdated()
    private void Update()
    {
        if (InputNodesCorrect() && BiasesCorrect() && EdgeWeightsCorrect())
        {
            gameObject.SetActive(false);
            /*network.nodeValueChanged -= NNParamteresUpdated;
            network.nodeBiasChanged -= NNParamteresUpdated;
            network.edgeWeightChanged -= NNParamteresUpdated;*/
            TaskFinished();
        }
    }

    private bool InputNodesCorrect()
    {
        if (inputNodesValues == null) {
            return true;
        }
        for (int i = 0; i < inputNodesValues.Length; i++) {
            if (network.GetLayerSize(0) < inputNodesIndices[i] + 1)
            {
                return false;
            }
            if (network.GetNodeValue(0, inputNodesIndices[i]) != inputNodesValues[i])
            {
                return false;
            }

        }
        return true;
    }

    private bool BiasesCorrect() {
        if (biases == null)
        {
            return true;
        }
        
        for (int i = 0; i < biases.Length; i++)
        {
            if (network.GetLayerSize(layersForBiases[i]) < indicesForBiases[i] + 1)
            {
                return false;
            }
            if (network.GetNodeValue(layersForBiases[i], indicesForBiases[i]) != biases[i])
            {
                return false;
            }

        }
        return true;
    }

    private bool EdgeWeightsCorrect() {
        if (edgeWeights == null)
        {
            return true;
        }
        for (int i = 0; i < edgeWeights.Length; i++)
        {
            if (network.GetLayerSize(leftLayersForEdgeWeights[i]) < leftIndicesForEdgeWeights[i] + 1 || network.GetLayerSize(leftLayersForEdgeWeights[i] + 1) < rightIndicesForEdgeWeights[i] + 1)
            {
                return false;
            }
            if (network.GetEdgeWeight(leftLayersForEdgeWeights[i], leftIndicesForEdgeWeights[i], rightIndicesForEdgeWeights[i]) != edgeWeights[i])
            {
                return false;
            }

        }
        return true;
    }


}
