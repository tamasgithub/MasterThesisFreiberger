using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StringFMInputEncoder : MonoBehaviour
{
    private DataPrefabHolder dataPrefabHolder;
    private FMInput input;
    private void Start()
    {
        dataPrefabHolder = FindObjectOfType<DataPrefabHolder>();
        input = GetComponent<FMInput>();
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){
            string value = (string)input.GetValue();
            
            
            GameObject intPrefab = dataPrefabHolder.GetDataPrefabForType(typeof(int));
            for (int i = 0; i < 4; i++)
            {
                GameObject intInput = Instantiate(intPrefab, transform.position, Quaternion.identity);
                intInput.GetComponentInChildren<TextMesh>().text = "0";
                intInput.AddComponent<EncodedInput>();
            }

            Destroy(gameObject);
        }
    }
}
