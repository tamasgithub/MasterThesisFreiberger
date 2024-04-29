using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NNEditor : MonoBehaviour
{
    private InputField inputField;
    private Node node;
    private Edge edge;

    string pattern = @"^[-+]?\d?([.,]\d?\d?)?$";
    private void Awake()
    {
        inputField = GetComponentInChildren<InputField>();
    }

    public void SetEditedEdge(Edge edge)
    {
        this.edge = edge;
    }

    public void SetEditedNode(Node node)
    {
        this.node = node;
    }

    // allow one optional minus sign and one digit before and after a single separator
    public void ValidateInput()
    {
        print("validate input");
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
        print("end edit");
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
        } else if (edge != null)
        {
            edge.SetWeight(float.Parse(input));
        }
        
    }

}
