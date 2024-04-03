using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class DiscriminantFunctionDisplay : MonoBehaviour
{
    public DecisionBoundary decisionBoundary;
    public PlottableData stone;
    private Text[] texts;
    private InputField[] inputFields;
    private float[] coeffs;
    // Start is called before the first frame update
    void Start()
    {
        coeffs = decisionBoundary.GetCoefficients();
        Text[] texts = GetComponentsInChildren<Text>();
        for (int i = 0; i < 3; i++)
        {
            float c = coeffs[i];
            texts[i].text = c == (int)c ? c.ToString("0") : c.ToString("0.0");
        }
        inputFields = GetComponentsInChildren<InputField>();
    }

    // Update is called once per frame
    void Update()
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
}
