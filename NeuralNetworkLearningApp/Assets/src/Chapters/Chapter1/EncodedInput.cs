using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodedInput : MonoBehaviour
{
    private FMInput input;
    private void Start()
    {
        input = GetComponent<FMInput>();
    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int value = (int)input.GetValue();
            print("value" + value);
            input.SetValue(value == 0 ? "1" : "0");
        }
    }
}
