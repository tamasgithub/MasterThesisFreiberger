using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscriminantFunctionDisplay : MonoBehaviour
{
    public DecisionBoundary decisionBoundary;
    public PlottableData stone;
    public NN network;
    public int boundaryIndex;
    private Text[] texts;
    private InputField[] inputFields;
    private float[] coeffs;
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        texts = GetComponentsInChildren<Text>();
        if (decisionBoundary != null)
        {
            coeffs = decisionBoundary.GetCoefficients();
            for (int i = 0; i < 3; i++)
            {
                float c = coeffs[i];
                texts[i].text = c == (int)c ? c.ToString("0") : c.ToString("0.0");
            }
        }
        inputFields = GetComponentsInChildren<InputField>();
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (stone != null)
        {
            UpdateValuesFromStoneFeatures();
        } else if (network != null)
        {
            rect.localPosition = new Vector2(rect.localPosition.x, -20 + (network.GetLayerSize(network.GetLayerCount() - 1) - 1) * 170 / 2f - boundaryIndex * 170);
            UpdateValuesFromNN();
        }
        
    }

    private void UpdateValuesFromStoneFeatures()
    {
        float[] featureValues = stone.GetFeatureValues();
        if (inputFields.Length != 3 || coeffs.Length != 3 || featureValues.Length != 2)
        {
            Debug.LogError("Unsupported dimensions. Only dimensions supported are input fields = coefficients = 3, feature values = 2");
            return;
        }

        float result = 0;
        for (int i = 0; i < featureValues.Length; i++)
        {
            float v = featureValues[i];
            string text = v == (int)v ? v.ToString("0") : v.ToString("0.0");
            if (i > 0 && char.IsDigit(text[0]))
            {
                text = "+" + text;
            }
            inputFields[i].SetTextWithoutNotify(v == (int)v ? v.ToString("0") : v.ToString("0.0"));
            result += v * coeffs[i];
        }
        result += coeffs[2];
        string resultText = result == (int)result ? result.ToString("0") : result.ToString("0.0");
        if (result < 0 && resultText[0] != '-')
        {
            resultText = "-" + resultText;
        }
        inputFields[2].SetTextWithoutNotify(resultText);
    }

    private void UpdateValuesFromNN()
    {
        coeffs = GetDecisionBoundaryCoeffsFromNN();
        float[] featureValues = GetInputValuesFromNN();
        if (inputFields.Length != featureValues.Length + 1) {
            Debug.LogError("Dimensions don't match");
            return;
        }
        if (coeffs == null)
        {
            for (int i = 0; i <= network.GetLayerSize(0); i++)
            {
                texts[i].text = "--";
            }
        } else
        {
            for (int i = 0; i < coeffs.Length; i++)
            {
                string coeffString = coeffs[i].ToString("0.00");
                texts[i].text = (i > 0 && char.IsDigit(coeffString[0])) ? "+" + coeffString : coeffString;
            }
        }
        float result = 0f;
        if (coeffs != null)
        {
            for (int i = 0; i < featureValues.Length; i++)
            {
                inputFields[i].SetTextWithoutNotify(featureValues[i].ToString("0.00"));
                result += coeffs[i] * featureValues[i];
            }

            result += coeffs[coeffs.Length - 1];
        }
        inputFields[inputFields.Length - 1].SetTextWithoutNotify(coeffs == null ? "--" : result.ToString("0.00"));

    }

    private float[] GetDecisionBoundaryCoeffsFromNN()
    {
        if (network.GetLayerCount() != 2 || network.GetLayerSize(1) < boundaryIndex - 1)
        {
            return null;
        }
        coeffs = new float[network.GetLayerSize(0) + 1];
        for (int i = 0; i < coeffs.Length - 1; i++)
        {
            coeffs[i] = network.GetEdgeWeight(0, i, boundaryIndex);
        }
        coeffs[coeffs.Length - 1] = network.GetNodeBias(1, boundaryIndex);
        return coeffs;
    }

    private float[] GetInputValuesFromNN()
    {
        float[] inputValues = new float[network.GetLayerSize(0)];
        for (int i = 0; i < inputValues.Length; i++)
        {
            inputValues[i] = network.GetNodeValue(0, i);
        }
        return inputValues;
    }
}
