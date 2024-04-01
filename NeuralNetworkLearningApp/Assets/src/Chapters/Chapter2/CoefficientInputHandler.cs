using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Windows;

[RequireComponent(typeof(InputField))]
public class CoefficientInputHandler : MonoBehaviour
{
    public DecisionBoundary decisionBoundary;
    private InputField inputField;
    public int coefficientIndex;
    public bool trailingPlusSign;
    string pattern = @"^[-+]?((\d?([.,]\d?)?)|(\d?\d?))$";

    private void Start()
    {
        inputField = GetComponent<InputField>();
    }
    // allow one optional minus sign and one digit before and after a single separator
    public void ValidateCoefficientInput()
    {
        string input = inputField.text;
        if (input == null || input == "") return;

        bool valid = Regex.IsMatch(input, pattern);
        if (!valid) {
            if (input.Length > 1) {
                //check if just the last digit can be removed
                input = input.Substring(0, input.Length - 1);
                valid = Regex.IsMatch(input, pattern);
                UpdateInputField(valid ? input : "");
            } else
            {
                UpdateInputField("");
            }
        }
    }

    public void OnEndEdit()
    {
        string input = inputField.text;
        print("replace dots");
        input = input.Replace(".", ",");
        print("input after replacing dots: " + input);
        if (input == null) return;
        if (input == "" ||  input == "-")
        {
            input = "0";
        }
        
        if (input.StartsWith(","))
        {
            input = "0" + input;
        }
        else if (input.StartsWith("-,"))
        {
            input = "-0." + input.Substring(2);
        }

        if (input.EndsWith(","))
        {
            input += "0";
        }

        if (trailingPlusSign && char.IsDigit(input[0]))
        {
            input = "+" + input;
        }
        UpdateInputField(input);
    }

    public void Increase()
    {
        float value = float.Parse(inputField.text);
        if (value < 10 && value >= -10)
        {
            value += 0.1f;
        } else if (value < 99 && value >= -99)
        {
            value += 1;
        } else
        {
            // don't go into triple digits
            return;
        }
        
        string formattedString = value == (int)value ? value.ToString("0") : value.ToString("0.0");
        UpdateInputField(formattedString);
        OnEndEdit();
    }

    public void Decrease()
    {
        float value = float.Parse(inputField.text);
        if (value <= 10 && value > -10)
        {
            value -= 0.1f;
        }
        else if (value <= 99 && value > -99)
        {
            value -= 1;
        }
        else
        {
            // don't go into triple digits
            return;
        }
        string formattedString = value == (int)value ? value.ToString("0") : value.ToString("0.0");
        UpdateInputField(formattedString);
        OnEndEdit();
    }

    private void UpdateInputField(string inputString)
    {
        inputField.SetTextWithoutNotify(inputString);
        try
        {
            float value = float.Parse(inputString);
            decisionBoundary.SetCoefficient(coefficientIndex, value);
        } catch
        {
            print("Unable to parse input " + inputString + " or no decison boundary found, decision boundary not updated.");
        }
        
    }
}
