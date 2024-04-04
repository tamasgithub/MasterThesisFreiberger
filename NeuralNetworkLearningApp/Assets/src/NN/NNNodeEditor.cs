using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NNNodeEditor : MonoBehaviour
{

    private InputField inputField;
    private Node node;

    string pattern = @"^[-+]?\d?([.,]\d?\d?)?$";
    private void Awake()
    {
        inputField = GetComponentInChildren<InputField>();
    }

    private void Start()
    {
        //formatting whatever was already written in by interacting with the decision boundary before start
        OnEndEdit();
        node = GetComponentInParent<Node>();
    }
    // allow one optional minus sign and one digit before and after a single separator
    public void ValidateInput()
    {
        string input = inputField.text;
        if (input == null || input == "") return;

        bool valid = Regex.IsMatch(input, pattern);
        if (!valid)
        {
            if (input.Length > 1)
            {
                //check if just the last digit can be removed
                input = input.Substring(0, input.Length - 1);
                valid = Regex.IsMatch(input, pattern);
                inputField.SetTextWithoutNotify(valid ? input : "");
            }
            else
            {
                inputField.SetTextWithoutNotify("");
            }
        }
    }

    public void OnEndEdit()
    {
        
        string input = inputField.text;
        if (input == null) return;
        input = input.Replace(".", ",").Replace("+", "");
        
        if (input == "" || input == "-")
        {
            input = "0";
        }

        if (input.StartsWith(","))
        {
            input = "0" + input;
        }
        else if (input.StartsWith("-,"))
        {
            input = "-0," + input.Substring(2);
        }

        if (input.EndsWith(","))
        {
            input += "0";
        }


        inputField.SetTextWithoutNotify(input);
        if (node != null)
        {
            if (node.GetLayerIndex() == 0)
            {
                node.SetInputValue(float.Parse(input));
            } else
            {
                node.SetBias(float.Parse(input));
            }
            
        }
        
    }

}
